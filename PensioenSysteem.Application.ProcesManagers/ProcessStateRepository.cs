using PensioenSysteem.Application.ProcesManagers.Model;
using Raven.Client;
using Raven.Client.Embedded;
using Raven.Database.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PensioenSysteem.Application.ProcesManagers
{
    internal class ProcessStateRepository
    {
        private EmbeddableDocumentStore _documentStore;

        public ProcessStateRepository()
        {
            InitializeDatastore();
        }

        public void RegistreerProcessStart(ProcessState state)
        {
            using (IDocumentSession session = _documentStore.OpenSession())
            {
                session.Store(state);
                session.SaveChanges();
            }
        }

        public ProcessState RaadpleegProcessState(Guid correlationId)
        {
            using (IDocumentSession session = _documentStore.OpenSession())
            {
                ProcessState state = session.Load<ProcessState>($"processstates/{correlationId.ToString("D")}");
                return state;
            }
        }

        public void UpdateProcessState(ProcessState state)
        {
            using (IDocumentSession session = _documentStore.OpenSession())
            {
                ProcessState storedState = session.Load<ProcessState>($"processstates/{state.Id.ToString("D")}");
                storedState.DeelnemerNummer = state.DeelnemerNummer;
                storedState.WerkgeverNummer = state.WerkgeverNummer;
                storedState.Status = state.Status;
                storedState.Foutmelding = state.Foutmelding;
                session.SaveChanges();
            }
        }

        private void InitializeDatastore()
        {
            NonAdminHttp.EnsureCanListenToWhenInNonAdminContext(9000);
            _documentStore = new EmbeddableDocumentStore
            {
                DefaultDatabase = "ProcessState",
                UseEmbeddedHttpServer = true
            };
            _documentStore.Initialize();

            Console.WriteLine("RavenDB Studio on http://localhost:9000");
        }
    }
}
