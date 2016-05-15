using PensioenSysteem.UI.DeelnemerBeheer.Model;
using Raven.Client;
using Raven.Client.Document;
using System.Collections.Generic;
using System.Linq;

namespace PensioenSysteem.UI.DeelnemerBeheer
{
    internal class DeelnemerRepository
    {
        private DocumentStore _documentStore;

        public DeelnemerRepository()
        {
            InitializeDatastore();
        }

        public void RegistreerDeelnemer(Deelnemer deelnemer)
        {
            using (IDocumentSession session = _documentStore.OpenSession())
            {
                session.Store(deelnemer);
                session.SaveChanges();
            }
        }

        public IList<Deelnemer> RaadpleegDeelnemers()
        {
            using (IDocumentSession session = _documentStore.OpenSession())
            {
                List<Deelnemer> deelnemers = session
                    .Query<Deelnemer>()
                    .Customize(x => x.WaitForNonStaleResultsAsOfNow()) // wait for any pending index udpates
                    .ToList();
                return deelnemers;
            }
        }

        public void RegistreerDeelnemerVerhuizing(Verhuizing verhuizing)
        {
            using (IDocumentSession session = _documentStore.OpenSession())
            {
                Deelnemer deelnemer = session.Load<Deelnemer>($"deelnemers/{verhuizing.Id.ToString("D")}");
                if (deelnemer != null)
                {
                    deelnemer.Version = verhuizing.Version;
                    deelnemer.WoonAdresStraat = verhuizing.Straat;
                    deelnemer.WoonAdresHuisnummer = verhuizing.Huisnummer;
                    deelnemer.WoonAdresHuisnummerToevoeging = verhuizing.HuisnummerToevoeging;
                    deelnemer.WoonAdresPostcode = verhuizing.Postcode;
                    deelnemer.WoonAdresPlaats = verhuizing.Plaats;
                    session.SaveChanges();
                }
                else
                {
                    // TODO: handle this situation
                }
            }
        }

        private void InitializeDatastore()
        {
            _documentStore = new DocumentStore
            {
                DefaultDatabase = "DeelnemerBeheer",
                Url = "http://localhost:8080"
            };
            _documentStore.Initialize();
        }
    }
}
