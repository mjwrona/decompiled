// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.BranchReferenceBinder
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal sealed class BranchReferenceBinder : BuildObjectBinder<BranchReference>
  {
    private SqlColumnBinder m_id = new SqlColumnBinder("BranchId");
    private SqlColumnBinder m_name = new SqlColumnBinder("BranchName");
    private SqlColumnBinder m_repoId = new SqlColumnBinder("RepositoryId");

    public BranchReferenceBinder(
      IVssRequestContext requestContext,
      BuildSqlComponentBase resourceComponent)
      : base(requestContext, resourceComponent)
    {
    }

    protected override BranchReference Bind() => new BranchReference()
    {
      Id = this.m_id.GetInt32((IDataReader) this.Reader),
      Name = this.m_name.GetString((IDataReader) this.Reader, false),
      RepositoryId = this.m_repoId.GetInt32((IDataReader) this.Reader)
    };
  }
}
