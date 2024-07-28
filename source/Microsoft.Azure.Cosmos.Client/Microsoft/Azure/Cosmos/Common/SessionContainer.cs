// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Common.SessionContainer
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Core.Trace;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Collections;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Microsoft.Azure.Cosmos.Common
{
  internal sealed class SessionContainer : ISessionContainer
  {
    private static readonly string sessionTokenSeparator = ":";
    private volatile SessionContainer.SessionContainerState state;

    public SessionContainer(string hostName) => this.state = new SessionContainer.SessionContainerState(hostName);

    public void ReplaceCurrrentStateWithStateOf(SessionContainer comrade) => this.state = comrade.state;

    public string HostName => this.state.hostName;

    public string GetSessionToken(string collectionLink) => SessionContainer.GetSessionToken(this.state, collectionLink);

    public string ResolveGlobalSessionToken(DocumentServiceRequest request) => SessionContainer.ResolveGlobalSessionToken(this.state, request);

    public ISessionToken ResolvePartitionLocalSessionToken(
      DocumentServiceRequest request,
      string partitionKeyRangeId)
    {
      return SessionContainer.ResolvePartitionLocalSessionToken(this.state, request, partitionKeyRangeId);
    }

    public string ResolvePartitionLocalSessionTokenForGateway(
      DocumentServiceRequest request,
      string partitionKeyRangeId)
    {
      return SessionContainer.ResolvePartitionLocalSessionTokenForGateway(this.state, request, partitionKeyRangeId);
    }

    public void ClearTokenByCollectionFullname(string collectionFullname) => SessionContainer.ClearTokenByCollectionFullname(this.state, collectionFullname);

    public void ClearTokenByResourceId(string resourceId) => SessionContainer.ClearTokenByResourceId(this.state, resourceId);

    public void SetSessionToken(
      string collectionRid,
      string collectionFullname,
      INameValueCollection responseHeaders)
    {
      SessionContainer.SetSessionToken(this.state, collectionRid, collectionFullname, responseHeaders);
    }

    public void SetSessionToken(
      DocumentServiceRequest request,
      INameValueCollection responseHeaders)
    {
      SessionContainer.SetSessionToken(this.state, request, responseHeaders);
    }

    public object MakeSnapshot() => (object) SessionContainer.MakeSnapshot(this.state);

    private static string GetSessionToken(
      SessionContainer.SessionContainerState self,
      string collectionLink)
    {
      string resourceIdOrFullName;
      bool isNameBased;
      int num1 = PathsHelper.TryParsePathSegments(collectionLink, out bool _, out string _, out resourceIdOrFullName, out isNameBased) ? 1 : 0;
      ConcurrentDictionary<string, ISessionToken> partitionKeyRangeIdToTokenMap = (ConcurrentDictionary<string, ISessionToken>) null;
      if (num1 != 0)
      {
        ulong? nullable = new ulong?();
        if (isNameBased)
        {
          string collectionPath = PathsHelper.GetCollectionPath(resourceIdOrFullName);
          ulong num2;
          if (self.collectionNameByResourceId.TryGetValue(collectionPath, out num2))
            nullable = new ulong?(num2);
        }
        else
        {
          ResourceId resourceId = ResourceId.Parse(resourceIdOrFullName);
          if (resourceId.DocumentCollection != 0U)
            nullable = new ulong?(resourceId.UniqueDocumentCollectionId);
        }
        if (nullable.HasValue)
          self.sessionTokensRIDBased.TryGetValue(nullable.Value, out partitionKeyRangeIdToTokenMap);
      }
      return partitionKeyRangeIdToTokenMap == null ? string.Empty : SessionContainer.GetSessionTokenString(partitionKeyRangeIdToTokenMap);
    }

    private static string ResolveGlobalSessionToken(
      SessionContainer.SessionContainerState self,
      DocumentServiceRequest request)
    {
      ConcurrentDictionary<string, ISessionToken> rangeIdToTokenMap = SessionContainer.GetPartitionKeyRangeIdToTokenMap(self, request);
      return rangeIdToTokenMap != null ? SessionContainer.GetSessionTokenString(rangeIdToTokenMap) : string.Empty;
    }

    private static ISessionToken ResolvePartitionLocalSessionToken(
      SessionContainer.SessionContainerState self,
      DocumentServiceRequest request,
      string partitionKeyRangeId)
    {
      return SessionTokenHelper.ResolvePartitionLocalSessionToken(request, partitionKeyRangeId, SessionContainer.GetPartitionKeyRangeIdToTokenMap(self, request));
    }

    private static string ResolvePartitionLocalSessionTokenForGateway(
      SessionContainer.SessionContainerState self,
      DocumentServiceRequest request,
      string partitionKeyRangeId)
    {
      ConcurrentDictionary<string, ISessionToken> rangeIdToTokenMap = SessionContainer.GetPartitionKeyRangeIdToTokenMap(self, request);
      if (rangeIdToTokenMap != null)
      {
        ISessionToken other;
        if (rangeIdToTokenMap.TryGetValue(partitionKeyRangeId, out other))
          return partitionKeyRangeId + SessionContainer.sessionTokenSeparator + other.ConvertToString();
        if (request.RequestContext.ResolvedPartitionKeyRange.Parents != null)
        {
          ISessionToken sessionToken = (ISessionToken) null;
          for (int index = request.RequestContext.ResolvedPartitionKeyRange.Parents.Count - 1; index >= 0; --index)
          {
            if (rangeIdToTokenMap.TryGetValue(request.RequestContext.ResolvedPartitionKeyRange.Parents[index], out other))
              sessionToken = sessionToken != null ? sessionToken.Merge(other) : other;
          }
          if (sessionToken != null)
            return partitionKeyRangeId + SessionContainer.sessionTokenSeparator + sessionToken.ConvertToString();
        }
      }
      return (string) null;
    }

    private static void ClearTokenByCollectionFullname(
      SessionContainer.SessionContainerState self,
      string collectionFullname)
    {
      if (string.IsNullOrEmpty(collectionFullname))
        return;
      string collectionPath = PathsHelper.GetCollectionPath(collectionFullname);
      self.rwlock.EnterWriteLock();
      try
      {
        if (!self.collectionNameByResourceId.ContainsKey(collectionPath))
          return;
        ulong key = self.collectionNameByResourceId[collectionPath];
        self.sessionTokensRIDBased.TryRemove(key, out ConcurrentDictionary<string, ISessionToken> _);
        self.collectionResourceIdByName.TryRemove(key, out string _);
        self.collectionNameByResourceId.TryRemove(collectionPath, out ulong _);
      }
      finally
      {
        self.rwlock.ExitWriteLock();
      }
    }

    private static void ClearTokenByResourceId(
      SessionContainer.SessionContainerState self,
      string resourceId)
    {
      if (string.IsNullOrEmpty(resourceId))
        return;
      ResourceId resourceId1 = ResourceId.Parse(resourceId);
      if (resourceId1.DocumentCollection == 0U)
        return;
      ulong documentCollectionId = resourceId1.UniqueDocumentCollectionId;
      self.rwlock.EnterWriteLock();
      try
      {
        if (!self.collectionResourceIdByName.ContainsKey(documentCollectionId))
          return;
        string key = self.collectionResourceIdByName[documentCollectionId];
        self.sessionTokensRIDBased.TryRemove(documentCollectionId, out ConcurrentDictionary<string, ISessionToken> _);
        self.collectionResourceIdByName.TryRemove(documentCollectionId, out string _);
        self.collectionNameByResourceId.TryRemove(key, out ulong _);
      }
      finally
      {
        self.rwlock.ExitWriteLock();
      }
    }

    private static void SetSessionToken(
      SessionContainer.SessionContainerState self,
      string collectionRid,
      string collectionFullname,
      INameValueCollection responseHeaders)
    {
      ResourceId resourceId = ResourceId.Parse(collectionRid);
      string collectionPath = PathsHelper.GetCollectionPath(collectionFullname);
      string responseHeader = responseHeaders["x-ms-session-token"];
      if (string.IsNullOrEmpty(responseHeader))
        return;
      SessionContainer.SetSessionToken(self, resourceId, collectionPath, responseHeader);
    }

    private static void SetSessionToken(
      SessionContainer.SessionContainerState self,
      DocumentServiceRequest request,
      INameValueCollection responseHeaders)
    {
      string responseHeader = responseHeaders["x-ms-session-token"];
      ResourceId resourceId;
      string collectionName;
      if (string.IsNullOrEmpty(responseHeader) || !SessionContainer.ShouldUpdateSessionToken(request, responseHeaders, out resourceId, out collectionName))
        return;
      SessionContainer.SetSessionToken(self, resourceId, collectionName, responseHeader);
    }

    private static SessionContainer.SessionContainerSnapshot MakeSnapshot(
      SessionContainer.SessionContainerState self)
    {
      self.rwlock.EnterReadLock();
      try
      {
        return new SessionContainer.SessionContainerSnapshot(self.collectionNameByResourceId, self.collectionResourceIdByName, self.sessionTokensRIDBased);
      }
      finally
      {
        self.rwlock.ExitReadLock();
      }
    }

    private static ConcurrentDictionary<string, ISessionToken> GetPartitionKeyRangeIdToTokenMap(
      SessionContainer.SessionContainerState self,
      DocumentServiceRequest request)
    {
      ulong? nullable = new ulong?();
      if (request.IsNameBased)
      {
        string collectionPath = PathsHelper.GetCollectionPath(request.ResourceAddress);
        ulong num;
        if (self.collectionNameByResourceId.TryGetValue(collectionPath, out num))
          nullable = new ulong?(num);
      }
      else if (!string.IsNullOrEmpty(request.ResourceId))
      {
        ResourceId resourceId = ResourceId.Parse(request.ResourceId);
        if (resourceId.DocumentCollection != 0U)
          nullable = new ulong?(resourceId.UniqueDocumentCollectionId);
      }
      ConcurrentDictionary<string, ISessionToken> rangeIdToTokenMap = (ConcurrentDictionary<string, ISessionToken>) null;
      if (nullable.HasValue)
        self.sessionTokensRIDBased.TryGetValue(nullable.Value, out rangeIdToTokenMap);
      return rangeIdToTokenMap;
    }

    private static void SetSessionToken(
      SessionContainer.SessionContainerState self,
      ResourceId resourceId,
      string collectionName,
      string encodedToken)
    {
      string partitionKeyRangeId;
      ISessionToken token;
      if (VersionUtility.IsLaterThan(HttpConstants.Versions.CurrentVersion, HttpConstants.VersionDates.v2015_12_16))
      {
        string[] strArray = encodedToken.Split(':');
        partitionKeyRangeId = strArray[0];
        token = SessionTokenHelper.Parse(strArray[1], HttpConstants.Versions.CurrentVersion);
      }
      else
      {
        partitionKeyRangeId = "0";
        token = SessionTokenHelper.Parse(encodedToken, HttpConstants.Versions.CurrentVersion);
      }
      DefaultTrace.TraceVerbose("Update Session token {0} {1} {2}", (object) resourceId.UniqueDocumentCollectionId, (object) collectionName, (object) token);
      bool flag = false;
      self.rwlock.EnterReadLock();
      try
      {
        ulong num;
        string str;
        flag = self.collectionNameByResourceId.TryGetValue(collectionName, out num) && self.collectionResourceIdByName.TryGetValue(resourceId.UniqueDocumentCollectionId, out str) && (long) num == (long) resourceId.UniqueDocumentCollectionId && str == collectionName;
        if (flag)
          SessionContainer.AddSessionToken(self, resourceId.UniqueDocumentCollectionId, partitionKeyRangeId, token);
      }
      finally
      {
        self.rwlock.ExitReadLock();
      }
      if (flag)
        return;
      self.rwlock.EnterWriteLock();
      try
      {
        ulong key;
        if (self.collectionNameByResourceId.TryGetValue(collectionName, out key))
        {
          self.sessionTokensRIDBased.TryRemove(key, out ConcurrentDictionary<string, ISessionToken> _);
          self.collectionResourceIdByName.TryRemove(key, out string _);
        }
        self.collectionNameByResourceId[collectionName] = resourceId.UniqueDocumentCollectionId;
        self.collectionResourceIdByName[resourceId.UniqueDocumentCollectionId] = collectionName;
        SessionContainer.AddSessionToken(self, resourceId.UniqueDocumentCollectionId, partitionKeyRangeId, token);
      }
      finally
      {
        self.rwlock.ExitWriteLock();
      }
    }

    private static void AddSessionToken(
      SessionContainer.SessionContainerState self,
      ulong rid,
      string partitionKeyRangeId,
      ISessionToken token)
    {
      ConcurrentDictionary<string, ISessionToken> concurrentDictionary;
      if (!self.sessionTokensRIDBased.TryGetValue(rid, out concurrentDictionary))
      {
        concurrentDictionary = new ConcurrentDictionary<string, ISessionToken>();
        if (!self.sessionTokensRIDBased.TryAdd(rid, concurrentDictionary) && !self.sessionTokensRIDBased.TryGetValue(rid, out concurrentDictionary))
          throw new InternalServerErrorException("AddSessionToken failed to get or add the session token dictionary.");
      }
      concurrentDictionary.AddOrUpdate(partitionKeyRangeId, token, (Func<string, ISessionToken, ISessionToken>) ((existingPartitionKeyRangeId, existingToken) => existingToken.Merge(token)));
    }

    private static string GetSessionTokenString(
      ConcurrentDictionary<string, ISessionToken> partitionKeyRangeIdToTokenMap)
    {
      StringBuilder stringBuilder = new StringBuilder();
      foreach (KeyValuePair<string, ISessionToken> keyRangeIdToToken in partitionKeyRangeIdToTokenMap)
      {
        if (stringBuilder.Length > 0)
          stringBuilder.Append(",");
        stringBuilder.Append(keyRangeIdToToken.Key);
        stringBuilder.Append(SessionContainer.sessionTokenSeparator);
        stringBuilder.Append(keyRangeIdToToken.Value.ConvertToString());
      }
      return stringBuilder.ToString();
    }

    private static bool AreDictionariesEqual(
      Dictionary<string, ISessionToken> first,
      Dictionary<string, ISessionToken> second)
    {
      if (first.Count != second.Count)
        return false;
      foreach (KeyValuePair<string, ISessionToken> keyValuePair in first)
      {
        ISessionToken sessionToken;
        if (second.TryGetValue(keyValuePair.Key, out sessionToken) && !sessionToken.Equals(keyValuePair.Value))
          return false;
      }
      return true;
    }

    private static bool ShouldUpdateSessionToken(
      DocumentServiceRequest request,
      INameValueCollection responseHeaders,
      out ResourceId resourceId,
      out string collectionName)
    {
      resourceId = (ResourceId) null;
      string resourceFullName = responseHeaders["x-ms-alt-content-path"];
      if (string.IsNullOrEmpty(resourceFullName))
        resourceFullName = request.ResourceAddress;
      collectionName = PathsHelper.GetCollectionPath(resourceFullName);
      string id;
      if (request.IsNameBased)
      {
        id = responseHeaders["x-ms-content-path"];
        if (string.IsNullOrEmpty(id))
          id = request.ResourceId;
      }
      else
        id = request.ResourceId;
      if (!string.IsNullOrEmpty(id))
      {
        resourceId = ResourceId.Parse(id);
        if (resourceId.DocumentCollection != 0U && collectionName != null && !ReplicatedResourceClient.IsReadingFromMaster(request.ResourceType, request.OperationType))
          return true;
      }
      return false;
    }

    private sealed class SessionContainerState
    {
      public readonly string hostName;
      public readonly ReaderWriterLockSlim rwlock = new ReaderWriterLockSlim();
      public readonly ConcurrentDictionary<string, ulong> collectionNameByResourceId = new ConcurrentDictionary<string, ulong>();
      public readonly ConcurrentDictionary<ulong, string> collectionResourceIdByName = new ConcurrentDictionary<ulong, string>();
      public readonly ConcurrentDictionary<ulong, ConcurrentDictionary<string, ISessionToken>> sessionTokensRIDBased = new ConcurrentDictionary<ulong, ConcurrentDictionary<string, ISessionToken>>();

      public SessionContainerState(string hostName) => this.hostName = hostName;

      ~SessionContainerState()
      {
        if (this.rwlock == null)
          return;
        this.rwlock.Dispose();
      }
    }

    private sealed class SessionContainerSnapshot
    {
      private readonly Dictionary<string, ulong> collectionNameByResourceId;
      private readonly Dictionary<ulong, string> collectionResourceIdByName;
      private readonly Dictionary<ulong, Dictionary<string, ISessionToken>> sessionTokensRIDBased;

      public SessionContainerSnapshot(
        ConcurrentDictionary<string, ulong> collectionNameByResourceId,
        ConcurrentDictionary<ulong, string> collectionResourceIdByName,
        ConcurrentDictionary<ulong, ConcurrentDictionary<string, ISessionToken>> sessionTokensRIDBased)
      {
        this.collectionNameByResourceId = new Dictionary<string, ulong>((IDictionary<string, ulong>) collectionNameByResourceId);
        this.collectionResourceIdByName = new Dictionary<ulong, string>((IDictionary<ulong, string>) collectionResourceIdByName);
        this.sessionTokensRIDBased = new Dictionary<ulong, Dictionary<string, ISessionToken>>();
        foreach (KeyValuePair<ulong, ConcurrentDictionary<string, ISessionToken>> keyValuePair in sessionTokensRIDBased)
          this.sessionTokensRIDBased.Add(keyValuePair.Key, new Dictionary<string, ISessionToken>((IDictionary<string, ISessionToken>) keyValuePair.Value));
      }

      public override int GetHashCode() => 1;

      public override bool Equals(object obj) => obj != null && obj is SessionContainer.SessionContainerSnapshot containerSnapshot && this.collectionNameByResourceId.Count == containerSnapshot.collectionNameByResourceId.Count && this.collectionResourceIdByName.Count == containerSnapshot.collectionResourceIdByName.Count && this.sessionTokensRIDBased.Count == containerSnapshot.sessionTokensRIDBased.Count && SessionContainer.SessionContainerSnapshot.AreDictionariesEqual<string, ulong>(this.collectionNameByResourceId, containerSnapshot.collectionNameByResourceId, (Func<ulong, ulong, bool>) ((x, y) => (long) x == (long) y)) && SessionContainer.SessionContainerSnapshot.AreDictionariesEqual<ulong, string>(this.collectionResourceIdByName, containerSnapshot.collectionResourceIdByName, (Func<string, string, bool>) ((x, y) => x == y)) && SessionContainer.SessionContainerSnapshot.AreDictionariesEqual<ulong, Dictionary<string, ISessionToken>>(this.sessionTokensRIDBased, containerSnapshot.sessionTokensRIDBased, (Func<Dictionary<string, ISessionToken>, Dictionary<string, ISessionToken>, bool>) ((x, y) => SessionContainer.SessionContainerSnapshot.AreDictionariesEqual<string, ISessionToken>(x, y, (Func<ISessionToken, ISessionToken, bool>) ((a, b) => a.Equals(b)))));

      private static bool AreDictionariesEqual<T, U>(
        Dictionary<T, U> left,
        Dictionary<T, U> right,
        Func<U, U, bool> areEqual)
      {
        if (left.Count != right.Count)
          return false;
        foreach (KeyValuePair<T, U> keyValuePair in left)
        {
          U u;
          if (!right.TryGetValue(keyValuePair.Key, out u) || !areEqual(keyValuePair.Value, u))
            return false;
        }
        return true;
      }
    }
  }
}
