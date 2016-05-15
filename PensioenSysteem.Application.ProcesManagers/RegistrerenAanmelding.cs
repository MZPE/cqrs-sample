using Newtonsoft.Json;
using PensioenSysteem.Application.ProcesManagers.Commands;
using PensioenSysteem.Infrastructure;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using PensioenSysteem.Domain.Messages.Deelnemer.Events;
using PensioenSysteem.Domain.Messages.Werkgever.Events;
using PensioenSysteem.Domain.Messages.Arbeidsverhouding.Events;
using PensioenSysteem.Domain.Messages.Deelnemer.Commands;
using PensioenSysteem.Domain.Messages.Werkgever.Commands;
using PensioenSysteem.Domain.Messages.Arbeidsverhouding.Commands;
using PensioenSysteem.Application.ProcesManagers.Model;

namespace PensioenSysteem.Application.ProcesManagers
{
    public class RegistrerenAanmelding
    {
        private RabbitMQDomainEventHandler _eventHandler;
        private ProcessStateRepository _repository;

        public void Start()
        {
            // initialiseer de repo
            _repository = new ProcessStateRepository();

            // start de eventhandler om de events die binnen dit proces een rol spelen op te vangen
            _eventHandler = new RabbitMQDomainEventHandler("127.0.0.1", "cqrs_user", "SeeQueErEs", "PensioenSysteem.RegistreerAanmelding", HandleEvent);
            _eventHandler.Start();
        }

        public void Stop()
        {
            _eventHandler.Stop();
        }

        public void RegistreerAanmelding(RegistreerAanmeldingCommand command)
        {
            // registreer een nieuwe instantie van het RegistreerAanmelding proces 
            ProcessState state = new ProcessState
            {
                Id = command.CorrelationId,
                InitierendCommand = command,
                DeelnemerNummer = null,
                WerkgeverNummer = null,
                StartTijdstip = DateTime.Now,
                Status = "Actief"
            };
            _repository.RegistreerProcessStart(state);

            // controleer aanwezigheid deelnemer
            // TODO

            try
            {
                // registreer de werknemer als deelnemer
                WerknemerGegevens gegevens = command.WerknemerGegevens;
                RegistreerDeelnemerCommand registreerDeelnemerCommand = new RegistreerDeelnemerCommand
                {
                    CorrelationId = command.CorrelationId,
                    Id = gegevens.Id,
                    Version = 0,
                    Naam = gegevens.Naam,
                    EmailAdres = gegevens.EmailAdres,
                    Straat = gegevens.Straat,
                    Huisnummer = gegevens.Huisnummer,
                    HuisnummerToevoeging = gegevens.HuisnummerToevoeging,
                    Postcode = gegevens.Postcode,
                    Plaats = gegevens.Plaats
                };
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:29713");
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage response = client.PostAsJsonAsync("/api/deelnemer", registreerDeelnemerCommand).Result;
                }
            }
            catch (Exception ex)
            {
                state.Status = "Fout";
                state.Foutmelding = ex.ToString();
                _repository.UpdateProcessState(state);
            }
        }

        private bool HandleEvent(string eventType, string eventData)
        {
            bool handled = false;
            switch (eventType)
            {
                case "DeelnemerGeregistreerd":
                    DeelnemerGeregistreerd deelnemerGeregistreerd = JsonConvert.DeserializeObject<DeelnemerGeregistreerd>(eventData);
                    handled = Handle(deelnemerGeregistreerd);
                    break;
                case "WerkgeverGeregistreerd":
                    WerkgeverGeregistreerd werkgeverGeregistreerd = JsonConvert.DeserializeObject<WerkgeverGeregistreerd>(eventData);
                    handled = Handle(werkgeverGeregistreerd);
                    break;
                case "ArbeidsverhoudingGeregistreerd":
                    ArbeidsverhoudingGeregistreerd arbeidsverhoudingGeregistreerd = JsonConvert.DeserializeObject<ArbeidsverhoudingGeregistreerd>(eventData);
                    handled = Handle(arbeidsverhoudingGeregistreerd);
                    break;
                default:
                    return false;
            }

            return handled;
        }

