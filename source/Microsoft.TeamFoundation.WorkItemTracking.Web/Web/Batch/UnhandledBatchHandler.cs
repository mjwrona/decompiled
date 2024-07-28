// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Batch.UnhandledBatchHandler
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Batch
{
  public class UnhandledBatchHandler : BatchHandler
  {
    public UnhandledBatchHandler(HttpServer server)
      : base(server)
    {
    }

    public override IEnumerable<HttpResponseMessage> ProcessBatch(
      IVssRequestContext requestContext,
      IEnumerable<BatchHttpRequestMessage> requests)
    {
      List<HttpResponseMessage> httpResponseMessageList = new List<HttpResponseMessage>();
      foreach (BatchHttpRequestMessage request in requests)
      {
        request.Handled = true;
        request.Handler = nameof (UnhandledBatchHandler);
        httpResponseMessageList.Add(request.CreateErrorResponse(HttpStatusCode.NotFound, ResourceStrings.UnsupportedBatchOperation()));
      }
      return (IEnumerable<HttpResponseMessage>) httpResponseMessageList;
    }
  }
}
