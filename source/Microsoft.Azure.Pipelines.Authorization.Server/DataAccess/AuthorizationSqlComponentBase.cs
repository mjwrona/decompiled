// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Authorization.Server.DataAccess.AuthorizationSqlComponentBase
// Assembly: Microsoft.Azure.Pipelines.Authorization.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 22B31FF9-0E6B-45B0-A4F8-77598802CAB3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Authorization.Server.dll

using Microsoft.Azure.Pipelines.Checks.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.Azure.Pipelines.Authorization.Server.DataAccess
{
  internal class AuthorizationSqlComponentBase : TeamFoundationSqlResourceComponent
  {
    protected const string BuildDataspaceCategory = "Build";

    public AuthorizationSqlComponentBase()
    {
      this.ContainerErrorCode = 50000;
      this.SelectedFeatures |= SqlResourceComponentFeatures.BindPartitionIdByDefault;
    }

    protected override string TraceArea => "PipelinePolicy.Authorization";

    protected string ToString<T>(IList<T> value)
    {
      List<T> list = ((IEnumerable<T>) value ?? Enumerable.Empty<T>()).Where<T>((Func<T, bool>) (item => (object) item != null)).ToList<T>();
      return list.Count > 0 ? JsonUtility.ToString<T>((IList<T>) list) : (string) null;
    }

    protected string ToString<TKey, TValue>(IDictionary<TKey, TValue> value) => value == null || value.Count == 0 ? (string) null : JsonUtility.ToString((object) value);

    protected IDisposable TraceScope(int tracepoint = 0, [CallerMemberName] string method = null) => (IDisposable) new AuthorizationSqlComponentBase.SqlMethodScope(this, tracepoint, method);

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
      private readonly AuthorizationSqlComponentBase m_component;

      public SqlMethodScope(AuthorizationSqlComponentBase component, int tracepoint = 0, [CallerMemberName] string method = null)
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
