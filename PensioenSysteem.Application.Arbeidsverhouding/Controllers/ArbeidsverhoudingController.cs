using PensioenSysteem.Domain.Arbeidsverhouding.Commands;
using PensioenSysteem.Domain.Core;
using PensioenSysteem.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PensioenSysteem.Application.Deelnemer.Controllers
{
    public class ArbeidsverhoudingController : ApiController
    {
        private IAggregateRepository<PensioenSysteem.Domain.Arbeidsverhouding.Arbeidsverhouding> _repo;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="repo">The repository to use for storing and retrieving aggregates.</param>
        public ArbeidsverhoudingController(IAggregateRepository<PensioenSysteem.Domain.Arbeidsverhouding.Arbeidsverhouding> repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// POST /api/arbeidsverhouding
        /// </summary>
        /// <param name="command">Het command om een arbeidsverhouding te registreren.</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/arbeidsverhouding")]
        public HttpResponseMessage Create([FromBody] RegistreerArbeidsverhoudingCommand command)
        {
            var arbeidsverhouding = new PensioenSysteem.Domain.Arbeidsverhouding.Arbeidsverhouding();
            arbeidsverhouding.Registreer(command);
            _repo.Save(arbeidsverhouding, -1);
            return new HttpResponseMessage(HttpStatusCode.Created);
        }

        /// <summary>
        /// GET /api/arbeidsverhouding/{id}
        /// </summary>
        /// <param name="command">Het command om een arbeidsverhouding te registreren.</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/arbeidsverhouding/{id}")]
        public Domain.Arbeidsverhouding.Arbeidsverhouding Get(Guid id)
        {
            var arbeidsverhouding = _repo.GetById(id);
            return arbeidsverhouding;
        }
    }
}
