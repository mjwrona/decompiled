// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.AddressSelector
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using Microsoft.Azure.Cosmos.Core.Trace;
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

    public async Task<IReadOnlyList<TransportAddressUri>> ResolveAllTransportAddressUriAsync(
      DocumentServiceRequest request,
      bool includePrimary,
      bool forceRefresh)
    {
      PerProtocolPartitionAddressInformation addressInformation = await this.ResolveAddressesAsync(request, forceRefresh);
      return includePrimary ? addressInformation.ReplicaTransportAddressUris : addressInformation.NonPrimaryReplicaTransportAddressUris;
    }

    public async Task<TransportAddressUri> ResolvePrimaryTransportAddressUriAsync(
      DocumentServiceRequest request,
      bool forceAddressRefresh)
    {
      return (await this.ResolveAddressesAsync(request, forceAddressRefresh)).GetPrimaryAddressUri(request);
    }

    public async Task<PerProtocolPartitionAddressInformation> ResolveAddressesAsync(
      DocumentServiceRequest request,
      bool forceAddressRefresh)
    {
      return (await this.addressResolver.ResolveAsync(request, forceAddressRefresh, CancellationToken.None)).Get(this.protocol);
    }

    public void StartBackgroundAddressRefresh(DocumentServiceRequest request)
    {
      try
      {
        this.ResolveAllTransportAddressUriAsync(request.Clone(), true, true).ContinueWith((Action<Task<IReadOnlyList<TransportAddressUri>>>) (task =>
        {
          if (!task.IsFaulted)
            return;
          DefaultTrace.TraceWarning("Background refresh of the addresses failed with {0}", (object) task.Exception.ToString());
        }));
      }
      catch (Exception ex)
      {
        DefaultTrace.TraceWarning("Background refresh of the addresses failed with {0}", (object) ex.ToString());
      }
    }
  }
}
