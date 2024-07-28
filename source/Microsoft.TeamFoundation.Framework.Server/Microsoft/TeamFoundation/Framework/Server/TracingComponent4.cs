// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TracingComponent4
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
  internal class TracingComponent4 : TracingComponent3
  {
    public override TraceFilter QueryTrace(IVssRequestContext requestContext, Guid traceId)
    {
      this.PrepareStoredProcedure("prc_QueryTrace");
      this.BindGuid("@traceId", traceId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<TraceFilter>((ObjectBinder<TraceFilter>) this.GetTraceColumnsBinder());
        return resultCollection.GetCurrent<TraceFilter>().Items.SingleOrDefault<TraceFilter>();
      }
    }

    public override IEnumerable<TraceFilter> QueryTraces(
      IVssRequestContext requestContext,
      Guid? ownerId = null)
    {
      this.PrepareStoredProcedure("prc_QueryTraces");
      this.BindNullableGuid("@ownerId", ownerId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<TraceFilter>((ObjectBinder<TraceFilter>) this.GetTraceColumnsBinder());
        return (IEnumerable<TraceFilter>) resultCollection.GetCurrent<TraceFilter>().Items;
      }
    }
  }
}
