using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PensioenSysteem.UI.DeelnemerBeheer.Model
{
    internal class Deelnemer
    {
        public Guid Id { get; set; }
        public int Version { get; set; }
        public string Nummer { get; set; }
        public string Naam { get; set; }
        public string EmailAdres { get; set; }
        public string WoonAdresStraat { get; set; }
        public int WoonAdresHuisnummer { get; set; }
        public string WoonAdresHuisnummerToevoeging { get; set; }
        public string WoonAdresPostcode { get; set; }
        public string WoonAdresPlaats { get; set; }
    }
}
