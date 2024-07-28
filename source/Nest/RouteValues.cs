// Decompiled with JetBrains decompiler
// Type: Nest.RouteValues
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System;
using System.Collections.Generic;

namespace Nest
{
  public class RouteValues : Dictionary<string, IUrlParameter>
  {
    internal bool ContainsId { get; private set; }

    internal ResolvedRouteValues Resolve(IConnectionSettingsValues configurationValues)
    {
      if (this.Count == 0)
        return ResolvedRouteValues.Empty;
      ResolvedRouteValues resolvedRouteValues = new ResolvedRouteValues(this.Count);
      foreach (KeyValuePair<string, IUrlParameter> keyValuePair in (Dictionary<string, IUrlParameter>) this)
      {
        string str = this[keyValuePair.Key].GetString((IConnectionConfigurationValues) configurationValues);
        if (!str.IsNullOrEmpty())
        {
          resolvedRouteValues[keyValuePair.Key] = str;
          if (RouteValues.IsId(keyValuePair.Key))
            this.ContainsId = true;
        }
      }
      return resolvedRouteValues;
    }

    private RouteValues Route(string name, IUrlParameter routeValue, bool required = true)
    {
      if (routeValue == null)
      {
        if (required)
          throw new ArgumentNullException(name, name + " is required to build a url to this API");
        if (!this.ContainsKey(name))
          return this;
        this.Remove(name);
        if (RouteValues.IsId(name))
          this.ContainsId = false;
        return this;
      }
      this[name] = routeValue;
      if (RouteValues.IsId(name))
        this.ContainsId = false;
      return this;
    }

    private static bool IsId(string key) => key.Equals("id", StringComparison.OrdinalIgnoreCase);

    internal RouteValues Required(string route, IUrlParameter value) => this.Route(route, value);

    internal RouteValues Optional(string route, IUrlParameter value) => this.Route(route, value, false);

    internal RouteValues Optional(string route, Metrics value) => this.Route(route, (IUrlParameter) value, false);

    internal RouteValues Optional(string route, IndexMetrics value) => this.Route(route, (IUrlParameter) value, false);

    internal TActual Get<TActual>(string route)
    {
      IUrlParameter urlParameter;
      return this.TryGetValue(route, out urlParameter) && urlParameter != null ? (TActual) urlParameter : default (TActual);
    }
  }
}
