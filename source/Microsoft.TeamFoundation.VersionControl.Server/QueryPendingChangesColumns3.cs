// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.QueryPendingChangesColumns3
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class QueryPendingChangesColumns3 : QueryPendingChangesColumns
  {
    protected SqlColumnBinder propertyId = new SqlColumnBinder("PropertyId");

    public QueryPendingChangesColumns3(IVssRequestContext requestContext)
      : base(requestContext)
    {
    }

    public QueryPendingChangesColumns3(
      IVssRequestContext requestContext,
      VersionControlSqlResourceComponent component)
      : base(requestContext, component)
    {
    }

    protected override PendingChange Bind()
    {
      PendingChange pendingChange = base.Bind();
      pendingChange.PropertyId = this.propertyId.GetInt32((IDataReader) this.Reader, -1);
      return pendingChange;
    }
  }
}
