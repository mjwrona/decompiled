// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.RepositoryReferenceBinder
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal sealed class RepositoryReferenceBinder : BuildObjectBinder<RepositoryReference>
  {
    private SqlColumnBinder m_id = new SqlColumnBinder("RepositoryId");
    private SqlColumnBinder m_repoId = new SqlColumnBinder("RepositoryIdentifier");
    private SqlColumnBinder m_repoString = new SqlColumnBinder("RepositoryString");
    private SqlColumnBinder m_repoType = new SqlColumnBinder("RepositoryType");

    public RepositoryReferenceBinder(
      IVssRequestContext requestContext,
      BuildSqlComponentBase resourceComponent)
      : base(requestContext, resourceComponent)
    {
    }

    protected override RepositoryReference Bind() => new RepositoryReference()
    {
      Id = this.m_id.GetInt32((IDataReader) this.Reader),
      Identifier = this.m_repoId.GetString((IDataReader) this.Reader, false),
      RepositoryString = this.m_repoString.GetString((IDataReader) this.Reader, false),
      Type = this.m_repoType.GetString((IDataReader) this.Reader, false)
    };
  }
}