        private bool Handle(DeelnemerGeregistreerd e)
        {
            // zoek de bijbehorende instantie van het RegistreerAanmelding proces 
            ProcessState state = _repository.RaadpleegProcessState(e.CorrelationId);
            if (state == null)
            {
                return false;
            }

            // als de deelnemer al bekend is, beschouw het event als afgehandeld (idempotentie)
            if (!string.IsNullOrEmpty(state.DeelnemerNummer))
            {
                return true;
            }

            // werk het deelnemernummer bij
            state.DeelnemerNummer = e.Nummer;
            _repository.UpdateProcessState(state);

            // controleer aanwezigheid werkgever
            // TODO

            try {
                // registreer de werkgever
                WerkgeverGegevens gegevens = state.InitierendCommand.WerkgeverGegevens;
                RegistreerWerkgeverCommand registreerWerkgeverCommand = new RegistreerWerkgeverCommand
                {
                    CorrelationId = e.CorrelationId,
                    Id = gegevens.Id,
                    Version = 0,
                    BedrijfsNaam = gegevens.BedrijfsNaam,
                    NaamContactPersoon = gegevens.NaamContactPersoon,
                    EmailAdres = gegevens.EmailAdres,
                    Straat = gegevens.Straat,
                    Huisnummer = gegevens.Huisnummer,
                    HuisnummerToevoeging = gegevens.HuisnummerToevoeging,
                    Postcode = gegevens.Postcode,
                    Plaats = gegevens.Plaats
                };
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:24275");
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage response = client.PostAsJsonAsync("/api/werkgever", registreerWerkgeverCommand).Result;
                }
            }
            catch (Exception ex)
            {
                state.Status = "Fout";
                state.Foutmelding = ex.ToString();
                _repository.UpdateProcessState(state);
                return false;
            }

            return true;
        }

        private bool Handle(WerkgeverGeregistreerd e)
        {
            // zoek de bijbehorende instantie van het RegistreerAanmelding proces 
            ProcessState state = _repository.RaadpleegProcessState(e.CorrelationId);
            if (state == null)
            {
                return false;
            }

            // als de werkgever al bekend is, beschouw het event als afgehandeld (idempotentie)
            if (!string.IsNullOrEmpty(state.WerkgeverNummer))
            {
                return true;
            }

            // werk het werkgevernummer bij
            state.WerkgeverNummer = e.Nummer;
            _repository.UpdateProcessState(state);

            try
            {
                // registreer de arbeidsverhouding
                RegistreerArbeidsverhoudingCommand registreerArbeidsverhoudingCommand = new RegistreerArbeidsverhoudingCommand
                {
                    CorrelationId = e.CorrelationId,
                    Id = Guid.NewGuid(),
                    Version = 0,
                    DeelnemerNummer = state.DeelnemerNummer,
                    WerkgeverNummer = state.WerkgeverNummer,
                    Ingangsdatum = state.InitierendCommand.IngangsDatum,
                    EindDatum = state.InitierendCommand.EindDatum
                };
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:24693");
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage response = client.PostAsJsonAsync("/api/arbeidsverhouding", registreerArbeidsverhoudingCommand).Result;
                }
            }
            catch (Exception ex)
            {
                state.Status = "Fout";
                state.Foutmelding = ex.ToString();
                _repository.UpdateProcessState(state);
                return false;
            }

            return true;
        }

        private bool Handle(ArbeidsverhoudingGeregistreerd e)
        {
            // zoek de bijbehorende instantie van het RegistreerAanmelding proces 
            ProcessState state = _repository.RaadpleegProcessState(e.CorrelationId);
            if (state == null)
            {
                return false;
            }

            // TODO: einde proces melden aan DWS

            // proces administratie bijwerken
            state.Status = "Afgerond";
            _repository.UpdateProcessState(state);

            return true;
        }
    }
}
