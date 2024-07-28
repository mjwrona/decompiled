// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.Models.ArtifactTraceabilityData
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.Constants;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.Models
{
  public class ArtifactTraceabilityData
  {
    private IDictionary<string, string> _resourceVersionProperties;
    private IDictionary<string, string> _resourceProperties;

    public ArtifactCategory ArtifactCategory { get; set; }

    public string ArtifactType { get; set; }

    public string UniqueResourceIdentifier { get; set; }

    public ArtifactTraceabilityResourceInfo Resource { get; set; }

    public string ArtifactName { get; set; }

    public string ArtifactAlias { get; set; }

    public bool IsSelfArtifact { get; set; }

    public bool DownloadAllArtifacts { get; set; }

    public string ArtifactVersionId { get; set; }

    public string ArtifactVersionName { get; set; }

    public EndpointReferenceData ArtifactConnectionData { get; set; }

    public IDictionary<string, string> ArtifactVersionProperties
    {
      get
      {
        if (this._resourceVersionProperties == null)
          this._resourceVersionProperties = (IDictionary<string, string>) new Dictionary<string, string>();
        return this._resourceVersionProperties;
      }
    }

    public IDictionary<string, string> ResourceProperties
    {
      get
      {
        if (this._resourceProperties == null)
          this._resourceProperties = (IDictionary<string, string>) new Dictionary<string, string>();
        return this._resourceProperties;
      }
      set => this._resourceProperties = value;
    }

    public Guid ProjectId { get; set; }

    public int PipelineDefinitionId { get; set; } = ArtifactTraceabilityConstants.IncorrectId;

    public int PipelineRunId { get; set; } = ArtifactTraceabilityConstants.IncorrectId;

    public string JobId { get; set; }

    public string JobName { get; set; }

    public int ResourcePipelineDefinitionId { get; set; } = ArtifactTraceabilityConstants.IncorrectId;

    public override string ToString() => string.Format("ArtifactCategory : {0}, ArtifactType : {1}, UniqueResourceIdentifier: {2}, Resource : {3}, ArtifactName : {4}, ArtifactAlias : {5}, IsSelfArtifact : {6}, DownloadAllArtifacts : {7}, ArtifactVersionId : {8}, ArtifactVersionName : {9}, ArtifactConnectionData : {10}, ProjectId : {11}, PipelineDefinitionId : {12}, PipelineRunId: {13}, JobId : {14}, JobName : {15}, ResourcePipelineDefinitionId : {16}", (object) this.ArtifactCategory, (object) this.ArtifactType, (object) this.UniqueResourceIdentifier, (object) this.Resource?.ToString(), (object) this.ArtifactName, (object) this.ArtifactAlias, (object) this.IsSelfArtifact, (object) this.DownloadAllArtifacts, (object) this.ArtifactVersionId, (object) this.ArtifactVersionName, (object) this.ArtifactConnectionData?.ToString(), (object) this.ProjectId.ToString(), (object) this.PipelineDefinitionId, (object) this.PipelineRunId, (object) this.JobId, (object) this.JobName, (object) this.ResourcePipelineDefinitionId);
  }
}
