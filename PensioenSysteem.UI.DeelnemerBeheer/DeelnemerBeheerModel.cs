namespace PensioenSysteem.UI.DeelnemerBeheer
{
    using Entities;
    using System;
    using System.Data.Entity;
    using System.Linq;

    public class DeelnemerBeheerModel : DbContext
    {
        // Your context has been configured to use a 'DeelnemerBeheerModel' connection string from your application's 
        // configuration file (App.config or Web.config). By default, this connection string targets the 
        // 'PensioenSysteem.UI.DeelnemerBeheer.DeelnemerBeheerModel' database on your LocalDb instance. 
        // 
        // If you wish to target a different database and/or database provider, modify the 'DeelnemerBeheerModel' 
        // connection string in the application configuration file.
        public DeelnemerBeheerModel() : base("name=DeelnemerBeheer")
        {
        }

        // Add a DbSet for each entity type that you want to include in your model. For more information 
        // on configuring and using a Code First model, see http://go.microsoft.com/fwlink/?LinkId=390109.

        public virtual DbSet<Deelnemer> Deelnemers { get; set; }
    }
}