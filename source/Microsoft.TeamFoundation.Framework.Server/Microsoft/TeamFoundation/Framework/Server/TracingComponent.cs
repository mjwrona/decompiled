// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TracingComponent
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class TracingComponent : TeamFoundationSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[4]
    {
      (IComponentCreator) new ComponentCreator<TracingComponent>(1),
      (IComponentCreator) new ComponentCreator<TracingComponent2>(2),
      (IComponentCreator) new ComponentCreator<TracingComponent3>(3),
      (IComponentCreator) new ComponentCreator<TracingComponent4>(4)
    }, "Tracing");

    public virtual void DeleteTrace(IVssRequestContext requestContext, Guid traceId)
    {
      this.PrepareStoredProcedure("prc_DeleteTrace");
      this.BindGuid("@traceId", traceId);
      this.ExecuteNonQuery();
    }

    public virtual void CreateTrace(IVssRequestContext requestContext, TraceFilter filter)
    {
      this.PrepareStoredProcedure("prc_CreateTrace");
      this.BindGuid("@traceId", filter.TraceId);
      this.BindOwnerId(filter);
      this.BindBoolean("@enabled", filter.IsEnabled);
      this.BindTracepoint(filter);
      this.BindString("@processName", filter.ProcessName, 260, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@userLogin", filter.UserLogin, 260, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@service", filter.Service, 260, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@method", filter.Method, 260, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@area", filter.Area, 260, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindByte("@level", (byte) filter.Level);
      this.BindString("@userAgent", filter.UserAgent, 260, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@layer", filter.Layer, 260, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      string parameterValue = string.Empty;
      if (filter.Tags != null && filter.Tags.Length != 0)
        parameterValue = string.Join(":", filter.Tags);
      this.BindString("@userDefined", parameterValue, 260, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindGuid("@serviceHost", filter.ServiceHost);
      this.ExecuteNonQuery();
    }

    protected virtual void BindOwnerId(TraceFilter traceFilter)
    {
    }

    protected virtual void BindTracepoint(TraceFilter traceFilter)
    {
    }

    public virtual TraceFilter QueryTrace(IVssRequestContext requestContext, Guid traceId) => this.QueryTraces(requestContext).Where<TraceFilter>((System.Func<TraceFilter, bool>) (t => t.TraceId == traceId)).SingleOrDefault<TraceFilter>();

    public virtual IEnumerable<TraceFilter> QueryTraces(
      IVssRequestContext requestContext,
      Guid? ownerId = null)
    {
      this.PrepareStoredProcedure("prc_QueryTraces");
      List<TraceFilter> source = (List<TraceFilter>) null;
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<TraceFilter>((ObjectBinder<TraceFilter>) this.GetTraceColumnsBinder());
        source = resultCollection.GetCurrent<TraceFilter>().Items;
      }
      if (ownerId.HasValue)
        source = source.Where<TraceFilter>((System.Func<TraceFilter, bool>) (f =>
        {
          Guid ownerId1 = f.OwnerId;
          Guid? nullable = ownerId;
          return nullable.HasValue && ownerId1 == nullable.GetValueOrDefault();
        })).ToList<TraceFilter>();
      return (IEnumerable<TraceFilter>) source;
    }

    protected virtual TraceColumns GetTraceColumnsBinder() => new TraceColumns();

    public virtual void DisableTrace(IVssRequestContext requestContext, Guid traceId)
    {
    }
  }
}
