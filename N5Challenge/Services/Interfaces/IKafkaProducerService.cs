using Confluent.Kafka;
using N5Challenge.Dtos;

namespace N5Challenge.Services.Interfaces;

public interface IKafkaProducerService
{
    Task<DeliveryResult<Null, string>> Send(KafkaMessageDto kafkaMessage, string topic = "operations" );
}