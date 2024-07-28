// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.DataAccess.ArtifactTraceabilityDataRow
// Assembly: Microsoft.Azure.Pipelines.Deployment.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B08F1AB-5B33-41F6-908E-9A985FD0544C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Deployment.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Pipelines.Deployment.DataAccess
{
  internal class ArtifactTraceabilityDataRow
  {
    public string ArtifactType;
    public string ArtifactAlias;
    public string ArtifactName;
    public string ArtifactVersionId;
    public string ArtifactVersionName;
    public IDictionary<string, string> ArtifactVersionProperties;
    public byte RepositoryType;
    public string RepositoryId;
    public string RepositoryName;
    public string Branch;
    public string CommitId;
    public IDictionary<string, string> RepositoryProperties;
    public string UniqueResourceIdentifier;
    public Guid? EndpointId;
  }
}
