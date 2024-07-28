// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Core.Util.NavigationHelper
// Assembly: Microsoft.Azure.Storage.Common, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 0978DA65-6954-4A99-9ACB-2EF3D979A5D5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Common.dll

using Microsoft.Azure.Storage.Auth;
using Microsoft.Azure.Storage.Core.Auth;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.Azure.Storage.Core.Util
{
  internal static class NavigationHelper
  {
    public const string RootContainerName = "$root";
    public const string Slash = "/";
    public const string Dot = ".";
    public const char SlashChar = '/';
    public static readonly char[] SlashAsSplitOptions = "/".ToCharArray();
    public static readonly char[] DotAsSplitOptions = ".".ToCharArray();

    internal static string GetContainerName(Uri blobAddress, bool? usePathStyleUris)
    {
      string containerName;
      NavigationHelper.GetContainerNameAndBlobName(blobAddress, usePathStyleUris, out containerName, out string _);
      return containerName;
    }

    internal static string GetBlobName(Uri blobAddress, bool? usePathStyleUris)
    {
      string blobName;
      NavigationHelper.GetContainerNameAndBlobName(blobAddress, usePathStyleUris, out string _, out blobName);
      return Uri.UnescapeDataString(blobName);
    }

    internal static string GetShareName(Uri fileAddress, bool? usePathStyleUris)
    {
      string shareName;
      NavigationHelper.GetShareNameAndFileName(fileAddress, usePathStyleUris, out shareName, out string _);
      return shareName;
    }

    internal static string GetFileName(Uri fileAddress, bool? usePathStyleUris)
    {
      string fileName;
      NavigationHelper.GetShareNameAndFileName(fileAddress, usePathStyleUris, out string _, out fileName);
      return Uri.UnescapeDataString(fileName);
    }

    internal static string GetFileAndDirectoryName(Uri fileAddress, bool? usePathStyleUris)
    {
      CommonUtility.AssertNotNull(nameof (fileAddress), (object) fileAddress);
      if (!usePathStyleUris.HasValue)
        usePathStyleUris = new bool?(CommonUtility.UsePathStyleAddressing(fileAddress));
      string[] segments = fileAddress.Segments;
      int num = usePathStyleUris.Value ? 2 : 1;
      if (segments.Length - 1 < num)
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Invalid file address '{0}', missing share information", (object) fileAddress), nameof (fileAddress));
      return segments.Length - 1 == num ? string.Empty : Uri.UnescapeDataString(string.Concat(((IEnumerable<string>) segments).Skip<string>(num + 1)));
    }

    internal static bool GetBlobParentNameAndAddress(
      StorageUri blobAddress,
      string delimiter,
      bool? usePathStyleUris,
      out string parentName,
      out StorageUri parentAddress)
    {
      CommonUtility.AssertNotNull("blobAbsoluteUriString", (object) blobAddress);
      CommonUtility.AssertNotNullOrEmpty(nameof (delimiter), delimiter);
      parentName = (string) null;
      parentAddress = (StorageUri) null;
      string containerName;
      StorageUri containerUri;
      if (!NavigationHelper.GetContainerNameAndAddress(blobAddress, usePathStyleUris, out containerName, out containerUri))
        return false;
      string str = containerUri.PrimaryUri.MakeRelativeUri(blobAddress.PrimaryUri).OriginalString;
      delimiter = Uri.EscapeUriString(delimiter);
      if (str.EndsWith(delimiter, StringComparison.Ordinal))
        str = str.Substring(0, str.Length - delimiter.Length);
      if (!string.IsNullOrEmpty(str))
      {
        int num = str.LastIndexOf(delimiter, StringComparison.Ordinal);
        if (num <= 0)
        {
          parentName = string.Empty;
          parentAddress = containerUri;
        }
        else
        {
          parentName = Uri.UnescapeDataString(str.Substring(0, num + delimiter.Length)).Substring(containerName.Length + 1);
          parentAddress = NavigationHelper.AppendPathToUri(containerUri, parentName);
        }
      }
      return parentName != null;
    }

    internal static bool GetFileParentNameAndAddress(
      StorageUri fileAddress,
      bool? usePathStyleUris,
      out string parentName,
      out StorageUri parentAddress)
    {
      CommonUtility.AssertNotNull("fileAbsoluteUriString", (object) fileAddress);
      parentName = (string) null;
      parentAddress = (StorageUri) null;
      string shareName;
      StorageUri shareUri;
      NavigationHelper.GetShareNameAndAddress(fileAddress, usePathStyleUris, out shareName, out shareUri);
      string str = shareUri.PrimaryUri.MakeRelativeUri(fileAddress.PrimaryUri).OriginalString;
      if (str.Length > 0 && str[str.Length - 1] == '/')
        str = str.Substring(0, str.Length - 1);
      if (!string.IsNullOrEmpty(str))
      {
        int length = str.LastIndexOf('/');
        if (length <= shareName.Length)
        {
          parentName = string.Empty;
          parentAddress = shareUri;
        }
        else
        {
          parentName = Uri.UnescapeDataString(str.Substring(0, length)).Substring(shareName.Length + 1);
          parentAddress = NavigationHelper.AppendPathToUri(shareUri, parentName);
          int num = parentName.LastIndexOf('/');
          if (num >= 0)
            parentName = parentName.Substring(num + 1);
        }
      }
      return parentName != null;
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

    internal static string GetContainerNameFromContainerAddress(Uri uri, bool? usePathStyleUris)
    {
      if (!usePathStyleUris.HasValue)
        usePathStyleUris = new bool?(CommonUtility.UsePathStyleAddressing(uri));
      if (!usePathStyleUris.Value)
        return uri.AbsolutePath.Substring(1);
      string[] strArray = uri.AbsolutePath.Split(NavigationHelper.SlashAsSplitOptions);
      return strArray.Length >= 3 ? strArray[2] : throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Cannot find account information inside Uri '{0}'", (object) uri.AbsoluteUri));
    }

    internal static string GetQueueNameFromUri(Uri uri, bool? usePathStyleUris) => NavigationHelper.GetContainerNameFromContainerAddress(uri, usePathStyleUris);

    internal static string GetTableNameFromUri(Uri uri, bool? usePathStyleUris) => NavigationHelper.GetContainerNameFromContainerAddress(uri, usePathStyleUris);

    internal static string GetShareNameFromShareAddress(Uri uri, bool? usePathStyleUris) => NavigationHelper.GetContainerNameFromContainerAddress(uri, usePathStyleUris);

    private static bool GetContainerNameAndAddress(
      StorageUri blobAddress,
      bool? usePathStyleUris,
      out string containerName,
      out StorageUri containerUri)
    {
      int num = NavigationHelper.GetContainerNameAndBlobName(blobAddress.PrimaryUri, usePathStyleUris, out containerName, out string _) ? 1 : 0;
      containerUri = NavigationHelper.AppendPathToUri(NavigationHelper.GetServiceClientBaseAddress(blobAddress, usePathStyleUris), containerName);
      return num != 0;
    }

    private static void GetShareNameAndAddress(
      StorageUri fileAddress,
      bool? usePathStyleUris,
      out string shareName,
      out StorageUri shareUri)
    {
      NavigationHelper.GetShareNameAndFileName(fileAddress.PrimaryUri, usePathStyleUris, out shareName, out string _);
      shareUri = NavigationHelper.AppendPathToUri(NavigationHelper.GetServiceClientBaseAddress(fileAddress, usePathStyleUris), shareName);
    }

    private static bool GetContainerNameAndBlobName(
      Uri blobAddress,
      bool? usePathStyleUris,
      out string containerName,
      out string blobName)
    {
      CommonUtility.AssertNotNull(nameof (blobAddress), (object) blobAddress);
      if (!usePathStyleUris.HasValue)
        usePathStyleUris = new bool?(CommonUtility.UsePathStyleAddressing(blobAddress));
      string[] segments = blobAddress.Segments;
      int index = usePathStyleUris.Value ? 2 : 1;
      int sourceIndex = index + 1;
      if (segments.Length - 1 < index)
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Invalid blob address '{0}', missing container information", (object) blobAddress), nameof (blobAddress));
      if (segments.Length - 1 == index)
      {
        string str = segments[index];
        if (str[str.Length - 1] == '/')
        {
          containerName = str.Trim('/');
          blobName = string.Empty;
        }
        else
        {
          containerName = "$root";
          blobName = str;
        }
        return false;
      }
      containerName = segments[index].Trim('/');
      string[] destinationArray = new string[segments.Length - sourceIndex];
      Array.Copy((Array) segments, sourceIndex, (Array) destinationArray, 0, destinationArray.Length);
      blobName = string.Concat(destinationArray);
      return true;
    }

    private static void GetShareNameAndFileName(
      Uri fileAddress,
      bool? usePathStyleUris,
      out string shareName,
      out string fileName)
    {
      CommonUtility.AssertNotNull(nameof (fileAddress), (object) fileAddress);
      if (!usePathStyleUris.HasValue)
        usePathStyleUris = new bool?(CommonUtility.UsePathStyleAddressing(fileAddress));
      string[] segments = fileAddress.Segments;
      int index = usePathStyleUris.Value ? 2 : 1;
      if (segments.Length - 1 < index)
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Invalid file address '{0}', missing share information", (object) fileAddress), nameof (fileAddress));
      if (segments.Length - 1 == index)
      {
        string str = segments[index];
        shareName = str.Trim('/');
        fileName = string.Empty;
      }
      else
      {
        shareName = segments[index].Trim('/');
        fileName = segments[segments.Length - 1];
      }
    }

    internal static DateTimeOffset ParseSnapshotTime(string snapshot)
    {
      DateTimeOffset result;
      if (!DateTimeOffset.TryParse(snapshot, (IFormatProvider) CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out result))
        CommonUtility.ArgumentOutOfRange(nameof (snapshot), (object) snapshot);
      return result;
    }

    internal static StorageUri ParseBlobQueryAndVerify(
      StorageUri address,
      out StorageCredentials parsedCredentials,
      out DateTimeOffset? parsedSnapshot)
    {
      return new StorageUri(NavigationHelper.ParseBlobQueryAndVerify(address.PrimaryUri, out parsedCredentials, out parsedSnapshot), NavigationHelper.ParseBlobQueryAndVerify(address.SecondaryUri, out StorageCredentials _, out DateTimeOffset? _));
    }

    private static Uri ParseBlobQueryAndVerify(
      Uri address,
      out StorageCredentials parsedCredentials,
      out DateTimeOffset? parsedSnapshot)
    {
      parsedCredentials = (StorageCredentials) null;
      parsedSnapshot = new DateTimeOffset?();
      if (address == (Uri) null)
        return (Uri) null;
      IDictionary<string, string> queryParameters = address.IsAbsoluteUri ? HttpWebUtility.ParseQueryString(address.Query) : throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Address '{0}' is a relative address. Only absolute addresses are permitted.", (object) address.ToString()), nameof (address));
      string snapshot;
      if (queryParameters.TryGetValue("snapshot", out snapshot) && !string.IsNullOrEmpty(snapshot))
        parsedSnapshot = new DateTimeOffset?(NavigationHelper.ParseSnapshotTime(snapshot));
      parsedCredentials = SharedAccessSignatureHelper.ParseQuery(queryParameters);
      return new Uri(address.GetComponents(UriComponents.SchemeAndServer | UriComponents.Path, UriFormat.UriEscaped));
    }

    internal static StorageUri ParseFileQueryAndVerify(
      StorageUri address,
      out StorageCredentials parsedCredentials,
      out DateTimeOffset? parsedShareSnapshot)
    {
      return new StorageUri(NavigationHelper.ParseFileQueryAndVerify(address.PrimaryUri, out parsedCredentials, out parsedShareSnapshot), NavigationHelper.ParseFileQueryAndVerify(address.SecondaryUri, out StorageCredentials _, out DateTimeOffset? _));
    }

    private static Uri ParseFileQueryAndVerify(
      Uri address,
      out StorageCredentials parsedCredentials,
      out DateTimeOffset? parsedShareSnapshot)
    {
      parsedCredentials = (StorageCredentials) null;
      parsedShareSnapshot = new DateTimeOffset?();
      if (address == (Uri) null)
        return (Uri) null;
      IDictionary<string, string> queryParameters = address.IsAbsoluteUri ? HttpWebUtility.ParseQueryString(address.Query) : throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Address '{0}' is a relative address. Only absolute addresses are permitted.", (object) address.ToString()), nameof (address));
      string snapshot;
      if (queryParameters.TryGetValue("sharesnapshot", out snapshot) && !string.IsNullOrEmpty(snapshot))
        parsedShareSnapshot = new DateTimeOffset?(NavigationHelper.ParseSnapshotTime(snapshot));
      parsedCredentials = SharedAccessSignatureHelper.ParseQuery(queryParameters);
      return new Uri(address.GetComponents(UriComponents.SchemeAndServer | UriComponents.Path, UriFormat.UriEscaped));
    }

    internal static StorageUri ParseQueueTableQueryAndVerify(
      StorageUri address,
      out StorageCredentials parsedCredentials)
    {
      return new StorageUri(NavigationHelper.ParseQueueTableQueryAndVerify(address.PrimaryUri, out parsedCredentials), NavigationHelper.ParseQueueTableQueryAndVerify(address.SecondaryUri, out StorageCredentials _));
    }

    private static Uri ParseQueueTableQueryAndVerify(
      Uri address,
      out StorageCredentials parsedCredentials)
    {
      parsedCredentials = (StorageCredentials) null;
      if (address == (Uri) null)
        return (Uri) null;
      IDictionary<string, string> queryParameters = address.IsAbsoluteUri ? HttpWebUtility.ParseQueryString(address.Query) : throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Address '{0}' is a relative address. Only absolute addresses are permitted.", (object) address.ToString()), nameof (address));
      parsedCredentials = SharedAccessSignatureHelper.ParseQuery(queryParameters);
      return new Uri(address.GetComponents(UriComponents.SchemeAndServer | UriComponents.Path, UriFormat.UriEscaped));
    }

    internal static string GetAccountNameFromUri(Uri clientUri, bool? usePathStyleUris)
    {
      if (!usePathStyleUris.HasValue)
        usePathStyleUris = new bool?(CommonUtility.UsePathStyleAddressing(clientUri));
      string[] strArray = clientUri.AbsoluteUri.Split('?')[0].Split(NavigationHelper.SlashAsSplitOptions, StringSplitOptions.RemoveEmptyEntries);
      if (usePathStyleUris.Value)
        return strArray.Length >= 3 ? strArray[2] : throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Cannot find account information inside Uri '{0}'", (object) clientUri.AbsoluteUri));
      int length = strArray.Length >= 2 ? strArray[1].IndexOf(".", StringComparison.Ordinal) : throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Cannot find account information inside Uri '{0}'", (object) clientUri.AbsoluteUri));
      return strArray[1].Substring(0, length);
    }
  }
}
