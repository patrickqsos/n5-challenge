using System.Text.Json.Serialization;
using N5Challenge.Enums;

namespace N5Challenge.Dtos;

public record KafkaMessageDto(Guid Id, [property: JsonConverter(typeof(JsonStringEnumConverter))] KafkaOperationEnum Operation);
    