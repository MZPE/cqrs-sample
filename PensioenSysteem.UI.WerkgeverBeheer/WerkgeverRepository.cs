using PensioenSysteem.UI.WerkgeverBeheer.Model;
using Raven.Client;
using Raven.Client.Embedded;
using Raven.Database.Server;
using System.Collections.Generic;
using System.Linq;

namespace PensioenSysteem.UI.WerkgeverBeheer
{
    internal class WerkgeverRepository
    {
        private EmbeddableDocumentStore _documentStore;

        public WerkgeverRepository()
        {
            InitializeDatastore();
        }

        public void RegistreerWerkgever(Werkgever werkgever)
        {
            using (IDocumentSession session = _documentStore.OpenSession())
            {
                session.Store(werkgever);
                session.SaveChanges();
            }
        }

        public IList<Werkgever> RaadpleegWerkgevers()
        {
            using (IDocumentSession session = _documentStore.OpenSession())
            {
                List<Werkgever> werkgevers = session
                    .Query<Werkgever>()
                    .Customize(x => x.WaitForNonStaleResultsAsOfNow()) // wait for any pending index udpates
                    .ToList();
                return werkgevers;
            }
        }

        private void InitializeDatastore()
        {
            NonAdminHttp.EnsureCanListenToWhenInNonAdminContext(9004);
            _documentStore = new EmbeddableDocumentStore
            {
                DefaultDatabase = "WerkgeverBeheer",
                UseEmbeddedHttpServer = true
            };
            _documentStore.Initialize();
        }
    }
}
