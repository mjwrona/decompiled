// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.SessionTokenHelper
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Cosmos.Core.Trace;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.Azure.Documents
{
  internal static class SessionTokenHelper
  {
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
        if (VersionUtility.IsLaterThan(string.IsNullOrEmpty(header) ? HttpConstants.Versions.CurrentVersion : header, HttpConstants.Versions.v2015_12_16))
          entity.Headers["x-ms-session-token"] = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}:{1}", (object) id, (object) entity.RequestContext.SessionToken.ConvertToString());
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
      if (!VersionUtility.IsLaterThan(string.IsNullOrEmpty(header) ? HttpConstants.Versions.CurrentVersion : header, HttpConstants.Versions.v2015_12_16))
      {
        ISessionToken parsedSessionToken;
        if (!SimpleSessionToken.TryCreate(globalSessionToken, out parsedSessionToken))
          throw new BadRequestException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, RMResources.InvalidSessionToken, (object) globalSessionToken));
        return parsedSessionToken;
      }
      string[] strArray1 = globalSessionToken.Split(new char[1]
      {
        ','
      }, StringSplitOptions.RemoveEmptyEntries);
      HashSet<string> stringSet = new HashSet<string>((IEqualityComparer<string>) StringComparer.Ordinal);
      stringSet.Add(partitionKeyRangeId);
      ISessionToken localSessionToken = (ISessionToken) null;
      if (request.RequestContext.ResolvedPartitionKeyRange != null && request.RequestContext.ResolvedPartitionKeyRange.Parents != null)
        stringSet.UnionWith((IEnumerable<string>) request.RequestContext.ResolvedPartitionKeyRange.Parents);
      foreach (string str in strArray1)
      {
        string[] strArray2 = str.Split(new char[1]{ ':' }, StringSplitOptions.RemoveEmptyEntries);
        ISessionToken other = strArray2.Length == 2 ? SessionTokenHelper.Parse(strArray2[1]) : throw new BadRequestException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, RMResources.InvalidSessionToken, (object) str));
        if (stringSet.Contains(strArray2[0]))
          localSessionToken = localSessionToken != null ? localSessionToken.Merge(other) : other;
      }
      return localSessionToken;
    }

    internal static ISessionToken ResolvePartitionLocalSessionToken(
      DocumentServiceRequest request,
      string partitionKeyRangeId,
      ConcurrentDictionary<string, ISessionToken> partitionKeyRangeIdToTokenMap)
    {
      if (partitionKeyRangeIdToTokenMap != null)
      {
        ISessionToken sessionToken;
        if (partitionKeyRangeIdToTokenMap.TryGetValue(partitionKeyRangeId, out sessionToken))
          return sessionToken;
        if (request.RequestContext.ResolvedPartitionKeyRange.Parents != null)
        {
          for (int index = request.RequestContext.ResolvedPartitionKeyRange.Parents.Count - 1; index >= 0; --index)
          {
            if (partitionKeyRangeIdToTokenMap.TryGetValue(request.RequestContext.ResolvedPartitionKeyRange.Parents[index], out sessionToken))
              return sessionToken;
          }
        }
      }
      return (ISessionToken) null;
    }

    internal static ISessionToken Parse(string sessionToken)
    {
      ISessionToken parsedSessionToken = (ISessionToken) null;
      if (SessionTokenHelper.TryParse(sessionToken, out parsedSessionToken))
        return parsedSessionToken;
      throw new BadRequestException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, RMResources.InvalidSessionToken, (object) sessionToken));
    }

    internal static bool TryParse(string sessionToken, out ISessionToken parsedSessionToken)
    {
      parsedSessionToken = (ISessionToken) null;
      if (string.IsNullOrEmpty(sessionToken))
        return false;
      string[] source = sessionToken.Split(':');
      return SimpleSessionToken.TryCreate(((IEnumerable<string>) source).Last<string>(), out parsedSessionToken) || VectorSessionToken.TryCreate(((IEnumerable<string>) source).Last<string>(), out parsedSessionToken);
    }

    internal static ISessionToken Parse(string sessionToken, string version)
    {
      if (!string.IsNullOrEmpty(sessionToken))
      {
        string[] source = sessionToken.Split(':');
        if (VersionUtility.IsLaterThan(version, HttpConstants.Versions.v2018_06_18))
        {
          ISessionToken parsedSessionToken;
          if (VectorSessionToken.TryCreate(((IEnumerable<string>) source).Last<string>(), out parsedSessionToken))
            return parsedSessionToken;
        }
        else
        {
          ISessionToken parsedSessionToken;
          if (SimpleSessionToken.TryCreate(((IEnumerable<string>) source).Last<string>(), out parsedSessionToken))
            return parsedSessionToken;
        }
      }
      DefaultTrace.TraceCritical("Unable to parse session token {0} for version {1}", (object) sessionToken, (object) version);
      throw new InternalServerErrorException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, RMResources.InvalidSessionToken, (object) sessionToken));
    }
  }
}
