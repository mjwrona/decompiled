// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.DataAccess.ArtifactVersionRepoInfoBinder
// Assembly: Microsoft.Azure.Pipelines.Deployment.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B08F1AB-5B33-41F6-908E-9A985FD0544C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Deployment.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.Azure.Pipelines.Deployment.DataAccess
{
  internal class ArtifactVersionRepoInfoBinder : ObjectBinder<ArtifactVersionRepoInfo>
  {
    private SqlColumnBinder m_SubArtifactVersionId = new SqlColumnBinder("SubArtifactVersionId");
    private SqlColumnBinder m_repoType = new SqlColumnBinder("RepoType");
    private SqlColumnBinder m_repoId = new SqlColumnBinder("RepoId");
    private SqlColumnBinder m_repoName = new SqlColumnBinder("RepoName");
    private SqlColumnBinder m_branch = new SqlColumnBinder("Branch");
    private SqlColumnBinder m_commitId = new SqlColumnBinder("CommitId");
    private SqlColumnBinder m_properties = new SqlColumnBinder("Properties");

    protected override ArtifactVersionRepoInfo Bind() => new ArtifactVersionRepoInfo()
    {
      SubArtifactVersionId = this.m_SubArtifactVersionId.GetInt32((IDataReader) this.Reader),
      RepoType = this.m_repoType.GetByte((IDataReader) this.Reader),
      RepoId = this.m_repoId.GetString((IDataReader) this.Reader, true),
      RepoName = this.m_repoName.GetString((IDataReader) this.Reader, true),
      Branch = this.m_branch.GetString((IDataReader) this.Reader, true),
      CommitId = this.m_commitId.GetString((IDataReader) this.Reader, true),
      Properties = this.m_properties.GetString((IDataReader) this.Reader, true)
    };
  }
}
