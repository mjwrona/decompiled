// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.WebApi.AuditTrail
// Assembly: Microsoft.VisualStudio.Services.Search.WebApi.Legacy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5D4CB2D3-3C08-46C7-B9C5-51E638F57F9E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.WebApi.Legacy.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.WebApi
{
  [DataContract]
  public sealed class AuditTrail
  {
    [DataMember(Name = "MaxEvents")]
    public const int MaxEvents = 5;
    [DataMember(Name = "MaxMessageLength")]
    public const int MaxMessageLength = 1024;
    [DataMember(Name = "m_events")]
    private Queue<AuditTrail.Event> m_events;

    public void Record(string message)
    {
      if (message.Length > 1024)
        message = message.Substring(0, 1024) + "...";
      if (this.m_events == null)
        this.m_events = new Queue<AuditTrail.Event>();
      if (this.m_events.Count == 5)
      {
        this.m_events.TrimExcess();
        this.m_events.Dequeue();
      }
      this.m_events.Enqueue(new AuditTrail.Event(message));
    }

    public IEnumerable<AuditTrail.Event> Events => (IEnumerable<AuditTrail.Event>) this.m_events ?? (IEnumerable<AuditTrail.Event>) new Queue<AuditTrail.Event>();

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      foreach (AuditTrail.Event @event in this.Events)
        stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[{0}]", (object) @event));
      return stringBuilder.ToString();
    }

    [DataContract]
    public class Event
    {
      public Event(string message)
      {
        this.Timestamp = DateTime.UtcNow;
        this.Message = message;
      }

      [DataMember(Name = "Timestamp")]
      public DateTime Timestamp { get; set; }

      [DataMember(Name = "Message")]
      public string Message { get; set; }

      public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}, {1}", (object) this.Timestamp.ToString("o", (IFormatProvider) CultureInfo.InvariantCulture), (object) this.Message);
    }
  }
}
