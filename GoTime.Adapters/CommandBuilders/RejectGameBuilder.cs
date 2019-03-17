using GoTime.Adapters.Dtos;
using GoTime.Domain.Commands;
using Newtonsoft.Json;

namespace GoTime.Adapters.CommandBuilders
{
    internal class RejectGameBuilder
    {
        private RejectNewGame _newCommand;

        private RejectGameBuilder(StreamMetadata metadata, RejectNewGame cmd)
        {
            _newCommand = cmd;
            _newCommand.CorrelationId = metadata.Metadata["$correlationId"];
        }

        public RejectGameBuilder(string metadataAndBody)
            : this(JsonConvert.DeserializeObject<StreamMetadata>(metadataAndBody),
                  JsonConvert.DeserializeObject<RejectNewGame>(metadataAndBody))
        { }

        internal RejectNewGame Build()
        {
            return _newCommand;
        }
    }
}