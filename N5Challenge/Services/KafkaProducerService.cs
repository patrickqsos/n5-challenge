using System.Text.Json;
using Confluent.Kafka;
using N5Challenge.Dtos;
using N5Challenge.Services.Interfaces;
using Serilog;
using ILogger = Serilog.ILogger;

namespace N5Challenge.Services;

public class KafkaProducerService : IKafkaProducerService
{
    private readonly ILogger _logger = Log.ForContext<KafkaProducerService>();
    private readonly IProducer<Null, string> _producer;

    public KafkaProducerService(IConfiguration configuration)
    {
        var configProducer = new ProducerConfig
        {
            BootstrapServers = configuration.GetSection("Kafka").GetValue<string>("BootstrapServer"),
        };

        _producer = new ProducerBuilder<Null, string>(configProducer).Build();
    }

    public async Task<DeliveryResult<Null, string>> Send(KafkaMessageDto kafkaMessage, string topic = "operations" )
    {
        _logger.Debug("Kafka message: {@kafkaMessage} ", kafkaMessage);

        var msg = new Message<Null, string>
        {
            Value = JsonSerializer.Serialize(kafkaMessage)
        };
        var dr = await _producer.ProduceAsync(topic, msg);
        _logger.Information("Kafka message sent to {kafkaTopic}/{kafkaPartition}/{kafkaOffset}", topic, dr.Partition.Value, dr.Offset.Value);
        return dr;
    }
}