using System.Text.Json;
using Confluent.Kafka;
using N5Challenge.Dtos;
using N5Challenge.Services.Interfaces;
using Serilog;
using Serilog.Events;
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
            MessageTimeoutMs = 2000
        };

        _producer = new ProducerBuilder<Null, string>(configProducer)
            .SetLogHandler((_, e) =>
            {
                LogEventLevel logLevel;
                switch (e.Level)
                {
                    case SyslogLevel.Emergency:
                    case SyslogLevel.Critical:
                        logLevel = LogEventLevel.Fatal;
                        break;
                    case SyslogLevel.Error:
                        logLevel = LogEventLevel.Error;
                        break;
                    case SyslogLevel.Alert:
                    case SyslogLevel.Warning:
                    case SyslogLevel.Notice:
                        logLevel = LogEventLevel.Warning;
                        break;
                    case SyslogLevel.Info:
                        logLevel = LogEventLevel.Information;
                        break;
                    case SyslogLevel.Debug:
                        logLevel = LogEventLevel.Debug;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                _logger.Write(logLevel, "{facility} - {message}", e.Facility, e.Message);
            }).Build();
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