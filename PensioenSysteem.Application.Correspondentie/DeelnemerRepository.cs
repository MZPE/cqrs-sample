using AutoMapper;
using PensioenSysteem.Application.Correspondentie.Model;
using PensioenSysteem.Domain.Messages.Deelnemer.Events;
using Raven.Client;
using Raven.Client.Embedded;
using System;

namespace PensioenSysteem.Application.Correspondentie
{
    internal class DeelnemerRepository
    {
        private EmbeddableDocumentStore _documentStore;
        private IMapper _deelnemerGeregistreerdToDeelnemerMapper;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DeelnemerRepository()
        {
            InitializeDatastore();
            InitializeMappers();
        }

        public void RegistreerDeelnemer(DeelnemerGeregistreerd deelnemerGeregistreerd)
        {
            Deelnemer deelnemer = _deelnemerGeregistreerdToDeelnemerMapper.Map<Deelnemer>(deelnemerGeregistreerd);
            RegistreerDeelnemer(deelnemer);
        }

        public void RegistreerDeelnemer(Deelnemer deelnemer)
        {
            using (IDocumentSession session = _documentStore.OpenSession())
            {
                session.Store(deelnemer);
                session.SaveChanges();
            }
        }

        public Deelnemer RaadpleegDeelnemer(Guid id)
        {
            using (IDocumentSession session = _documentStore.OpenSession())
            {
                Deelnemer deelnemer = session.Load<Deelnemer>($"deelnemers/{id.ToString("D")}");
                return deelnemer;
            }
        }

        private void InitializeDatastore()
        {
            _documentStore = new EmbeddableDocumentStore
            {
                DefaultDatabase = "Correspondentie"
            };
            _documentStore.Initialize();
        }

        private void InitializeMappers()
        {
            var config = new MapperConfiguration(cfg =>
               cfg.CreateMap<DeelnemerGeregistreerd, Deelnemer>());
            _deelnemerGeregistreerdToDeelnemerMapper = config.CreateMapper();
        }
    }
}
