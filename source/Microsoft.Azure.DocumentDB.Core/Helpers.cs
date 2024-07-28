// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Helpers
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Documents.Collections;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.Azure.Documents
{
  internal static class Helpers
  {
    internal static int ValidateNonNegativeInteger(string name, int value) => value >= 0 ? value : throw new BadRequestException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.NegativeInteger, (object) name));

    internal static int ValidatePositiveInteger(string name, int value) => value > 0 ? value : throw new BadRequestException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.PositiveInteger, (object) name));

    internal static void ValidateEnumProperties<TEnum>(TEnum enumValue)
    {
      foreach (TEnum @enum in Enum.GetValues(typeof (TEnum)))
      {
        if (@enum.Equals((object) enumValue))
          return;
      }
      throw new BadRequestException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Invalid value {0} for type{1}", (object) enumValue.ToString(), (object) enumValue.GetType().ToString()));
    }

    public static byte GetHeaderValueByte(
      INameValueCollection headerValues,
      string headerName,
      byte defaultValue = 255)
    {
      byte result = defaultValue;
      string headerValue = headerValues[headerName];
      if (!string.IsNullOrWhiteSpace(headerValue) && !byte.TryParse(headerValue, NumberStyles.None, (IFormatProvider) CultureInfo.InvariantCulture, out result))
        result = defaultValue;
      return result;
    }

    public static string GetDateHeader(INameValueCollection headerValues)
    {
      if (headerValues == null)
        return string.Empty;
      string headerValue = headerValues["x-ms-date"];
      if (string.IsNullOrEmpty(headerValue))
        headerValue = headerValues["date"];
      return headerValue ?? string.Empty;
    }

    public static long GetHeaderValueLong(
      INameValueCollection headerValues,
      string headerName,
      long defaultValue = -1)
    {
      long result = defaultValue;
      string headerValue = headerValues[headerName];
      if (!string.IsNullOrEmpty(headerValue) && !long.TryParse(headerValue, NumberStyles.Number, (IFormatProvider) CultureInfo.InvariantCulture, out result))
        result = defaultValue;
      return result;
    }

    public static double GetHeaderValueDouble(
      INameValueCollection headerValues,
      string headerName,
      double defaultValue = -1.0)
    {
      double result = defaultValue;
      string headerValue = headerValues[headerName];
      if (!string.IsNullOrEmpty(headerValue) && !double.TryParse(headerValue, NumberStyles.AllowDecimalPoint, (IFormatProvider) CultureInfo.InvariantCulture, out result))
        result = defaultValue;
      return result;
    }

    internal static string[] ExtractValuesFromHTTPHeaders(System.Net.Http.Headers.HttpHeaders httpHeaders, string[] keys)
    {
      string[] array = Enumerable.Repeat<string>("", keys.Length).ToArray<string>();
      if (httpHeaders == null)
        return array;
      foreach (KeyValuePair<string, IEnumerable<string>> httpHeader in httpHeaders)
      {
        KeyValuePair<string, IEnumerable<string>> pair = httpHeader;
        int index = Array.FindIndex<string>(keys, (Predicate<string>) (t => t.Equals(pair.Key, StringComparison.OrdinalIgnoreCase)));
        if (index >= 0 && pair.Value.Count<string>() > 0)
          array[index] = pair.Value.First<string>();
      }
      return array;
    }

    internal static string GetAppSpecificUserAgentSuffix(string appName, string appVersion)
    {
      if (string.IsNullOrEmpty(appName))
        throw new ArgumentNullException(nameof (appName));
      if (string.IsNullOrEmpty(appVersion))
        throw new ArgumentNullException(nameof (appVersion));
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) appName, (object) appVersion);
    }

    internal static void SetupJsonReader(
      JsonReader reader,
      JsonSerializerSettings serializerSettings)
    {
      if (serializerSettings == null)
        return;
      if (serializerSettings.Culture != null)
        reader.Culture = serializerSettings.Culture;
      reader.DateTimeZoneHandling = serializerSettings.DateTimeZoneHandling;
      reader.DateParseHandling = serializerSettings.DateParseHandling;
      reader.FloatParseHandling = serializerSettings.FloatParseHandling;
      if (serializerSettings.MaxDepth.HasValue)
        reader.MaxDepth = serializerSettings.MaxDepth;
      if (serializerSettings.DateFormatString == null)
        return;
      reader.DateFormatString = serializerSettings.DateFormatString;
    }

    internal static string GetScriptLogHeader(INameValueCollection headerValues)
    {
      string headerValue = headerValues?["x-ms-documentdb-script-log-results"];
      return !string.IsNullOrEmpty(headerValue) ? Uri.UnescapeDataString(headerValue) : headerValue;
    }

    internal static long ToUnixTime(DateTimeOffset dt) => (long) (dt - new DateTimeOffset(1970, 1, 1, 0, 0, 0, new TimeSpan(0L))).TotalSeconds;

    internal static string GetStatusFromStatusCode(string statusCode)
    {
      int result;
      if (!int.TryParse(statusCode, out result))
        return "Other";
      if (result >= 200 && result < 300)
        return "Success";
      switch (result)
      {
        case 304:
          return "NotModified";
        case 400:
          return "BadRequestError";
        case 401:
          return "AuthorizationError";
        case 408:
          return "ServerTimeoutError";
        case 429:
          return "ClientThrottlingError";
        default:
          if (result > 400 && result < 500)
            return "ClientOtherError";
          if (result == 500)
            return "ServerOtherError";
          return result == 503 ? "ServiceBusyError" : "Other";
      }
    }
  }
}
