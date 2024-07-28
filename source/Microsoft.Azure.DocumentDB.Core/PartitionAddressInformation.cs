// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.PartitionAddressInformation
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Documents
{
  internal sealed class PartitionAddressInformation
  {
    private static readonly int AllProtocolsCount = Enum.GetNames(typeof (Protocol)).Length;
    private readonly PerProtocolPartitionAddressInformation[] perProtocolAddressInformation;

    public IReadOnlyList<AddressInformation> AllAddresses { get; }

    public AddressCacheToken AddressCacheToken { get; }

    public PartitionAddressInformation(AddressInformation[] replicaAddresses)
      : this(replicaAddresses, (PartitionKeyRangeIdentity) null, (Uri) null)
    {
    }

    public PartitionAddressInformation(
      AddressInformation[] replicaAddresses,
      PartitionKeyRangeIdentity partitionKeyRangeIdentity,
      Uri serviceEndpoint)
    {
      this.AllAddresses = replicaAddresses != null ? (IReadOnlyList<AddressInformation>) (AddressInformation[]) replicaAddresses.Clone() : throw new ArgumentNullException(nameof (replicaAddresses));
      this.perProtocolAddressInformation = new PerProtocolPartitionAddressInformation[PartitionAddressInformation.AllProtocolsCount];
      foreach (Protocol protocol in (Protocol[]) Enum.GetValues(typeof (Protocol)))
        this.perProtocolAddressInformation[(int) protocol] = new PerProtocolPartitionAddressInformation(protocol, this.AllAddresses);
      if (partitionKeyRangeIdentity == null || !(serviceEndpoint != (Uri) null))
        return;
      this.AddressCacheToken = new AddressCacheToken(partitionKeyRangeIdentity, serviceEndpoint);
    }

    public Uri GetPrimaryUri(DocumentServiceRequest request, Protocol protocol) => this.perProtocolAddressInformation[(int) protocol].GetPrimaryUri(request);

    public PerProtocolPartitionAddressInformation Get(Protocol protocol) => this.perProtocolAddressInformation[(int) protocol];
  }
}
