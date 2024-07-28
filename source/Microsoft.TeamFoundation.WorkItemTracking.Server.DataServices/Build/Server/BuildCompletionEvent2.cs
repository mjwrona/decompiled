// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.BuildCompletionEvent2
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 929F0284-16B2-4277-9F4A-B615689A77D1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices.dll

using System;

namespace Microsoft.TeamFoundation.Build.Server
{
  [Serializable]
  public sealed class BuildCompletionEvent2
  {
    public string BuildUri { get; set; }

    public string TeamFoundationServerUrl { get; set; }

    public string Url { get; set; }

    public string Title { get; set; }

    public string TeamProject { get; set; }

    public string AgentPath { get; set; }

    public string DefinitionPath { get; set; }

    public string BuildNumber { get; set; }

    public string StartTime { get; set; }

    public string FinishTime { get; set; }

    public string ConfigurationFolderUri { get; set; }

    public string SourceGetVersion { get; set; }

    public string Quality { get; set; }

    public string Status { get; set; }

    public string StatusCode { get; set; }

    public string CompilationStatus { get; set; }

    public string TestStatus { get; set; }

    public string LogLocation { get; set; }

    public string DropLocation { get; set; }

    public string RequestedFor { get; set; }

    public string RequestedBy { get; set; }

    public string LastChangedOn { get; set; }

    public string LastChangedBy { get; set; }

    public bool KeepForever { get; set; }

    public string TimeZone { get; set; }

    public string TimeZoneOffset { get; set; }

    public string Subscriber { get; set; }
  }
}
