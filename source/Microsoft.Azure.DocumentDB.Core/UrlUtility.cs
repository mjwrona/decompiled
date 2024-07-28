// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.UrlUtility
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Cosmos.Core.Trace;
using Microsoft.Azure.Documents.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;

namespace Microsoft.Azure.Documents
{
  internal static class UrlUtility
  {
    internal static string ConcatenateUrlsString(string baseUrl, params string[] relativeParts)
    {
      StringBuilder stringBuilder = new StringBuilder(UrlUtility.RemoveTrailingSlash(baseUrl));
      foreach (string relativePart in relativeParts)
      {
        stringBuilder.Append(RuntimeConstants.Separators.Url[0]);
        stringBuilder.Append(UrlUtility.RemoveLeadingSlash(relativePart));
      }
      return stringBuilder.ToString();
    }

    internal static string ConcatenateUrlsString(string baseUrl, string relativePart) => UrlUtility.AddTrailingSlash(baseUrl) + UrlUtility.RemoveLeadingSlash(relativePart);

    internal static string ConcatenateUrlsString(Uri baseUrl, string relativePart) => UrlUtility.ConcatenateUrlsString(UrlUtility.GetLeftPartOfPath(baseUrl), relativePart);

    internal static void ExtractTargetInfo(
      Uri uri,
      out string tenantId,
      out string applicationName,
      out string serviceId,
      out string partitionKey,
      out string replicaId)
    {
      if (uri.Segments == null || uri.Segments.Length < 9)
      {
        DefaultTrace.TraceError("Uri {0} is invalid", (object) uri);
        throw new ArgumentException(nameof (uri));
      }
      tenantId = UrlUtility.ExtractTenantIdFromUri(uri);
      applicationName = uri.Segments[2].Substring(0, uri.Segments[2].Length - 1);
      serviceId = uri.Segments[4].Substring(0, uri.Segments[4].Length - 1);
      partitionKey = uri.Segments[6].Substring(0, uri.Segments[6].Length - 1);
      replicaId = uri.Segments[8].Substring(0, uri.Segments[8].Length);
    }

    internal static string ConcatenateUrlsString(Uri baseUrl, Uri relativePart) => relativePart.IsAbsoluteUri ? relativePart.ToString() : UrlUtility.ConcatenateUrlsString(UrlUtility.GetLeftPartOfPath(baseUrl), relativePart.OriginalString);

    internal static Uri ConcatenateUrls(string baseUrl, string relativePart) => new Uri(UrlUtility.ConcatenateUrlsString(baseUrl, relativePart));

    internal static Uri ConcatenateUrls(Uri baseUrl, string relativePart) => new Uri(UrlUtility.ConcatenateUrlsString(baseUrl, relativePart));

    internal static Uri ConcatenateUrls(Uri baseUrl, Uri relativePart) => relativePart.IsAbsoluteUri ? relativePart : new Uri(UrlUtility.ConcatenateUrlsString(baseUrl, relativePart));

    internal static NameValueCollection ParseQuery(string queryString)
    {
      queryString = UrlUtility.RemoveLeadingQuestionMark(queryString);
      NameValueCollection query;
      if (string.IsNullOrEmpty(queryString))
      {
        query = new NameValueCollection(0);
      }
      else
      {
        string[] strArray1 = UrlUtility.SplitAndRemoveEmptyEntries(queryString, new char[1]
        {
          RuntimeConstants.Separators.Query[1]
        });
        query = new NameValueCollection(strArray1.Length);
        for (int index = 0; index < strArray1.Length; ++index)
        {
          string[] strArray2 = UrlUtility.SplitAndRemoveEmptyEntries(strArray1[index], new char[1]
          {
            RuntimeConstants.Separators.Query[2]
          }, 2);
          query.Add(strArray2[0], strArray2.Length > 1 ? strArray2[1] : (string) null);
        }
      }
      return query;
    }

    internal static string CreateQuery(INameValueCollection parsedQuery)
    {
      if (parsedQuery == null)
        return string.Empty;
      StringBuilder stringBuilder = new StringBuilder();
      parsedQuery.Count();
      foreach (string key in (IEnumerable) parsedQuery)
      {
        string str = parsedQuery[key];
        if (!string.IsNullOrEmpty(key))
        {
          if (stringBuilder.Length > 0)
            stringBuilder.Append(RuntimeConstants.Separators.Query[1]);
          stringBuilder.Append(key);
          if (str != null)
          {
            stringBuilder.Append(RuntimeConstants.Separators.Query[2]);
            stringBuilder.Append(str);
          }
        }
      }
      return stringBuilder.ToString();
    }

