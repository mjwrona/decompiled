// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Routing.GitRouteExtensions
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System.Web.Routing;

namespace Microsoft.TeamFoundation.Git.Server.Routing
{
  public static class GitRouteExtensions
  {
    public static T GetRouteValue<T>(this RouteData routeData, string key) => routeData.GetRouteValue<T>(key, default (T));

    public static T GetRouteValue<T>(this RouteData routeData, string key, T defaultValue)
    {
      object obj;
      if (!routeData.Values.TryGetValue(key, out obj))
        routeData.DataTokens.TryGetValue(key, out obj);
      return obj == null ? defaultValue : (T) obj;
    }

    public static T GetValue<T>(this RouteValueDictionary values, string key) => values.GetValue<T>(key, default (T));

    public static T GetValue<T>(this RouteValueDictionary values, string key, T defaultValue)
    {
      object obj;
      return !values.TryGetValue(key, out obj) ? defaultValue : (T) obj;
    }
  }
}
