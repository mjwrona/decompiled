// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Approval.Server.DataAccess.ApprovalSqlComponentBase
// Assembly: Microsoft.Azure.Pipelines.Approval.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03DD9218-2C79-49A0-9EA7-F497B1327A4F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Approval.Server.dll

using Microsoft.Azure.Pipelines.Approval.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Runtime.CompilerServices;

namespace Microsoft.Azure.Pipelines.Approval.Server.DataAccess
{
  public abstract class ApprovalSqlComponentBase : TeamFoundationSqlResourceComponent
  {
    protected const string PipelinePolicyDataspaceCategory = "PipelinePolicy";
    private IDictionary<int, SqlExceptionFactory> m_translatedExceptions;

    protected ApprovalSqlComponentBase()
    {
      this.ContainerErrorCode = 50000;
      this.SelectedFeatures |= SqlResourceComponentFeatures.BindPartitionIdByDefault;
    }

    protected override string TraceArea => "PipelinePolicy.Approval";

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions
    {
      get
      {
        if (this.m_translatedExceptions == null)
          this.m_translatedExceptions = ApprovalSqlComponentBase.CreateExceptionMap();
        return this.m_translatedExceptions;
      }
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
        case 1804000:
          exception = (Exception) new ApprovalExistsException(ApprovalResources.ApprovalExists((object) sqlError.ExtractString("approvalId")));
          break;
        case 1804001:
          exception = (Exception) new ApprovalIdNotProvidedException(ApprovalResources.ApprovalIdNotProvided());
          break;
      }
      return exception;
    }

    private static IDictionary<int, SqlExceptionFactory> CreateExceptionMap() => (IDictionary<int, SqlExceptionFactory>) new Dictionary<int, SqlExceptionFactory>()
    {
      {
        1804000,
        new SqlExceptionFactory(typeof (ApprovalExistsException), ApprovalSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException ?? (ApprovalSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException = new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(ApprovalSqlComponentBase.CreateException)))
      },
      {
        1804001,
        new SqlExceptionFactory(typeof (ApprovalIdNotProvidedException), ApprovalSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException ?? (ApprovalSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException = new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(ApprovalSqlComponentBase.CreateException)))
      }
    };

    protected struct SqlMethodScope : IDisposable
    {
      private readonly int m_tracepoint;
      private readonly string m_method;
      private readonly ApprovalSqlComponentBase m_component;

      public SqlMethodScope(ApprovalSqlComponentBase component, int tracepoint = 0, [CallerMemberName] string method = null)
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
