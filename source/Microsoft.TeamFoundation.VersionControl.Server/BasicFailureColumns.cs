// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.BasicFailureColumns
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class BasicFailureColumns : VersionControlObjectBinder<Failure>
  {
    public SqlColumnBinder errorCode = new SqlColumnBinder("ErrorCode");
    public SqlColumnBinder serverItem = new SqlColumnBinder("TargetServerItem");
    private string m_sqlProcName;

    public BasicFailureColumns(string sqlProcName) => this.m_sqlProcName = sqlProcName;

    public BasicFailureColumns(string sqlProcName, VersionControlSqlResourceComponent component)
      : base(component)
    {
      this.m_sqlProcName = sqlProcName;
    }

    protected virtual string BindServerItem() => this.serverItem.GetServerItem(this.Reader, false);

    protected override Failure Bind()
    {
      string serverItem = this.BindServerItem();
      switch (this.errorCode.GetInt32((IDataReader) this.Reader))
      {
        case 500021:
          return new Failure((Exception) new ItemExistsException(serverItem));
        case 500026:
          return new Failure((Exception) new PendingParentDeleteException(serverItem));
        case 500037:
        case 500041:
          return new Failure((Exception) new ItemNotCheckedOutException("NoPendingChanges", serverItem));
        case 500039:
          return new Failure((Exception) new ChangeAlreadyPendingException(serverItem));
        case 500052:
          return new Failure((Exception) new CannotChangeRootFolderException());
        case 500058:
          return new Failure((Exception) new ShelvedChangeNotFoundException(serverItem));
        default:
          throw new UnexpectedDatabaseResultException(this.m_sqlProcName);
      }
    }
  }
}
