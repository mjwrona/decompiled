// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.VssHttpUriUtility
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Microsoft.VisualStudio.Services.WebApi
{
  public static class VssHttpUriUtility
  {
    public static string ReplaceRouteValues(
      string routeTemplate,
      Dictionary<string, object> routeValues,
      bool escapeUri = false,
      bool appendUnusedAsQueryParams = false,
      bool requireExplicitRouteParams = false)
    {
      RouteReplacementOptions routeReplacementOptions = (RouteReplacementOptions) ((escapeUri ? 1 : 0) | (appendUnusedAsQueryParams ? 2 : 0) | (requireExplicitRouteParams ? 4 : 0));
      return VssHttpUriUtility.ReplaceRouteValues(routeTemplate, routeValues, routeReplacementOptions);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static string ReplaceRouteValues(
      string routeTemplate,
      Dictionary<string, object> routeValues,
      RouteReplacementOptions routeReplacementOptions)
    {
      StringBuilder stringBuilder1 = new StringBuilder();
      StringBuilder stringBuilder2 = new StringBuilder();
      int startIndex = -1;
      int length = 0;
      bool flag1 = false;
      HashSet<string> stringSet = new HashSet<string>((IEnumerable<string>) routeValues.Keys, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      Dictionary<string, object> dictionary = new Dictionary<string, object>((IDictionary<string, object>) routeValues, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      for (int index = 0; index < routeTemplate.Length; ++index)
      {
        char ch = routeTemplate[index];
        if (flag1)
        {
          if (ch == '}')
          {
            flag1 = false;
            string key = routeTemplate.Substring(startIndex, length);
            length = 0;
            if (key.StartsWith("*"))
            {
              if (!routeReplacementOptions.HasFlag((Enum) RouteReplacementOptions.WildcardAsQueryParams))
                key = key.Substring(1);
              else
                continue;
            }
            object obj;
            if (dictionary.TryGetValue(key, out obj))
            {
              if (obj != null)
              {
                stringBuilder2.Append(obj.ToString());
                stringSet.Remove(key);
              }
            }
            else if (routeReplacementOptions.HasFlag((Enum) RouteReplacementOptions.RequireExplicitRouteParams))
              throw new ArgumentException("Missing route param " + key);
          }
          else
            ++length;
        }
        else
        {
          switch (ch)
          {
            case '/':
              if (stringBuilder2.Length > 0)
              {
                stringBuilder1.Append('/');
                stringBuilder1.Append(stringBuilder2.ToString());
                stringBuilder2.Clear();
                continue;
              }
              continue;
            case '{':
              if (index + 1 < routeTemplate.Length && routeTemplate[index + 1] == '{')
              {
                stringBuilder2.Append(ch);
                ++index;
                continue;
              }
              flag1 = true;
              startIndex = index + 1;
              continue;
            case '}':
              stringBuilder2.Append(ch);
              if (index + 1 < routeTemplate.Length && routeTemplate[index + 1] == '}')
              {
                ++index;
                continue;
              }
              continue;
            default:
              stringBuilder2.Append(ch);
              continue;
          }
        }
      }
      if (stringBuilder2.Length > 0)
      {
        stringBuilder1.Append('/');
        stringBuilder1.Append(stringBuilder2.ToString());
      }
      if (routeReplacementOptions.HasFlag((Enum) RouteReplacementOptions.EscapeUri))
        stringBuilder1 = new StringBuilder(Uri.EscapeUriString(stringBuilder1.ToString()));
      if (routeReplacementOptions.HasFlag((Enum) RouteReplacementOptions.AppendUnusedAsQueryParams) && stringSet.Count > 0)
      {
        bool flag2 = true;
        foreach (string str in stringSet)
        {
          object obj;
          if (dictionary.TryGetValue(str, out obj) && obj != null)
          {
            stringBuilder1.Append(flag2 ? '?' : '&');
            flag2 = false;
            stringBuilder1.Append(Uri.EscapeDataString(str));
            stringBuilder1.Append('=');
            stringBuilder1.Append(Uri.EscapeDataString(obj.ToString()));
          }
        }
      }
      return stringBuilder1.ToString();
    }

    public static Dictionary<string, object> ToRouteDictionary(
      object routeValues,
      string area,
      string resourceName)
    {
      Dictionary<string, object> routeDictionary = VssHttpUriUtility.ToRouteDictionary(routeValues);
      VssHttpUriUtility.AddRouteValueIfNotPresent(routeDictionary, nameof (area), (object) area);
      VssHttpUriUtility.AddRouteValueIfNotPresent(routeDictionary, "resource", (object) resourceName);
      return routeDictionary;
    }

    public static Uri ConcatUri(Uri baseUri, string relativeUri)
    {
      StringBuilder stringBuilder = new StringBuilder(baseUri.GetLeftPart(UriPartial.Path).TrimEnd('/'));
      stringBuilder.Append('/');
      stringBuilder.Append(relativeUri.TrimStart('/'));
      stringBuilder.Append(baseUri.Query);
      return new Uri(stringBuilder.ToString());
    }

    public static Dictionary<string, object> ToRouteDictionary(object values)
    {
      if (values == null)
        return new Dictionary<string, object>();
      if (values is Dictionary<string, object>)
        return (Dictionary<string, object>) values;
      Dictionary<string, object> routeDictionary = new Dictionary<string, object>();
      foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(values))
        routeDictionary[property.Name] = property.GetValue(values);
      return routeDictionary;
    }

    private static void AddRouteValueIfNotPresent(
      Dictionary<string, object> dictionary,
      string key,
      object value)
    {
      if (dictionary.ContainsKey(key))
        return;
      dictionary.Add(key, value);
    }
  }
}
