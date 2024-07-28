// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.BuildProviders.PipelineInfo
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.BuildProviders
{
  public class PipelineInfo
  {
    public int Id { get; set; }

    public int DefinitionId { get; set; }

    public string PipelineNumber { get; set; }

    public string DefinitionName { get; set; }

    public Uri Uri { get; set; }

    public string SourceBranch { get; set; }

    public string SourceCommit { get; set; }

    public string RepositoryType { get; set; }

    public IdentityRef RequestedFor { get; set; }

    public BuildReason Reason { get; set; }
  }
}
