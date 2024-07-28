// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.DeploymentQueryParameters
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi
{
  [DataContract]
  public class DeploymentQueryParameters
  {
    [DataMember]
    public IList<DefinitionEnvironmentReference> Environments { get; set; }

    [DataMember]
    public DeploymentStatus DeploymentStatus { get; set; }

    [DataMember]
    public DeploymentOperationStatus OperationStatus { get; set; }

    [DataMember]
    public string ArtifactTypeId { get; set; }

    [DataMember]
    public string ArtifactSourceId { get; set; }

    [DataMember]
    public string SourceBranch { get; set; }

    [DataMember]
    public IList<string> ArtifactVersions { get; set; }

    [DataMember]
    public DeploymentExpands Expands { get; set; }

    [DataMember]
    public int DeploymentsPerEnvironment { get; set; }

    [DataMember]
    public DeploymentsQueryType QueryType { get; set; }

    [DataMember]
    public bool IsDeleted { get; set; }

    [DataMember]
    [Obsolete]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool LatestDeploymentsOnly { get; set; }

    [DataMember]
    [Obsolete]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public ReleaseQueryOrder QueryOrder { get; set; }

    [DataMember]
    [Obsolete]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public int MaxDeploymentsPerEnvironment { get; set; }

    [DataMember]
    [Obsolete]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public DateTime? MinModifiedTime { get; set; }

    [DataMember]
    [Obsolete]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public DateTime? MaxModifiedTime { get; set; }
  }
}
