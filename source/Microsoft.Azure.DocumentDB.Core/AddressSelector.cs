// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.AddressSelector
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Documents
{
  internal sealed class AddressSelector
  {
    private readonly IAddressResolver addressResolver;
    private readonly Protocol protocol;

    public AddressSelector(IAddressResolver addressResolver, Protocol protocol)
    {
      this.addressResolver = addressResolver;
      this.protocol = protocol;
    }

    public async Task<Tuple<IReadOnlyList<Uri>, AddressCacheToken>> ResolveAllUriAsync(
      DocumentServiceRequest request,
      bool includePrimary,
      bool forceRefresh)
    {
      Tuple<PerProtocolPartitionAddressInformation, AddressCacheToken> tuple = await this.ResolveAddressesAsync(request, forceRefresh);
      return Tuple.Create<IReadOnlyList<Uri>, AddressCacheToken>(includePrimary ? tuple.Item1.ReplicaUris : tuple.Item1.NonPrimaryReplicaUris, tuple.Item2);
    }

    public async Task<Tuple<Uri, AddressCacheToken>> ResolvePrimaryUriAsync(
      DocumentServiceRequest request,
      bool forceAddressRefresh)
    {
      Tuple<PerProtocolPartitionAddressInformation, AddressCacheToken> tuple = await this.ResolveAddressesAsync(request, forceAddressRefresh);
      return Tuple.Create<Uri, AddressCacheToken>(tuple.Item1.GetPrimaryUri(request), tuple.Item2);
    }

    public async Task<Tuple<PerProtocolPartitionAddressInformation, AddressCacheToken>> ResolveAddressesAsync(
      DocumentServiceRequest request,
      bool forceAddressRefresh)
    {
      PartitionAddressInformation addressInformation = await this.addressResolver.ResolveAsync(request, forceAddressRefresh, CancellationToken.None);
      return Tuple.Create<PerProtocolPartitionAddressInformation, AddressCacheToken>(addressInformation.Get(this.protocol), addressInformation.AddressCacheToken);
    }
  }
}
