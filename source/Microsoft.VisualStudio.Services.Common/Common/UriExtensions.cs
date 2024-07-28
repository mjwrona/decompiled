// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.UriExtensions
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace Microsoft.VisualStudio.Services.Common
{
  public static class UriExtensions
  {
    public static Uri AppendQuery(this Uri uri, string name, string value)
    {
      ArgumentUtility.CheckForNull<Uri>(uri, nameof (uri));
      ArgumentUtility.CheckStringForNullOrEmpty(name, nameof (name));
      ArgumentUtility.CheckStringForNullOrEmpty(value, nameof (value));
      StringBuilder builder = new StringBuilder(uri.Query.TrimStart('?'));
      UriExtensions.AppendSingleQueryValue(builder, name, value);
      return new UriBuilder(uri)
      {
        Query = builder.ToString()
      }.Uri;
    }

    public static Uri AppendQuery(
      this Uri uri,
      IEnumerable<KeyValuePair<string, string>> queryValues)
    {
      ArgumentUtility.CheckForNull<Uri>(uri, nameof (uri));
      ArgumentUtility.CheckForNull<IEnumerable<KeyValuePair<string, string>>>(queryValues, nameof (queryValues));
      StringBuilder builder = new StringBuilder(uri.Query.TrimStart('?'));
      foreach (KeyValuePair<string, string> queryValue in queryValues)
        UriExtensions.AppendSingleQueryValue(builder, queryValue.Key, queryValue.Value);
      return new UriBuilder(uri)
      {
        Query = builder.ToString()
      }.Uri;
    }

    public static Uri AppendQuery(this Uri uri, NameValueCollection queryValues)
    {
      ArgumentUtility.CheckForNull<Uri>(uri, nameof (uri));
      ArgumentUtility.CheckForNull<NameValueCollection>(queryValues, nameof (queryValues));
      StringBuilder builder = new StringBuilder(uri.Query.TrimStart('?'));
      foreach (string queryValue in (NameObjectCollectionBase) queryValues)
        UriExtensions.AppendSingleQueryValue(builder, queryValue, queryValues[queryValue]);
      return new UriBuilder(uri)
      {
        Query = builder.ToString()
      }.Uri;
    }

    public static void Add<T>(
      this IList<KeyValuePair<string, string>> collection,
      string key,
      T value,
      Func<T, string> convert = null)
    {
      IList<KeyValuePair<string, string>> collection1 = collection;
      string key1 = key;
      List<T> values = new List<T>();
      values.Add(value);
      Func<T, string> convert1 = convert;
      collection1.AddMultiple<T>(key1, (IEnumerable<T>) values, convert1);
    }

    public static void AddMultiple<T>(
      this IList<KeyValuePair<string, string>> collection,
      string key,
      IEnumerable<T> values,
      Func<T, string> convert)
    {
      ArgumentUtility.CheckForNull<IList<KeyValuePair<string, string>>>(collection, nameof (collection));
      ArgumentUtility.CheckStringForNullOrEmpty(key, "name");
      if (convert == null)
        convert = (Func<T, string>) (val => val.ToString());
      if (values == null || !values.Any<T>())
        return;
      StringBuilder stringBuilder = new StringBuilder();
      KeyValuePair<string, string> keyValuePair = collection.FirstOrDefault<KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, bool>) (kvp => kvp.Key.Equals(key)));
      if (keyValuePair.Key == key)
      {
        collection.Remove(keyValuePair);
        stringBuilder.Append(keyValuePair.Value);
      }
      foreach (T obj in values)
      {
        if (stringBuilder.Length > 0)
          stringBuilder.Append(",");
        stringBuilder.Append(convert(obj));
      }
      collection.Add(new KeyValuePair<string, string>(key, stringBuilder.ToString()));
    }

    public static void Add(
      this IList<KeyValuePair<string, string>> collection,
      string key,
      string value)
    {
      collection.AddMultiple(key, (IEnumerable<string>) new string[1]
      {
        value
      });
    }

    public static void AddMultiple(
      this IList<KeyValuePair<string, string>> collection,
      string key,
      IEnumerable<string> values)
    {
      collection.AddMultiple<string>(key, values, (Func<string, string>) (val => val));
    }

    public static void AddMultiple<T>(
      this NameValueCollection collection,
      string name,
      IEnumerable<T> values,
      Func<T, string> convert)
    {
      ArgumentUtility.CheckForNull<NameValueCollection>(collection, nameof (collection));
      ArgumentUtility.CheckStringForNullOrEmpty(name, nameof (name));
      if (convert == null)
        convert = (Func<T, string>) (val => val.ToString());
      if (values == null)
        return;
      foreach (T obj in values)
        collection.Add(name, convert(obj));
    }

    public static void AddMultiple(
      this NameValueCollection collection,
      string name,
      IEnumerable<string> values)
    {
      ArgumentUtility.CheckForNull<NameValueCollection>(collection, nameof (collection));
      collection.AddMultiple<string>(name, values, (Func<string, string>) (val => val));
    }

    public static string AbsoluteUri(this Uri uri) => !uri.IsAbsoluteUri ? uri.ToString() : uri.AbsoluteUri;

    private static void AppendSingleQueryValue(StringBuilder builder, string name, string value)
    {
      if (builder.Length > 0)
        builder.Append("&");
      builder.Append(Uri.EscapeDataString(name));
      builder.Append("=");
      builder.Append(Uri.EscapeDataString(value));
    }
  }
}
