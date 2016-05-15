using PensioenSysteem.Domain.Messages.Deelnemer.Events;
using PensioenSysteem.UI.DeelnemerBeheer.Model;
using Raven.Client;
using Raven.Client.Embedded;
using Raven.Database.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PensioenSysteem.UI.DeelnemerBeheer
{
    internal class DeelnemerRepository
    {
        private EmbeddableDocumentStore _documentStore;

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
            NonAdminHttp.EnsureCanListenToWhenInNonAdminContext(9003);
            _documentStore = new EmbeddableDocumentStore
            {
                DefaultDatabase = "DeelnemerBeheer",
                UseEmbeddedHttpServer = true
            };
            _documentStore.Initialize();
        }
    }
}
