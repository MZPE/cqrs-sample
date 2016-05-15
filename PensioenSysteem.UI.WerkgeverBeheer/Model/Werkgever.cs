using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PensioenSysteem.UI.WerkgeverBeheer.Model
{
    internal class Werkgever
    {
        public Guid Id { get; set; }
        public int Version { get; set; }
        public string Nummer { get; set; }
        public string BedrijfsNaam { get; set; }
        public string NaamContactpersoon { get; set; }
        public string EmailAdres { get; set; }
        public string VestigingsAdresStraat { get; set; }
        public int VestigingsAdresHuisnummer { get; set; }
        public string VestigingsAdresHuisnummerToevoeging { get; set; }
        public string VestigingsAdresPostcode { get; set; }
        public string VestigingsAdresPlaats { get; set; }
    }
}
