using LiteDB;
using PensioenSysteem.UI.DeelnemerBeheer.Model;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PensioenSysteem.UI.DeelnemerBeheer
{
    internal class DeelnemerRepository
    {
        private const string _databaseFolder = @"D:\PensioenSysteem\Databases\DeelnemerBeheer\";
        private const string _databaseFile = _databaseFolder + @"DeelnemerBeheer.db";

        public DeelnemerRepository()
        {
            Directory.CreateDirectory(_databaseFolder);
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

        public IList<Deelnemer> RaadpleegDeelnemers()
        {
            using (var db = new LiteDatabase(_databaseFile))
            {
                var col = db.GetCollection<Deelnemer>("deelnemer");
                return col.FindAll().ToList();
            }
        }

        public void RegistreerDeelnemerVerhuizing(Verhuizing verhuizing)
        {
            using (var db = new LiteDatabase(_databaseFile))
            {
                var col = db.GetCollection<Deelnemer>("deelnemer");
                Deelnemer deelnemer = col.FindById(verhuizing.Id);
                deelnemer.WoonAdresStraat = verhuizing.Straat;
                deelnemer.WoonAdresHuisnummer = verhuizing.Huisnummer;
                deelnemer.WoonAdresHuisnummerToevoeging = verhuizing.HuisnummerToevoeging;
                deelnemer.WoonAdresPostcode = verhuizing.Postcode;
                deelnemer.WoonAdresPlaats = verhuizing.Plaats;
                col.Update(deelnemer);
                db.Commit();
            }
        }
    }
}
