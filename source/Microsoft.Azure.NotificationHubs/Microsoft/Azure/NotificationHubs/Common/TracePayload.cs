// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Common.TracePayload
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

namespace Microsoft.Azure.NotificationHubs.Common
{
  internal struct TracePayload
  {
    private string serializedException;
    private string eventSource;
    private string appDomainFriendlyName;
    private string extendedData;
    private string hostReference;

    public TracePayload(
      string serializedException,
      string eventSource,
      string appDomainFriendlyName,
      string extendedData,
      string hostReference)
    {
      this.serializedException = serializedException;
      this.eventSource = eventSource;
      this.appDomainFriendlyName = appDomainFriendlyName;
      this.extendedData = extendedData;
      this.hostReference = hostReference;
    }

    public string SerializedException => this.serializedException;

    public string EventSource => this.eventSource;

    public string AppDomainFriendlyName => this.appDomainFriendlyName;

    public string ExtendedData => this.extendedData;

    public string HostReference => this.hostReference;
  }
}
