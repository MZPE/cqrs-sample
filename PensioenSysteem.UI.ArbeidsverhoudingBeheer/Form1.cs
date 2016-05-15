using AutoMapper;
using Newtonsoft.Json;
using PensioenSysteem.Domain.Messages.Arbeidsverhouding.Events;
using PensioenSysteem.Infrastructure;
using PensioenSysteem.UI.ArbeidsverhoudingBeheer.Model;
using Raven.Client;
using Raven.Client.Embedded;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace PensioenSysteem.UI.ArbeidsverhoudingBeheer
{
    public partial class Form1 : Form
    {
        private RabbitMQDomainEventHandler _eventHandler;
        private ArbeidsverhoudingRepository _repo;
        private IMapper _arbeidsverhoudingGeregistreerdToArbeidsverhoudingMapper;

        public Form1()
        {
            InitializeComponent();
            _repo = new ArbeidsverhoudingRepository();
            InitializeMappers();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            UpdateList();

            _eventHandler = new RabbitMQDomainEventHandler("127.0.0.1", "cqrs_user", "SeeQueErEs", "PensioenSysteem.Arbeidsverhouding", HandleEvent);
            _eventHandler.Start();
        }

        private bool HandleEvent(string eventType, string eventData)
        {
            bool handled = false;
            switch (eventType)
            {
                case "ArbeidsverhoudingGeregistreerd":
                    ArbeidsverhoudingGeregistreerd arbeidsverhoudingGeregistreerd = JsonConvert.DeserializeObject<ArbeidsverhoudingGeregistreerd>(eventData);
                    handled = HandleEvent(arbeidsverhoudingGeregistreerd);
                    break;
            }

            // refresh datagrid
            this.Invoke((MethodInvoker)delegate
            {
                UpdateList();
            });

            return handled;
        }

        private bool HandleEvent(ArbeidsverhoudingGeregistreerd e)
        {
            Arbeidsverhouding arbeidsverhouding =
                _arbeidsverhoudingGeregistreerdToArbeidsverhoudingMapper.Map<Arbeidsverhouding>(e);
            _repo.RegistreerArbeidsverhouding(arbeidsverhouding);
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

        private void arbeidsverhoudingBindingSource_ListChanged(object sender, ListChangedEventArgs e)
        {
            recordCountStatusLabel.Text = "Aantal items : "  + this.arbeidsverhoudingBindingSource.List.Count;
        }

        private void UpdateList()
        {
            this.arbeidsverhoudingBindingSource.SuspendBinding();
            this.arbeidsverhoudingBindingSource.List.Clear();
            foreach (Arbeidsverhouding arbeidsverhouding in _repo.RaadpleegArbeidsverhoudingen())
            {
                this.arbeidsverhoudingBindingSource.List.Add(arbeidsverhouding);
            }
            this.arbeidsverhoudingBindingSource.ResumeBinding();
        }

        private void InitializeMappers()
        {
            var config = new MapperConfiguration(cfg =>
                cfg.CreateMap<ArbeidsverhoudingGeregistreerd, Arbeidsverhouding>());
            _arbeidsverhoudingGeregistreerdToArbeidsverhoudingMapper = config.CreateMapper();
        }
    }
}
