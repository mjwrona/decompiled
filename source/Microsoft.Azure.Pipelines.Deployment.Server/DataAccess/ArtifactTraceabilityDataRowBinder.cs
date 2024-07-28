// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.DataAccess.ArtifactTraceabilityDataRowBinder
// Assembly: Microsoft.Azure.Pipelines.Deployment.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B08F1AB-5B33-41F6-908E-9A985FD0544C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Deployment.Server.dll

using Microsoft.Azure.Pipelines.Deployment.Utilities;
using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.Azure.Pipelines.Deployment.DataAccess
{
  internal class ArtifactTraceabilityDataRowBinder : ObjectBinder<ArtifactTraceabilityDataRow>
  {
    private SqlColumnBinder m_ArtifactType = new SqlColumnBinder("ArtifactType");
    private SqlColumnBinder m_ArtifactAlias = new SqlColumnBinder("ArtifactAlias");
    private SqlColumnBinder m_ArtifactName = new SqlColumnBinder("ArtifactName");
    private SqlColumnBinder m_ResourceVersionId = new SqlColumnBinder("VersionId");
    private SqlColumnBinder m_ResourceVersionName = new SqlColumnBinder("VersionName");
    private SqlColumnBinder m_ResourceVersionProperties = new SqlColumnBinder("ArtifactVersionProperties");
    private SqlColumnBinder m_RepoType = new SqlColumnBinder("RepoType");
    private SqlColumnBinder m_RepoId = new SqlColumnBinder("RepoId");
    private SqlColumnBinder m_RepoName = new SqlColumnBinder("RepoName");
    private SqlColumnBinder m_Branch = new SqlColumnBinder("Branch");
    private SqlColumnBinder m_CommitId = new SqlColumnBinder("CommitId");
    private SqlColumnBinder m_RepoProperties = new SqlColumnBinder("RepoProperties");
    private SqlColumnBinder m_UniqueResourceIdentifier = new SqlColumnBinder("UniqueResourceIdentifier");
    private SqlColumnBinder m_ConnectionId = new SqlColumnBinder("ConnectionId");

    protected override ArtifactTraceabilityDataRow Bind() => new ArtifactTraceabilityDataRow()
    {
      ArtifactType = this.m_ArtifactType.GetString((IDataReader) this.Reader, false),
      ArtifactAlias = this.m_ArtifactAlias.GetString((IDataReader) this.Reader, false),
      ArtifactName = this.m_ArtifactName.GetString((IDataReader) this.Reader, false),
      ArtifactVersionId = this.m_ResourceVersionId.GetString((IDataReader) this.Reader, false),
      ArtifactVersionName = this.m_ResourceVersionName.GetString((IDataReader) this.Reader, true),
      ArtifactVersionProperties = (IDictionary<string, string>) ArtifactTraceabilityHelper.GetPropertyDictionary(this.m_ResourceVersionProperties.GetString((IDataReader) this.Reader, true)),
      RepositoryType = this.m_RepoType.GetByte((IDataReader) this.Reader, (byte) 0),
      RepositoryId = this.m_RepoId.GetString((IDataReader) this.Reader, true),
      RepositoryName = this.m_RepoName.GetString((IDataReader) this.Reader, true),
      Branch = this.m_Branch.GetString((IDataReader) this.Reader, true),
      CommitId = this.m_CommitId.GetString((IDataReader) this.Reader, true),
      RepositoryProperties = (IDictionary<string, string>) ArtifactTraceabilityHelper.GetPropertyDictionary(this.m_RepoProperties.GetString((IDataReader) this.Reader, true)),
      UniqueResourceIdentifier = this.m_UniqueResourceIdentifier.GetString((IDataReader) this.Reader, false),
      EndpointId = this.m_ConnectionId.GetNullableGuid((IDataReader) this.Reader)
    };
  }
}
