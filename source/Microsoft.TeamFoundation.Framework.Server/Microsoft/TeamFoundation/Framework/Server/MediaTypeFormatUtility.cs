// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.MediaTypeFormatUtility
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class MediaTypeFormatUtility
  {
    private const double c_defaultQuality = 1.0;
    public const string c_formatParameterName = "$format";
    private static IReadOnlyDictionary<RequestMediaType, string> s_mediaTypeStrings = (IReadOnlyDictionary<RequestMediaType, string>) new Dictionary<RequestMediaType, string>()
    {
      {
        RequestMediaType.None,
        "none/unspecified"
      },
      {
        RequestMediaType.Json,
        "application/json"
      },
      {
        RequestMediaType.Zip,
        "application/zip"
      },
      {
        RequestMediaType.Text,
        "text/"
      },
      {
        RequestMediaType.OctetStream,
        "application/octet-stream"
      }
    };

    public static string AcceptHeaderToString(RequestMediaType acceptType) => MediaTypeFormatUtility.s_mediaTypeStrings[acceptType];

    public static RequestMediaType GetFormatParameterType(
      HttpRequestMessage request,
      List<RequestMediaType> supportedTypes = null,
      bool throwOnInvalidFormatParameter = true)
    {
      RequestMediaType result = RequestMediaType.None;
      string queryStringValue = request.GetQueryStringValue("$format");
      if (queryStringValue != null)
      {
        bool flag = EnumUtility.TryParse<RequestMediaType>(queryStringValue, true, out result);
        if (flag && supportedTypes != null && result != RequestMediaType.None)
          flag = supportedTypes.Contains(result);
        if (!flag & throwOnInvalidFormatParameter)
        {
          if (supportedTypes != null)
            throw new ArgumentException(FrameworkResources.InvalidQueryParam((object) queryStringValue, (object) "$format", (object) string.Join(", ", supportedTypes.Select<RequestMediaType, string>((Func<RequestMediaType, string>) (type => type.ToString())))));
          throw new ArgumentException(FrameworkResources.InvalidQueryParam((object) queryStringValue, (object) "$format", (object) string.Join(", ", Enum.GetNames(typeof (RequestMediaType)))));
        }
      }
      return result;
    }

    public static List<RequestMediaType> GetPrioritizedAcceptHeaders(
      HttpRequestMessage request,
      List<RequestMediaType> supportedTypes = null,
      bool useFormatParameter = true,
      bool throwOnInvalidFormatParameter = true)
    {
      HttpRequestHeaders headers = request.Headers;
      Dictionary<RequestMediaType, double> source = new Dictionary<RequestMediaType, double>();
      if (useFormatParameter)
      {
        RequestMediaType formatParameterType = MediaTypeFormatUtility.GetFormatParameterType(request, supportedTypes, throwOnInvalidFormatParameter);
        if (formatParameterType != RequestMediaType.None)
          source.Add(formatParameterType, double.MaxValue);
      }
      foreach (MediaTypeWithQualityHeaderValue qualityHeaderValue in headers.Accept)
      {
        MediaTypeWithQualityHeaderValue accept = qualityHeaderValue;
        if (accept != null)
        {
          RequestMediaType key1 = MediaTypeFormatUtility.s_mediaTypeStrings.FirstOrDefault<KeyValuePair<RequestMediaType, string>>((Func<KeyValuePair<RequestMediaType, string>, bool>) (pair => accept.MediaType.StartsWith(pair.Value, StringComparison.OrdinalIgnoreCase))).Key;
          double? quality;
          if (key1 != RequestMediaType.None)
          {
            if (source.ContainsKey(key1))
            {
              Dictionary<RequestMediaType, double> dictionary = source;
              int key2 = (int) key1;
              quality = accept.Quality;
              double num = Math.Max(quality.GetValueOrDefault(1.0), source[key1]);
              dictionary[(RequestMediaType) key2] = num;
            }
            else
            {
              Dictionary<RequestMediaType, double> dictionary = source;
              int key3 = (int) key1;
              quality = accept.Quality;
              double valueOrDefault = quality.GetValueOrDefault(1.0);
              dictionary.Add((RequestMediaType) key3, valueOrDefault);
            }
          }
        }
      }
      if (!source.Any<KeyValuePair<RequestMediaType, double>>())
        return new List<RequestMediaType>()
        {
          RequestMediaType.None
        };
      return supportedTypes == null ? source.OrderByDescending<KeyValuePair<RequestMediaType, double>, double>((Func<KeyValuePair<RequestMediaType, double>, double>) (pair => pair.Value)).Select<KeyValuePair<RequestMediaType, double>, RequestMediaType>((Func<KeyValuePair<RequestMediaType, double>, RequestMediaType>) (pair => pair.Key)).ToList<RequestMediaType>() : source.Where<KeyValuePair<RequestMediaType, double>>((Func<KeyValuePair<RequestMediaType, double>, bool>) (pair => supportedTypes.Contains(pair.Key))).OrderByDescending<KeyValuePair<RequestMediaType, double>, double>((Func<KeyValuePair<RequestMediaType, double>, double>) (pair => pair.Value)).ThenBy<KeyValuePair<RequestMediaType, double>, int>((Func<KeyValuePair<RequestMediaType, double>, int>) (pair => supportedTypes.IndexOf(pair.Key))).Select<KeyValuePair<RequestMediaType, double>, RequestMediaType>((Func<KeyValuePair<RequestMediaType, double>, RequestMediaType>) (pair => pair.Key)).DefaultIfEmpty<RequestMediaType>(RequestMediaType.None).ToList<RequestMediaType>();
    }

    public static string GetSafeResponseContentType(string actualContentType)
    {
      string responseContentType = "application/octet-stream";
      if (!string.IsNullOrEmpty(actualContentType))
      {
        string lowerInvariant = actualContentType.ToLowerInvariant();
        if (lowerInvariant == "text/plain")
          responseContentType = lowerInvariant;
        else if (lowerInvariant.StartsWith("image/") || lowerInvariant.StartsWith("audio/") || lowerInvariant.StartsWith("video/"))
          responseContentType = lowerInvariant;
        else if (lowerInvariant.StartsWith("text/"))
          responseContentType = "text/plain";
      }
      return responseContentType;
    }

    public static bool GetExcludeUrlsAcceptHeaderOptionValue(HttpRequestMessage request)
    {
      foreach (MediaTypeHeaderValue mediaTypeHeaderValue in request.Headers.Accept)
      {
        if (mediaTypeHeaderValue.Parameters.Contains(new NameValueHeaderValue("excludeUrls", "true")))
          return true;
      }
      return false;
    }

    public static bool GetLightweightAcceptHeaderOptionValue(HttpRequestMessage request)
    {
      foreach (MediaTypeHeaderValue mediaTypeHeaderValue in request.Headers.Accept)
      {
        if (mediaTypeHeaderValue.Parameters.Contains(new NameValueHeaderValue("lightweight", "true")))
          return true;
      }
      return false;
    }

    public static bool DoesRequestAcceptMediaType(
      string[] requestAcceptTypes,
      string acceptType,
      params string[] otherSupportedTypes)
    {
      ArgumentUtility.CheckForNull<string>(acceptType, nameof (acceptType));
      if (requestAcceptTypes == null || requestAcceptTypes.Length == 0)
        return true;
      string[] acceptTypesWithoutQuality = ((IEnumerable<string>) requestAcceptTypes).Select<string, string>((Func<string, string>) (at => MediaTypeFormatUtility.GetAcceptTypeWithoutQuality(at))).ToArray<string>();
      if (((IEnumerable<string>) acceptTypesWithoutQuality).Contains<string>(acceptType, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))
        return true;
      if (otherSupportedTypes != null && otherSupportedTypes.Length != 0 && ((IEnumerable<string>) acceptTypesWithoutQuality).Any<string>((Func<string, bool>) (at => ((IEnumerable<string>) otherSupportedTypes).Contains<string>(at, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))))
        return false;
      if (((IEnumerable<string>) acceptTypesWithoutQuality).Contains<string>(MediaTypeFormatUtility.GetMediaTypePrefix(acceptType) + "/*"))
        return true;
      if (!((IEnumerable<string>) acceptTypesWithoutQuality).Contains<string>("*/*"))
        return false;
      return otherSupportedTypes == null || otherSupportedTypes.Length == 0 || !((IEnumerable<string>) otherSupportedTypes).Any<string>((Func<string, bool>) (ost => ((IEnumerable<string>) acceptTypesWithoutQuality).Contains<string>(MediaTypeFormatUtility.GetMediaTypePrefix(ost) + "/*", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)));
    }

    private static string GetMediaTypePrefix(string mediaType)
    {
      int length = mediaType.IndexOf('/');
      return length >= 0 ? mediaType.Substring(0, length) : throw new ArgumentException(string.Format("Invalid media type: {0}", (object) mediaType), nameof (mediaType));
    }

    private static string GetAcceptTypeWithoutQuality(string acceptType)
    {
      int length = acceptType.IndexOf(';');
      return length < 0 ? acceptType : acceptType.Substring(0, length);
    }
  }
}
