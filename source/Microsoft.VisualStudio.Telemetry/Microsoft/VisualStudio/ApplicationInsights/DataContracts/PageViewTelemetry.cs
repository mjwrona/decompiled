// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ApplicationInsights.DataContracts.PageViewTelemetry
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.ApplicationInsights.Channel;
using Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation;
using Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.External;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.ApplicationInsights.DataContracts
{
  public sealed class PageViewTelemetry : ITelemetry, ISupportProperties
  {
    internal const string TelemetryName = "PageView";
    internal readonly string BaseType = typeof (PageViewData).Name;
    internal readonly PageViewData Data;
    private readonly TelemetryContext context;

    public PageViewTelemetry()
    {
      this.Data = new PageViewData();
      this.context = new TelemetryContext(this.Data.properties, (IDictionary<string, string>) new Dictionary<string, string>());
    }

    public PageViewTelemetry(string pageName)
      : this()
    {
      this.Name = pageName;
    }

    public DateTimeOffset Timestamp { get; set; }

    public string Sequence { get; set; }

    public TelemetryContext Context => this.context;

    public string Name
    {
      get => this.Data.name;
      set => this.Data.name = value;
    }

    public Uri Url
    {
      get => this.Data.url.IsNullOrWhiteSpace() ? (Uri) null : new Uri(this.Data.url);
      set
      {
        if (value == (Uri) null)
          this.Data.url = (string) null;
        else
          this.Data.url = value.ToString();
      }
    }

    public TimeSpan Duration
    {
      get => Utils.ValidateDuration(this.Data.duration);
      set => this.Data.duration = value.ToString();
    }

    public IDictionary<string, double> Metrics => this.Data.measurements;

    public IDictionary<string, string> Properties => this.Data.properties;

    void ITelemetry.Sanitize()
    {
      this.Name = this.Name.SanitizeName();
      this.Name = Utils.PopulateRequiredStringValue(this.Name, "name", typeof (PageViewTelemetry).FullName);
      this.Properties.SanitizeProperties();
      this.Metrics.SanitizeMeasurements();
      this.Url = this.Url.SanitizeUri();
    }
  }
}
