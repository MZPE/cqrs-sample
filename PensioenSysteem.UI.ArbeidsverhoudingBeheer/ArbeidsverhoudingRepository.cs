using LiteDB;
using PensioenSysteem.UI.ArbeidsverhoudingBeheer.Model;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PensioenSysteem.UI.ArbeidsverhoudingBeheer
{
    internal class ArbeidsverhoudingRepository
    {
        private const string _databaseFolder = @"D:\PensioenSysteem\Databases\ArbeidsverhoudingBeheer\";
        private const string _databaseFile = _databaseFolder + @"ArbeidsverhoudingBeheer.db";

        public ArbeidsverhoudingRepository()
        {
            Directory.CreateDirectory(_databaseFolder);
        }

        public void RegistreerArbeidsverhouding(Arbeidsverhouding arbeidsverhouding)
        {
            using (var db = new LiteDatabase(_databaseFile))
            {
                var col = db.GetCollection<Arbeidsverhouding>("arbeidsverhouding");
                col.Insert(arbeidsverhouding);
                db.Commit();
            }
        }

        public IList<Arbeidsverhouding> RaadpleegArbeidsverhoudingen()
        {
            using (var db = new LiteDatabase(_databaseFile))
            {
                var col = db.GetCollection<Arbeidsverhouding>("arbeidsverhouding");
                return col.FindAll().ToList();
            }
        }
    }
}
