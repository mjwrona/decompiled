// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Results.UpdatedODataResult`1
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Adapters;
using Microsoft.AspNet.OData.Interfaces;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;

namespace Microsoft.AspNet.OData.Results
{
  public class UpdatedODataResult<T> : IHttpActionResult
  {
    private readonly NegotiatedContentResult<T> _innerResult;

    public UpdatedODataResult(T entity, ApiController controller)
      : this(new NegotiatedContentResult<T>(HttpStatusCode.OK, UpdatedODataResult<T>.CheckNull(entity), controller))
    {
    }

    public UpdatedODataResult(
      T entity,
      IContentNegotiator contentNegotiator,
      HttpRequestMessage request,
      IEnumerable<MediaTypeFormatter> formatters)
      : this(new NegotiatedContentResult<T>(HttpStatusCode.OK, UpdatedODataResult<T>.CheckNull(entity), contentNegotiator, request, formatters))
    {
    }

    private UpdatedODataResult(NegotiatedContentResult<T> innerResult) => this._innerResult = innerResult;

    public T Entity => this._innerResult.Content;

    public IContentNegotiator ContentNegotiator => this._innerResult.ContentNegotiator;

    public HttpRequestMessage Request => this._innerResult.Request;

    public IEnumerable<MediaTypeFormatter> Formatters => this._innerResult.Formatters;

    public virtual async Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
    {
      UpdatedODataResult<T> updatedOdataResult = this;
      HttpResponseMessage response = await updatedOdataResult.GetInnerActionResult().ExecuteAsync(cancellationToken);
      // ISSUE: reference to a compiler-generated method
      ResultHelpers.AddServiceVersion(response, new Func<string>(updatedOdataResult.\u003CExecuteAsync\u003Eb__12_0));
      return response;
    }

    internal IHttpActionResult GetInnerActionResult() => RequestPreferenceHelpers.RequestPrefersReturnContent((IWebApiHeaders) new WebApiRequestHeaders(this._innerResult.Request.Headers)) ? (IHttpActionResult) this._innerResult : (IHttpActionResult) new StatusCodeResult(HttpStatusCode.NoContent, this._innerResult.Request);

    private static T CheckNull(T entity) => (object) entity != null ? entity : throw new ArgumentNullException(nameof (entity));
  }
}
