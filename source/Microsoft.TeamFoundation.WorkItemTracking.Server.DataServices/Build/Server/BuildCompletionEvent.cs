// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.BuildCompletionEvent
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 929F0284-16B2-4277-9F4A-B615689A77D1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices.dll

using System;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Build.Server
{
  [Serializable]
  public sealed class BuildCompletionEvent
  {
    public string BuildUri { get; set; }

    [XmlElement(DataType = "anyURI")]
    public string TeamFoundationServerUrl { get; set; }

    public string TeamProject { get; set; }

    public string Id { get; set; }

    [XmlElement(DataType = "anyURI")]
    public string Url { get; set; }

    public string Type { get; set; }

    public string Title { get; set; }

    public string CompletionStatus { get; set; }

    public string Subscriber { get; set; }

    public string Configuration { get; set; }

    public string RequestedBy { get; set; }

    public string TimeZone { get; set; }

    public string TimeZoneOffset { get; set; }

    public string BuildStartTime { get; set; }

    public string BuildCompleteTime { get; set; }

    public string BuildMachine { get; set; }
  }
}
