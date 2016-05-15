using PensioenSysteem.Domain.Core;
using PensioenSysteem.Domain.Messages.Deelnemer.Commands;
using PensioenSysteem.Infrastructure;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PensioenSysteem.Application.Deelnemer.Controllers
{
    public class DeelnemerController : ApiController
    {
        private IAggregateRepository<PensioenSysteem.Domain.Deelnemer.Deelnemer> _repo;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="repo">The repository to use for storing and retrieving aggregates.</param>
        public DeelnemerController(IAggregateRepository<PensioenSysteem.Domain.Deelnemer.Deelnemer> repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// POST /api/deelnemer
        /// </summary>
        /// <param name="command">Het command om een deelnemer te registreren.</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/deelnemer")]
        public HttpResponseMessage Create([FromBody] RegistreerDeelnemerCommand command)
        {
            var deelnemer = new PensioenSysteem.Domain.Deelnemer.Deelnemer();
            deelnemer.Registreer(command);
            _repo.Save(deelnemer, -1);
            return new HttpResponseMessage(HttpStatusCode.Created);
        }

        /// <summary>
        /// PUT /api/deelnemer/{id}/verhuis
        /// </summary>
        /// <param name="command">Het command om een deelnemer te verhuizen.</param>
        /// <returns></returns>
        [HttpPut]
        [Route("api/deelnemer/{id}/verhuis")]
        public HttpResponseMessage Verhuis(Guid id, [FromBody] VerhuisDeelnemerCommand command)
        {
            Domain.Deelnemer.Deelnemer deelnemer;
            try
            {
                deelnemer = _repo.GetById(id);
            }
            catch (AggregateNotFoundException ex)
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }

            deelnemer.Verhuis(command);
            
            _repo.Save(deelnemer, command.Version);

            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        /// <summary>
        /// GET /api/deelnemer/{id}
        /// </summary>
        [HttpGet]
        [Route("api/deelnemer/{id}")]
        public Domain.Deelnemer.Deelnemer Get(Guid id)
        {
            var deelnemer = _repo.GetById(id);
            return deelnemer;
        }
    }
}
