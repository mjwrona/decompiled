// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Results.CreatedODataResult`1
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
  public class CreatedODataResult<T> : IHttpActionResult
  {
    private readonly NegotiatedContentResult<T> _innerResult;
    private Uri _locationHeader;

    public CreatedODataResult(T entity, ApiController controller)
      : this(new NegotiatedContentResult<T>(HttpStatusCode.Created, CreatedODataResult<T>.CheckNull(entity), controller))
    {
    }

    public CreatedODataResult(
      T entity,
      IContentNegotiator contentNegotiator,
      HttpRequestMessage request,
      IEnumerable<MediaTypeFormatter> formatters,
      Uri locationHeader)
      : this(new NegotiatedContentResult<T>(HttpStatusCode.Created, CreatedODataResult<T>.CheckNull(entity), contentNegotiator, request, formatters))
    {
      this._locationHeader = !(locationHeader == (Uri) null) ? locationHeader : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (locationHeader));
    }

    private CreatedODataResult(NegotiatedContentResult<T> innerResult) => this._innerResult = innerResult;

    public T Entity => this._innerResult.Content;

    public IContentNegotiator ContentNegotiator => this._innerResult.ContentNegotiator;

    public HttpRequestMessage Request => this._innerResult.Request;

    public IEnumerable<MediaTypeFormatter> Formatters => this._innerResult.Formatters;

    public Uri LocationHeader
    {
      get
      {
        Uri locationHeader = this._locationHeader;
        if ((object) locationHeader == null)
          locationHeader = this.GenerateLocationHeader(this.Request);
        this._locationHeader = locationHeader;
        return this._locationHeader;
      }
    }

    public virtual async Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
    {
      CreatedODataResult<T> createdOdataResult = this;
      HttpResponseMessage response = await createdOdataResult.GetInnerActionResult(createdOdataResult.Request).ExecuteAsync(cancellationToken);
      response.Headers.Location = createdOdataResult.LocationHeader;
      // ISSUE: reference to a compiler-generated method
      ResultHelpers.AddEntityId(response, new Func<Uri>(createdOdataResult.\u003CExecuteAsync\u003Eb__15_0));
      // ISSUE: reference to a compiler-generated method
      ResultHelpers.AddServiceVersion(response, new Func<string>(createdOdataResult.\u003CExecuteAsync\u003Eb__15_1));
      return response;
    }

    internal IHttpActionResult GetInnerActionResult(HttpRequestMessage request) => RequestPreferenceHelpers.RequestPrefersReturnNoContent((IWebApiHeaders) new WebApiRequestHeaders(request.Headers)) ? (IHttpActionResult) new StatusCodeResult(HttpStatusCode.NoContent, request) : (IHttpActionResult) this._innerResult;

    internal Uri GenerateEntityId(HttpRequestMessage request) => ResultHelpers.GenerateODataLink(request, (object) this.Entity, true);

    internal Uri GenerateLocationHeader(HttpRequestMessage request) => ResultHelpers.GenerateODataLink(request, (object) this.Entity, false);

    private static T CheckNull(T entity) => (object) entity != null ? entity : throw new ArgumentNullException(nameof (entity));
  }
}
