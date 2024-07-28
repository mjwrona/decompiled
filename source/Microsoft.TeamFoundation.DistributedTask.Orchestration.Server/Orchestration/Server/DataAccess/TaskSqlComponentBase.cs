// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess.TaskSqlComponentBase
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.CompilerServices;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess
{
  [SupportedSqlAccessIntent(SqlAccessIntent.ReadWrite | SqlAccessIntent.ReadOnly, null)]
  internal abstract class TaskSqlComponentBase : TeamFoundationSqlResourceComponent
  {
    private static Lazy<IDictionary<int, SqlExceptionFactory>> s_translatedExceptions = new Lazy<IDictionary<int, SqlExceptionFactory>>(new Func<IDictionary<int, SqlExceptionFactory>>(TaskSqlComponentBase.CreateExceptionMap));

    protected TaskSqlComponentBase() => this.ContainerErrorCode = 50000;

    protected override string TraceArea => "DistributedTask";

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => TaskSqlComponentBase.s_translatedExceptions.Value;

    protected override SqlParameter BindDateTime2(string parameterName, DateTime parameterValue)
    {
      SqlParameter sqlParameter = this.Command.Parameters.Add(parameterName, SqlDbType.DateTime2);
      sqlParameter.Size = 7;
      sqlParameter.Value = (object) parameterValue;
      return sqlParameter;
    }

    protected override SqlParameter BindNullableDateTime2(
      string parameterName,
      DateTime? parameterValue)
    {
      SqlParameter sqlParameter = this.Command.Parameters.Add(parameterName, SqlDbType.DateTime2);
      sqlParameter.Size = 7;
      if (parameterValue.HasValue)
        sqlParameter.Value = (object) parameterValue.Value;
      else
        sqlParameter.Value = (object) DBNull.Value;
      return sqlParameter;
    }

    protected SqlParameter BindNullableGuid(
      string parameterName,
      Guid? value,
      bool convertEmptyToNull = false)
    {
      if (value.HasValue)
      {
        if (convertEmptyToNull)
        {
          Guid? nullable = value;
          Guid empty = Guid.Empty;
          if ((nullable.HasValue ? (nullable.HasValue ? (nullable.GetValueOrDefault() == empty ? 1 : 0) : 1) : 0) != 0)
            goto label_3;
        }
        return this.BindGuid(parameterName, value.Value);
      }
label_3:
      return this.BindNullValue(parameterName, SqlDbType.UniqueIdentifier);
    }

    private static Exception CreateException(
      IVssRequestContext requestContext,
      int errorNumber,
      SqlException sqlException,
      SqlError sqlError)
    {
      Exception exception = (Exception) null;
      switch (errorNumber)
      {
        case 907010:
          exception = (Exception) new TaskOrchestrationPlanNotFoundException(TaskResources.PlanNotFound((object) sqlError.ExtractString("planId")));
          break;
        case 907011:
          exception = (Exception) new TimelineNotFoundException(TaskResources.TimelineNotFound((object) sqlError.ExtractString("planId"), (object) sqlError.ExtractString("timelineId")));
          break;
        case 907020:
          exception = (Exception) new TimelineExistsException(TaskResources.TimelineExists((object) sqlError.ExtractString("planId"), (object) sqlError.ExtractString("timelineId")));
          break;
        case 907021:
          exception = (Exception) new TaskOrchestrationJobNotFoundException(TaskResources.JobNotFound((object) sqlError.ExtractString("jobId"), (object) sqlError.ExtractString("planId")));
          break;
        case 907022:
          exception = (Exception) new TaskOrchestrationPlanGroupNotFoundException(TaskResources.PlanGroupNotFound((object) sqlError.ExtractString("planGroup")));
          break;
        case 907023:
          exception = (Exception) new TimelineRecordUpdateException(TaskResources.TimelineRecordAttemptInvalid((object) sqlError.ExtractString("planId"), (object) sqlError.ExtractString("identifier"), (object) sqlError.ExtractString("currentAttempt"), (object) sqlError.ExtractString("newAttempt")));
          break;
        case 907025:
          exception = (Exception) new TimelineRecordUpdateException(TaskResources.TimelineUpdateFailure((object) sqlError.ExtractString("recordId")));
          break;
        case 907051:
          exception = (Exception) new TaskHubExistsException(TaskResources.HubExists((object) sqlError.ExtractString("hubName")));
          break;
        case 907052:
          exception = (Exception) new TimelineRecordUpdateException(TaskResources.TimelineRecordInvalid((object) sqlError.ExtractString("recordId")));
          break;
        case 907053:
          exception = (Exception) new TimelineRecordNotFoundException(TaskResources.TimelineRecordNotFound((object) sqlError.ExtractString("planId"), (object) sqlError.ExtractString("timelineId"), (object) sqlError.ExtractString("recordId")));
          break;
        case 907077:
          exception = (Exception) new TaskOrchestrationPlanLogNotFoundException(TaskResources.PlanLogNotFound((object) sqlError.ExtractInt("logId"), (object) sqlError.ExtractString("planId")));
          break;
      }
      return exception;
    }

    private static IDictionary<int, SqlExceptionFactory> CreateExceptionMap() => (IDictionary<int, SqlExceptionFactory>) new Dictionary<int, SqlExceptionFactory>()
    {
      {
        907010,
        new SqlExceptionFactory(typeof (TaskOrchestrationPlanNotFoundException), TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException ?? (TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException = new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(TaskSqlComponentBase.CreateException)))
      },
      {
        907021,
        new SqlExceptionFactory(typeof (TaskOrchestrationJobNotFoundException), TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException ?? (TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException = new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(TaskSqlComponentBase.CreateException)))
      },
      {
        907011,
        new SqlExceptionFactory(typeof (TimelineNotFoundException), TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException ?? (TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException = new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(TaskSqlComponentBase.CreateException)))
      },
      {
        907020,
        new SqlExceptionFactory(typeof (TimelineExistsException), TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException ?? (TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException = new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(TaskSqlComponentBase.CreateException)))
      },
      {
        907023,
        new SqlExceptionFactory(typeof (TimelineRecordUpdateException), TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException ?? (TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException = new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(TaskSqlComponentBase.CreateException)))
      },
      {
        907053,
        new SqlExceptionFactory(typeof (TimelineRecordNotFoundException), TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException ?? (TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException = new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(TaskSqlComponentBase.CreateException)))
      },
      {
        907051,
        new SqlExceptionFactory(typeof (TaskHubExistsException), TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException ?? (TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException = new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(TaskSqlComponentBase.CreateException)))
      },
      {
        907052,
        new SqlExceptionFactory(typeof (TimelineRecordUpdateException), TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException ?? (TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException = new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(TaskSqlComponentBase.CreateException)))
      },
      {
        907022,
        new SqlExceptionFactory(typeof (TaskOrchestrationPlanGroupNotFoundException), TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException ?? (TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException = new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(TaskSqlComponentBase.CreateException)))
      },
      {
        907077,
        new SqlExceptionFactory(typeof (TaskOrchestrationPlanLogNotFoundException), TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException ?? (TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException = new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(TaskSqlComponentBase.CreateException)))
      },
      {
        907025,
        new SqlExceptionFactory(typeof (TimelineRecordUpdateException), TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException ?? (TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException = new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(TaskSqlComponentBase.CreateException)))
      }
    };

    protected struct SqlMethodScope : IDisposable
    {
      private readonly string m_method;
      private readonly TaskSqlComponentBase m_component;

      public SqlMethodScope(TaskSqlComponentBase component, [CallerMemberName] string method = null)
      {
        this.m_method = method;
        this.m_component = component;
        this.m_component.TraceEnter(0, this.m_method);
      }

      public void Dispose() => this.m_component.TraceLeave(0, this.m_method);
    }
  }
}
