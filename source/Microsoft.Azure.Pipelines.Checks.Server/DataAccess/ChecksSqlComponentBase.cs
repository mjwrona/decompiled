// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Checks.Server.DataAccess.ChecksSqlComponentBase
// Assembly: Microsoft.Azure.Pipelines.Checks.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D08C7285-654E-4A4D-BA46-748F0D1E96AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Checks.Server.dll

using Microsoft.Azure.Pipelines.Checks.Common;
using Microsoft.Azure.Pipelines.Checks.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.Azure.Pipelines.Checks.Server.DataAccess
{
  internal abstract class ChecksSqlComponentBase : TeamFoundationSqlResourceComponent
  {
    protected const string PipelinePolicyDataspaceCategory = "PipelinePolicy";
    private IDictionary<int, SqlExceptionFactory> m_translatedExceptions;

    protected ChecksSqlComponentBase()
    {
      this.ContainerErrorCode = 50000;
      this.SelectedFeatures |= SqlResourceComponentFeatures.BindPartitionIdByDefault;
    }

    protected override string TraceArea => "Pipeline.Checks";

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions
    {
      get
      {
        if (this.m_translatedExceptions == null)
          this.m_translatedExceptions = ChecksSqlComponentBase.CreateExceptionMap();
        return this.m_translatedExceptions;
      }
    }

    protected string ToString<T>(IList<T> value)
    {
      List<T> list = ((IEnumerable<T>) value ?? Enumerable.Empty<T>()).Where<T>((Func<T, bool>) (item => (object) item != null)).ToList<T>();
      return list.Count > 0 ? JsonUtility.ToString<T>((IList<T>) list) : (string) null;
    }

    protected string ToString<TKey, TValue>(IDictionary<TKey, TValue> value) => value == null || value.Count == 0 ? (string) null : JsonUtility.ToString((object) value);

    protected IDisposable TraceScope(int tracepoint = 0, [CallerMemberName] string method = null) => (IDisposable) new ChecksSqlComponentBase.SqlMethodScope(this, tracepoint, method);

    private static Exception CreateException(
      IVssRequestContext requestContext,
      int errorNumber,
      SqlException sqlException,
      SqlError sqlError)
    {
      Exception exception = (Exception) null;
      switch (errorNumber)
      {
        case 1803800:
          ChecksSqlComponentBase.TraceException(requestContext, 1803800, sqlException);
          exception = (Exception) new GenericDatabaseUpdateErrorException(PipelineChecksResources.GenericDatabaseUpdateError());
          break;
        case 1803801:
          exception = (Exception) new CheckRunExistsException(PipelineChecksResources.ChecksRunExits((object) sqlError.ExtractString("recordCount")));
          break;
        case 1803802:
          exception = (Exception) new CheckConfigurationNotFoundException(PipelineChecksResources.ChecksConfigurationNotFound((object) sqlError.ExtractInt("assignmentId")));
          break;
        case 1803803:
          exception = (Exception) new CheckRunRequestNotFoundException(PipelineChecksResources.ChecksRunNotFound((object) sqlError.ExtractString("requestId")));
          break;
        case 1803804:
          exception = (Exception) new CheckSuiteRequestNotFoundException(PipelineChecksResources.ChecksSuiteIdNotFound((object) sqlError.ExtractString("batchRequestId")));
          break;
        case 1803805:
          exception = (Exception) new CheckSuiteRequestIdExistsException(PipelineChecksResources.ChecksSuiteIdExits((object) sqlError.ExtractString("batchRequestId")));
          break;
      }
      return exception;
    }

    private static IDictionary<int, SqlExceptionFactory> CreateExceptionMap() => (IDictionary<int, SqlExceptionFactory>) new Dictionary<int, SqlExceptionFactory>()
    {
      {
        1803800,
        new SqlExceptionFactory(typeof (GenericDatabaseUpdateErrorException), ChecksSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException ?? (ChecksSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException = new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(ChecksSqlComponentBase.CreateException)))
      },
      {
        1803802,
        new SqlExceptionFactory(typeof (CheckConfigurationNotFoundException), ChecksSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException ?? (ChecksSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException = new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(ChecksSqlComponentBase.CreateException)))
      },
      {
        1803801,
        new SqlExceptionFactory(typeof (CheckRunExistsException), ChecksSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException ?? (ChecksSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException = new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(ChecksSqlComponentBase.CreateException)))
      },
      {
        1803803,
        new SqlExceptionFactory(typeof (CheckRunRequestNotFoundException), ChecksSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException ?? (ChecksSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException = new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(ChecksSqlComponentBase.CreateException)))
      },
      {
        1803804,
        new SqlExceptionFactory(typeof (CheckSuiteRequestNotFoundException), ChecksSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException ?? (ChecksSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException = new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(ChecksSqlComponentBase.CreateException)))
      },
      {
        1803805,
        new SqlExceptionFactory(typeof (CheckSuiteRequestIdExistsException), ChecksSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException ?? (ChecksSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException = new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(ChecksSqlComponentBase.CreateException)))
      }
    };

    private static void TraceException(
      IVssRequestContext requestContext,
      int tracePoint,
      SqlException sqlException)
    {
      if (requestContext == null)
        return;
      requestContext.TraceException(tracePoint, "Service", (Exception) sqlException);
    }

    protected struct SqlMethodScope : IDisposable
    {
      private readonly int m_tracepoint;
      private readonly string m_method;
      private readonly ChecksSqlComponentBase m_component;

      public SqlMethodScope(ChecksSqlComponentBase component, int tracepoint = 0, [CallerMemberName] string method = null)
      {
        this.m_tracepoint = tracepoint;
        this.m_method = method;
        this.m_component = component;
        this.m_component.TraceEnter(this.m_tracepoint, this.m_method);
      }

      public void Dispose() => this.m_component.TraceLeave(this.m_tracepoint, this.m_method);
    }
  }
}
