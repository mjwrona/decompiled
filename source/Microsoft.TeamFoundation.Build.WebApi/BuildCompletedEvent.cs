// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.BuildCompletedEvent
// Assembly: Microsoft.TeamFoundation.Build.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 97B7A530-2EF1-42C1-8A2A-360BCF05C7EF
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.WebApi.dll

using Microsoft.TeamFoundation.Core.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build.WebApi
{
  [DataContract]
  public class BuildCompletedEvent
  {
    public BuildCompletedEvent()
    {
      this.BuildDefinition = new ShallowReference();
      this.Project = new TeamProjectReference();
      this.ProjectCollection = new TeamProjectCollectionReference();
    }

    [DataMember]
    public int BuildId { get; set; }

    [DataMember]
    public ShallowReference BuildDefinition { get; set; }

    [DataMember]
    public BuildStatus BuildStatus { get; set; }

    [DataMember]
    public Guid DeploymentServiceHostInstanceId { get; set; }

    [DataMember]
    public TeamProjectReference Project { get; set; }

    [DataMember]
    public TeamProjectCollectionReference ProjectCollection { get; set; }

    [DataMember]
    public string BuildNumber { get; set; }
  }
}
