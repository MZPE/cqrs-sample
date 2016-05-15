using Newtonsoft.Json;
using PensioenSysteem.Infrastructure;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Raven.Client.Embedded;
using PensioenSysteem.UI.DeelnemerBeheer.Model;
using Raven.Client;
using AutoMapper;
using System.Collections.Generic;
using PensioenSysteem.Domain.Messages.Deelnemer.Events;

namespace PensioenSysteem.UI.DeelnemerBeheer
{
    public partial class Form1 : Form
    {
        private RabbitMQDomainEventHandler _eventHandler;
        private EmbeddableDocumentStore _documentStore;
        private IMapper _deelnemerGeregistreerdToDeelnemerMapper;

        public Form1()
        {
            InitializeComponent();
            InitializeDatastore();
            InitializeMappers();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            UpdateList();

            _eventHandler = new RabbitMQDomainEventHandler("127.0.0.1", "cqrs_user", "SeeQueErEs", "PensioenSysteem.Deelnemer", HandleEvent);
            _eventHandler.Start();
        }

        private bool HandleEvent(string eventType, string eventData)
        {
            bool handled = false;
            switch (eventType)
            {
                case "DeelnemerGeregistreerd":
                    DeelnemerGeregistreerd deelnemerGeregistreerd = JsonConvert.DeserializeObject<DeelnemerGeregistreerd>(eventData);
                    handled = HandleEvent(deelnemerGeregistreerd);
                    break;
                case "DeelnemerVerhuisd":
                    DeelnemerVerhuisd deelnemerVerhuisd = JsonConvert.DeserializeObject<DeelnemerVerhuisd>(eventData);
                    handled = HandleEvent(deelnemerVerhuisd);
                    break;
            }

            // refresh datagrid
            this.Invoke((MethodInvoker)delegate
            {
                UpdateList();
            });

            return handled;
        }

        private bool HandleEvent(DeelnemerGeregistreerd e)
        {
            using (IDocumentSession session = _documentStore.OpenSession()) 
            {
                Deelnemer deelnemer = _deelnemerGeregistreerdToDeelnemerMapper.Map<Deelnemer>(e);
                session.Store(deelnemer); 
                session.SaveChanges(); 
            }
            return true;
        }

        private bool HandleEvent(DeelnemerVerhuisd e)
        {
            using (IDocumentSession session = _documentStore.OpenSession())
            {
                Deelnemer deelnemer = session.Load<Deelnemer>($"deelnemers/{e.Id.ToString("D")}");

                if (deelnemer != null)
                {
                    deelnemer.WoonAdresStraat = e.Straat;
                    deelnemer.WoonAdresHuisnummer = e.Huisnummer;
                    deelnemer.WoonAdresHuisnummerToevoeging = e.HuisnummerToevoeging;
                    deelnemer.WoonAdresPostcode = e.Postcode;
                    deelnemer.WoonAdresPlaats = e.Plaats;
                    session.SaveChanges();
                }
                else
                {
                    // TODO: handle this situation
                }
            }
            return true;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_eventHandler != null)
            {
                _eventHandler.Stop();
            }
            if (_documentStore != null)
            {
                _documentStore.Dispose();
            }
        }

        private void verhuisToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Deelnemer deelnemer = deelnemerBindingSource.Current as Deelnemer;
            if (deelnemer != null)
            {
                Verhuis dialog = new Verhuis(deelnemer.Id, deelnemer.Version, deelnemer.WoonAdresStraat, 
                    deelnemer.WoonAdresHuisnummer, deelnemer.WoonAdresHuisnummerToevoeging, deelnemer.WoonAdresPostcode, 
                    deelnemer.WoonAdresPlaats);
                dialog.ShowDialog(this);
            }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            UpdateList();
        }

        private void deelnemerBindingSource_ListChanged(object sender, ListChangedEventArgs e)
        {
            recordCountStatusLabel.Text = "Aantal items : " + this.deelnemerBindingSource.List.Count;
        }

        private void UpdateList()
        {
            this.deelnemerBindingSource.SuspendBinding();
            this.deelnemerBindingSource.List.Clear();
            
            using (IDocumentSession session = _documentStore.OpenSession())
            {
                List<Deelnemer> deelnemers = session
                    .Query<Deelnemer>()
                    .Customize(x => x.WaitForNonStaleResultsAsOfNow()) // wait for any pending index udpates
                    .ToList();
                foreach (Deelnemer deelnemer in deelnemers)
                {
                    this.deelnemerBindingSource.List.Add(deelnemer);
                }
            }

            this.deelnemerBindingSource.ResumeBinding();
        }

        private void InitializeDatastore()
        {
            _documentStore = new EmbeddableDocumentStore
            {
                DefaultDatabase = "DeelnemerBeheer"
            };
            _documentStore.Initialize();
        }

        private void InitializeMappers()
        {
            var config = new MapperConfiguration(cfg =>
                cfg.CreateMap<DeelnemerGeregistreerd, Deelnemer>()
                    .ForMember(dest => dest.WoonAdresStraat, opt => opt.MapFrom(src => src.Straat))
                    .ForMember(dest => dest.WoonAdresHuisnummer, opt => opt.MapFrom(src => src.Huisnummer))
                    .ForMember(dest => dest.WoonAdresHuisnummerToevoeging, opt => opt.MapFrom(src => src.HuisnummerToevoeging))
                    .ForMember(dest => dest.WoonAdresPostcode, opt => opt.MapFrom(src => src.Postcode))
                    .ForMember(dest => dest.WoonAdresPlaats, opt => opt.MapFrom(src => src.Plaats)));
            _deelnemerGeregistreerdToDeelnemerMapper = config.CreateMapper();
        }
    }
}
