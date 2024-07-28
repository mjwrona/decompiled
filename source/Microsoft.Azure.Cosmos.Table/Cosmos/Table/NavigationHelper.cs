// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.NavigationHelper
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.Azure.Cosmos.Table
{
  internal static class NavigationHelper
  {
    public static readonly char[] SlashAsSplitOptions = "/".ToCharArray();
    public const string Slash = "/";

    internal static StorageUri AppendPathToUri(StorageUri uriList, string relativeUri) => NavigationHelper.AppendPathToUri(uriList, relativeUri, "/");

    internal static StorageUri AppendPathToUri(StorageUri uriList, string relativeUri, string sep) => new StorageUri(NavigationHelper.AppendPathToSingleUri(uriList.PrimaryUri, relativeUri, sep), NavigationHelper.AppendPathToSingleUri(uriList.SecondaryUri, relativeUri, sep));

    internal static Uri AppendPathToSingleUri(Uri uri, string relativeUri) => NavigationHelper.AppendPathToSingleUri(uri, relativeUri, "/");

    internal static Uri AppendPathToSingleUri(Uri uri, string relativeUri, string sep)
    {
      if (uri == (Uri) null || relativeUri.Length == 0)
        return uri;
      sep = Uri.EscapeUriString(sep);
      relativeUri = Uri.EscapeUriString(relativeUri);
      UriBuilder uriBuilder = new UriBuilder(uri);
      uriBuilder.Path += !uriBuilder.Path.EndsWith(sep, StringComparison.Ordinal) ? sep + relativeUri : relativeUri;
      return uriBuilder.Uri;
    }

    internal static StorageUri ParseTableQueryAndVerify(
      StorageUri address,
      out StorageCredentials parsedCredentials)
    {
      return new StorageUri(NavigationHelper.ParseTableQueryAndVerify(address.PrimaryUri, out parsedCredentials), NavigationHelper.ParseTableQueryAndVerify(address.SecondaryUri, out StorageCredentials _));
    }

    private static Uri ParseTableQueryAndVerify(
      Uri address,
      out StorageCredentials parsedCredentials)
    {
      parsedCredentials = (StorageCredentials) null;
      if (address == (Uri) null)
        return (Uri) null;
      IDictionary<string, string> queryParameters = address.IsAbsoluteUri ? HttpWebUtility.ParseQueryString(address.Query) : throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Address '{0}' is a relative address. Only absolute addresses are permitted.", new object[1]
      {
        (object) address.ToString()
      }), nameof (address));
      parsedCredentials = SharedAccessSignatureHelper.ParseQuery(queryParameters);
      return new Uri(address.GetComponents(UriComponents.SchemeAndServer | UriComponents.Path, UriFormat.UriEscaped));
    }

    internal static StorageUri GetServiceClientBaseAddress(
      StorageUri addressUri,
      bool? usePathStyleUris)
    {
      return new StorageUri(NavigationHelper.GetServiceClientBaseAddress(addressUri.PrimaryUri, usePathStyleUris), NavigationHelper.GetServiceClientBaseAddress(addressUri.SecondaryUri, usePathStyleUris));
    }

    internal static Uri GetServiceClientBaseAddress(Uri addressUri, bool? usePathStyleUris)
    {
      if (addressUri == (Uri) null)
        return (Uri) null;
      if (!usePathStyleUris.HasValue)
        usePathStyleUris = new bool?(CommonUtility.UsePathStyleAddressing(addressUri));
      Uri baseUri = new Uri(addressUri.GetComponents(UriComponents.SchemeAndServer, UriFormat.UriEscaped));
      if (!usePathStyleUris.Value)
        return baseUri;
      string[] segments = addressUri.Segments;
      return segments.Length >= 2 ? new Uri(baseUri, segments[1]) : throw new ArgumentException("address", string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Missing account name information inside path style uri. Path style uris should be of the form http://<IPAddressPlusPort>/<accountName>"));
    }

    internal static string GetTableNameFromUri(Uri uri, bool? usePathStyleUris) => NavigationHelper.GetContainerNameFromContainerAddress(uri, usePathStyleUris);

    internal static string GetContainerNameFromContainerAddress(Uri uri, bool? usePathStyleUris)
    {
      if (!usePathStyleUris.HasValue)
        usePathStyleUris = new bool?(CommonUtility.UsePathStyleAddressing(uri));
      if (!usePathStyleUris.Value)
        return uri.AbsolutePath.Substring(1);
      string[] strArray = uri.AbsolutePath.Split(NavigationHelper.SlashAsSplitOptions);
      return strArray.Length >= 3 ? strArray[2] : throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Cannot find account information inside Uri '{0}'", new object[1]
      {
        (object) uri.AbsoluteUri
      }));
    }

    internal static string GetAccountNameFromUri(Uri clientUri, bool? usePathStyleUris)
    {
      if (!usePathStyleUris.HasValue)
        usePathStyleUris = new bool?(CommonUtility.UsePathStyleAddressing(clientUri));
      string[] strArray = clientUri.AbsoluteUri.Split(NavigationHelper.SlashAsSplitOptions, StringSplitOptions.RemoveEmptyEntries);
      if (usePathStyleUris.Value)
        return strArray.Length >= 3 ? strArray[2] : throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Cannot find account information inside Uri '{0}'", new object[1]
        {
          (object) clientUri.AbsoluteUri
        }));
      int length = strArray.Length >= 2 ? strArray[1].IndexOf(".", StringComparison.Ordinal) : throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Cannot find account information inside Uri '{0}'", new object[1]
      {
        (object) clientUri.AbsoluteUri
      }));
      return strArray[1].Substring(0, length);
    }
  }
}
