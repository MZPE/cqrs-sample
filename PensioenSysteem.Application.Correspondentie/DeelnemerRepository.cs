using AutoMapper;
using LiteDB;
using PensioenSysteem.Application.Correspondentie.Model;
using PensioenSysteem.Domain.Messages.Deelnemer.Events;
using System;
using System.IO;

namespace PensioenSysteem.Application.Correspondentie
{
    internal class DeelnemerRepository
    {
        private IMapper _deelnemerGeregistreerdToDeelnemerMapper;
        private const string _databaseFolder = @"D:\PensioenSysteem\Databases\Correspondentie\";
        private const string _databaseFile = _databaseFolder + @"Correspondentie.db";

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DeelnemerRepository()
        {
            Directory.CreateDirectory(_databaseFolder);
            InitializeMappers();
        }

        public void RegistreerDeelnemer(DeelnemerGeregistreerd deelnemerGeregistreerd)
        {
            Deelnemer deelnemer = _deelnemerGeregistreerdToDeelnemerMapper.Map<Deelnemer>(deelnemerGeregistreerd);
            RegistreerDeelnemer(deelnemer);
        }

        public void RegistreerDeelnemer(Deelnemer deelnemer)
        {
            using (var db = new LiteDatabase(_databaseFile))
            {
                var col = db.GetCollection<Deelnemer>("deelnemer");
                col.Insert(deelnemer);
                db.Commit();
            }
        }

        public Deelnemer RaadpleegDeelnemer(Guid id)
        {
            using (var db = new LiteDatabase(_databaseFile))
            {
                var col = db.GetCollection<Deelnemer>("deelnemer");
                var deelnemer = col.FindById(id);
                return deelnemer;
            }
        }

        private void InitializeMappers()
        {
            var config = new MapperConfiguration(cfg =>
               cfg.CreateMap<DeelnemerGeregistreerd, Deelnemer>());
            _deelnemerGeregistreerdToDeelnemerMapper = config.CreateMapper();
        }
    }
}
