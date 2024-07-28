// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure.HttpRequestMessageExtensions
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using Microsoft.OData;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Security;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure
{
  internal static class HttpRequestMessageExtensions
  {
    private static ApiExceptionMapping s_exceptionMap = new ApiExceptionMapping();
    private const string WarningsKey = "VSTS.Warnings";

    static HttpRequestMessageExtensions()
    {
      HttpRequestMessageExtensions.s_exceptionMap.AddTranslation<AccessCheckException, UnauthorizedRequestException>();
      HttpRequestMessageExtensions.s_exceptionMap.AddStatusCode<ProjectDoesNotExistWithNameException>(HttpStatusCode.NotFound);
      HttpRequestMessageExtensions.s_exceptionMap.AddStatusCode<TeamNotFoundException>(HttpStatusCode.NotFound);
      HttpRequestMessageExtensions.s_exceptionMap.AddStatusCode<ODataException>(HttpStatusCode.BadRequest);
      HttpRequestMessageExtensions.s_exceptionMap.AddStatusCode<ModelNotReadyException>(HttpStatusCode.ServiceUnavailable);
      HttpRequestMessageExtensions.s_exceptionMap.AddStatusCode<ModelSyncingException>(HttpStatusCode.ServiceUnavailable);
      HttpRequestMessageExtensions.s_exceptionMap.AddStatusCode<AnalyticsNotEnabledException>(HttpStatusCode.BadRequest);
      HttpRequestMessageExtensions.s_exceptionMap.AddStatusCode<AnalyticsAccessCheckException>(HttpStatusCode.Forbidden);
      HttpRequestMessageExtensions.s_exceptionMap.AddStatusCode<UnsupportedODataModelVersionException>(HttpStatusCode.Gone);
      HttpRequestMessageExtensions.s_exceptionMap.AddStatusCode<AccessCheckException>(HttpStatusCode.Forbidden);
    }

    public static void ThrowWrappedException(this HttpRequestMessage request, Exception ex)
    {
      HttpStatusCode statusCode = (HttpStatusCode) ((int) HttpRequestMessageExtensions.s_exceptionMap.GetStatusCode(ex.GetType()) ?? 500);
      WrappedException wrappedException = new WrappedException(HttpRequestMessageExtensions.s_exceptionMap.TranslateException(ex), request.ShouldIncludeErrorDetail(), request.GetApiVersion());
      throw new HttpResponseException(request.CreateResponse<WrappedException>(statusCode, wrappedException));
    }

    public static IEdmEntitySet EntitySet(this HttpRequestMessage request) => ((EntitySetSegment) request.ODataProperties().Path.Segments.First<ODataPathSegment>()).EntitySet;

    public static ODataQueryOptions CreateODataQueryOptions(
      this HttpRequestMessage request,
      IEdmEntityType entityType = null)
    {
      entityType = entityType ?? request.EntitySet().EntityType();
      request.PreValidateQuery();
      ODataQueryOptions odataQueryOptions = new ODataQueryOptions(new ODataQueryContext(request.GetModel(), EdmTypeUtils.GetType((IEdmType) entityType), request.ODataProperties().Path), request);
      if (odataQueryOptions.SelectExpand != null)
        odataQueryOptions.SelectExpand.LevelsMaxLiteralExpansionDepth = 1;
      return odataQueryOptions;
    }

    private static void PreValidateQuery(this HttpRequestMessage request)
    {
      IEnumerable<KeyValuePair<string, string>> source = request.GetQueryNameValuePairs().Where<KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, bool>) (d => d.Key.ToLowerInvariant() == "$select" || d.Key.ToLowerInvariant() == "$expand"));
      if (source.Any<KeyValuePair<string, string>>() && source.All<KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, bool>) (d => string.IsNullOrEmpty(d.Value))))
        throw new ODataException(AnalyticsResources.ODATA_SELECT_EXPAND_EMPTY());
    }

    public static List<string> ODataWarnings(this HttpRequestMessage request)
    {
      ArgumentUtility.CheckForNull<HttpRequestMessage>(request, nameof (request));
      object obj;
      List<string> stringList;
      if (request.Properties.TryGetValue("VSTS.Warnings", out obj))
      {
        stringList = obj as List<string>;
      }
      else
      {
        stringList = new List<string>();
        request.Properties["VSTS.Warnings"] = (object) stringList;
      }
      return stringList;
    }

    public static MediaTypeHeaderValue GetMediaTypeHeader(this HttpRequestMessage request)
    {
      HttpHeaderValueCollection<MediaTypeWithQualityHeaderValue> accept = request.Headers.Accept;
      string input = accept != null ? accept.Select<MediaTypeWithQualityHeaderValue, string>((Func<MediaTypeWithQualityHeaderValue, string>) (h => h.ToString())).FirstOrDefault<string>((Func<string, bool>) (h => h.Contains("odata.metadata"))) : (string) null;
      return input == null ? (MediaTypeHeaderValue) null : MediaTypeHeaderValue.Parse(input);
    }
  }
}
