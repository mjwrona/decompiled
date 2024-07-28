// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.ExternalIntegration.HostIdMapping.TracingPoints
// Assembly: Microsoft.TeamFoundation.ExternalIntegration.HostIdMapping, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D6C9F672-10C9-4D5D-90D5-E07E56C1D4C0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.ExternalIntegration.HostIdMapping.dll

namespace Microsoft.TeamFoundation.ExternalIntegration.HostIdMapping
{
  public static class TracingPoints
  {
    public static string ExternalServiceEventUrlHostRoutingArea = "ExternalServiceEventUrlHostRouting";

    public static class EventsRouting
    {
      public static readonly int RequestInvalid = 15287551;
      public static readonly int RequestRouted = 15287552;
      public static readonly int Mapping = 15287553;
      public static readonly int RoutingException = 15287554;
      public static readonly int EventReceived = 15287555;
      public static readonly int HostRoutingNotRequired = 15287556;
    }

    public static class EventProperties
    {
      public static readonly string Properties = nameof (Properties);
      public static readonly string EventType = nameof (EventType);
      public static readonly string EventId = nameof (EventId);
      public static readonly string Payload = nameof (Payload);
      public static readonly string RoutedTo = nameof (RoutedTo);
      public static readonly string RequestUri = nameof (RequestUri);
    }

    public enum EventType
    {
      EventReceived,
      EventRouted,
      NoActionableEvents,
      NoMatchingCollection,
      JobCompleted,
      HandleEvent,
    }
  }
}
