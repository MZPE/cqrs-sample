using PensioenSysteem.UI.ArbeidsverhoudingBeheer.Model;
using Raven.Client;
using Raven.Client.Document;
using System.Collections.Generic;
using System.Linq;

namespace PensioenSysteem.UI.ArbeidsverhoudingBeheer
{
    internal class ArbeidsverhoudingRepository
    {
        private DocumentStore _documentStore;

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
            _documentStore = new DocumentStore
            {
                DefaultDatabase = "ArbeidsverhoudingBeheer",
                Url = "http://localhost:8080"
            };
            _documentStore.Initialize();
        }
    }
}
