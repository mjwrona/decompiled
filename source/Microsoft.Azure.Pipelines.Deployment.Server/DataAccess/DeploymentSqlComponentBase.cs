// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.DataAccess.DeploymentSqlComponentBase
// Assembly: Microsoft.Azure.Pipelines.Deployment.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B08F1AB-5B33-41F6-908E-9A985FD0544C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Deployment.Server.dll

using Microsoft.Azure.Pipelines.Deployment.WebApi.Exceptions;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Runtime.CompilerServices;

namespace Microsoft.Azure.Pipelines.Deployment.DataAccess
{
  public abstract class DeploymentSqlComponentBase : TeamFoundationSqlResourceComponent
  {
    protected const string DeploymentDataspaceCategory = "Deployment";
    private static Lazy<IDictionary<int, SqlExceptionFactory>> s_translatedExceptions = new Lazy<IDictionary<int, SqlExceptionFactory>>(new Func<IDictionary<int, SqlExceptionFactory>>(DeploymentSqlComponentBase.CreateExceptionMap));

    protected DeploymentSqlComponentBase() => this.ContainerErrorCode = 50000;

    protected override string TraceArea => "Deployment";

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => DeploymentSqlComponentBase.s_translatedExceptions.Value;

    protected void BindDataspaceId(Guid dataspaceIdentifier) => this.BindInt("@dataspaceId", this.GetDataspaceId(dataspaceIdentifier, true));

    private static Exception CreateException(
      IVssRequestContext requestContext,
      int errorNumber,
      SqlException sqlException,
      SqlError sqlError)
    {
      Exception exception = (Exception) null;
      switch (errorNumber)
      {
        case 2000021:
          exception = (Exception) new NoteNotFoundException(DeploymentResources.NoteNotFound((object) sqlError.ExtractString("NoteId")));
          break;
        case 2000022:
          exception = (Exception) new OccurrenceNotFoundException(DeploymentResources.OccurrenceNotFound((object) sqlError.ExtractString("OccurrenceId")));
          break;
        case 2000023:
          exception = (Exception) new OccurrenceTagExistsException(DeploymentResources.OccurrenceTagExists((object) sqlError.ExtractString("TagName")));
          break;
        case 2000024:
          exception = (Exception) new NoteExistsException(DeploymentResources.NoteExists((object) sqlError.ExtractString("NoteId")));
          break;
      }
      return exception;
    }

    private static IDictionary<int, SqlExceptionFactory> CreateExceptionMap() => (IDictionary<int, SqlExceptionFactory>) new Dictionary<int, SqlExceptionFactory>()
    {
      {
        2000021,
        new SqlExceptionFactory(typeof (NoteNotFoundException), DeploymentSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException ?? (DeploymentSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException = new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(DeploymentSqlComponentBase.CreateException)))
      },
      {
        2000024,
        new SqlExceptionFactory(typeof (NoteExistsException), DeploymentSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException ?? (DeploymentSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException = new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(DeploymentSqlComponentBase.CreateException)))
      },
      {
        2000022,
        new SqlExceptionFactory(typeof (OccurrenceNotFoundException), DeploymentSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException ?? (DeploymentSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException = new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(DeploymentSqlComponentBase.CreateException)))
      },
      {
        2000023,
        new SqlExceptionFactory(typeof (OccurrenceTagExistsException), DeploymentSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException ?? (DeploymentSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException = new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(DeploymentSqlComponentBase.CreateException)))
      }
    };

    protected struct SqlMethodScope : IDisposable
    {
      private readonly string m_method;
      private readonly DeploymentSqlComponentBase m_component;

      public SqlMethodScope(DeploymentSqlComponentBase component, [CallerMemberName] string method = null)
      {
        this.m_method = method;
        this.m_component = component;
        this.m_component.TraceEnter(0, this.m_method);
      }

      public void Dispose() => this.m_component.TraceLeave(0, this.m_method);
    }
  }
}
