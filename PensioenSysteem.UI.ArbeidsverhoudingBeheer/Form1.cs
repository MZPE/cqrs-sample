using AutoMapper;
using Dapper;
using Newtonsoft.Json;
using PensioenSysteem.Domain.Messages.Arbeidsverhouding.Events;
using PensioenSysteem.Infrastructure;
using PensioenSysteem.UI.ArbeidsverhoudingBeheer.Model;
using Raven.Client;
using Raven.Client.Embedded;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;

namespace PensioenSysteem.UI.ArbeidsverhoudingBeheer
{
    public partial class Form1 : Form
    {
        private RabbitMQDomainEventHandler _eventHandler;
        private EmbeddableDocumentStore _documentStore;
        private IMapper _arbeidsverhoudingGeregistreerdToArbeidsverhoudingMapper;

        public Form1()
        {
            InitializeComponent();
            InitializeDatastore();
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
            using (IDocumentSession session = _documentStore.OpenSession())
            {
                Arbeidsverhouding arbeidsverhouding = 
                    _arbeidsverhoudingGeregistreerdToArbeidsverhoudingMapper.Map<Arbeidsverhouding>(e);
                session.Store(arbeidsverhouding);
                session.SaveChanges();
            }
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

            using (IDocumentSession session = _documentStore.OpenSession())
            {
                List<Arbeidsverhouding> arbeidsverhoudingen = session
                    .Query<Arbeidsverhouding>()
                    .Customize(x => x.WaitForNonStaleResultsAsOfNow()) // wait for any pending index udpates
                    .ToList();
                foreach (Arbeidsverhouding arbeidsverhouding in arbeidsverhoudingen)
                {
                    this.arbeidsverhoudingBindingSource.List.Add(arbeidsverhouding);
                }
            }

            this.arbeidsverhoudingBindingSource.ResumeBinding();
        }

        private void InitializeDatastore()
        {
            _documentStore = new EmbeddableDocumentStore
            {
                DefaultDatabase = "ArbeidsverhoudingBeheer"
            };
            _documentStore.Initialize();
        }

        private void InitializeMappers()
        {
            var config = new MapperConfiguration(cfg =>
                cfg.CreateMap<ArbeidsverhoudingGeregistreerd, Arbeidsverhouding>());
            _arbeidsverhoudingGeregistreerdToArbeidsverhoudingMapper = config.CreateMapper();
        }
    }
}
