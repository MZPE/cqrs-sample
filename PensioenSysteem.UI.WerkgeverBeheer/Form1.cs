using AutoMapper;
using Newtonsoft.Json;
using PensioenSysteem.Domain.Messages.Werkgever.Events;
using PensioenSysteem.Infrastructure;
using PensioenSysteem.UI.WerkgeverBeheer.Model;
using Raven.Client;
using Raven.Client.Embedded;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace PensioenSysteem.UI.WerkgeverBeheer
{
    public partial class Form1 : Form
    {
        private RabbitMQDomainEventHandler _eventHandler;
        private WerkgeverRepository _repo;
        private IMapper _werkgeverGeregistreerdToWerkgeverMapper;

        public Form1()
        {
            InitializeComponent();
            _repo = new WerkgeverRepository();
            InitializeMappers();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            UpdateList();

            _eventHandler = new RabbitMQDomainEventHandler("127.0.0.1", "cqrs_user", "SeeQueErEs", "PensioenSysteem.Werkgever", HandleEvent);
            _eventHandler.Start();
        }

        private bool HandleEvent(string eventType, string eventData)
        {
            bool handled = false;
            switch (eventType)
            {
                case "WerkgeverGeregistreerd":
                    WerkgeverGeregistreerd werkgeverGeregistreerd = JsonConvert.DeserializeObject<WerkgeverGeregistreerd>(eventData);
                    handled = HandleEvent(werkgeverGeregistreerd);
                    break;
            }

            // refresh datagrid
            this.Invoke((MethodInvoker)delegate
            {
                UpdateList();
            });

            return handled;
        }

        private bool HandleEvent(WerkgeverGeregistreerd e)
        {
            Werkgever werkgever = _werkgeverGeregistreerdToWerkgeverMapper.Map<Werkgever>(e);
            _repo.RegistreerWerkgever(werkgever);
            return true;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_eventHandler != null)
            {
                _eventHandler.Stop();
            }
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UpdateList();
        }

        private void werkgeverBindingSource_ListChanged(object sender, ListChangedEventArgs e)
        {
            recordCountStatusLabel.Text = "Aantal items : " + this.werkgeverBindingSource.List.Count;
        }

        private void UpdateList()
        {
            this.werkgeverBindingSource.SuspendBinding();
            this.werkgeverBindingSource.List.Clear();
            foreach (Werkgever werkgever in _repo.RaadpleegWerkgevers())
            {
                this.werkgeverBindingSource.List.Add(werkgever);
            }
            this.werkgeverBindingSource.ResumeBinding();
        }

        private void InitializeMappers()
        {
            var config = new MapperConfiguration(cfg =>
                cfg.CreateMap<WerkgeverGeregistreerd, Werkgever>()
                    .ForMember(dest => dest.VestigingsAdresStraat, opt => opt.MapFrom(src => src.Straat))
                    .ForMember(dest => dest.VestigingsAdresHuisnummer, opt => opt.MapFrom(src => src.Huisnummer))
                    .ForMember(dest => dest.VestigingsAdresHuisnummerToevoeging, opt => opt.MapFrom(src => src.HuisnummerToevoeging))
                    .ForMember(dest => dest.VestigingsAdresPostcode, opt => opt.MapFrom(src => src.Postcode))
                    .ForMember(dest => dest.VestigingsAdresPlaats, opt => opt.MapFrom(src => src.Plaats)));
            _werkgeverGeregistreerdToWerkgeverMapper = config.CreateMapper();
        }
    }
}
