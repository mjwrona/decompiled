// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.BuildStatusChangeEvent
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.VisualStudio.Services.Notifications;
using System;

namespace Microsoft.TeamFoundation.Build.Server
{
  [NotificationEventBindings(EventSerializerType.Xml, "ms.vss-build.build-status-changed-event")]
  [Serializable]
  public sealed class BuildStatusChangeEvent
  {
    public string BuildUri { get; set; }

    public string TeamFoundationServerUrl { get; set; }

    public string TeamProject { get; set; }

    public string Title { get; set; }

    public string Subscriber { get; set; }

    public string Id { get; set; }

    public string Url { get; set; }

    public string TimeZone { get; set; }

    public string TimeZoneOffset { get; set; }

    public string ChangedTime { get; set; }

    public Change StatusChange { get; set; }

    public string ChangedBy { get; set; }

    public static class Roles
    {
      public static string LastChangedBy = "lastChangedBy";
    }
  }
}
