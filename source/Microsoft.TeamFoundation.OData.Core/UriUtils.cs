// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriUtils
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using System;
using System.Xml;

namespace Microsoft.OData
{
  internal static class UriUtils
  {
    internal static Uri UriToAbsoluteUri(Uri baseUri, Uri relativeUri) => new Uri(baseUri, relativeUri);

    internal static Uri EnsureEscapedRelativeUri(Uri uri)
    {
      string components = uri.GetComponents(UriComponents.SerializationInfoString, UriFormat.UriEscaped);
      return string.CompareOrdinal(uri.OriginalString, components) == 0 ? uri : new Uri(components, UriKind.Relative);
    }

    internal static string EnsureEscapedFragment(string fragmentString) => "#" + Uri.EscapeDataString(fragmentString.Substring(1));

    internal static string UriToString(Uri uri) => !uri.IsAbsoluteUri ? uri.OriginalString : uri.AbsoluteUri;

    internal static Uri StringToUri(string uriString)
    {
      try
      {
        return new Uri(uriString, UriKind.RelativeOrAbsolute);
      }
      catch (FormatException ex)
      {
        return new Uri(uriString, UriKind.Relative);
      }
    }

    internal static Uri EnsureTaillingSlash(Uri uri)
    {
      if (uri == (Uri) null)
        return (Uri) null;
      string str = UriUtils.UriToString(uri);
      return str[str.Length - 1] != '/' ? new Uri(str + "/", UriKind.RelativeOrAbsolute) : uri;
    }

    internal static bool UriInvariantInsensitiveIsBaseOf(Uri baseUri, Uri uri) => UriUtils.CreateBaseComparableUri(baseUri).IsBaseOf(UriUtils.CreateBaseComparableUri(uri));

    internal static bool TryUriStringToGuid(string text, out Guid targetValue)
    {
      try
      {
        string str = text.Trim();
        if (str.Length != 36 || str.IndexOf('-') != 8)
        {
          targetValue = new Guid();
          return false;
        }
        targetValue = XmlConvert.ToGuid(text);
        return true;
      }
      catch (FormatException ex)
      {
        targetValue = new Guid();
        return false;
      }
    }

    internal static bool ConvertUriStringToDateTimeOffset(
      string text,
      out DateTimeOffset targetValue)
    {
      targetValue = new DateTimeOffset();
      try
      {
        targetValue = PlatformHelper.ConvertStringToDateTimeOffset(text);
        return true;
      }
      catch (FormatException ex)
      {
        if (PlatformHelper.PotentialDateTimeOffsetValidator.Match(text).Success)
          throw new ODataException(Strings.UriUtils_DateTimeOffsetInvalidFormat((object) text), (Exception) ex);
        return false;
      }
      catch (ArgumentOutOfRangeException ex)
      {
        throw new ODataException(Strings.UriUtils_DateTimeOffsetInvalidFormat((object) text), (Exception) ex);
      }
    }

    internal static bool TryUriStringToDate(string text, out Date targetValue) => PlatformHelper.TryConvertStringToDate(text, out targetValue);

    internal static bool TryUriStringToTimeOfDay(string text, out TimeOfDay targetValue) => PlatformHelper.TryConvertStringToTimeOfDay(text, out targetValue);

    internal static Uri CreateMockAbsoluteUri(Uri uri = null)
    {
      Uri baseUri = new Uri("http://host/");
      if (uri == (Uri) null)
        return baseUri;
      return uri.IsAbsoluteUri ? uri : new Uri(baseUri, uri);
    }

    private static Uri CreateBaseComparableUri(Uri uri)
    {
      uri = new Uri(UriUtils.UriToString(uri).ToUpperInvariant(), UriKind.RelativeOrAbsolute);
      return new UriBuilder(uri)
      {
        Host = "h",
        Port = 80,
        Scheme = "http"
      }.Uri;
    }
  }
}
