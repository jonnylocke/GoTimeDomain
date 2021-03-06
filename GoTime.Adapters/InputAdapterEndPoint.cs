﻿using Evento;
using EventStore.ClientAPI;
using GoTime.Adapters.CommandBuilders;
using GoTime.Domain.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GoTime.Adapters
{
    public class InputAdapterEndPoint
    {
        private IDomainRepository _repo;
        private IEventStoreConnection _subscriberConnection;
        private string _streamName;
        private InputCommandHandler _commandHandler;
        private Dictionary<string, Func<object, IAggregate>> _eventHandlerMapping;
        private Dictionary<string, Func<string[], Command>> _deserialisers;
        private const string EventStoreGroup = "input-actions";

        public InputAdapterEndPoint(IDomainRepository repo, IEventStoreConnection subscriberConnection, string streamName, InputCommandHandler handler)
        {
            _repo = repo;
            _subscriberConnection = subscriberConnection;
            _streamName = streamName;
            _commandHandler = handler;
        }

        public bool Start()
        {
            _deserialisers = CreateDeserialisersMapping();
            _eventHandlerMapping = CreateEventHandlerMapping();
            _subscriberConnection.Reconnecting += Connection_Reconnecting;
            _subscriberConnection.Connected += Subscriber_Connected;

            SubscribeMe();

            return true;
        }

        private void SubscribeMe()
        {
            try
            {
                _subscriberConnection.ConnectToPersistentSubscriptionAsync(_streamName, EventStoreGroup, EventAppeared, SubscriptionDropped).Wait();
            }
            catch (Exception e)
            {
                //Console.WriteLine(e);
                throw;
            }
        }

        private Dictionary<string, Func<string[], Command>> CreateDeserialisersMapping()
        {
            return new Dictionary<string, Func<string[], Command>>
            {
                {"NewGame", ToCreateNewGameCommand},
                {"AcceptGame", ToStartGameCommand},
                {"RejectGame", ToRejectGameCommand},
                {"GoMove", ToMakeGoMoveCommand}
            };
        }

        private Dictionary<string, Func<object, IAggregate>> CreateEventHandlerMapping()
        {
            return new Dictionary<string, Func<object, IAggregate>>
            {
                {"NewGame", o => _commandHandler.Handle(o as NewGame)},
                {"AcceptGame", o => _commandHandler.Handle(o as AcceptNewGame)},
                {"RejectGame", o => _commandHandler.Handle(o as RejectNewGame)},
                {"GoMove", o => _commandHandler.Handle(o as AddStone)},
            };
        }

        private Command ToCreateNewGameCommand(string[] metadataAndBody)
        {
            return new NewGameBuilder(metadataAndBody[1]).Build();
        }

        private Command ToStartGameCommand(string[] metadataAndBody)
        {
            return new StartGameBuilder(metadataAndBody[1]).Build();
        }

        private Command ToRejectGameCommand(string[] metadataAndBody)
        {
            return new RejectGameBuilder(metadataAndBody[1]).Build();
        }

        private Command ToMakeGoMoveCommand(string[] metadataAndBody)
        {
            return new MovePieceBuilder(metadataAndBody[1]).Build();
        }

        private void Subscriber_Connected(object sender, ClientConnectionEventArgs e)
        {
            //Console.WriteLine("Connected");
        }

        private void Connection_Reconnecting(object sender, ClientReconnectingEventArgs e)
        {
            //Console.WriteLine("Re-connecting");
            SubscribeMe();
        }


        private void SubscriptionDropped(EventStorePersistentSubscriptionBase persistentSubscriptionBase, SubscriptionDropReason reason, Exception ex)
        {
            //Console.WriteLine("Dropped");
            //Console.WriteLine(reason);
            SubscribeMe();
        }

        private Task EventAppeared(EventStorePersistentSubscriptionBase persistentSubscriptionBase, ResolvedEvent resolvedEvent, int? arg3)
        {
            try
            {
                Process(resolvedEvent.Event.EventType, resolvedEvent.Event.Metadata, resolvedEvent.Event.Data);
            }
            catch (Exception ex)
            {
                persistentSubscriptionBase.Fail(resolvedEvent, PersistentSubscriptionNakEventAction.Park, ex.GetBaseException().Message);
            }

            return Task.CompletedTask;
        }

        private void Process(string eventType, byte[] metadata, byte[] data)
        {
            if (eventType.StartsWith("$"))
                return;

            if (!_eventHandlerMapping.ContainsKey(eventType))
                return;

            var command = _deserialisers[eventType](new[]
            {
                Encoding.UTF8.GetString(metadata),
                Encoding.UTF8.GetString(data)
            });

            if (command == null)
            {
                // log cmd not found
                return;
            }

            // Handle
            var aggregate = _eventHandlerMapping[eventType](command);

            // Save Domain Events
            _repo.Save(aggregate);
        }

        public void Stop()
        {
            _subscriberConnection.Close();
        }
    }
}
