// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.WebApiRouteAuditingProvider
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Routing;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class WebApiRouteAuditingProvider : ITfsRouteAuditingProvider
  {
    public IEnumerable<EndpointDescription> GetEndpoints(IVssRequestContext requestContext)
    {
      HttpConfiguration httpConfiguration = WebApiConfiguration.GetHttpConfiguration(requestContext);
      IDictionary<string, HttpControllerDescriptor> controllerMap = httpConfiguration.Services.GetHttpControllerSelector().GetControllerMapping();
      Dictionary<WebApiRouteAuditingProvider.AreaResourceKey, HttpControllerDescriptor> arsMap = new Dictionary<WebApiRouteAuditingProvider.AreaResourceKey, HttpControllerDescriptor>(WebApiRouteAuditingProvider.AreaResourceKey.Comparer);
      string[] routingKeys = new string[3]
      {
        "controller",
        "area",
        "resource"
      };
      foreach (HttpControllerDescriptor controllerDescriptor in (IEnumerable<HttpControllerDescriptor>) controllerMap.Values)
      {
        if (((IEnumerable<object>) controllerDescriptor.ControllerType.GetCustomAttributes(typeof (VersionedApiControllerCustomNameAttribute), false)).FirstOrDefault<object>() is VersionedApiControllerCustomNameAttribute customNameAttribute)
          arsMap[customNameAttribute.ResourceName != null ? new WebApiRouteAuditingProvider.AreaResourceKey(customNameAttribute.Area, customNameAttribute.ResourceName) : new WebApiRouteAuditingProvider.AreaResourceKey(customNameAttribute.Area, "<null>")] = controllerDescriptor;
      }
      foreach (IHttpRoute route in httpConfiguration.Routes)
      {
        Dictionary<string, string> dictionary = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        EndpointDescription endpoint = new EndpointDescription()
        {
          RouteType = "WebAPI",
          RoutePath = route.RouteTemplate
        };
        foreach (KeyValuePair<string, object> constraint in (IEnumerable<KeyValuePair<string, object>>) route.Constraints)
        {
          if (constraint.Value is string)
            endpoint.RoutePath = endpoint.RoutePath.Replace("{" + constraint.Key + "}", ((string) constraint.Value).ToLowerInvariant());
          if (constraint.Key == "TFS_HostType")
          {
            TfsApiHostTypeConstraint hostTypeConstraint = (TfsApiHostTypeConstraint) constraint.Value;
            endpoint.HostTypes = hostTypeConstraint.HostType.ToString();
          }
          foreach (string x in routingKeys)
          {
            if (StringComparer.OrdinalIgnoreCase.Equals(x, constraint.Key) && constraint.Value is string)
              dictionary[constraint.Key] = (string) constraint.Value;
          }
        }
        foreach (KeyValuePair<string, object> keyValuePair in (IEnumerable<KeyValuePair<string, object>>) route.Defaults)
        {
          foreach (string str in routingKeys)
          {
            if (!dictionary.ContainsKey(str) && StringComparer.OrdinalIgnoreCase.Equals(str, keyValuePair.Key) && keyValuePair.Value is string)
              dictionary[keyValuePair.Key] = (string) keyValuePair.Value;
          }
        }
        HttpControllerDescriptor controllerDescriptor = (HttpControllerDescriptor) null;
        if (dictionary.ContainsKey("controller"))
          controllerMap.TryGetValue(dictionary["controller"], out controllerDescriptor);
        if (controllerDescriptor == null && dictionary.ContainsKey("area") && dictionary.ContainsKey("resource") && !arsMap.TryGetValue(new WebApiRouteAuditingProvider.AreaResourceKey(dictionary["area"], dictionary["resource"]), out controllerDescriptor))
          controllerMap.TryGetValue(dictionary["area"] + dictionary["resource"], out controllerDescriptor);
        if (controllerDescriptor != null)
        {
          endpoint.ControllerType = controllerDescriptor.ControllerType.FullName;
          string name = controllerDescriptor.ControllerType.Name;
          if (name.EndsWith("Controller"))
          {
            string str = name.Substring(0, name.Length - "Controller".Length);
            endpoint.RoutePath = endpoint.RoutePath.Replace("{controller}", str.ToLowerInvariant());
          }
        }
        yield return endpoint;
      }
    }

    private struct AreaResourceKey
    {
      public readonly string Area;
      public readonly string Resource;
      public static readonly IEqualityComparer<WebApiRouteAuditingProvider.AreaResourceKey> Comparer = (IEqualityComparer<WebApiRouteAuditingProvider.AreaResourceKey>) new WebApiRouteAuditingProvider.AreaResourceKey.AreaResourceKeyComparer();

      public AreaResourceKey(string area, string resource)
      {
        ArgumentUtility.CheckStringForNullOrEmpty(area, nameof (area));
        ArgumentUtility.CheckStringForNullOrEmpty(resource, nameof (resource));
        this.Area = area;
        this.Resource = resource;
      }

      private class AreaResourceKeyComparer : 
        IEqualityComparer<WebApiRouteAuditingProvider.AreaResourceKey>
      {
        public bool Equals(
          WebApiRouteAuditingProvider.AreaResourceKey x,
          WebApiRouteAuditingProvider.AreaResourceKey y)
        {
          return StringComparer.OrdinalIgnoreCase.Equals(x.Area, y.Area) && StringComparer.OrdinalIgnoreCase.Equals(x.Resource, y.Resource);
        }

        public int GetHashCode(WebApiRouteAuditingProvider.AreaResourceKey obj) => StringComparer.OrdinalIgnoreCase.GetHashCode(obj.Area) ^ StringComparer.OrdinalIgnoreCase.GetHashCode(obj.Resource);
      }
    }
  }
}
