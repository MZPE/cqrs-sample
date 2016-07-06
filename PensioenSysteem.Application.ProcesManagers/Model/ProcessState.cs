using PensioenSysteem.Application.ProcesManagers.Commands;
using System;

namespace PensioenSysteem.Application.ProcesManagers.Model
{
    public class ProcessState
    {
        public Guid Id { get; set; }
        public RegistreerAanmeldingCommand InitierendCommand { get; set; }
        public string Status { get; set; }
        public string DeelnemerNummer { get; set; }
        public string WerkgeverNummer { get; set; }
        public DateTime StartTijdstip { get; set; }
        public string Foutmelding { get; set; }
    }
}
