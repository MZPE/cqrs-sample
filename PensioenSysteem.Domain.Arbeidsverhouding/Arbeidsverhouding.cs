using PensioenSysteem.Domain.Core;
using PensioenSysteem.Domain.Messages.Arbeidsverhouding.Commands;
using PensioenSysteem.Domain.Messages.Arbeidsverhouding.Events;
using System;

namespace PensioenSysteem.Domain.Arbeidsverhouding
{
    public class Arbeidsverhouding : AggregateRoot, IHandle<ArbeidsverhoudingGeregistreerd>
    {
        private Guid _id;
        private string _nummer;
        private string _deelnemerNummer;
        private string _werkgeverNummer;
        private DateTime _ingangsdatum;
        private DateTime? _einddatum;

        public override Guid Id
        {
            get
            {
                return _id;
            }
        }

        public string Nummer
        {
            get
            {
                return _nummer;
            }
        }

        public string DeelnemerNummer
        {
            get
            {
                return _deelnemerNummer;
            }
        }
        public string WerkgeverNummer
        {
            get
            {
                return _werkgeverNummer;
            }
        }
        public DateTime Ingangsdatum
        {
            get
            {
                return _ingangsdatum;
            }
        }
        public DateTime? Einddatum
        {
            get
            {
                return _einddatum;
            }
        }

        public void Registreer(RegistreerArbeidsverhoudingCommand command)
        {
            this.ApplyChange(new ArbeidsverhoudingGeregistreerd
                {
                    RoutingKey = "Arbeidsverhouding.Geregistreerd",
                    CorrelationId = command.CorrelationId,
                    Id = command.Id,
                    Nummer = GenereerArbeidsverhoudingNummer(),
                    DeelnemerNummer = command.DeelnemerNummer,
                    WerkgeverNummer = command.WerkgeverNummer,
                    Ingangsdatum = command.Ingangsdatum,
                    EindDatum = command.EindDatum
                });
        }

        public void Handle(ArbeidsverhoudingGeregistreerd e)
        {
            _id = e.Id;
            _deelnemerNummer = e.DeelnemerNummer;
            _werkgeverNummer = e.WerkgeverNummer;
            _ingangsdatum = e.Ingangsdatum;
            _einddatum = e.EindDatum;
        }

        private string GenereerArbeidsverhoudingNummer()
        {
            return DateTime.Now.ToString("AVyyyyMMddhhMMssffffff");
        }
    }
}
