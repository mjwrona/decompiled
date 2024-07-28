// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.Events.BuildCompletedEvent
// Assembly: Microsoft.TeamFoundation.Build2.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0683407D-7C61-4505-8CA6-86AD7E3B6BCA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build2.WebApi.dll

using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build.WebApi.Events
{
  [DataContract]
  [ServiceEventObject]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class BuildCompletedEvent : BuildUpdatedEvent
  {
    public BuildCompletedEvent(Microsoft.TeamFoundation.Build.WebApi.Build build)
      : base(build)
    {
    }

    [DataMember]
    public List<Change> Changes { get; set; }

    [DataMember]
    public List<AssociatedWorkItem> WorkItems { get; set; }

    [DataMember]
    public List<TimelineRecord> TimelineRecords { get; set; }

    [DataMember]
    public AggregatedResultsAnalysis TestResults { get; set; }

    [DataMember]
    public PullRequest PullRequest { get; set; }
  }
}
