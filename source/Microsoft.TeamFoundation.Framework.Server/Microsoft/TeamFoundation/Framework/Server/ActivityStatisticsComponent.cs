// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ActivityStatisticsComponent
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Data;
using System.Text;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class ActivityStatisticsComponent : TeamFoundationSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[1]
    {
      (IComponentCreator) new ComponentCreator<ActivityStatisticsComponent>(1)
    }, "ActivityStatistic");
    private static readonly string s_area = nameof (ActivityStatisticsComponent);

    protected override string TraceArea => ActivityStatisticsComponent.s_area;

    public virtual ResultCollection QueryActivityStatistics(Guid activityId)
    {
      try
      {
        this.TraceEnter(70210, nameof (QueryActivityStatistics));
        this.PrepareStoredProcedure("prc_QueryActivityStatistics");
        this.BindPartitionId();
        this.BindGuid("@activityId", activityId);
        ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
        resultCollection.AddBinder<ActivityStatistic>((ObjectBinder<ActivityStatistic>) this.GetActivityStatisticsColumns());
        return resultCollection;
      }
      catch (Exception ex)
      {
        this.TraceException(70215, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(70220, nameof (QueryActivityStatistics));
      }
    }

    protected virtual ActivityStatisticsColumns GetActivityStatisticsColumns() => new ActivityStatisticsColumns(this.RequestContext);

    protected virtual void BindPartitionId()
    {
    }

    protected virtual void BindPartitionIdText(StringBuilder sqlStmt) => sqlStmt.Append(" WHERE 1 = 1 ");
  }
}
