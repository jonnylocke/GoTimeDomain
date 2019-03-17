using GoTime.Adapters.Dtos;
using GoTime.Domain.Commands;
using Newtonsoft.Json;

namespace GoTime.Adapters.CommandBuilders
{
    internal class StartGameBuilder
    {
        private AcceptNewGame _newCommand;

        private StartGameBuilder(StreamMetadata metadata, AcceptNewGame cmd)
        {
            _newCommand = cmd;
            _newCommand.CorrelationId = metadata.Metadata["$correlationId"];
        }

        public StartGameBuilder(string metadataAndBody)
            : this(JsonConvert.DeserializeObject<StreamMetadata>(metadataAndBody),
                  JsonConvert.DeserializeObject<AcceptNewGame>(metadataAndBody))
        { }

        internal AcceptNewGame Build()
        {
            return _newCommand;
        }
    }
}