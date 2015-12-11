using Microsoft.Practices.Unity;
using PensioenSysteem.Domain.Core;
using PensioenSysteem.Infrastructure;
using System.Web.Http;
using Unity.WebApi;

namespace PensioenSysteem.Application.Arbeidsverhouding
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();

            // register types
            container.RegisterType<IEventPublisher, RabbitMQEventPublisher>();
            container.RegisterType<IEventStore, FileEventStore>();
            container.RegisterType<
                IAggregateRepository<PensioenSysteem.Domain.Arbeidsverhouding.Arbeidsverhouding>,
                EventSourcedAggregateRepository<PensioenSysteem.Domain.Arbeidsverhouding.Arbeidsverhouding>>();

            // set Unity as dependency-resolver
            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}