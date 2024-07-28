// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Common.Diagnostics.TraceRecord
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;
using System.Xml;

namespace Microsoft.Azure.NotificationHubs.Common.Diagnostics
{
  [Serializable]
  internal class TraceRecord
  {
    protected const string EventIdBase = "http://schemas.microsoft.com/2006/08/ServiceModel/";
    protected const string NamespaceSuffix = "TraceRecord";

    internal virtual string EventId => TraceRecord.BuildEventId("Empty");

    internal virtual void WriteTo(XmlWriter writer)
    {
    }

    protected static string BuildEventId(string eventId) => "http://schemas.microsoft.com/2006/08/ServiceModel/" + eventId + nameof (TraceRecord);
  }
}
