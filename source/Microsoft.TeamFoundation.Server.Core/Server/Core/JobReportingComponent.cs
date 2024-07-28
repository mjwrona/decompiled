// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.JobReportingComponent
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;

namespace Microsoft.TeamFoundation.Server.Core
{
  internal class JobReportingComponent : TeamFoundationSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[2]
    {
      (IComponentCreator) new ComponentCreator<JobReportingComponent>(1),
      (IComponentCreator) new ComponentCreator<JobReportingComponent2>(2)
    }, "JobReporting");

    public virtual ResultCollection QueryRunTime(
      DateTime startTime,
      DateTime endTime,
      int maxRowsToReturn)
    {
      this.PrepareStoredProcedure("prc_JobReportQueryRunTime");
      this.BindDateTime("@startTime", startTime.ToUniversalTime());
      this.BindDateTime("@endTime", endTime.ToUniversalTime());
      this.BindInt("@maxNumberOfRows", maxRowsToReturn);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<TeamFoundationJobReportingHistoryQueueTime>((ObjectBinder<TeamFoundationJobReportingHistoryQueueTime>) new JobReportingJobCountsAndRunTimeColumns());
      return resultCollection;
    }

    public virtual ResultCollection QueryJobResultCount(DateTime startTime, DateTime endTime)
    {
      this.PrepareStoredProcedure("prc_JobReportJobResultCount");
      this.BindDateTime("@startTime", startTime.ToUniversalTime());
      this.BindDateTime("@endTime", endTime.ToUniversalTime());
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<TeamFoundationJobReportingResultTypeCount>((ObjectBinder<TeamFoundationJobReportingResultTypeCount>) new JobReportingResultTypeCountColumns());
      return resultCollection;
    }

    public virtual ResultCollection QueryHistory(
      DateTime startTime,
      DateTime endTime,
      int maxNumberOfRows,
      Guid? jobId,
      int? result)
    {
      this.PrepareStoredProcedure("prc_JobReportQueryHistory");
      if (!jobId.HasValue)
        jobId = new Guid?(Guid.Empty);
      if (!result.HasValue)
        result = new int?(int.MinValue);
      this.BindDateTime("@startTime", startTime.ToUniversalTime());
      this.BindDateTime("@endTime", endTime.ToUniversalTime());
      this.BindInt("@maxNumberOfRows", maxNumberOfRows);
      this.BindNullableGuid("@jobId", jobId.Value);
      this.BindNullableInt("@resultValue", result.Value, int.MinValue);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<TeamFoundationJobReportingHistory>((ObjectBinder<TeamFoundationJobReportingHistory>) new JobReportingQueryHistoryColumns(this.Version));
      return resultCollection;
    }

    public virtual ResultCollection QueryHistory(long historyId)
    {
      this.PrepareStoredProcedure("prc_JobReportingQueryHistoryById");
      this.BindLong("@historyId", historyId);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<TeamFoundationJobReportingHistory>((ObjectBinder<TeamFoundationJobReportingHistory>) new JobReportingQueryHistoryColumns(this.Version));
      return resultCollection;
    }

    public virtual ResultCollection QueryHistoryRunQueueTime(
      DateTime startTime,
      DateTime endTime,
      int maxNumberOfRows,
      Guid? jobId)
    {
      this.PrepareStoredProcedure("prc_JobReportQueryHistoryRunQueueTime");
      if (!jobId.HasValue)
        jobId = new Guid?(Guid.Empty);
      this.BindDateTime("@startTime", startTime.ToUniversalTime());
      this.BindDateTime("@endTime", endTime.ToUniversalTime());
      this.BindInt("@maxNumberOfRows", maxNumberOfRows);
      this.BindNullableGuid("@jobId", jobId.Value);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<TeamFoundationJobReportingJobCountsAndRunTime>((ObjectBinder<TeamFoundationJobReportingJobCountsAndRunTime>) new JobReportingHistoryQueueTimeColumns());
      return resultCollection;
    }

    public virtual ResultCollection QueryQueuePositions(int position, int maxRowsReturned)
    {
      this.PrepareStoredProcedure("prc_JobReportQueryQueuePositions");
      this.BindInt("@position", position);
      this.BindInt("@maxRows", maxRowsReturned);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<TeamFoundationJobReportingQueuePositions>((ObjectBinder<TeamFoundationJobReportingQueuePositions>) new JobReportingQueuePositionsColumns(this.Version));
      return resultCollection;
    }

    public virtual ResultCollection QueryQueuePositionCount()
    {
      this.PrepareStoredProcedure("prc_JobReportQueryQueuePositionCount");
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<TeamFoundationJobReportingQueuePositionCount>((ObjectBinder<TeamFoundationJobReportingQueuePositionCount>) new JobReportingQueuePositionsCountColumns());
      return resultCollection;
    }

    public virtual ResultCollection QueryResultsOverTime(
      DateTime startTime,
      DateTime endTime,
      int maxRowsReturned)
    {
      this.PrepareStoredProcedure("prc_JobReportQueryResultsOverTime");
      this.BindDateTime("@startTime", startTime.ToUniversalTime());
      this.BindDateTime("@endTime", endTime.ToUniversalTime());
      this.BindInt("@maxRows", maxRowsReturned);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<TeamFoundationJobReportingResultsOverTime>((ObjectBinder<TeamFoundationJobReportingResultsOverTime>) new JobReportingResultsOverTimeColumns());
      return resultCollection;
    }
  }
}
