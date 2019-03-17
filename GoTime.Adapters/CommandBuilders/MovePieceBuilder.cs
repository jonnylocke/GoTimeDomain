using GoTime.Adapters.Dtos;
using GoTime.Domain.Commands;
using Newtonsoft.Json;

namespace GoTime.Adapters.CommandBuilders
{
    internal class MovePieceBuilder
    {
        private AddStone _newCommand;

        private MovePieceBuilder(StreamMetadata metadata, AddStone cmd)
        {
            _newCommand = cmd;
            _newCommand.CorrelationId = metadata.Metadata["$correlationId"];
        }

        public MovePieceBuilder(string metadataAndBody)
            : this(JsonConvert.DeserializeObject<StreamMetadata>(metadataAndBody),
                  JsonConvert.DeserializeObject<AddStone>(metadataAndBody))
        { }

        internal AddStone Build()
        {
            return _newCommand;
        }
    }
}