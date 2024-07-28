// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.PipelineOidcFederationClaims
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server
{
  internal class PipelineOidcFederationClaims : IOidcFederationClaims
  {
    public string Subject { get; set; }

    public string Audience { get; set; }

    public Guid ProjectId { get; set; }

    public Guid OrganizationId { get; set; }

    public int PipelineDefinitionId { get; set; }

    public string RepositoryId { get; set; }

    public string RepositoryUri { get; set; }

    public string RepositoryVersion { get; set; }

    public string RepositoryRef { get; set; }

    public IDictionary<string, string> JsonClaims => (IDictionary<string, string>) new Dictionary<string, string>()
    {
      ["sub"] = this.Subject,
      ["aud"] = this.Audience,
      ["org_id"] = this.OrganizationId.ToString("D"),
      ["prj_id"] = this.ProjectId.ToString("D"),
      ["def_id"] = this.PipelineDefinitionId.ToString(),
      ["rpo_id"] = this.RepositoryId,
      ["rpo_uri"] = this.RepositoryUri,
      ["rpo_ver"] = this.RepositoryVersion,
      ["rpo_ref"] = this.RepositoryRef
    };
  }
}
