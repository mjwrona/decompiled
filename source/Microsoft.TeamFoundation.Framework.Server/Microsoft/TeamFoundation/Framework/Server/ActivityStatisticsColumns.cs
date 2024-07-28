// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ActivityStatisticsColumns
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class ActivityStatisticsColumns : ObjectBinder<ActivityStatistic>
  {
    private SqlColumnBinder activityIdColumn = new SqlColumnBinder("ActivityId");
    private SqlColumnBinder statisticIdColumn = new SqlColumnBinder("StatisticId");
    private SqlColumnBinder processIdColumn = new SqlColumnBinder("ProcessId");
    private SqlColumnBinder threadIdColumn = new SqlColumnBinder("ThreadId");
    private SqlColumnBinder relativeTimestampColumn = new SqlColumnBinder("RelativeTimestamp");
    private SqlColumnBinder eventNameColumn = new SqlColumnBinder("EventName");
    private SqlColumnBinder activityMessageColumn = new SqlColumnBinder("ActivityMessage");
    private SqlColumnBinder dataIdColumn = new SqlColumnBinder("DataId");
    private IVssRequestContext m_requestContext;
    private static readonly string s_area = "ActivityStatisticsComponent";
    private static readonly string s_layer = "ObjectBinder";

    public ActivityStatisticsColumns(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(70325, ActivityStatisticsColumns.s_area, ActivityStatisticsColumns.s_layer, "ActivityStatisticsColumns.ctor");
      this.m_requestContext = requestContext;
      this.m_requestContext.TraceLeave(70330, ActivityStatisticsColumns.s_area, ActivityStatisticsColumns.s_layer, "ActivityStatisticsColumns.ctor");
    }

    protected override ActivityStatistic Bind()
    {
      this.m_requestContext.TraceEnter(70335, ActivityStatisticsColumns.s_area, ActivityStatisticsColumns.s_layer, nameof (Bind));
      ActivityStatistic activityStatistic = new ActivityStatistic();
      activityStatistic.StatisticId = this.statisticIdColumn.GetInt32((IDataReader) this.Reader);
      activityStatistic.ProcessId = this.processIdColumn.GetInt32((IDataReader) this.Reader);
      activityStatistic.ThreadId = this.threadIdColumn.GetInt32((IDataReader) this.Reader);
      activityStatistic.ActivityId = this.activityIdColumn.GetGuid((IDataReader) this.Reader);
      activityStatistic.RelativeTimestamp = this.relativeTimestampColumn.GetDouble((IDataReader) this.Reader);
      activityStatistic.EventName = this.eventNameColumn.GetString((IDataReader) this.Reader, false);
      activityStatistic.ActivityMessage = this.activityMessageColumn.GetString((IDataReader) this.Reader, true);
      activityStatistic.DataId = this.dataIdColumn.GetGuid((IDataReader) this.Reader, true);
      this.m_requestContext.TraceLeave(70340, ActivityStatisticsColumns.s_area, ActivityStatisticsColumns.s_layer, nameof (Bind));
      return activityStatistic;
    }
  }
}
