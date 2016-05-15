using Newtonsoft.Json;
using PensioenSysteem.Infrastructure;
using System;
using System.ComponentModel;
using System.Windows.Forms;
using PensioenSysteem.UI.DeelnemerBeheer.Model;
using AutoMapper;
using PensioenSysteem.Domain.Messages.Deelnemer.Events;

namespace PensioenSysteem.UI.DeelnemerBeheer
{
    public partial class Form1 : Form
    {
        private RabbitMQDomainEventHandler _eventHandler;
        private DeelnemerRepository _repo;
        private IMapper _deelnemerGeregistreerdToDeelnemerMapper;
        private IMapper _deelnemerVerhuisdToVerhuizingMapper;

        public Form1()
        {
            InitializeComponent();
            _repo = new DeelnemerRepository();
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
            Deelnemer deelnemer = _deelnemerGeregistreerdToDeelnemerMapper.Map<Deelnemer>(e);
            _repo.RegistreerDeelnemer(deelnemer);
            return true;
        }

        private bool HandleEvent(DeelnemerVerhuisd e)
        {
            Verhuizing verhuizing = _deelnemerVerhuisdToVerhuizingMapper.Map<Verhuizing>(e);
            _repo.RegistreerDeelnemerVerhuizing(verhuizing);
            return true;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_eventHandler != null)
            {
                _eventHandler.Stop();
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
            this.deelnemerBindingSource.DataSource = _repo.RaadpleegDeelnemers();
            this.deelnemerBindingSource.ResumeBinding();
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

            config = new MapperConfiguration(cfg => cfg.CreateMap<DeelnemerVerhuisd, Verhuizing>());
            _deelnemerVerhuisdToVerhuizingMapper = config.CreateMapper();
        }
    }
}
