// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.AuthorizationHelper
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Core.Trace;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Collections;
using System;
using System.Buffers;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace Microsoft.Azure.Cosmos
{
  internal static class AuthorizationHelper
  {
    public const int MaxAuthorizationHeaderSize = 1024;
    public const int DefaultAllowedClockSkewInSeconds = 900;
    public const int DefaultMasterTokenExpiryInSeconds = 900;
    private const int MaxAadAuthorizationHeaderSize = 16384;
    private const int MaxResourceTokenAuthorizationHeaderSize = 8192;
    private static readonly string AuthorizationFormatPrefixUrlEncoded = HttpUtility.UrlEncode(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "type={0}&ver={1}&sig={2}", (object) "master", (object) "1.0", (object) string.Empty));
    private static readonly Encoding AuthorizationEncoding = (Encoding) new UTF8Encoding(false);

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
      string resourceType;
      string resourceId;
      AuthorizationHelper.GetResourceTypeAndIdOrFullName(uri, out bool _, out resourceType, out resourceId, clientVersion);
      AuthorizationHelper.ArrayOwner payload;
      string authorizationSignature = AuthorizationHelper.GenerateKeyAuthorizationSignature(verb, resourceId, resourceType, headers, stringHMACSHA256Helper, out payload);
      using (payload)
        return authorizationSignature;
    }

    public static string GenerateKeyAuthorizationSignature(
      string verb,
      string resourceId,
      string resourceType,
      INameValueCollection headers,
      string key,
      bool bUseUtcNowForMissingXDate = false)
    {
      string authorizationCore = AuthorizationHelper.GenerateKeyAuthorizationCore(verb, resourceId, resourceType, headers, key);
      return AuthorizationHelper.AuthorizationFormatPrefixUrlEncoded + HttpUtility.UrlEncode(authorizationCore);
    }

    public static string GenerateKeyAuthorizationSignature(
      string verb,
      string resourceId,
      string resourceType,
      INameValueCollection headers,
      IComputeHash stringHMACSHA256Helper)
    {
      AuthorizationHelper.ArrayOwner payload;
      string tokenWithHashCore = AuthorizationHelper.GenerateUrlEncodedAuthorizationTokenWithHashCore(verb, resourceId, resourceType, headers, stringHMACSHA256Helper, out payload);
      using (payload)
        return AuthorizationHelper.AuthorizationFormatPrefixUrlEncoded + tokenWithHashCore;
    }

    public static string GenerateKeyAuthorizationSignature(
      string verb,
      string resourceId,
      string resourceType,
      INameValueCollection headers,
      IComputeHash stringHMACSHA256Helper,
      out string payload)
    {
      AuthorizationHelper.ArrayOwner payload1;
      string tokenWithHashCore = AuthorizationHelper.GenerateUrlEncodedAuthorizationTokenWithHashCore(verb, resourceId, resourceType, headers, stringHMACSHA256Helper, out payload1);
      using (payload1)
      {
        ref string local = ref payload;
        Encoding authorizationEncoding = AuthorizationHelper.AuthorizationEncoding;
        byte[] array = payload1.Buffer.Array;
        ArraySegment<byte> buffer = payload1.Buffer;
        int offset = buffer.Offset;
        buffer = payload1.Buffer;
        int count = buffer.Count;
        string str = authorizationEncoding.GetString(array, offset, count);
        local = str;
        return AuthorizationHelper.AuthorizationFormatPrefixUrlEncoded + tokenWithHashCore;
      }
    }

    public static string GenerateKeyAuthorizationSignature(
      string verb,
      string resourceId,
      string resourceType,
      INameValueCollection headers,
      IComputeHash stringHMACSHA256Helper,
      out AuthorizationHelper.ArrayOwner payload)
    {
      string tokenWithHashCore = AuthorizationHelper.GenerateUrlEncodedAuthorizationTokenWithHashCore(verb, resourceId, resourceType, headers, stringHMACSHA256Helper, out payload);
      try
      {
        return AuthorizationHelper.AuthorizationFormatPrefixUrlEncoded + tokenWithHashCore;
      }
      catch
      {
        payload.Dispose();
        throw;
      }
    }

    public static void ParseAuthorizationToken(
      string authorizationTokenString,
      out ReadOnlyMemory<char> typeOutput,
      out ReadOnlyMemory<char> versionOutput,
      out ReadOnlyMemory<char> tokenOutput)
    {
      typeOutput = new ReadOnlyMemory<char>();
      versionOutput = new ReadOnlyMemory<char>();
      tokenOutput = new ReadOnlyMemory<char>();
      if (string.IsNullOrEmpty(authorizationTokenString))
      {
        DefaultTrace.TraceError("Auth token missing");
        throw new UnauthorizedException(RMResources.MissingAuthHeader);
      }
      int length1 = authorizationTokenString.Length;
      authorizationTokenString = HttpUtility.UrlDecode(authorizationTokenString);
      ReadOnlyMemory<char> readOnlyMemory1 = MemoryExtensions.AsMemory(authorizationTokenString);
      int length2 = readOnlyMemory1.Span.IndexOf<char>('&');
      ReadOnlyMemory<char> readOnlyMemory2 = length2 != -1 ? readOnlyMemory1.Slice(0, length2) : throw new UnauthorizedException(RMResources.InvalidAuthHeaderFormat);
      readOnlyMemory1 = readOnlyMemory1.Slice(length2 + 1, readOnlyMemory1.Length - length2 - 1);
      int length3 = readOnlyMemory1.Span.IndexOf<char>('&');
      ReadOnlyMemory<char> readOnlyMemory3 = length3 != -1 ? readOnlyMemory1.Slice(0, length3) : throw new UnauthorizedException(RMResources.InvalidAuthHeaderFormat);
      readOnlyMemory1 = readOnlyMemory1.Slice(length3 + 1, readOnlyMemory1.Length - length3 - 1);
      ReadOnlyMemory<char> readOnlyMemory4 = readOnlyMemory1;
      int length4 = readOnlyMemory1.Span.IndexOf<char>(',');
      if (length4 != -1)
        readOnlyMemory4 = readOnlyMemory1.Slice(0, length4);
      int length5 = readOnlyMemory2.Span.IndexOf<char>('=');
      if (length5 != -1)
      {
        ReadOnlySpan<char> readOnlySpan = readOnlyMemory2.Span;
        if (readOnlySpan.Slice(0, length5).SequenceEqual<char>(MemoryExtensions.AsSpan("type")))
        {
          readOnlySpan = readOnlyMemory2.Span;
          readOnlySpan = readOnlySpan.Slice(0, length5);
          if (readOnlySpan.ToString().Equals("type", StringComparison.OrdinalIgnoreCase))
          {
            ReadOnlyMemory<char> readOnlyMemory5 = readOnlyMemory2.Slice(length5 + 1);
            if (MemoryExtensions.Equals(readOnlyMemory5.Span, MemoryExtensions.AsSpan("aad"), StringComparison.OrdinalIgnoreCase))
            {
              if (length1 > 16384)
              {
                readOnlySpan = readOnlyMemory5.Span;
                DefaultTrace.TraceError(string.Format("Token of type [{0}] was of size [{1}] while the max allowed size is [{2}].", (object) readOnlySpan.ToString(), (object) length1, (object) 16384));
                throw new UnauthorizedException(RMResources.InvalidAuthHeaderFormat, SubStatusCodes.InvalidAuthHeaderFormat);
              }
            }
            else if (MemoryExtensions.Equals(readOnlyMemory5.Span, MemoryExtensions.AsSpan("resource"), StringComparison.OrdinalIgnoreCase))
            {
              if (length1 > 8192)
              {
                readOnlySpan = readOnlyMemory5.Span;
                DefaultTrace.TraceError(string.Format("Token of type [{0}] was of size [{1}] while the max allowed size is [{2}].", (object) readOnlySpan.ToString(), (object) length1, (object) 8192));
                throw new UnauthorizedException(RMResources.InvalidAuthHeaderFormat, SubStatusCodes.InvalidAuthHeaderFormat);
              }
            }
            else if (length1 > 1024)
            {
              readOnlySpan = readOnlyMemory5.Span;
              DefaultTrace.TraceError(string.Format("Token of type [{0}] was of size [{1}] while the max allowed size is [{2}].", (object) readOnlySpan.ToString(), (object) length1, (object) 1024));
              throw new UnauthorizedException(RMResources.InvalidAuthHeaderFormat, SubStatusCodes.InvalidAuthHeaderFormat);
            }
            int length6 = readOnlyMemory3.Span.IndexOf<char>('=');
            if (length6 != -1)
            {
              readOnlySpan = readOnlyMemory3.Span;
              if (readOnlySpan.Slice(0, length6).SequenceEqual<char>(MemoryExtensions.AsSpan("ver")))
              {
                ReadOnlyMemory<char> readOnlyMemory6 = readOnlyMemory3.Slice(0, length6);
                if (readOnlyMemory6.ToString().Equals("ver", StringComparison.OrdinalIgnoreCase))
                {
                  ReadOnlyMemory<char> readOnlyMemory7 = readOnlyMemory3.Slice(length6 + 1);
                  int length7 = readOnlyMemory4.Span.IndexOf<char>('=');
                  readOnlyMemory6 = length7 != -1 ? readOnlyMemory4.Slice(0, length7) : throw new UnauthorizedException(RMResources.InvalidAuthHeaderFormat);
                  if (readOnlyMemory6.Span.SequenceEqual<char>(MemoryExtensions.AsSpan("sig")))
                  {
                    readOnlyMemory6 = readOnlyMemory4.Slice(0, length7);
                    if (readOnlyMemory6.ToString().Equals("sig", StringComparison.OrdinalIgnoreCase))
                    {
                      ReadOnlyMemory<char> readOnlyMemory8 = readOnlyMemory4.Slice(length7 + 1);
                      if (readOnlyMemory5.IsEmpty || readOnlyMemory7.IsEmpty || readOnlyMemory8.IsEmpty)
                        throw new UnauthorizedException(RMResources.InvalidAuthHeaderFormat);
                      typeOutput = readOnlyMemory5;
                      versionOutput = readOnlyMemory7;
                      tokenOutput = readOnlyMemory8;
                      return;
                    }
                  }
                }
              }
            }
            throw new UnauthorizedException(RMResources.InvalidAuthHeaderFormat);
          }
        }
      }
      throw new UnauthorizedException(RMResources.InvalidAuthHeaderFormat);
    }

    public static bool CheckPayloadUsingKey(
      ReadOnlyMemory<char> inputToken,
      string verb,
      string resourceId,
      string resourceType,
      INameValueCollection headers,
      string key)
    {
      string authorizationCore = AuthorizationHelper.GenerateKeyAuthorizationCore(verb, resourceId, resourceType, headers, key);
      return inputToken.Span.SequenceEqual<char>(MemoryExtensions.AsSpan(authorizationCore)) || inputToken.ToString().Equals(authorizationCore, StringComparison.OrdinalIgnoreCase);
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

    public static void CheckTimeRangeIsCurrent(
      int allowedClockSkewInSeconds,
      DateTime startDateTime,
      DateTime expiryDateTime)
    {
      if ((startDateTime <= DateTime.MinValue.AddSeconds((double) allowedClockSkewInSeconds) ? 1 : (expiryDateTime >= DateTime.MaxValue.AddSeconds((double) -allowedClockSkewInSeconds) ? 1 : 0)) != 0 || startDateTime.AddSeconds((double) -allowedClockSkewInSeconds) > DateTime.UtcNow || expiryDateTime.AddSeconds((double) allowedClockSkewInSeconds) < DateTime.UtcNow)
      {
        string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, RMResources.InvalidTokenTimeRange, (object) startDateTime.ToString("r", (IFormatProvider) CultureInfo.InvariantCulture), (object) expiryDateTime.ToString("r", (IFormatProvider) CultureInfo.InvariantCulture), (object) Rfc1123DateTimeCache.UtcNow());
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
      if (PathsHelper.TryParsePathSegments(uri.PathAndQuery, out bool _, out resourceType, out resourceId, out isNameBased, clientVersion))
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

    public static int SerializeMessagePayload(
      Span<byte> stream,
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
        headers["x-ms-date"] = Rfc1123DateTimeCache.UtcNow();
        headerValue1 = AuthorizationHelper.GetHeaderValue(headers, "x-ms-date");
      }
      if (!PathsHelper.IsNameBased(resourceId))
        resourceId = resourceId.ToLowerInvariant();
      int start1 = stream.Write(verb.ToLowerInvariant());
      int num1 = 0 + start1;
      stream = stream.Slice(start1);
      int start2 = stream.Write("\n");
      int num2 = start2;
      int num3 = num1 + num2;
      stream = stream.Slice(start2);
      int start3 = stream.Write(resourceType.ToLowerInvariant());
      int num4 = start3;
      int num5 = num3 + num4;
      stream = stream.Slice(start3);
      int start4 = stream.Write("\n");
      int num6 = start4;
      int num7 = num5 + num6;
      stream = stream.Slice(start4);
      int start5 = stream.Write(resourceId);
      int num8 = start5;
      int num9 = num7 + num8;
      stream = stream.Slice(start5);
      int start6 = stream.Write("\n");
      int num10 = start6;
      int num11 = num9 + num10;
      stream = stream.Slice(start6);
      int start7 = stream.Write(headerValue1.ToLowerInvariant());
      int num12 = start7;
      int num13 = num11 + num12;
      stream = stream.Slice(start7);
      int start8 = stream.Write("\n");
      int num14 = start8;
      int num15 = num13 + num14;
      stream = stream.Slice(start8);
      int start9 = stream.Write(headerValue1.Equals(string.Empty, StringComparison.OrdinalIgnoreCase) ? headerValue2.ToLowerInvariant() : string.Empty);
      int num16 = start9;
      int num17 = num15 + num16;
      stream = stream.Slice(start9);
      int num18 = stream.Write("\n");
      return num17 + num18;
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

    internal static string GetHeaderValue(INameValueCollection headerValues, string key) => headerValues == null ? string.Empty : headerValues[key] ?? string.Empty;

    internal static string GetHeaderValue(IDictionary<string, string> headerValues, string key)
    {
      if (headerValues == null)
        return string.Empty;
      string headerValue;
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
        string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, RMResources.InvalidTokenTimeRange, (object) result.ToString("r", (IFormatProvider) CultureInfo.InvariantCulture), (object) DateTime.MaxValue.ToString("r", (IFormatProvider) CultureInfo.InvariantCulture), (object) Rfc1123DateTimeCache.UtcNow());
        DefaultTrace.TraceError(message);
        throw new ForbiddenException(message);
      }
      DateTime expiryDateTime = result + TimeSpan.FromSeconds((double) masterTokenExpiryInSeconds);
      AuthorizationHelper.CheckTimeRangeIsCurrent(allowedClockSkewInSeconds, result, expiryDateTime);
    }

    internal static string GenerateAuthorizationTokenWithHashCore(
      string verb,
      string resourceId,
      string resourceType,
      INameValueCollection headers,
      IComputeHash stringHMACSHA256Helper,
      out AuthorizationHelper.ArrayOwner payload)
    {
      return AuthorizationHelper.GenerateAuthorizationTokenWithHashCore(verb, resourceId, resourceType, headers, stringHMACSHA256Helper, false, out payload);
    }

    private static string GenerateUrlEncodedAuthorizationTokenWithHashCore(
      string verb,
      string resourceId,
      string resourceType,
      INameValueCollection headers,
      IComputeHash stringHMACSHA256Helper,
      out AuthorizationHelper.ArrayOwner payload)
    {
      return AuthorizationHelper.GenerateAuthorizationTokenWithHashCore(verb, resourceId, resourceType, headers, stringHMACSHA256Helper, true, out payload);
    }

    private static string GenerateAuthorizationTokenWithHashCore(
      string verb,
      string resourceId,
      string resourceType,
      INameValueCollection headers,
      IComputeHash stringHMACSHA256Helper,
      bool urlEncode,
      out AuthorizationHelper.ArrayOwner payload)
    {
      if (string.IsNullOrEmpty(verb))
        throw new ArgumentException(RMResources.StringArgumentNullOrEmpty, nameof (verb));
      if (resourceType == null)
        throw new ArgumentNullException(nameof (resourceType));
      if (stringHMACSHA256Helper == null)
        throw new ArgumentNullException(nameof (stringHMACSHA256Helper));
      if (headers == null)
        throw new ArgumentNullException(nameof (headers));
      string str1 = verb ?? string.Empty;
      string resourceIdOrFullName1 = resourceId ?? string.Empty;
      string str2 = resourceType ?? string.Empty;
      string resourceIdOrFullName2 = AuthorizationHelper.GetAuthorizationResourceIdOrFullName(str2, resourceIdOrFullName1);
      byte[] numArray = ArrayPool<byte>.Shared.Rent(AuthorizationHelper.ComputeMemoryCapacity(str1, resourceIdOrFullName2, str2));
      try
      {
        int count = AuthorizationHelper.SerializeMessagePayload((Span<byte>) numArray, str1, resourceIdOrFullName2, str2, headers);
        payload = new AuthorizationHelper.ArrayOwner(ArrayPool<byte>.Shared, new ArraySegment<byte>(numArray, 0, count));
        return AuthorizationHelper.OptimizedConvertToBase64string(stringHMACSHA256Helper.ComputeHash(payload.Buffer), urlEncode);
      }
      catch
      {
        ArrayPool<byte>.Shared.Return(numArray);
        throw;
      }
    }

    private static string OptimizedConvertToBase64string(byte[] hashPayLoad, bool urlEncode)
    {
      byte[] array = ArrayPool<byte>.Shared.Rent(Base64.GetMaxEncodedToUtf8Length(hashPayLoad.Length) * 3);
      try
      {
        Span<byte> span = (Span<byte>) array;
        int bytesWritten;
        OperationStatus utf8 = Base64.EncodeToUtf8((ReadOnlySpan<byte>) hashPayLoad, span, out int _, out bytesWritten);
        if (utf8 != OperationStatus.Done)
          throw new ArgumentException(string.Format("Authorization key payload is invalid. {0}", (object) utf8));
        return urlEncode ? AuthorizationHelper.UrlEncodeBase64SpanInPlace(span, bytesWritten) : Encoding.UTF8.GetString((ReadOnlySpan<byte>) span.Slice(0, bytesWritten));
      }
      finally
      {
        if (array != null)
          ArrayPool<byte>.Shared.Return(array);
      }
    }

    internal static int ComputeMemoryCapacity(
      string verbInput,
      string authResourceId,
      string resourceTypeInput)
    {
      return verbInput.Length + AuthorizationHelper.AuthorizationEncoding.GetMaxByteCount(authResourceId.Length) + resourceTypeInput.Length + 5 + 30;
    }

    private static string GenerateKeyAuthorizationCore(
      string verb,
      string resourceId,
      string resourceType,
      INameValueCollection headers,
      string key)
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
        string str1 = verb ?? string.Empty;
        string resourceIdOrFullName1 = resourceId ?? string.Empty;
        string str2 = resourceType ?? string.Empty;
        string resourceIdOrFullName2 = AuthorizationHelper.GetAuthorizationResourceIdOrFullName(str2, resourceIdOrFullName1);
        byte[] numArray = ArrayPool<byte>.Shared.Rent(AuthorizationHelper.ComputeMemoryCapacity(str1, resourceIdOrFullName2, str2));
        try
        {
          int count = AuthorizationHelper.SerializeMessagePayload((Span<byte>) numArray, str1, resourceIdOrFullName2, str2, headers);
          return Convert.ToBase64String(hmacshA256.ComputeHash(numArray, 0, count));
        }
        finally
        {
          ArrayPool<byte>.Shared.Return(numArray);
        }
      }
    }

    public static unsafe string UrlEncodeBase64SpanInPlace(Span<byte> base64Bytes, int length)
    {
      if (base64Bytes == new Span<byte>())
        throw new ArgumentNullException(nameof (base64Bytes));
      if (base64Bytes.Length < length * 3)
        throw new ArgumentException("base64Bytes should be 3x to avoid running out of space in worst case scenario where all characters are special");
      if (length == 0)
        return string.Empty;
      int num1 = base64Bytes.Length - 1;
      for (int index1 = length - 1; index1 >= 0; --index1)
      {
        byte num2 = base64Bytes[index1];
        switch (num2)
        {
          case 43:
            ref Span<byte> local1 = ref base64Bytes;
            int index2 = num1;
            int num3 = index2 - 1;
            local1[index2] = (byte) 98;
            ref Span<byte> local2 = ref base64Bytes;
            int index3 = num3;
            int num4 = index3 - 1;
            local2[index3] = (byte) 50;
            ref Span<byte> local3 = ref base64Bytes;
            int index4 = num4;
            num1 = index4 - 1;
            local3[index4] = (byte) 37;
            break;
          case 47:
            ref Span<byte> local4 = ref base64Bytes;
            int index5 = num1;
            int num5 = index5 - 1;
            local4[index5] = (byte) 102;
            ref Span<byte> local5 = ref base64Bytes;
            int index6 = num5;
            int num6 = index6 - 1;
            local5[index6] = (byte) 50;
            ref Span<byte> local6 = ref base64Bytes;
            int index7 = num6;
            num1 = index7 - 1;
            local6[index7] = (byte) 37;
            break;
          case 61:
            ref Span<byte> local7 = ref base64Bytes;
            int index8 = num1;
            int num7 = index8 - 1;
            local7[index8] = (byte) 100;
            ref Span<byte> local8 = ref base64Bytes;
            int index9 = num7;
            int num8 = index9 - 1;
            local8[index9] = (byte) 51;
            ref Span<byte> local9 = ref base64Bytes;
            int index10 = num8;
            num1 = index10 - 1;
            local9[index10] = (byte) 37;
            break;
          default:
            base64Bytes[num1--] = num2;
            break;
        }
      }
      Span<byte> span = base64Bytes.Slice(num1 + 1);
      fixed (byte* bytes = &span.GetPinnableReference())
        return Encoding.UTF8.GetString(bytes, span.Length);
    }

    private static int Write(this Span<byte> stream, string contentToWrite) => AuthorizationHelper.AuthorizationEncoding.GetBytes(contentToWrite, stream);

    public struct ArrayOwner : IDisposable
    {
      private readonly ArrayPool<byte> pool;

      public ArrayOwner(ArrayPool<byte> pool, ArraySegment<byte> buffer)
      {
        this.pool = pool;
        this.Buffer = buffer;
      }

      public ArraySegment<byte> Buffer { get; private set; }

      public void Dispose()
      {
        if (this.Buffer.Array == null)
          return;
        this.pool?.Return(this.Buffer.Array);
        this.Buffer = new ArraySegment<byte>();
      }
    }
  }
}
