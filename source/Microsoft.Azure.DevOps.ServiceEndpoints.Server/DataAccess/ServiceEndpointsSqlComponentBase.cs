// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Server.DataAccess.ServiceEndpointsSqlComponentBase
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B7D66E3F-07ED-4CF3-859D-36958D465656
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DevOps.ServiceEndpoints.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.CompilerServices;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Server.DataAccess
{
  public abstract class ServiceEndpointsSqlComponentBase : TeamFoundationSqlResourceComponent
  {
    private static Lazy<IDictionary<int, SqlExceptionFactory>> s_translatedExceptions = new Lazy<IDictionary<int, SqlExceptionFactory>>(new Func<IDictionary<int, SqlExceptionFactory>>(ServiceEndpointsSqlComponentBase.CreateExceptionMap));

    protected ServiceEndpointsSqlComponentBase() => this.ContainerErrorCode = 50000;

    protected override string TraceArea => "ServiceEndpoints";

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => ServiceEndpointsSqlComponentBase.s_translatedExceptions.Value;

    protected void BindDataspaceId(Guid dataspaceIdentifier) => this.BindInt("@dataspaceId", this.GetDataspaceId(dataspaceIdentifier, true));

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

    protected void PrepareStoredProcedure(string storedProcedure, Guid dataspaceIdentifier)
    {
      this.PrepareStoredProcedure(storedProcedure);
      this.BindInt("dataspaceId", this.GetDataspaceId(dataspaceIdentifier, "DistributedTask", true));
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
        case 907017:
          exception = (Exception) new DuplicateServiceConnectionException(ServiceEndpointResources.ServiceEndpointNameAlreadyExists((object) sqlError.ExtractString("name")));
          break;
        case 907075:
          exception = (Exception) new OAuthConfigurationExistsException(ServiceEndpointResources.OAuthConfigurationExist((object) sqlError.ExtractString("name")));
          break;
        case 907076:
          exception = (Exception) new OAuthConfigurationNotFoundException(ServiceEndpointResources.OAuthConfigurationNotFound((object) sqlError.ExtractInt("configurationId")));
          break;
      }
      return exception;
    }

    private static IDictionary<int, SqlExceptionFactory> CreateExceptionMap() => (IDictionary<int, SqlExceptionFactory>) new Dictionary<int, SqlExceptionFactory>()
    {
      {
        907075,
        new SqlExceptionFactory(typeof (OAuthConfigurationExistsException), ServiceEndpointsSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException ?? (ServiceEndpointsSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException = new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(ServiceEndpointsSqlComponentBase.CreateException)))
      },
      {
        907076,
        new SqlExceptionFactory(typeof (OAuthConfigurationNotFoundException), ServiceEndpointsSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException ?? (ServiceEndpointsSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException = new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(ServiceEndpointsSqlComponentBase.CreateException)))
      },
      {
        907017,
        new SqlExceptionFactory(typeof (DuplicateServiceConnectionException), ServiceEndpointsSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException ?? (ServiceEndpointsSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException = new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(ServiceEndpointsSqlComponentBase.CreateException)))
      }
    };

    protected struct SqlMethodScope : IDisposable
    {
      private readonly string m_method;
      private readonly ServiceEndpointsSqlComponentBase m_component;

      public SqlMethodScope(ServiceEndpointsSqlComponentBase component, [CallerMemberName] string method = null)
      {
        this.m_method = method;
        this.m_component = component;
        this.m_component.TraceEnter(0, this.m_method);
      }

      public void Dispose() => this.m_component.TraceLeave(0, this.m_method);
    }
  }
}
