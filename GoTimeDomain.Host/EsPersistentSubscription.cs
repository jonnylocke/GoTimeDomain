using System;
using System.Threading;
using System.Threading.Tasks;
using Evento.Repository;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;
using GoTime.Adapters;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GoTimeDomain.Host
{
    internal class EsPersistentSubscription : IHostedService 
    {
        private IEventStoreConnection _inputAdapterSubscriberConnection;
        private Settings _settings;
        private string StreamName;
        private InputAdapterEndPoint _endpoint;
        private string _subscriptionGroup => "gotime-actions";
        public ILogger<EsPersistentSubscription> _logger { get; }

        public EsPersistentSubscription (ILogger<EsPersistentSubscription> logger, Settings settings) 
        {
            _logger = logger;
            _settings = settings;
        }

        public Task StartAsync (CancellationToken cancellationToken) 
        {
            var connSettingsBuilder = ConnectionSettings.Create ()
                .SetHeartbeatInterval (TimeSpan.FromSeconds (3000))
                .SetHeartbeatTimeout (TimeSpan.FromSeconds (5000))
                .KeepReconnecting ()
                .KeepRetrying ();

            _inputAdapterSubscriberConnection = EventStoreConnection.Create (_settings.EsConnection, connSettingsBuilder, "InputAdapter-Subscriber");

            _inputAdapterSubscriberConnection.Reconnecting += InputAdapterSubscriberConnection_Reconnecting;
            _inputAdapterSubscriberConnection.Connected += InputAdapterSubscriberConnection_Connected;

            _inputAdapterSubscriberConnection.ConnectAsync ().Wait();

            CreateSubscription (_inputAdapterSubscriberConnection);

            var repo = new EventStoreDomainRepository ("domain", _inputAdapterSubscriberConnection);
            _endpoint = new InputAdapterEndPoint (repo, _inputAdapterSubscriberConnection, _settings.StreamName, new InputCommandHandler (repo));
            _endpoint.Start ();

            return Task.CompletedTask;
        }

        private void CreateSubscription (IEventStoreConnection connection) 
        {
            try 
            {
                connection.CreatePersistentSubscriptionAsync (_settings.StreamName, _subscriptionGroup,
                    PersistentSubscriptionSettings.Create ().StartFromBeginning ().DoNotResolveLinkTos(),
                    new UserCredentials (_settings.Username, _settings.Password)).Wait();
            } 
            catch (Exception) 
            {
                // subscription already exists
            }
        }

        private void InputAdapterSubscriberConnection_Connected (object sender, ClientConnectionEventArgs e) 
        {
            _logger.LogInformation("PersistentSubscriptionc Connected");
        }

        private void InputAdapterSubscriberConnection_Reconnecting (object sender, ClientReconnectingEventArgs e) 
        {
            _logger.LogInformation("PersistentSubscriptionc Reconnecting");
        }

        public Task StopAsync (CancellationToken cancellationToken) 
        {
            _logger.LogInformation("PersistentSubscriptionc Disconnected");

            return Task.FromResult ("Disconnected!");
        }
    }
}