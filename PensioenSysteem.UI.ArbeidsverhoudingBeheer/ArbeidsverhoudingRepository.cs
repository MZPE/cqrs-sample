using PensioenSysteem.UI.ArbeidsverhoudingBeheer.Model;
using Raven.Client;
using Raven.Client.Embedded;
using Raven.Database.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PensioenSysteem.UI.ArbeidsverhoudingBeheer
{
    internal class ArbeidsverhoudingRepository
    {
        private EmbeddableDocumentStore _documentStore;

        public ArbeidsverhoudingRepository()
        {
            InitializeDatastore();
        }

        public void RegistreerArbeidsverhouding(Arbeidsverhouding arbeidsverhouding)
        {
            using (IDocumentSession session = _documentStore.OpenSession())
            {
                session.Store(arbeidsverhouding);
                session.SaveChanges();
            }
        }

        public IList<Arbeidsverhouding> RaadpleegArbeidsverhoudingen()
        {
            using (IDocumentSession session = _documentStore.OpenSession())
            {
                List<Arbeidsverhouding> arbeidsverhoudingen = session
                    .Query<Arbeidsverhouding>()
                    .Customize(x => x.WaitForNonStaleResultsAsOfNow()) // wait for any pending index udpates
                    .ToList();
                return arbeidsverhoudingen;
            }
        }

        private void InitializeDatastore()
        {
            NonAdminHttp.EnsureCanListenToWhenInNonAdminContext(9002);
            _documentStore = new EmbeddableDocumentStore
            {
                DefaultDatabase = "ArbeidsverhoudingBeheer",
                UseEmbeddedHttpServer = true
            };
            _documentStore.Initialize();
        }
    }
}
