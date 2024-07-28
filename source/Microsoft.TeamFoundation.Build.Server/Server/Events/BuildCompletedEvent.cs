// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.Events.BuildCompletedEvent
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Build.Server.Events
{
  [XmlType(Namespace = "http://schemas.microsoft.com/TeamFoundation/2010/Build")]
  public sealed class BuildCompletedEvent
  {
    private List<QueuedBuild> m_requests;

    public BuildCompletedEvent() => this.m_requests = new List<QueuedBuild>();

    public BuildDetail Build { get; set; }

    public BuildController Controller { get; set; }

    public BuildDefinition Definition { get; set; }

    public List<QueuedBuild> Requests => this.m_requests;

    public string Subscriber { get; set; }

    [XmlElement(ElementName = "TeamProjectCollectionUrl", DataType = "anyURI")]
    public string TeamProjectCollectionUrl { get; set; }

    public string TimeZone { get; set; }

    public string TimeZoneOffset { get; set; }

    public string Title { get; set; }

    [XmlElement(ElementName = "Uri", DataType = "anyURI")]
    public string Uri { get; set; }

    [XmlElement(ElementName = "WebAccessUri", DataType = "anyURI")]
    public string WebAccessUri { get; set; }

    public int Duration { get; set; }

    public DateTime FinishTimeLocal { get; set; }

    public DateTime StartTimeLocal { get; set; }

    public static class Roles
    {
      public static string LastChangedBy = "lastChangedBy";
      public static string RequestedBy = "requestedBy";
      public static string RequestedFor = "requestedFor";
    }
  }
}
