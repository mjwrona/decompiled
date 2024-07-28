// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.TemporaryDataController
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Routing;

namespace Microsoft.TeamFoundation.Server.Core
{
  [VersionedApiControllerCustomName("properties", "properties", 1)]
  [ClientEditorBrowsable(EditorBrowsableState.Never)]
  public class TemporaryDataController : ServerCoreApiController
  {
    protected static readonly Dictionary<Type, HttpStatusCode> s_httpExceptions = new Dictionary<Type, HttpStatusCode>()
    {
      {
        typeof (TemporaryDataNotFoundException),
        HttpStatusCode.NotFound
      },
      {
        typeof (TemporaryDataTooLargeException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (DuplicateTemporaryDataIdException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (TemporaryDataUnknownPropertyException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (ArgumentException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (SqlException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (SqlTypeException),
        HttpStatusCode.BadRequest
      }
    };
    private IUrlHelperExtensionsWrapper m_urlGenerator;

    public override IDictionary<Type, HttpStatusCode> HttpExceptions => (IDictionary<Type, HttpStatusCode>) TemporaryDataController.s_httpExceptions;

    public TemporaryDataController() => this.m_urlGenerator = (IUrlHelperExtensionsWrapper) new UrlHelperExtensionsWrapper();

    internal TemporaryDataController(IUrlHelperExtensionsWrapper urlGenerator)
    {
      ArgumentUtility.CheckForNull<IUrlHelperExtensionsWrapper>(urlGenerator, nameof (urlGenerator));
      this.m_urlGenerator = urlGenerator;
    }

    [HttpPost]
    [ClientResponseType(typeof (TemporaryDataCreatedDTO), null, null)]
    [TraceFilter(10003200, 10003201)]
    public HttpResponseMessage CreateTemporaryData([FromBody] TemporaryDataDTO requestBody)
    {
      if (requestBody == null || !(requestBody.Value is JObject))
        return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, Resources.TemporaryDataInvalid());
      ITeamFoundationTemporaryDataService service = this.TfsRequestContext.GetService<ITeamFoundationTemporaryDataService>();
      string str = (requestBody.Value as JObject).ToString(Formatting.None);
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      string data = str;
      return this.Request.CreateResponse<TemporaryDataCreatedDTO>(HttpStatusCode.Created, this.CreateTemporaryDataResponseModel(this.TfsRequestContext, service.CreateTemporaryData(tfsRequestContext, data), this.Url));
    }

    [HttpPut]
    [ClientResponseType(typeof (TemporaryDataCreatedDTO), null, null)]
    [TraceFilter(10003200, 10003201)]
    public HttpResponseMessage CreateTemporaryData(Guid id, [FromBody] TemporaryDataDTO requestBody)
    {
      ArgumentUtility.CheckForEmptyGuid(id, nameof (id));
      if (requestBody == null || !(requestBody.Value is JObject))
        return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, Resources.TemporaryDataInvalid());
      ITeamFoundationTemporaryDataService service = this.TfsRequestContext.GetService<ITeamFoundationTemporaryDataService>();
      string str = (requestBody.Value as JObject).ToString(Formatting.None);
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      string data = str;
      Guid id1 = id;
      string origin = requestBody.Origin;
      int expirationSeconds = requestBody.ExpirationSeconds;
      return this.Request.CreateResponse<TemporaryDataCreatedDTO>(HttpStatusCode.Created, this.CreateTemporaryDataResponseModel(this.TfsRequestContext, service.CreateTemporaryData(tfsRequestContext, data, id1, origin, expirationSeconds), this.Url));
    }

    [HttpGet]
    [ClientResponseType(typeof (TemporaryDataCreatedDTO), null, null)]
    [TraceFilter(10003202, 10003203)]
    public HttpResponseMessage GetTemporaryData(Guid id)
    {
      ServiceTemporaryDataDTO temporaryData = this.TfsRequestContext.GetService<ITeamFoundationTemporaryDataService>().GetTemporaryData(this.TfsRequestContext, id);
      return temporaryData != null && temporaryData.Value != null ? this.Request.CreateResponse<TemporaryDataCreatedDTO>(HttpStatusCode.OK, this.CreateTemporaryDataResponseModel(this.TfsRequestContext, temporaryData, this.Url)) : this.Request.CreateErrorResponse(HttpStatusCode.NotFound, Resources.TemporaryDataNotFoundException());
    }

    private TemporaryDataCreatedDTO CreateTemporaryDataResponseModel(
      IVssRequestContext requestContext,
      ServiceTemporaryDataDTO model,
      UrlHelper url)
    {
      TemporaryDataCreatedDTO dataResponseModel = new TemporaryDataCreatedDTO();
      dataResponseModel.Id = model.Id;
      dataResponseModel.Url = this.m_urlGenerator.RestLink(url, this.TfsRequestContext, TemporaryDataConstants.TemporaryDataWebApiLocationId, (object) new
      {
        id = model.Id
      });
      dataResponseModel.Value = (object) JObject.Parse(model.Value);
      dataResponseModel.ExpirationDate = model.ExpirationDate;
      dataResponseModel.Origin = model.Origin;
      return dataResponseModel;
    }
  }
}
