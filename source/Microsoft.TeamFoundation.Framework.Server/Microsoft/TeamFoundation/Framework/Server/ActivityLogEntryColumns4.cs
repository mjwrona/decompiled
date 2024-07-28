// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ActivityLogEntryColumns4
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class ActivityLogEntryColumns4 : ActivityLogEntryColumns3
  {
    private SqlColumnBinder responseCodeColumn = new SqlColumnBinder("ResponseCode");

    public ActivityLogEntryColumns4(IVssRequestContext requestContext)
      : base(requestContext)
    {
    }

    protected override ActivityLogEntry Bind()
    {
      ActivityLogEntry activityLogEntry = base.Bind();
      activityLogEntry.ResponseCode = this.responseCodeColumn.GetInt32((IDataReader) this.Reader, 0);
      return activityLogEntry;
    }
  }
}
