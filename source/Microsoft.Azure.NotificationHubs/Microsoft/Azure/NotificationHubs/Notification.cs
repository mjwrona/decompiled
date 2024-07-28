// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Notification
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Azure.NotificationHubs
{
  public abstract class Notification
  {
    private Dictionary<string, string> headers;
    internal const string FormatHeaderName = "ServiceBusNotification-Format";
    internal string tag;

    protected Notification(IDictionary<string, string> additionalHeaders, string tag)
    {
      this.headers = additionalHeaders != null ? new Dictionary<string, string>(additionalHeaders, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) : new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.tag = tag;
      this.ContentType = "application/xml";
    }

    protected void AddOrUpdateHeader(string key, string value)
    {
      if (!this.Headers.ContainsKey(key))
        this.Headers.Add(key, value);
      else
        this.Headers[key] = value;
    }

    internal void ValidateAndPopulateHeaders()
    {
      this.AddOrUpdateHeader("ServiceBusNotification-Format", this.PlatformType);
      this.OnValidateAndPopulateHeaders();
    }

    protected abstract void OnValidateAndPopulateHeaders();

    protected abstract string PlatformType { get; }

    public Dictionary<string, string> Headers
    {
      get => this.headers;
      set => this.headers = new Dictionary<string, string>((IDictionary<string, string>) value, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    }

    public string Body { get; set; }

    public string ContentType { get; set; }

    [Obsolete("This property is obsolete.")]
    public string Tag
    {
      get => this.tag;
      set => this.tag = value;
    }
  }
}
