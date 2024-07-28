// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Extensions.HttpRequestMessageExtensions
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.AspNet.OData.Formatter;
using Microsoft.AspNet.OData.Formatter.Deserialization;
using Microsoft.AspNet.OData.Formatter.Serialization;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNet.OData.Routing.Conventions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData;
using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace Microsoft.AspNet.OData.Extensions
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class HttpRequestMessageExtensions
  {
    private const string PropertiesKey = "Microsoft.AspNet.OData.Properties";
    private const string RequestContainerKey = "Microsoft.AspNet.OData.RequestContainer";
    private const string RequestScopeKey = "Microsoft.AspNet.OData.RequestScope";

    public static HttpRequestMessageProperties ODataProperties(this HttpRequestMessage request)
    {
      if (request == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (request));
      object obj;
      HttpRequestMessageProperties messageProperties;
      if (request.Properties.TryGetValue("Microsoft.AspNet.OData.Properties", out obj))
      {
        messageProperties = obj as HttpRequestMessageProperties;
      }
      else
      {
        messageProperties = new HttpRequestMessageProperties(request);
        request.Properties["Microsoft.AspNet.OData.Properties"] = (object) messageProperties;
      }
      return messageProperties;
    }

    public static HttpResponseMessage CreateErrorResponse(
      this HttpRequestMessage request,
      HttpStatusCode statusCode,
      ODataError oDataError)
    {
      if (request.ShouldIncludeErrorDetail())
        return request.CreateResponse<ODataError>(statusCode, oDataError);
      return request.CreateResponse<ODataError>(statusCode, new ODataError()
      {
        ErrorCode = oDataError.ErrorCode,
        Message = oDataError.Message
      });
    }

    public static ETag GetETag(
      this HttpRequestMessage request,
      EntityTagHeaderValue entityTagHeaderValue)
    {
      if (request == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (request));
      if (entityTagHeaderValue != null)
      {
        if (entityTagHeaderValue.Equals((object) EntityTagHeaderValue.Any))
          return new ETag() { IsAny = true };
        IList<object> second = (IList<object>) ((request.GetConfiguration() ?? throw Microsoft.AspNet.OData.Common.Error.InvalidOperation(SRResources.RequestMustContainConfiguration)).GetETagHandler().ParseETag(entityTagHeaderValue) ?? (IDictionary<string, object>) new Dictionary<string, object>()).Select<KeyValuePair<string, object>, object>((Func<KeyValuePair<string, object>, object>) (property => property.Value)).AsList<object>();
        ODataPath path = request.ODataProperties().Path;
        IEdmModel model = request.GetModel();
        IEdmNavigationSource navigationSource = path.NavigationSource;
        if (model != null && navigationSource != null)
        {
          IList<IEdmStructuralProperty> list = (IList<IEdmStructuralProperty>) model.GetConcurrencyProperties(navigationSource).ToList<IEdmStructuralProperty>();
          IList<string> first = (IList<string>) list.OrderBy<IEdmStructuralProperty, string>((Func<IEdmStructuralProperty, string>) (c => c.Name)).Select<IEdmStructuralProperty, string>((Func<IEdmStructuralProperty, string>) (c => c.Name)).AsList<string>();
          ETag etag = new ETag();
          if (second.Count != first.Count)
            etag.IsWellFormed = false;
          foreach (KeyValuePair<string, object> keyValuePair in first.Zip<string, object, KeyValuePair<string, object>>((IEnumerable<object>) second, (Func<string, object, KeyValuePair<string, object>>) ((name, value) => new KeyValuePair<string, object>(name, value))))
          {
            KeyValuePair<string, object> nameValue = keyValuePair;
            Type clrType = EdmLibHelpers.GetClrType(list.SingleOrDefault<IEdmStructuralProperty>((Func<IEdmStructuralProperty, bool>) (e => e.Name == nameValue.Key)).Type, model);
            if (nameValue.Value != null)
            {
              Type type = nameValue.Value.GetType();
              etag[nameValue.Key] = type != clrType ? Convert.ChangeType(nameValue.Value, clrType, (IFormatProvider) CultureInfo.InvariantCulture) : nameValue.Value;
            }
            else
              etag[nameValue.Key] = nameValue.Value;
          }
          return etag;
        }
      }
      return (ETag) null;
    }

    public static ETag<TEntity> GetETag<TEntity>(
      this HttpRequestMessage request,
      EntityTagHeaderValue entityTagHeaderValue)
    {
      ETag etag1 = request.GetETag(entityTagHeaderValue);
      if (etag1 == null)
        return (ETag<TEntity>) null;
      ETag<TEntity> etag2 = new ETag<TEntity>();
      etag2.ConcurrencyProperties = etag1.ConcurrencyProperties;
      etag2.IsWellFormed = etag1.IsWellFormed;
      etag2.IsAny = etag1.IsAny;
      return etag2;
    }

    public static Uri GetNextPageLink(this HttpRequestMessage request, int pageSize) => request.GetNextPageLink(pageSize, (object) null, (Func<object, string>) null);

    public static Uri GetNextPageLink(
      this HttpRequestMessage request,
      int pageSize,
      object instance,
      Func<object, string> objToSkipTokenValue)
    {
      CompatibilityOptions options = request != null && !(request.RequestUri == (Uri) null) ? request.GetCompatibilityOptions() : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (request));
      return GetNextPageHelper.GetNextPageLink(request.RequestUri, request.GetQueryNameValuePairs(), pageSize, instance, objToSkipTokenValue, options);
    }

    public static IServiceProvider GetRequestContainer(this HttpRequestMessage request)
    {
      if (request == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (request));
      object obj;
      return request.Properties.TryGetValue("Microsoft.AspNet.OData.RequestContainer", out obj) ? (IServiceProvider) obj : request.CreateRequestContainer((string) null);
    }

    public static IServiceProvider CreateRequestContainer(
      this HttpRequestMessage request,
      string routeName)
    {
      IServiceScope serviceScope = !request.Properties.ContainsKey("Microsoft.AspNet.OData.RequestContainer") ? request.CreateRequestScope(routeName) : throw Microsoft.AspNet.OData.Common.Error.InvalidOperation(SRResources.RequestContainerAlreadyExists);
      IServiceProvider serviceProvider = serviceScope.ServiceProvider;
      request.Properties["Microsoft.AspNet.OData.RequestScope"] = (object) serviceScope;
      request.Properties["Microsoft.AspNet.OData.RequestContainer"] = (object) serviceProvider;
      return serviceProvider;
    }

    public static void DeleteRequestContainer(this HttpRequestMessage request, bool dispose)
    {
      object obj;
      if (!request.Properties.TryGetValue("Microsoft.AspNet.OData.RequestScope", out obj))
        return;
      IServiceScope serviceScope = (IServiceScope) obj;
      request.Properties.Remove("Microsoft.AspNet.OData.RequestScope");
      request.Properties.Remove("Microsoft.AspNet.OData.RequestContainer");
      if (!dispose)
        return;
      serviceScope.Dispose();
    }

    public static IEdmModel GetModel(this HttpRequestMessage request) => request != null ? ServiceProviderServiceExtensions.GetRequiredService<IEdmModel>(request.GetRequestContainer()) : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (request));

    public static ODataMessageWriterSettings GetWriterSettings(this HttpRequestMessage request) => request != null ? ServiceProviderServiceExtensions.GetRequiredService<ODataMessageWriterSettings>(request.GetRequestContainer()) : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (request));

    public static ODataMessageReaderSettings GetReaderSettings(this HttpRequestMessage request) => request != null ? ServiceProviderServiceExtensions.GetRequiredService<ODataMessageReaderSettings>(request.GetRequestContainer()) : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (request));

    public static IODataPathHandler GetPathHandler(this HttpRequestMessage request) => request != null ? ServiceProviderServiceExtensions.GetRequiredService<IODataPathHandler>(request.GetRequestContainer()) : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (request));

    public static ODataSerializerProvider GetSerializerProvider(this HttpRequestMessage request) => request != null ? ServiceProviderServiceExtensions.GetRequiredService<ODataSerializerProvider>(request.GetRequestContainer()) : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (request));

    public static ODataDeserializerProvider GetDeserializerProvider(this HttpRequestMessage request) => request != null ? ServiceProviderServiceExtensions.GetRequiredService<ODataDeserializerProvider>(request.GetRequestContainer()) : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (request));

    public static IEnumerable<IODataRoutingConvention> GetRoutingConventions(
      this HttpRequestMessage request)
    {
      return request != null ? ServiceProviderServiceExtensions.GetServices<IODataRoutingConvention>(request.GetRequestContainer()) : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (request));
    }

    internal static CompatibilityOptions GetCompatibilityOptions(this HttpRequestMessage request)
    {
      HttpConfiguration configuration = request.GetConfiguration();
      return configuration == null ? CompatibilityOptions.None : configuration.GetCompatibilityOptions();
    }

    private static IServiceScope CreateRequestScope(
      this HttpRequestMessage request,
      string routeName)
    {
      IServiceScope scope = ServiceProviderServiceExtensions.GetRequiredService<IServiceScopeFactory>((request.GetConfiguration() ?? throw Microsoft.AspNet.OData.Common.Error.Argument(nameof (request), SRResources.RequestMustContainConfiguration)).GetODataRootContainer(routeName)).CreateScope();
      if (routeName != null)
        ServiceProviderServiceExtensions.GetRequiredService<HttpRequestScope>(scope.ServiceProvider).HttpRequest = request;
      return scope;
    }
  }
}
