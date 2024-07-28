// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.SessionTokenHelper
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using Microsoft.Azure.Cosmos.Core.Trace;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.Azure.Documents
{
  internal static class SessionTokenHelper
  {
    public static readonly char[] CharArrayWithColon = new char[1]
    {
      ':'
    };
    public static readonly char[] CharArrayWithComma = new char[1]
    {
      ','
    };
    private static readonly char[] CharArrayWithCommaAndColon = new char[2]
    {
      ',',
      ':'
    };

    public static void SetOriginalSessionToken(
      DocumentServiceRequest request,
      string originalSessionToken)
    {
      if (request == null)
        throw new ArgumentException(nameof (request));
      if (originalSessionToken == null)
        request.Headers.Remove("x-ms-session-token");
      else
        request.Headers["x-ms-session-token"] = originalSessionToken;
    }

    public static void ValidateAndRemoveSessionToken(DocumentServiceRequest request)
    {
      string header = request.Headers["x-ms-session-token"];
      if (string.IsNullOrEmpty(header))
        return;
      SessionTokenHelper.GetLocalSessionToken(request, header, string.Empty);
      request.Headers.Remove("x-ms-session-token");
    }

    public static void SetPartitionLocalSessionToken(
      DocumentServiceRequest entity,
      ISessionContainer sessionContainer)
    {
      string globalSessionToken = entity != null ? entity.Headers["x-ms-session-token"] : throw new ArgumentException(nameof (entity));
      string id = entity.RequestContext.ResolvedPartitionKeyRange.Id;
      if (string.IsNullOrEmpty(id))
        throw new InternalServerErrorException(RMResources.PartitionKeyRangeIdAbsentInContext);
      if (!string.IsNullOrEmpty(globalSessionToken))
      {
        ISessionToken localSessionToken = SessionTokenHelper.GetLocalSessionToken(entity, globalSessionToken, id);
        entity.RequestContext.SessionToken = localSessionToken;
      }
      else
      {
        ISessionToken sessionToken = sessionContainer.ResolvePartitionLocalSessionToken(entity, id);
        entity.RequestContext.SessionToken = sessionToken;
      }
      if (entity.RequestContext.SessionToken == null)
      {
        entity.Headers.Remove("x-ms-session-token");
      }
      else
      {
        string header = entity.Headers["x-ms-version"];
        if (VersionUtility.IsLaterThan(string.IsNullOrEmpty(header) ? HttpConstants.Versions.CurrentVersion : header, HttpConstants.VersionDates.v2015_12_16))
          entity.Headers["x-ms-session-token"] = SessionTokenHelper.SerializeSessionToken(id, entity.RequestContext.SessionToken);
        else
          entity.Headers["x-ms-session-token"] = entity.RequestContext.SessionToken.ConvertToString();
      }
    }

    internal static ISessionToken GetLocalSessionToken(
      DocumentServiceRequest request,
      string globalSessionToken,
      string partitionKeyRangeId)
    {
      string header = request.Headers["x-ms-version"];
      if (!VersionUtility.IsLaterThan(string.IsNullOrEmpty(header) ? HttpConstants.Versions.CurrentVersion : header, HttpConstants.VersionDates.v2015_12_16))
      {
        ISessionToken parsedSessionToken;
        if (!SimpleSessionToken.TryCreate(globalSessionToken, out parsedSessionToken))
          throw new BadRequestException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, RMResources.InvalidSessionToken, (object) globalSessionToken));
        return parsedSessionToken;
      }
      HashSet<string> stringSet = new HashSet<string>((IEqualityComparer<string>) StringComparer.Ordinal);
      stringSet.Add(partitionKeyRangeId);
      ISessionToken localSessionToken1 = (ISessionToken) null;
      if (request.RequestContext.ResolvedPartitionKeyRange != null && request.RequestContext.ResolvedPartitionKeyRange.Parents != null)
        stringSet.UnionWith((IEnumerable<string>) request.RequestContext.ResolvedPartitionKeyRange.Parents);
      foreach (string localSessionToken2 in SessionTokenHelper.SplitPartitionLocalSessionTokens(globalSessionToken))
      {
        string[] strArray = localSessionToken2.Split(SessionTokenHelper.CharArrayWithColon, StringSplitOptions.RemoveEmptyEntries);
        ISessionToken other = strArray.Length == 2 ? SessionTokenHelper.Parse(strArray[1]) : throw new BadRequestException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, RMResources.InvalidSessionToken, (object) localSessionToken2));
        if (stringSet.Contains(strArray[0]))
          localSessionToken1 = localSessionToken1 != null ? localSessionToken1.Merge(other) : other;
      }
      return localSessionToken1;
    }

    internal static ISessionToken ResolvePartitionLocalSessionToken(
      DocumentServiceRequest request,
      string partitionKeyRangeId,
      ConcurrentDictionary<string, ISessionToken> partitionKeyRangeIdToTokenMap)
    {
      if (partitionKeyRangeIdToTokenMap != null)
      {
        ISessionToken other;
        if (partitionKeyRangeIdToTokenMap.TryGetValue(partitionKeyRangeId, out other))
          return other;
        if (request.RequestContext.ResolvedPartitionKeyRange.Parents != null)
        {
          ISessionToken sessionToken = (ISessionToken) null;
          for (int index = request.RequestContext.ResolvedPartitionKeyRange.Parents.Count - 1; index >= 0; --index)
          {
            if (partitionKeyRangeIdToTokenMap.TryGetValue(request.RequestContext.ResolvedPartitionKeyRange.Parents[index], out other))
              sessionToken = sessionToken != null ? sessionToken.Merge(other) : other;
          }
          if (sessionToken != null)
            return sessionToken;
        }
      }
      return (ISessionToken) null;
    }

    internal static ISessionToken Parse(string sessionToken)
    {
      ISessionToken parsedSessionToken;
      if (SessionTokenHelper.TryParse(sessionToken, out parsedSessionToken))
        return parsedSessionToken;
      throw new BadRequestException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, RMResources.InvalidSessionToken, (object) sessionToken));
    }

    internal static bool TryParse(string sessionToken, out ISessionToken parsedSessionToken) => SessionTokenHelper.TryParse(sessionToken, out string _, out parsedSessionToken);

    internal static bool TryParse(
      string sessionToken,
      out string partitionKeyRangeId,
      out ISessionToken parsedSessionToken)
    {
      parsedSessionToken = (ISessionToken) null;
      string sessionToken1;
      return SessionTokenHelper.TryParse(sessionToken, out partitionKeyRangeId, out sessionToken1) && SessionTokenHelper.TryParseSessionToken(sessionToken1, out parsedSessionToken);
    }

    internal static bool TryParseSessionToken(
      string sessionToken,
      out ISessionToken parsedSessionToken)
    {
      parsedSessionToken = (ISessionToken) null;
      if (string.IsNullOrEmpty(sessionToken))
        return false;
      return VectorSessionToken.TryCreate(sessionToken, out parsedSessionToken) || SimpleSessionToken.TryCreate(sessionToken, out parsedSessionToken);
    }

    internal static bool TryParse(
      string sessionTokenString,
      out string partitionKeyRangeId,
      out string sessionToken)
    {
      partitionKeyRangeId = (string) null;
      if (string.IsNullOrEmpty(sessionTokenString))
      {
        sessionToken = (string) null;
        return false;
      }
      int length = sessionTokenString.IndexOf(':');
      if (length < 0)
      {
        sessionToken = sessionTokenString;
        return true;
      }
      partitionKeyRangeId = sessionTokenString.Substring(0, length);
      sessionToken = sessionTokenString.Substring(length + 1);
      return true;
    }

    internal static ISessionToken Parse(string sessionToken, string version)
    {
      string sessionToken1;
      if (SessionTokenHelper.TryParse(sessionToken, out string _, out sessionToken1))
      {
        if (VersionUtility.IsLaterThan(version, HttpConstants.VersionDates.v2018_06_18))
        {
          ISessionToken parsedSessionToken;
          if (VectorSessionToken.TryCreate(sessionToken1, out parsedSessionToken))
            return parsedSessionToken;
        }
        else
        {
          ISessionToken parsedSessionToken;
          if (SimpleSessionToken.TryCreate(sessionToken1, out parsedSessionToken))
            return parsedSessionToken;
        }
      }
      DefaultTrace.TraceCritical("Unable to parse session token {0} for version {1}", (object) sessionToken, (object) version);
      throw new InternalServerErrorException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, RMResources.InvalidSessionToken, (object) sessionToken));
    }

    internal static bool IsSingleGlobalLsnSessionToken(string sessionToken) => sessionToken != null && sessionToken.IndexOfAny(SessionTokenHelper.CharArrayWithCommaAndColon) < 0;

    internal static bool TryFindPartitionLocalSessionToken(
      string sessionTokens,
      string partitionKeyRangeId,
      out string partitionLocalSessionToken)
    {
      foreach (string localSessionToken in SessionTokenHelper.SplitPartitionLocalSessionTokens(sessionTokens))
      {
        string partitionKeyRangeId1;
        if (SessionTokenHelper.TryParse(localSessionToken, out partitionKeyRangeId1, out partitionLocalSessionToken) && partitionKeyRangeId1 == partitionKeyRangeId)
          return true;
      }
      partitionLocalSessionToken = (string) null;
      return false;
    }

    private static IEnumerable<string> SplitPartitionLocalSessionTokens(string sessionTokens)
    {
      if (sessionTokens != null)
      {
        string[] strArray = sessionTokens.Split(SessionTokenHelper.CharArrayWithComma, StringSplitOptions.RemoveEmptyEntries);
        for (int index = 0; index < strArray.Length; ++index)
          yield return strArray[index];
        strArray = (string[]) null;
      }
    }

    internal static string SerializeSessionToken(
      string partitionKeyRangeId,
      ISessionToken parsedSessionToken)
    {
      if (partitionKeyRangeId != null)
        return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}:{1}", (object) partitionKeyRangeId, (object) parsedSessionToken.ConvertToString());
      return parsedSessionToken?.ConvertToString();
    }
  }
}
