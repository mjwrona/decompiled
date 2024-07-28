// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.QueryPendingChangesFailureColumns
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class QueryPendingChangesFailureColumns : VersionControlObjectBinder<Failure>
  {
    public SqlColumnBinder errorCode = new SqlColumnBinder("ErrorCode");
    public SqlColumnBinder localItem = new SqlColumnBinder("LocalItem");
    private string m_sqlProcName;

    public QueryPendingChangesFailureColumns(string sqlProcName) => this.m_sqlProcName = sqlProcName;

    public QueryPendingChangesFailureColumns(
      string sqlProcName,
      VersionControlSqlResourceComponent component)
      : base(component)
    {
      this.m_sqlProcName = sqlProcName;
    }

    protected override Failure Bind()
    {
      string localItem = this.localItem.GetLocalItem(this.Reader, false);
      if (this.errorCode.GetInt32((IDataReader) this.Reader) == 500030)
        return new Failure((Exception) new LocalVersionNotFoundException(localItem));
      throw new UnexpectedDatabaseResultException(this.m_sqlProcName);
    }
  }
}
