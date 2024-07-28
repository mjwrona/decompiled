// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.HttpRouteValueExtensions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Web.Http.Routing;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class HttpRouteValueExtensions
  {
    public static void AddRouteValueIfNotPresent(
      this HttpRouteValueDictionary dictionary,
      string key,
      object value)
    {
      if (dictionary.ContainsKey(key))
        return;
      dictionary.Add(key, value);
    }

    public static HttpRouteValueDictionary ToRouteDictionary(object values)
    {
      if (values == null)
        return new HttpRouteValueDictionary();
      return values is HttpRouteValueDictionary ? (HttpRouteValueDictionary) values : new HttpRouteValueDictionary(values);
    }
  }
}
