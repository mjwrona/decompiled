// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.ConnectionStateListener
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Cosmos.Core.Trace;
using Microsoft.Azure.Documents.Rntbd;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Azure.Documents
{
  internal sealed class ConnectionStateListener : IConnectionStateListener
  {
    private readonly IAddressResolverExtension addressResolver;

    public ConcurrentDictionary<ServerKey, HashSet<AddressCacheToken>> partitionAddressCache { get; }

    public ConnectionStateListener(IAddressResolverExtension addressResolver)
    {
      this.partitionAddressCache = new ConcurrentDictionary<ServerKey, HashSet<AddressCacheToken>>();
      this.addressResolver = addressResolver;
    }

    public void OnConnectionEvent(
      ConnectionEvent connectionEvent,
      DateTime eventTime,
      ServerKey serverKey)
    {
      DefaultTrace.TraceInformation("OnConnectionEvent fired, connectionEvent :{0}, eventTime: {1}, serverKey: {2}", (object) connectionEvent, (object) eventTime, (object) serverKey.ToString());
      if (connectionEvent != ConnectionEvent.ReadEof && connectionEvent != ConnectionEvent.ReadFailure)
        return;
      Task.Run((Func<Task>) (async () => await this.UpdateAddressCacheAsync(serverKey))).ContinueWith((Action<Task>) (task => DefaultTrace.TraceWarning("AddressCache update failed: {0}", (object) task.Exception?.InnerException)), TaskContinuationOptions.OnlyOnFaulted);
    }

    public void UpdateConnectionState(Uri serverAddress, AddressCacheToken addressCacheToken)
    {
      if (addressCacheToken == null)
        return;
      this.UpdatePartitionAddressCache(serverAddress, addressCacheToken);
    }

    public void UpdateConnectionState(
      IReadOnlyList<Uri> serverAddresses,
      AddressCacheToken addressCacheToken)
    {
      if (addressCacheToken == null)
        return;
      foreach (Uri serverAddress in (IEnumerable<Uri>) serverAddresses)
        this.UpdatePartitionAddressCache(serverAddress, addressCacheToken);
    }

    private async Task UpdateAddressCacheAsync(ServerKey serverKey)
    {
      HashSet<AddressCacheToken> collection;
      if (this.partitionAddressCache.TryGetValue(serverKey, out collection))
        await this.addressResolver.UpdateAsync((IReadOnlyList<AddressCacheToken>) new List<AddressCacheToken>((IEnumerable<AddressCacheToken>) collection));
      else
        DefaultTrace.TraceInformation("Serverkey {0} not found in the partitionAddressCache", (object) serverKey);
    }

    private void UpdatePartitionAddressCache(Uri serverAddress, AddressCacheToken addressCacheToken)
    {
      DefaultTrace.TraceInformation("Adding {0} serverAddress key to partitionAddressCache with values partitionKeyRangeIdentity: {1}, serviceEndpoint: {2}", (object) serverAddress, (object) addressCacheToken.PartitionKeyRangeIdentity.ToHeader(), (object) addressCacheToken.ServiceEndpoint);
      ConcurrentDictionary<ServerKey, HashSet<AddressCacheToken>> partitionAddressCache = this.partitionAddressCache;
      ServerKey key = new ServerKey(serverAddress);
      HashSet<AddressCacheToken> addValue = new HashSet<AddressCacheToken>();
      addValue.Add(addressCacheToken);
      Func<ServerKey, HashSet<AddressCacheToken>, HashSet<AddressCacheToken>> updateValueFactory = (Func<ServerKey, HashSet<AddressCacheToken>, HashSet<AddressCacheToken>>) ((serverKey, addressCacheTokens) =>
      {
        addressCacheTokens.Add(addressCacheToken);
        return addressCacheTokens;
      });
      partitionAddressCache.AddOrUpdate(key, addValue, updateValueFactory);
    }
  }
}