    internal static Uri SetQuery(Uri url, string query)
    {
      if (url == (Uri) null)
        throw new ArgumentNullException(nameof (url));
      string path;
      UriKind uriKind;
      if (url.IsAbsoluteUri)
      {
        path = url.GetComponents(UriComponents.SchemeAndServer | UriComponents.UserInfo | UriComponents.Path, UriFormat.Unescaped);
        uriKind = UriKind.Absolute;
      }
      else
      {
        uriKind = UriKind.Relative;
        path = url.ToString();
        int startIndex = path.LastIndexOf(RuntimeConstants.Separators.Query[0]);
        if (startIndex >= 0)
          path = path.Remove(startIndex, path.Length - startIndex);
      }
      query = UrlUtility.RemoveLeadingQuestionMark(query);
      return !string.IsNullOrEmpty(query) ? new Uri(UrlUtility.AddTrailingSlash(path) + RuntimeConstants.Separators.Query[0].ToString() + query, uriKind) : new Uri(UrlUtility.AddTrailingSlash(path), uriKind);
    }

    internal static string RemoveLeadingQuestionMark(string path) => string.IsNullOrEmpty(path) || (int) path[0] != (int) RuntimeConstants.Separators.Query[0] ? path : path.Remove(0, 1);

    internal static string RemoveTrailingSlash(string path)
    {
      if (string.IsNullOrEmpty(path))
        return path;
      int length = path.Length;
      return (int) path[length - 1] == (int) RuntimeConstants.Separators.Url[0] ? path.Remove(length - 1, 1) : path;
    }

    internal static string RemoveTrailingSlashes(string path) => string.IsNullOrEmpty(path) ? path : path.TrimEnd(RuntimeConstants.Separators.Url);

    internal static string RemoveLeadingSlash(string path) => string.IsNullOrEmpty(path) || (int) path[0] != (int) RuntimeConstants.Separators.Url[0] ? path : path.Remove(0, 1);

    internal static string RemoveLeadingSlashes(string path) => string.IsNullOrEmpty(path) ? path : path.TrimStart(RuntimeConstants.Separators.Url);

    internal static string AddTrailingSlash(string path)
    {
      if (string.IsNullOrEmpty(path))
        path = new string(RuntimeConstants.Separators.Url);
      else if ((int) path[path.Length - 1] != (int) RuntimeConstants.Separators.Url[0])
        path += RuntimeConstants.Separators.Url[0].ToString();
      return path;
    }

    internal static string AddLeadingSlash(string path)
    {
      if (string.IsNullOrEmpty(path))
        path = new string(RuntimeConstants.Separators.Url);
      else if ((int) path[0] != (int) RuntimeConstants.Separators.Url[0])
        path = RuntimeConstants.Separators.Url[0].ToString() + path;
      return path;
    }

    internal static string GetLeftPartOfAuthority(Uri uri) => uri.GetLeftPart(UriPartial.Authority);

    internal static string GetLeftPartOfPath(Uri uri) => uri.GetLeftPart(UriPartial.Path);

    public static string[] SplitAndRemoveEmptyEntries(string str, char[] seperators) => UrlUtility.SplitAndRemoveEmptyEntries(str, seperators, int.MaxValue);

    public static string[] SplitAndRemoveEmptyEntries(string str, char[] seperators, int count) => str.Split(seperators, count, StringSplitOptions.RemoveEmptyEntries);

    internal static string ExtractIdFromItemUri(Uri uri, int i) => UrlUtility.RemoveTrailingSlash(uri.Segments[i]);

    internal static string ExtractTenantIdFromUri(Uri uri)
    {
      string dnsSafeHost = uri.DnsSafeHost;
      int length = dnsSafeHost.IndexOf('.');
      return length != -1 ? dnsSafeHost.Substring(0, length) : dnsSafeHost;
    }

    internal static string ExtractIdOrFullNameFromUri(string path, out bool isNameBased)
    {
      string resourceIdOrFullName;
      return PathsHelper.TryParsePathSegments(path, out bool _, out string _, out resourceIdOrFullName, out isNameBased) ? resourceIdOrFullName : (string) null;
    }

    internal static string ExtractIdFromItemUri(Uri uri) => UrlUtility.RemoveTrailingSlash(uri.Segments[uri.Segments.Length - 1]);

    internal static string ExtractIdFromCollectionUri(Uri uri) => UrlUtility.RemoveTrailingSlash(uri.Segments[uri.Segments.Length - 2]);

    internal static string ExtractItemIdAndCollectionIdFromUri(Uri uri, out string collectionId)
    {
      collectionId = UrlUtility.RemoveTrailingSlash(uri.Segments[uri.Segments.Length - 3]);
      return UrlUtility.RemoveTrailingSlash(uri.Segments[uri.Segments.Length - 1]);
    }

