using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PensioenSysteem.UI.ArbeidsverhoudingBeheer.Model
{
    public class Arbeidsverhouding
    {
        public Guid Id { get; set; }
        public int Version { get; set; }
        public string Nummer { get; set; }
        public string DeelnemerNummer { get; set; }
        public string WerkgeverNummer { get; set; }
        public DateTime IngangsDatum { get; set; }
        public DateTime EindDatum { get; set; }
    }
}
