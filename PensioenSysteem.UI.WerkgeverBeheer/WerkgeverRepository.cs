using LiteDB;
using PensioenSysteem.UI.WerkgeverBeheer.Model;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PensioenSysteem.UI.WerkgeverBeheer
{
    internal class WerkgeverRepository
    {
        private const string _databaseFolder = @"D:\PensioenSysteem\Databases\WerkgeverBeheer\";
        private const string _databaseFile = _databaseFolder + @"WerkgeverBeheer.db";

        public WerkgeverRepository()
        {
            Directory.CreateDirectory(_databaseFolder);
        }

        public void RegistreerWerkgever(Werkgever werkgever)
        {
            using (var db = new LiteDatabase(_databaseFile))
            {
                var col = db.GetCollection<Werkgever>("werkgever");
                col.Insert(werkgever);
                db.Commit();
            }
        }

        public IList<Werkgever> RaadpleegWerkgevers()
        {
            using (var db = new LiteDatabase(_databaseFile))
            {
                var col = db.GetCollection<Werkgever>("werkgever");
                return col.FindAll().ToList();
            }
        }
    }
}