    internal static string ExtractFileNameFromUri(Uri uri) => UrlUtility.RemoveTrailingSlash(uri.Segments[uri.Segments.Length - 1]);

    internal static bool IsLocalHostUri(Uri uri)
    {
      IPAddress address;
      if (!IPAddress.TryParse(uri.DnsSafeHost, out address))
        throw new ArgumentException(nameof (uri));
      if (IPAddress.IsLoopback(address))
        return true;
      List<IPAddress> ipAddressList = new List<IPAddress>();
      foreach (NetworkInterface networkInterface in NetworkInterface.GetAllNetworkInterfaces())
      {
        foreach (IPAddressInformation unicastAddress in networkInterface.GetIPProperties().UnicastAddresses)
        {
          if (unicastAddress.Address.Equals((object) address))
            return true;
        }
      }
      return false;
    }

    internal static bool IsAstoriaUrl(Uri url) => url.AbsolutePath.IndexOf(RuntimeConstants.Separators.Parenthesis[0]) != -1 && url.AbsolutePath.IndexOf(RuntimeConstants.Separators.Parenthesis[1]) != -1;

    internal static Uri ToNativeUrl(Uri astoriaUrl)
    {
      Uri baseUri = (Uri) null;
      if (astoriaUrl.IsAbsoluteUri)
        baseUri = new Uri(UrlUtility.GetLeftPartOfAuthority(astoriaUrl));
      string query = astoriaUrl.Query;
      string absolutePath = astoriaUrl.AbsolutePath;
      UrlUtility.Element[] elementArray = (UrlUtility.Element[]) null;
      ref UrlUtility.Element[] local = ref elementArray;
      if (!UrlUtility.ParseAstoriaUrl(absolutePath, out local))
        return astoriaUrl;
      List<string> stringList = new List<string>();
      foreach (UrlUtility.Element element in elementArray)
      {
        if (!string.IsNullOrEmpty(element.Name))
          stringList.Add(element.Name);
        if (!string.IsNullOrEmpty(element.Id))
        {
          string str = element.Id.Trim(RuntimeConstants.Separators.Quote);
          if (str.StartsWith("urn:uuid:", StringComparison.Ordinal))
            str = str.Substring("urn:uuid:".Length);
          stringList.Add(str);
        }
      }
      string baseUrl = stringList[0];
      stringList.RemoveAt(0);
      string[] array = stringList.ToArray();
      string str1 = UrlUtility.ConcatenateUrlsString(baseUrl, array);
      Uri url = !(baseUri != (Uri) null) ? new Uri(str1, UriKind.Relative) : new Uri(baseUri, str1);
      if (!string.IsNullOrEmpty(query))
        UrlUtility.SetQuery(url, query);
      return url;
    }

    private static bool ParseAstoriaUrl(string astoriaUrl, out UrlUtility.Element[] urlElements)
    {
      urlElements = (UrlUtility.Element[]) null;
      if (astoriaUrl == null)
        return false;
      string[] strArray = UrlUtility.SplitAndRemoveEmptyEntries(astoriaUrl, RuntimeConstants.Separators.Url);
      if (strArray == null || strArray.Length < 1)
        return false;
      List<UrlUtility.Element> elementList = new List<UrlUtility.Element>();
      foreach (string urlPart in strArray)
      {
        string name;
        string id;
        if (!UrlUtility.ParseAstoriaUrlPart(urlPart, out name, out id))
          return false;
        elementList.Add(new UrlUtility.Element(name, id));
      }
      urlElements = elementList.ToArray();
      return true;
    }

    private static bool ParseAstoriaUrlPart(string urlPart, out string name, out string id)
    {
      name = (string) null;
      id = (string) null;
      int num1 = urlPart.IndexOf(RuntimeConstants.Separators.Parenthesis[0]);
      int num2 = urlPart.IndexOf(RuntimeConstants.Separators.Parenthesis[1]);
      if (num1 == -1)
      {
        if (num2 != -1)
          return false;
        name = urlPart;
      }
      else
      {
        if (num2 == -1 || num2 != urlPart.Length - 1)
          return false;
        name = urlPart.Substring(0, num1);
        id = urlPart.Substring(num1, num2 - num1).Trim(RuntimeConstants.Separators.Parenthesis);
      }
      return true;
    }

    private class Element
    {
      public Element()
      {
      }

      public Element(string name, string id)
      {
        this.Name = name;
        this.Id = id;
      }

      public string Name { get; set; }

      public string Id { get; set; }
    }
  }
}
