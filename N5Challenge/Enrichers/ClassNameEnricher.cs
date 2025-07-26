using N5Challenge.Constants;
using Serilog.Core;
using Serilog.Events;

namespace N5Challenge.Enrichers;

public class ClassNameEnricher : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        if (logEvent.Properties.TryGetValue(SerilogConstants.SourceContext, out var sourceContextProperty))
        {
            var originalValue = ((ScalarValue)sourceContextProperty).Value?.ToString();
            var newValue = originalValue?.Split('`').First().Split('.').Last();
            logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty(SerilogConstants.SourceContext, newValue));
        }
    }
}