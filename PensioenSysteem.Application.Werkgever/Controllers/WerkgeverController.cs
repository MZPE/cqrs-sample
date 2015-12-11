using PensioenSysteem.Domain.Core;
using PensioenSysteem.Domain.Werkgever.Commands;
using PensioenSysteem.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PensioenSysteem.Application.Deelnemer.Controllers
{
    public class WerkgeverController : ApiController
    {
        private IAggregateRepository<PensioenSysteem.Domain.Werkgever.Werkgever> _repo;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="repo">The repository to use for storing and retrieving aggregates.</param>
        public WerkgeverController(IAggregateRepository<PensioenSysteem.Domain.Werkgever.Werkgever> repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// POST /api/werkgever
        /// </summary>
        /// <param name="command">Het command om een werkgever te registreren.</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/werkgever")]
        public HttpResponseMessage Create([FromBody] RegistreerWerkgeverCommand command)
        {
            var werkgever = new PensioenSysteem.Domain.Werkgever.Werkgever();
            werkgever.Registreer(command);
            _repo.Save(werkgever, -1);
            return new HttpResponseMessage(HttpStatusCode.Created);
        }

        /// <summary>
        /// GET /api/werkgever/{id}
        /// </summary>
        [HttpGet]
        [Route("api/werkgever/{id}")]
        public Domain.Werkgever.Werkgever Get(Guid id)
        {
            var werkgever = _repo.GetById(id);
            return werkgever;
        }
    }
}
