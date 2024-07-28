// Decompiled with JetBrains decompiler
// Type: Nest.ApiUrls
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Nest
{
  internal class ApiUrls
  {
    private static readonly RouteValues EmptyRouteValues = new RouteValues();
    private readonly string _errorMessageSuffix;
    private readonly string _fixedUrl;

    public Dictionary<int, List<UrlLookup>> Routes { get; }

    internal ApiUrls(string[] routes)
    {
      if (routes == null || routes.Length == 0)
        throw new ArgumentException("urls is null or empty", nameof (routes));
      if (routes.Length == 1 && !routes[0].Contains("{"))
      {
        this._fixedUrl = routes[0];
      }
      else
      {
        foreach (string route in routes)
        {
          int key = route.Count<char>((Func<char, bool>) (c => c.Equals('{')));
          if (this.Routes == null)
            this.Routes = new Dictionary<int, List<UrlLookup>>();
          if (this.Routes.ContainsKey(key))
            this.Routes[key].Add(new UrlLookup(route));
          else
            this.Routes.Add(key, new List<UrlLookup>()
            {
              new UrlLookup(route)
            });
        }
      }
      this._errorMessageSuffix = string.Join(",", routes);
      if (this.Routes != null)
        return;
      this._fixedUrl = routes[0];
    }

    public string Resolve(RouteValues routeValues, IConnectionSettingsValues settings)
    {
      if (this._fixedUrl != null)
        return this._fixedUrl;
      ResolvedRouteValues values = routeValues.Resolve(settings);
      List<UrlLookup> urlLookupList;
      if (!this.Routes.TryGetValue(values.Count, out urlLookupList))
        throw new Exception(string.Format("No route taking {0} parameters{1}", (object) values.Count, (object) this._errorMessageSuffix));
      if (urlLookupList.Count == 1)
        return urlLookupList[0].ToUrl(values);
      foreach (UrlLookup urlLookup in urlLookupList)
      {
        if (urlLookup.Matches(values))
          return urlLookup.ToUrl(values);
      }
      throw new Exception(string.Format("No route taking {0} parameters{1}", (object) routeValues.Count, (object) this._errorMessageSuffix));
    }
  }
}
