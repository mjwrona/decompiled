// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.WorkspaceComponent4
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using System;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class WorkspaceComponent4 : WorkspaceComponent3
  {
    public virtual Guid QueryPendingChangeSignature(string workspaceName, Guid ownerId)
    {
      this.PrepareStoredProcedure("prc_QueryPendingChangeSignature");
      this.BindString("@workspaceName", workspaceName, 64, true, SqlDbType.NVarChar);
      this.BindGuid("@ownerId", ownerId);
      SqlDataReader sqlDataReader = this.ExecuteReader();
      sqlDataReader.Read();
      return sqlDataReader.GetGuid(sqlDataReader.GetOrdinal("PendingChangeSignature"));
    }
  }
}
