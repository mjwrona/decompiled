// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ApplicationInsights.DataContracts.RequestTelemetry
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.ApplicationInsights.Channel;
using Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation;
using Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.External;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.VisualStudio.ApplicationInsights.DataContracts
{
  public sealed class RequestTelemetry : ITelemetry, ISupportProperties
  {
    internal const string TelemetryName = "Request";
    internal readonly string BaseType = typeof (RequestData).Name;
    internal readonly RequestData Data;
    private readonly TelemetryContext context;

    public RequestTelemetry()
    {
      this.Data = new RequestData();
      this.context = new TelemetryContext(this.Data.properties, (IDictionary<string, string>) new Dictionary<string, string>());
      this.Id = WeakConcurrentRandom.Instance.Next().ToString((IFormatProvider) CultureInfo.InvariantCulture);
      this.ResponseCode = "200";
      this.Success = true;
    }

    public RequestTelemetry(
      string name,
      DateTimeOffset timestamp,
      TimeSpan duration,
      string responseCode,
      bool success)
      : this()
    {
      this.Name = name;
      this.Timestamp = timestamp;
      this.Duration = duration;
      this.ResponseCode = responseCode;
      this.Success = success;
    }

    public DateTimeOffset Timestamp
    {
      get => this.ValidateDateTimeOffset(this.Data.startTime);
      set => this.Data.startTime = value.ToString("o", (IFormatProvider) CultureInfo.InvariantCulture);
    }

    public string Sequence { get; set; }

    public TelemetryContext Context => this.context;

    public string Id
    {
      get => this.Data.id;
      set => this.Data.id = value;
    }

    public string Name
    {
      get => this.Data.name;
      set => this.Data.name = value;
    }

    public string ResponseCode
    {
      get => this.Data.responseCode;
      set => this.Data.responseCode = value;
    }

    public bool Success
    {
      get => this.Data.success;
      set => this.Data.success = value;
    }

    public TimeSpan Duration
    {
      get => Utils.ValidateDuration(this.Data.duration);
      set => this.Data.duration = value.ToString();
    }

    public IDictionary<string, string> Properties => this.Data.properties;

    public Uri Url
    {
      get => this.Data.url.IsNullOrWhiteSpace() ? (Uri) null : new Uri(this.Data.url);
      set => this.Data.url = value == (Uri) null ? (string) null : value.ToString();
    }

    public IDictionary<string, double> Metrics => this.Data.measurements;

    public string HttpMethod
    {
      get => this.Data.httpMethod;
      set => this.Data.httpMethod = value;
    }

    void ITelemetry.Sanitize()
    {
      this.Name = this.Name.SanitizeName();
      this.Properties.SanitizeProperties();
      this.Metrics.SanitizeMeasurements();
      this.Url = this.Url.SanitizeUri();
      this.Id = this.Id.SanitizeName();
      this.Id = Utils.PopulateRequiredStringValue(this.Id, "id", typeof (RequestTelemetry).FullName);
      this.ResponseCode = Utils.PopulateRequiredStringValue(this.ResponseCode, "responseCode", typeof (RequestTelemetry).FullName);
    }

    private DateTimeOffset ValidateDateTimeOffset(string value)
    {
      DateTimeOffset result;
      return !DateTimeOffset.TryParse(value, (IFormatProvider) CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out result) ? DateTimeOffset.MinValue : result;
    }
  }
}
