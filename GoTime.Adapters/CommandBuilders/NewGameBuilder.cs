using GoTime.Adapters.Dtos;
using GoTime.Domain.Commands;
using Newtonsoft.Json;
using System;

namespace GoTime.Adapters.CommandBuilders
{
    internal class NewGameBuilder
    {
        private NewGame _newCommand;

        private NewGameBuilder(StreamMetadata metadata, NewGame cmd)
        {
            _newCommand = cmd;
            _newCommand.SetCorrelationId(metadata.Metadata["$correlationId"]);
            _newCommand.SetNewGameId();
        }

        public NewGameBuilder(string metadataAndBody)
            : this(JsonConvert.DeserializeObject<StreamMetadata>(metadataAndBody),
                  JsonConvert.DeserializeObject<NewGame>(metadataAndBody))
        { }

        internal NewGame Build()
        {
            return _newCommand;
        }
    }
}