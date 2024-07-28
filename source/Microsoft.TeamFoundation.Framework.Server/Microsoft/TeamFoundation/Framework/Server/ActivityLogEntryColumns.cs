// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ActivityLogEntryColumns
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class ActivityLogEntryColumns : ObjectBinder<ActivityLogEntry>
  {
    private SqlColumnBinder commandIdColumn = new SqlColumnBinder("CommandId");
    private SqlColumnBinder applicationColumn = new SqlColumnBinder("Application");
    private SqlColumnBinder commandColumn = new SqlColumnBinder("Command");
    private SqlColumnBinder statusColumn = new SqlColumnBinder("Status");
    private SqlColumnBinder startTimeColumn = new SqlColumnBinder("StartTime");
    private SqlColumnBinder executionTimeColumn = new SqlColumnBinder("ExecutionTime");
    private SqlColumnBinder identityNameColumn = new SqlColumnBinder("IdentityName");
    private SqlColumnBinder ipAddressColumn = new SqlColumnBinder("IPAddress");
    private SqlColumnBinder uniqueIdentifierColumn = new SqlColumnBinder("UniqueIdentifier");
    private SqlColumnBinder userAgentColumn = new SqlColumnBinder("UserAgent");
    private SqlColumnBinder commandIdentifierColumn = new SqlColumnBinder("CommandIdentifier");
    private SqlColumnBinder executionCountColumn = new SqlColumnBinder("ExecutionCount");
    private SqlColumnBinder correlationIdColumn = new SqlColumnBinder("TempCorrelationId");
    private IVssRequestContext m_requestContext;
    private static readonly string s_area = "CommandComponent";
    private static readonly string s_layer = "ObjectBinder";

    public ActivityLogEntryColumns(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(70325, ActivityLogEntryColumns.s_area, ActivityLogEntryColumns.s_layer, "ActivityLogEntryColumns.ctor");
      this.m_requestContext = requestContext;
      this.m_requestContext.TraceLeave(70330, ActivityLogEntryColumns.s_area, ActivityLogEntryColumns.s_layer, "ActivityLogEntryColumns.ctor");
    }

    protected override ActivityLogEntry Bind()
    {
      this.m_requestContext.TraceEnter(70335, ActivityLogEntryColumns.s_area, ActivityLogEntryColumns.s_layer, nameof (Bind));
      ActivityLogEntry activityLogEntry = new ActivityLogEntry();
      activityLogEntry.CommandId = this.commandIdColumn.GetInt32((IDataReader) this.Reader);
      activityLogEntry.Application = this.applicationColumn.GetString((IDataReader) this.Reader, false);
      activityLogEntry.Command = this.commandColumn.GetString((IDataReader) this.Reader, false);
      activityLogEntry.Status = this.statusColumn.GetInt32((IDataReader) this.Reader);
      activityLogEntry.StartTime = this.startTimeColumn.GetDateTime((IDataReader) this.Reader);
      activityLogEntry.ExecutionTime = this.executionTimeColumn.GetInt64((IDataReader) this.Reader);
      activityLogEntry.IdentityName = this.identityNameColumn.GetString((IDataReader) this.Reader, false);
      activityLogEntry.IpAddress = this.ipAddressColumn.GetString((IDataReader) this.Reader, false);
      activityLogEntry.UniqueIdentifier = this.uniqueIdentifierColumn.GetGuid((IDataReader) this.Reader);
      activityLogEntry.UserAgent = this.userAgentColumn.GetString((IDataReader) this.Reader, true);
      activityLogEntry.CommandIdentifier = this.commandIdentifierColumn.GetString((IDataReader) this.Reader, true);
      activityLogEntry.ExecutionCount = this.executionCountColumn.GetInt32((IDataReader) this.Reader);
      this.m_requestContext.TraceLeave(70340, ActivityLogEntryColumns.s_area, ActivityLogEntryColumns.s_layer, nameof (Bind));
      return activityLogEntry;
    }
  }
}
