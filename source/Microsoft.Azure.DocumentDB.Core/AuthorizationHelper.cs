// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.AuthorizationHelper
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Cosmos.Core.Trace;
using Microsoft.Azure.Documents.Collections;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace Microsoft.Azure.Documents
{
  internal sealed class AuthorizationHelper
  {
    public const int MaxAuthorizationHeaderSize = 1024;
    public const int DefaultAllowedClockSkewInSeconds = 900;
    public const int DefaultMasterTokenExpiryInSeconds = 900;

    public static string GenerateGatewayAuthSignatureWithAddressResolution(
      string verb,
      Uri uri,
      INameValueCollection headers,
      IComputeHash stringHMACSHA256Helper,
      string clientVersion = "")
    {
      if (uri == (Uri) null)
        throw new ArgumentNullException(nameof (uri));
      if (uri.AbsolutePath.Equals("//addresses/", StringComparison.OrdinalIgnoreCase))
        uri = AuthorizationHelper.GenerateUriFromAddressRequestUri(uri);
      return AuthorizationHelper.GenerateKeyAuthorizationSignature(verb, uri, headers, stringHMACSHA256Helper, clientVersion);
    }

    public static string GenerateKeyAuthorizationSignature(
      string verb,
      Uri uri,
      INameValueCollection headers,
      IComputeHash stringHMACSHA256Helper,
      string clientVersion = "")
    {
      if (string.IsNullOrEmpty(verb))
        throw new ArgumentException(RMResources.StringArgumentNullOrEmpty, nameof (verb));
      if (uri == (Uri) null)
        throw new ArgumentNullException(nameof (uri));
      if (stringHMACSHA256Helper == null)
        throw new ArgumentNullException(nameof (stringHMACSHA256Helper));
      if (headers == null)
        throw new ArgumentNullException(nameof (headers));
      string resourceType = string.Empty;
      string resourceId = string.Empty;
      bool isNameBased = false;
      AuthorizationHelper.GetResourceTypeAndIdOrFullName(uri, out isNameBased, out resourceType, out resourceId, clientVersion);
      return AuthorizationHelper.GenerateKeyAuthorizationSignature(verb, resourceId, resourceType, headers, stringHMACSHA256Helper, out string _);
    }

    [SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = "HTTP Headers are ASCII")]
    public static string GenerateKeyAuthorizationSignature(
      string verb,
      string resourceId,
      string resourceType,
      INameValueCollection headers,
      string key,
      bool bUseUtcNowForMissingXDate = false)
    {
      return AuthorizationHelper.GenerateKeyAuthorizationSignature(verb, resourceId, resourceType, headers, key, out string _, bUseUtcNowForMissingXDate);
    }

    [SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = "HTTP Headers are ASCII")]
    public static string GenerateKeyAuthorizationSignature(
      string verb,
      string resourceId,
      string resourceType,
      INameValueCollection headers,
      string key,
      out string payload,
      bool bUseUtcNowForMissingXDate = false)
    {
      if (string.IsNullOrEmpty(verb))
        throw new ArgumentException(RMResources.StringArgumentNullOrEmpty, nameof (verb));
      if (resourceType == null)
        throw new ArgumentNullException(nameof (resourceType));
      if (string.IsNullOrEmpty(key))
        throw new ArgumentException(RMResources.StringArgumentNullOrEmpty, nameof (key));
      if (headers == null)
        throw new ArgumentNullException(nameof (headers));
      using (HMACSHA256 hmacshA256 = new HMACSHA256(Convert.FromBase64String(key)))
      {
        string verb1 = verb ?? string.Empty;
        string resourceIdOrFullName1 = resourceId ?? string.Empty;
        string resourceType1 = resourceType ?? string.Empty;
        string resourceIdOrFullName2 = AuthorizationHelper.GetAuthorizationResourceIdOrFullName(resourceType1, resourceIdOrFullName1);
        payload = AuthorizationHelper.GenerateMessagePayload(verb1, resourceIdOrFullName2, resourceType1, headers, bUseUtcNowForMissingXDate);
        return HttpUtility.UrlEncode(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "type={0}&ver={1}&sig={2}", (object) "master", (object) "1.0", (object) Convert.ToBase64String(hmacshA256.ComputeHash(Encoding.UTF8.GetBytes(payload)))));
      }
    }

    [SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = "HTTP Headers are ASCII")]
    public static string GenerateKeyAuthorizationSignature(
      string verb,
      string resourceId,
      string resourceType,
      INameValueCollection headers,
      IComputeHash stringHMACSHA256Helper)
    {
      return AuthorizationHelper.GenerateKeyAuthorizationSignature(verb, resourceId, resourceType, headers, stringHMACSHA256Helper, out string _);
    }

    [SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = "HTTP Headers are ASCII")]
    public static string GenerateKeyAuthorizationSignature(
      string verb,
      string resourceId,
      string resourceType,
      INameValueCollection headers,
      IComputeHash stringHMACSHA256Helper,
      out string payload)
    {
      if (string.IsNullOrEmpty(verb))
        throw new ArgumentException(RMResources.StringArgumentNullOrEmpty, nameof (verb));
      if (resourceType == null)
        throw new ArgumentNullException(nameof (resourceType));
      if (stringHMACSHA256Helper == null)
        throw new ArgumentNullException(nameof (stringHMACSHA256Helper));
      if (headers == null)
        throw new ArgumentNullException(nameof (headers));
      string verb1 = verb ?? string.Empty;
      string resourceIdOrFullName1 = resourceId ?? string.Empty;
      string resourceType1 = resourceType ?? string.Empty;
      string resourceIdOrFullName2 = AuthorizationHelper.GetAuthorizationResourceIdOrFullName(resourceType1, resourceIdOrFullName1);
      payload = AuthorizationHelper.GenerateMessagePayload(verb1, resourceIdOrFullName2, resourceType1, headers);
      return HttpUtility.UrlEncode(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "type={0}&ver={1}&sig={2}", (object) "master", (object) "1.0", (object) Convert.ToBase64String(stringHMACSHA256Helper.ComputeHash(Encoding.UTF8.GetBytes(payload)))));
    }

    public static void ParseAuthorizationToken(
      string authorizationToken,
      out string typeOutput,
      out string versionOutput,
      out string tokenOutput)
    {
      typeOutput = (string) null;
      versionOutput = (string) null;
      tokenOutput = (string) null;
      if (string.IsNullOrEmpty(authorizationToken))
      {
        DefaultTrace.TraceError("Auth token missing");
        throw new UnauthorizedException(RMResources.MissingAuthHeader);
      }
      authorizationToken = authorizationToken.Length <= 1024 ? HttpUtility.UrlDecode(authorizationToken) : throw new UnauthorizedException(RMResources.InvalidAuthHeaderFormat);
      int length1 = authorizationToken.IndexOf('&');
      string str1 = length1 != -1 ? authorizationToken.Substring(0, length1) : throw new UnauthorizedException(RMResources.InvalidAuthHeaderFormat);
      authorizationToken = authorizationToken.Substring(length1 + 1, authorizationToken.Length - length1 - 1);
      int length2 = authorizationToken.IndexOf('&');
      string str2 = length2 != -1 ? authorizationToken.Substring(0, length2) : throw new UnauthorizedException(RMResources.InvalidAuthHeaderFormat);
      authorizationToken = authorizationToken.Substring(length2 + 1, authorizationToken.Length - length2 - 1);
      string str3 = authorizationToken;
      int length3 = authorizationToken.IndexOf(',');
      if (length3 != -1)
        str3 = authorizationToken.Substring(0, length3);
      int length4 = str1.IndexOf('=');
      string str4 = length4 != -1 && str1.Substring(0, length4).Equals("type", StringComparison.OrdinalIgnoreCase) ? str1.Substring(length4 + 1) : throw new UnauthorizedException(RMResources.InvalidAuthHeaderFormat);
      int length5 = str2.IndexOf('=');
      string str5 = length5 != -1 && str2.Substring(0, length5).Equals("ver", StringComparison.OrdinalIgnoreCase) ? str2.Substring(length5 + 1) : throw new UnauthorizedException(RMResources.InvalidAuthHeaderFormat);
      int length6 = str3.IndexOf('=');
      string str6 = length6 != -1 && str3.Substring(0, length6).Equals("sig", StringComparison.OrdinalIgnoreCase) ? str3.Substring(length6 + 1) : throw new UnauthorizedException(RMResources.InvalidAuthHeaderFormat);
      if (string.IsNullOrEmpty(str4) || string.IsNullOrEmpty(str5) || string.IsNullOrEmpty(str6))
        throw new UnauthorizedException(RMResources.InvalidAuthHeaderFormat);
      typeOutput = str4;
      versionOutput = str5;
      tokenOutput = str6;
    }

    public static bool CheckPayloadUsingKey(
      string inputToken,
      string verb,
      string resourceId,
      string resourceType,
      INameValueCollection headers,
      string key)
    {
      string str1 = HttpUtility.UrlDecode(AuthorizationHelper.GenerateKeyAuthorizationSignature(verb, resourceId, resourceType, headers, key, out string _));
      string str2 = str1.Substring(str1.IndexOf("sig=", StringComparison.OrdinalIgnoreCase) + 4);
      return inputToken.Equals(str2, StringComparison.OrdinalIgnoreCase);
    }

    public static void ValidateInputRequestTime(
      INameValueCollection requestHeaders,
      int masterTokenExpiryInSeconds,
      int allowedClockSkewInSeconds)
    {
      AuthorizationHelper.ValidateInputRequestTime<INameValueCollection>(requestHeaders, (Func<INameValueCollection, string, string>) ((headers, field) => AuthorizationHelper.GetHeaderValue(headers, field)), masterTokenExpiryInSeconds, allowedClockSkewInSeconds);
    }

    public static void ValidateInputRequestTime<T>(
      T requestHeaders,
      Func<T, string, string> headerGetter,
      int masterTokenExpiryInSeconds,
      int allowedClockSkewInSeconds)
    {
      if ((object) requestHeaders == null)
      {
        DefaultTrace.TraceError("Null request headers for validating auth time");
        throw new UnauthorizedException(RMResources.MissingDateForAuthorization);
      }
      string dateToCompare = headerGetter(requestHeaders, "x-ms-date");
      if (string.IsNullOrEmpty(dateToCompare))
        dateToCompare = headerGetter(requestHeaders, "date");
      AuthorizationHelper.ValidateInputRequestTime(dateToCompare, masterTokenExpiryInSeconds, allowedClockSkewInSeconds);
    }

    private static void ValidateInputRequestTime(
      string dateToCompare,
      int masterTokenExpiryInSeconds,
      int allowedClockSkewInSeconds)
    {
      if (string.IsNullOrEmpty(dateToCompare))
        throw new UnauthorizedException(RMResources.MissingDateForAuthorization);
      DateTime result;
      if (!DateTime.TryParse(dateToCompare, (IFormatProvider) CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out result))
        throw new UnauthorizedException(RMResources.InvalidDateHeader);
      if (result >= DateTime.MaxValue.AddSeconds((double) -masterTokenExpiryInSeconds))
      {
        string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, RMResources.InvalidTokenTimeRange, (object) result.ToString("r", (IFormatProvider) CultureInfo.InvariantCulture), (object) DateTime.MaxValue.ToString("r", (IFormatProvider) CultureInfo.InvariantCulture), (object) DateTime.UtcNow.ToString("r", (IFormatProvider) CultureInfo.InvariantCulture));
        DefaultTrace.TraceError(message);
        throw new ForbiddenException(message);
      }
      DateTime expiryDateTime = result + TimeSpan.FromSeconds((double) masterTokenExpiryInSeconds);
      AuthorizationHelper.CheckTimeRangeIsCurrent(allowedClockSkewInSeconds, result, expiryDateTime);
    }

    public static void CheckTimeRangeIsCurrent(
      int allowedClockSkewInSeconds,
      DateTime startDateTime,
      DateTime expiryDateTime)
    {
      if ((startDateTime <= DateTime.MinValue.AddSeconds((double) allowedClockSkewInSeconds) ? 1 : (expiryDateTime >= DateTime.MaxValue.AddSeconds((double) -allowedClockSkewInSeconds) ? 1 : 0)) != 0 || startDateTime.AddSeconds((double) -allowedClockSkewInSeconds) > DateTime.UtcNow || expiryDateTime.AddSeconds((double) allowedClockSkewInSeconds) < DateTime.UtcNow)
      {
        string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, RMResources.InvalidTokenTimeRange, (object) startDateTime.ToString("r", (IFormatProvider) CultureInfo.InvariantCulture), (object) expiryDateTime.ToString("r", (IFormatProvider) CultureInfo.InvariantCulture), (object) DateTime.UtcNow.ToString("r", (IFormatProvider) CultureInfo.InvariantCulture));
        DefaultTrace.TraceError(message);
        throw new ForbiddenException(message);
      }
    }

    internal static void GetResourceTypeAndIdOrFullName(
      Uri uri,
      out bool isNameBased,
      out string resourceType,
      out string resourceId,
      string clientVersion = "")
    {
      if (uri == (Uri) null)
        throw new ArgumentNullException(nameof (uri));
      resourceType = string.Empty;
      resourceId = string.Empty;
      if (uri.Segments.Length < 1)
        throw new ArgumentException(RMResources.InvalidUrl);
      bool isFeed = false;
      if (PathsHelper.TryParsePathSegments(uri.PathAndQuery, out isFeed, out resourceType, out resourceId, out isNameBased, clientVersion))
        return;
      resourceType = string.Empty;
      resourceId = string.Empty;
    }

    public static bool IsUserRequest(string resourceType) => string.Compare(resourceType, "/", StringComparison.OrdinalIgnoreCase) != 0 && string.Compare(resourceType, "presplitaction", StringComparison.OrdinalIgnoreCase) != 0 && string.Compare(resourceType, "postsplitaction", StringComparison.OrdinalIgnoreCase) != 0 && string.Compare(resourceType, "controllerbatchgetoutput", StringComparison.OrdinalIgnoreCase) != 0 && string.Compare(resourceType, "controllerbatchreportcharges", StringComparison.OrdinalIgnoreCase) != 0 && string.Compare(resourceType, "getstorageaccountkey", StringComparison.OrdinalIgnoreCase) != 0;

    public static AuthorizationTokenType GetSystemOperationType(
      bool readOnlyRequest,
      string resourceType)
    {
      return !AuthorizationHelper.IsUserRequest(resourceType) ? (readOnlyRequest ? AuthorizationTokenType.SystemReadOnly : AuthorizationTokenType.SystemAll) : (readOnlyRequest ? AuthorizationTokenType.SystemReadOnly : AuthorizationTokenType.SystemReadWrite);
    }

    public static string GenerateMessagePayload(
      string verb,
      string resourceId,
      string resourceType,
      INameValueCollection headers,
      bool bUseUtcNowForMissingXDate = false)
    {
      string headerValue1 = AuthorizationHelper.GetHeaderValue(headers, "x-ms-date");
      string headerValue2 = AuthorizationHelper.GetHeaderValue(headers, "date");
      if (string.IsNullOrEmpty(headerValue1) && string.IsNullOrWhiteSpace(headerValue2))
      {
        if (!bUseUtcNowForMissingXDate)
          throw new UnauthorizedException(RMResources.InvalidDateHeader);
        headers["x-ms-date"] = DateTime.UtcNow.ToString("r", (IFormatProvider) CultureInfo.InvariantCulture);
        headerValue1 = AuthorizationHelper.GetHeaderValue(headers, "x-ms-date");
      }
      if (!PathsHelper.IsNameBased(resourceId))
        resourceId = resourceId.ToLowerInvariant();
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}\n{1}\n{2}\n{3}\n{4}\n", (object) verb.ToLowerInvariant(), (object) resourceType.ToLowerInvariant(), (object) resourceId, (object) headerValue1.ToLowerInvariant(), headerValue1.Equals("", StringComparison.OrdinalIgnoreCase) ? (object) headerValue2.ToLowerInvariant() : (object) "");
    }

    public static bool IsResourceToken(string token)
    {
      int length1 = token.IndexOf('&');
      if (length1 == -1)
        return false;
      string str = token.Substring(0, length1);
      int length2 = str.IndexOf('=');
      return length2 != -1 && str.Substring(0, length2).Equals("type", StringComparison.OrdinalIgnoreCase) && str.Substring(length2 + 1).Equals("resource", StringComparison.OrdinalIgnoreCase);
    }

    internal static string GetHeaderValue(INameValueCollection headerValues, string key) => headerValues == null ? string.Empty : headerValues[key] ?? "";

    internal static string GetHeaderValue(IDictionary<string, string> headerValues, string key)
    {
      if (headerValues == null)
        return string.Empty;
      string headerValue = (string) null;
      headerValues.TryGetValue(key, out headerValue);
      return headerValue;
    }

    internal static string GetAuthorizationResourceIdOrFullName(
      string resourceType,
      string resourceIdOrFullName)
    {
      if (string.IsNullOrEmpty(resourceType) || string.IsNullOrEmpty(resourceIdOrFullName) || PathsHelper.IsNameBased(resourceIdOrFullName) || resourceType.Equals("offers", StringComparison.OrdinalIgnoreCase) || resourceType.Equals("partitions", StringComparison.OrdinalIgnoreCase) || resourceType.Equals("topology", StringComparison.OrdinalIgnoreCase) || resourceType.Equals("ridranges", StringComparison.OrdinalIgnoreCase) || resourceType.Equals("snapshots", StringComparison.OrdinalIgnoreCase))
        return resourceIdOrFullName;
      ResourceId resourceId = ResourceId.Parse(resourceIdOrFullName);
      if (resourceType.Equals("dbs", StringComparison.OrdinalIgnoreCase))
        return resourceId.DatabaseId.ToString();
      if (resourceType.Equals("users", StringComparison.OrdinalIgnoreCase))
        return resourceId.UserId.ToString();
      if (resourceType.Equals("udts", StringComparison.OrdinalIgnoreCase))
        return resourceId.UserDefinedTypeId.ToString();
      if (resourceType.Equals("colls", StringComparison.OrdinalIgnoreCase))
        return resourceId.DocumentCollectionId.ToString();
      if (resourceType.Equals("clientencryptionkeys", StringComparison.OrdinalIgnoreCase))
        return resourceId.ClientEncryptionKeyId.ToString();
      return resourceType.Equals("docs", StringComparison.OrdinalIgnoreCase) ? resourceId.DocumentId.ToString() : resourceIdOrFullName;
    }

    public static Uri GenerateUriFromAddressRequestUri(Uri uri)
    {
      string str = UrlUtility.ParseQuery(uri.Query)["$resolveFor"] ?? UrlUtility.ParseQuery(uri.Query)["$generateFor"] ?? UrlUtility.ParseQuery(uri.Query)["$getChildResourcePartitions"];
      if (string.IsNullOrEmpty(str))
        throw new BadRequestException(RMResources.BadUrl);
      return new Uri(uri.Scheme + "://" + uri.Host + "/" + HttpUtility.UrlDecode(str).Trim('/'));
    }
  }
}
