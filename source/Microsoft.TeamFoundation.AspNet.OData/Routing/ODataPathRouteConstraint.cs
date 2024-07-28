// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Routing.ODataPathRouteConstraint
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNet.OData.Routing.Conventions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http.Routing;

namespace Microsoft.AspNet.OData.Routing
{
  public class ODataPathRouteConstraint : IHttpRouteConstraint
  {
    private static readonly string _escapedSlash = Uri.EscapeDataString("/");

    public virtual bool Match(
      HttpRequestMessage request,
      IHttpRoute route,
      string parameterName,
      IDictionary<string, object> values,
      HttpRouteDirection routeDirection)
    {
      if (request == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (request));
      if (values == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (values));
      if (routeDirection != HttpRouteDirection.UriResolution)
        return true;
      ODataPath path = (ODataPath) null;
      object oDataPathString;
      if (values.TryGetValue(ODataRouteConstants.ODataPath, out oDataPathString))
      {
        string leftPart = request.RequestUri.GetLeftPart(UriPartial.Path);
        string query = request.RequestUri.Query;
        path = ODataPathRouteConstraint.GetODataPath(oDataPathString as string, leftPart, query, (Func<IServiceProvider>) (() => request.CreateRequestContainer(this.RouteName)));
      }
      if (path != null)
      {
        HttpRequestMessageProperties messageProperties = request.ODataProperties();
        messageProperties.Path = path;
        messageProperties.RouteName = this.RouteName;
        if (!values.ContainsKey(ODataRouteConstants.Controller))
        {
          string str = this.SelectControllerName(path, request);
          if (str != null)
            values[ODataRouteConstants.Controller] = (object) str;
        }
        return true;
      }
      request.DeleteRequestContainer(true);
      return false;
    }

    protected virtual string SelectControllerName(ODataPath path, HttpRequestMessage request)
    {
      foreach (IODataRoutingConvention routingConvention in request.GetRoutingConventions())
      {
        string str = routingConvention.SelectController(path, request);
        if (str != null)
          return str;
      }
      return (string) null;
    }

    public ODataPathRouteConstraint(string routeName) => this.RouteName = routeName != null ? routeName : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (routeName));

    public string RouteName { get; private set; }

    private static ODataPath GetODataPath(
      string oDataPathString,
      string uriPathString,
      string queryString,
      Func<IServiceProvider> requestContainerFactory)
    {
      try
      {
        string str = uriPathString;
        if (!string.IsNullOrEmpty(oDataPathString))
          str = ODataPathRouteConstraint.RemoveODataPath(str, oDataPathString);
        string odataPath = uriPathString.Substring(str.Length);
        if (!string.IsNullOrEmpty(queryString))
          odataPath += queryString;
        if (str.EndsWith(ODataPathRouteConstraint._escapedSlash, StringComparison.OrdinalIgnoreCase))
          str = str.Substring(0, str.Length - ODataPathRouteConstraint._escapedSlash.Length);
        IServiceProvider serviceProvider = requestContainerFactory();
        return ServiceProviderServiceExtensions.GetRequiredService<IODataPathHandler>(serviceProvider).Parse(str, odataPath, serviceProvider);
      }
      catch (ODataException ex)
      {
        return (ODataPath) null;
      }
    }

    private static string RemoveODataPath(string uriString, string oDataPathString)
    {
      int num1 = uriString.Length - oDataPathString.Length - 1;
      string str = num1 > 0 ? uriString.Substring(0, num1 + 1) : throw Microsoft.AspNet.OData.Common.Error.InvalidOperation(SRResources.RequestUriTooShortForODataPath, (object) uriString, (object) oDataPathString);
      if (string.Equals(uriString.Substring(num1 + 1), oDataPathString, StringComparison.Ordinal))
        return str;
      do
      {
        int num2 = str.LastIndexOf('/', num1 - 1);
        int num3 = str.LastIndexOf(ODataPathRouteConstraint._escapedSlash, num1 - 1, StringComparison.OrdinalIgnoreCase);
        if (num2 > num3)
          num1 = num2;
        else if (num3 >= 0)
          num1 = num3 + 2;
        else
          throw Microsoft.AspNet.OData.Common.Error.InvalidOperation(SRResources.ODataPathNotFound, (object) uriString, (object) oDataPathString);
        str = uriString.Substring(0, num1 + 1);
        if (string.Equals(Uri.UnescapeDataString(uriString.Substring(num1 + 1)), oDataPathString, StringComparison.Ordinal))
          return str;
      }
      while (num1 != 0);
      throw Microsoft.AspNet.OData.Common.Error.InvalidOperation(SRResources.ODataPathNotFound, (object) uriString, (object) oDataPathString);
    }
  }
}
