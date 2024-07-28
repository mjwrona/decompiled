// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Batch.ODataBatchHandler
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Interfaces;
using Microsoft.OData;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Batch;

namespace Microsoft.AspNet.OData.Batch
{
  public abstract class ODataBatchHandler : HttpBatchHandler
  {
    private ODataMessageQuotas _messageQuotas = new ODataMessageQuotas()
    {
      MaxReceivedMessageSize = long.MaxValue
    };
    internal const string PreferenceContinueOnError = "continue-on-error";
    internal const string PreferenceContinueOnErrorFalse = "continue-on-error=false";

    protected ODataBatchHandler(HttpServer httpServer)
      : base(httpServer)
    {
    }

    public virtual Task<HttpResponseMessage> CreateResponseMessageAsync(
      IEnumerable<ODataBatchResponseItem> responses,
      HttpRequestMessage request,
      CancellationToken cancellationToken)
    {
      if (request == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (request));
      return request.CreateODataBatchResponseAsync(responses, this.MessageQuotas);
    }

    public virtual void ValidateRequest(HttpRequestMessage request)
    {
      if (request == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (request));
      request.ValidateODataBatchRequest();
    }

    public virtual Uri GetBaseUri(HttpRequestMessage request) => request != null ? request.GetODataBatchBaseUri(this.ODataRouteName) : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (request));

    public ODataMessageQuotas MessageQuotas => this._messageQuotas;

    public string ODataRouteName { get; set; }

    internal bool ContinueOnError { get; private set; }

    internal void SetContinueOnError(IWebApiHeaders header, bool enableContinueOnErrorHeader)
    {
      string requestPreferHeader = RequestPreferenceHelpers.GetRequestPreferHeader(header);
      if (requestPreferHeader != null && requestPreferHeader.Contains("continue-on-error") && !requestPreferHeader.Contains("continue-on-error=false") || !enableContinueOnErrorHeader)
        this.ContinueOnError = true;
      else
        this.ContinueOnError = false;
    }
  }
}
