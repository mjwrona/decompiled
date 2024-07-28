// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.BranchesViewItemRepositoryBranchBinder
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal sealed class BranchesViewItemRepositoryBranchBinder : 
    BuildObjectBinder<BranchesViewItemRepositoryBranch>
  {
    private SqlColumnBinder m_buildId = new SqlColumnBinder("BuildId");
    private SqlColumnBinder m_repositoryId = new SqlColumnBinder("RepositoryId");
    private SqlColumnBinder m_repositoryIdentifier = new SqlColumnBinder("RepositoryIdentifier");
    private SqlColumnBinder m_repositoryType = new SqlColumnBinder("RepositoryType");
    private SqlColumnBinder m_branchId = new SqlColumnBinder("BranchId");
    private SqlColumnBinder m_branchName = new SqlColumnBinder("BranchName");

    protected override BranchesViewItemRepositoryBranch Bind() => new BranchesViewItemRepositoryBranch(this.m_buildId.GetInt32((IDataReader) this.Reader), this.m_repositoryId.GetInt32((IDataReader) this.Reader, 0), this.m_repositoryIdentifier.GetString((IDataReader) this.Reader, true), this.m_repositoryType.GetString((IDataReader) this.Reader, true), this.m_branchId.GetInt32((IDataReader) this.Reader, 0), this.m_branchName.GetString((IDataReader) this.Reader, true));
  }
}
