// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.TelemetryEvent
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Utilities.Internal;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Telemetry
{
  public class TelemetryEvent
  {
    private const int SchemaVersion = 5;
    private const string DataModelApiSource = "DataModelApi";
    private const TelemetrySeverity DefaultSeverity = TelemetrySeverity.Normal;
    internal const string ReservedPropertyPrefix = "Reserved.";
    private readonly string eventName;
    private readonly TelemetryPropertyBags.NotConcurrent<object> eventProperties = new TelemetryPropertyBags.NotConcurrent<object>();
    private readonly TelemetryPropertyBags.PrefixedNotConcurrent<object> reservedEventProperties = new TelemetryPropertyBags.PrefixedNotConcurrent<object>("Reserved.");
    private readonly HashSet<TelemetryPropertyBag> sharedPropertyBags = new HashSet<TelemetryPropertyBag>();
    private Guid eventId;
    private TelemetrySeverity severity;

    internal Dictionary<TelemetryEventCorrelation, string> CorrelatedWith { get; private set; }

    public bool IsOptOutFriendly { get; set; }

    public TelemetrySeverity Severity
    {
      get => this.severity;
      set
      {
        this.severity = value;
        this.ReservedProperties.AddPrefixed("Reserved.DataModel.Severity", (object) (int) this.severity);
      }
    }

    public DataModelEventType EventType => this.Correlation.EventType;

    public int EventSchemaVersion { get; }

    public string DataSource => "DataModelApi";

    public TelemetryEventCorrelation Correlation { get; private set; }

    public TelemetryEvent(string eventName)
      : this(eventName, TelemetrySeverity.Normal)
    {
    }

    public TelemetryEvent(string eventName, TelemetrySeverity severity)
      : this(eventName, severity, DataModelEventType.Trace)
    {
    }

    public void Correlate(params TelemetryEventCorrelation[] correlations)
    {
      if (correlations == null)
        return;
      foreach (TelemetryEventCorrelation correlation in correlations)
        this.CorrelateWithDescription(correlation, (string) null);
    }

    internal TelemetryEvent(
      string eventName,
      TelemetrySeverity severity,
      DataModelEventType eventType)
      : this(eventName, severity, new TelemetryEventCorrelation(Guid.NewGuid(), eventType))
    {
    }

    internal TelemetryEvent(
      string eventName,
      TelemetrySeverity severity,
      TelemetryEventCorrelation correlation)
    {
      eventName.RequiresArgumentNotNullAndNotWhiteSpace(nameof (eventName));
      correlation.RequireNotEmpty(nameof (correlation));
      TelemetryService.EnsureEtwProviderInitialized();
      this.eventName = eventName.ToLower(CultureInfo.InvariantCulture);
      this.Severity = severity;
      this.Correlation = correlation;
      this.EventSchemaVersion = 5;
      this.InitDataModelBasicProperties();
    }

    internal TelemetryEvent BuildChannelEvent(long processStartTime, string sessionId)
    {
      this.eventId = Guid.NewGuid();
      TelemetryEvent telemetryEvent = new TelemetryEvent(this.eventName, this.Severity, this.EventType);
      telemetryEvent.IsOptOutFriendly = this.IsOptOutFriendly;
      telemetryEvent.Correlation = this.Correlation;
      foreach (KeyValuePair<string, object> allProperty in this.GetAllProperties(DateTime.UtcNow.Ticks, processStartTime, sessionId))
        telemetryEvent.eventProperties[allProperty.Key] = allProperty.Value;
      telemetryEvent.ReservedProperties.Clear();
      telemetryEvent.ReservedProperties.AddRangePrefixed(this.reservedEventProperties.PrefixedEnumerable);
      return telemetryEvent;
    }

    internal TelemetryEvent CloneTelemetryEvent()
    {
      TelemetryEvent telemetryEvent = new TelemetryEvent(this.eventName, this.Severity, this.EventType);
      telemetryEvent.eventId = this.eventId;
      telemetryEvent.IsOptOutFriendly = this.IsOptOutFriendly;
      telemetryEvent.Correlation = this.Correlation;
      telemetryEvent.PostTimestamp = this.PostTimestamp;
      telemetryEvent.eventProperties.AddRange<string, object>((IDictionary<string, object>) this.eventProperties);
      telemetryEvent.ReservedProperties.Clear();
      telemetryEvent.ReservedProperties.AddRangePrefixed(this.reservedEventProperties.PrefixedEnumerable);
      return telemetryEvent;
    }

    protected void CorrelateWithDescription(
      TelemetryEventCorrelation correlation,
      string description)
    {
      if (description != null && description.Contains<char>(','))
        throw new ArgumentException("Comma is not allowed.", nameof (description));
      if (correlation.IsEmpty || this.Correlation.Equals((object) correlation))
        return;
      if (this.CorrelatedWith == null)
        this.CorrelatedWith = new Dictionary<TelemetryEventCorrelation, string>();
      this.CorrelatedWith[correlation] = description;
    }

    public string Name => this.eventName;

    public IDictionary<string, object> Properties => (IDictionary<string, object>) this.eventProperties;

    public bool HasProperties => this.eventProperties.HasProperties<object>();

    internal DateTimeOffset PostTimestamp { get; set; }

    public HashSet<TelemetryPropertyBag> SharedPropertyBags => this.sharedPropertyBags;

    internal TelemetryPropertyBags.PrefixedNotConcurrent<object> ReservedProperties => this.reservedEventProperties;

    internal bool HasReservedProperties => this.reservedEventProperties.HasProperties<object>();

    internal static bool IsPropertyNameReserved(string propertyName) => propertyName.StartsWith("Reserved.", StringComparison.Ordinal);

    protected virtual IEnumerable<KeyValuePair<string, object>> GetDefaultEventProperties(
      long eventTime,
      long processStartTime,
      string sessionId)
    {
      sessionId.RequiresArgumentNotNullAndNotWhiteSpace(nameof (sessionId));
      yield return new KeyValuePair<string, object>("Reserved.TimeSinceSessionStart", (object) Math.Round(new TimeSpan(eventTime - processStartTime).TotalMilliseconds));
      yield return new KeyValuePair<string, object>("Reserved.EventId", (object) this.eventId);
      yield return new KeyValuePair<string, object>("Reserved.SessionId", (object) sessionId);
    }

    private static KeyValuePair<string, object> AsReservedProperty(
      KeyValuePair<string, object> property)
    {
      return TelemetryEvent.IsPropertyNameReserved(property.Key) ? property : new KeyValuePair<string, object>("Reserved." + property.Key, property.Value);
    }

    private IEnumerable<KeyValuePair<string, object>> GetCorrelatedWithProperties()
    {
      if (this.CorrelatedWith != null && this.CorrelatedWith.Any<KeyValuePair<TelemetryEventCorrelation, string>>())
      {
        int index = 0;
        foreach (KeyValuePair<TelemetryEventCorrelation, string> keyValuePair in this.CorrelatedWith)
        {
          ++index;
          string[] values = new string[3];
          TelemetryEventCorrelation key = keyValuePair.Key;
          values[0] = key.Id.ToString("D");
          key = keyValuePair.Key;
          values[1] = DataModelEventTypeNames.GetName(key.EventType);
          values[2] = keyValuePair.Value ?? string.Empty;
          string str = ((IEnumerable<string>) values).Join(",");
          yield return new KeyValuePair<string, object>("Reserved." + "DataModel.Correlation." + index.ToString(), (object) str);
        }
      }
    }

    private IEnumerable<KeyValuePair<string, object>> GetAllProperties(
      long eventTime,
      long processStartTime,
      string sessionId)
    {
      IEnumerable<KeyValuePair<string, object>> second = this.GetDefaultEventProperties(eventTime, processStartTime, sessionId).Select<KeyValuePair<string, object>, KeyValuePair<string, object>>(new Func<KeyValuePair<string, object>, KeyValuePair<string, object>>(TelemetryEvent.AsReservedProperty)).Concat<KeyValuePair<string, object>>(this.ReservedProperties.PrefixedEnumerable).Concat<KeyValuePair<string, object>>(this.GetCorrelatedWithProperties());
      return this.eventProperties.Concat<KeyValuePair<string, object>>(this.sharedPropertyBags.Where<TelemetryPropertyBag>((Func<TelemetryPropertyBag, bool>) (bag => bag != null)).SelectMany<TelemetryPropertyBag, KeyValuePair<string, object>>((Func<TelemetryPropertyBag, IEnumerable<KeyValuePair<string, object>>>) (bag => (IEnumerable<KeyValuePair<string, object>>) bag))).Concat<KeyValuePair<string, object>>(second);
    }

    private void InitDataModelBasicProperties()
    {
      this.ReservedProperties.AddPrefixed("Reserved.DataModel.Source", (object) "DataModelApi");
      this.ReservedProperties.AddPrefixed("Reserved.DataModel.EntityType", (object) DataModelEventTypeNames.GetName(this.EventType));
      this.ReservedProperties.AddPrefixed("Reserved.DataModel.EntitySchemaVersion", (object) this.EventSchemaVersion);
      this.ReservedProperties.AddPrefixed("Reserved.DataModel.CorrelationId", (object) this.Correlation.Id);
    }

    public override string ToString() => this.Name ?? "";
  }
}
