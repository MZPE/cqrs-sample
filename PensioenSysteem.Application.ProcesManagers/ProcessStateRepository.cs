using LiteDB;
using PensioenSysteem.Application.ProcesManagers.Model;
using System;
using System.IO;

namespace PensioenSysteem.Application.ProcesManagers
{
    internal class ProcessStateRepository
    {
        private const string _databaseFolder = @"D:\PensioenSysteem\Databases\ProcessManagers\";
        private const string _databaseFile = _databaseFolder + @"ProcessManagers.db";

        public ProcessStateRepository()
        {
            Directory.CreateDirectory(_databaseFolder);
        }

        public void RegistreerProcessStart(ProcessState state)
        {
            using (var db = new LiteDatabase(_databaseFile))
            {
                var col = db.GetCollection<ProcessState>("processtate");
                col.Insert(state);
                db.Commit();
            }
        }

        public ProcessState RaadpleegProcessState(Guid correlationId)
        {
            using (var db = new LiteDatabase(_databaseFile))
            {
                var col = db.GetCollection<ProcessState>("processtate");
                return col.FindById(correlationId);
            }
        }

        public void UpdateProcessState(ProcessState state)
        {
            using (var db = new LiteDatabase(_databaseFile))
            {
                var col = db.GetCollection<ProcessState>("processtate");
                ProcessState storedState = col.FindById(state.Id);
                storedState.DeelnemerNummer = state.DeelnemerNummer;
                storedState.WerkgeverNummer = state.WerkgeverNummer;
                storedState.Status = state.Status;
                storedState.Foutmelding = state.Foutmelding;
                col.Update(storedState);
                db.Commit();
            }
        }
    }
}
