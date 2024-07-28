// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.PartitionAddressInformation
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.Documents
{
  internal sealed class PartitionAddressInformation : IEquatable<PartitionAddressInformation>
  {
    private static readonly int AllProtocolsCount = Enum.GetNames(typeof (Protocol)).Length;
    private readonly PerProtocolPartitionAddressInformation[] perProtocolAddressInformation;
    private readonly Lazy<int> generateHashCode;

    public IReadOnlyList<AddressInformation> AllAddresses { get; }

    public bool IsLocalRegion { get; set; }

    public PartitionAddressInformation(IReadOnlyList<AddressInformation> replicaAddresses)
      : this(replicaAddresses, false)
    {
    }

    public PartitionAddressInformation(
      IReadOnlyList<AddressInformation> replicaAddresses,
      bool inNetworkRequest)
    {
      if (replicaAddresses == null)
        throw new ArgumentNullException(nameof (replicaAddresses));
      for (int index = 1; index < replicaAddresses.Count; ++index)
      {
        if (replicaAddresses[index - 1].CompareTo(replicaAddresses[index]) > 0)
        {
          AddressInformation[] array = replicaAddresses.ToArray<AddressInformation>();
          Array.Sort<AddressInformation>(array);
          replicaAddresses = (IReadOnlyList<AddressInformation>) array;
          break;
        }
      }
      this.AllAddresses = replicaAddresses;
      this.generateHashCode = new Lazy<int>((Func<int>) (() =>
      {
        int num = 17;
        foreach (AddressInformation allAddress in (IEnumerable<AddressInformation>) this.AllAddresses)
          num = num * 397 ^ allAddress.GetHashCode();
        return num;
      }));
      this.perProtocolAddressInformation = new PerProtocolPartitionAddressInformation[PartitionAddressInformation.AllProtocolsCount];
      foreach (Protocol protocol in (Protocol[]) Enum.GetValues(typeof (Protocol)))
        this.perProtocolAddressInformation[(int) protocol] = new PerProtocolPartitionAddressInformation(protocol, this.AllAddresses);
      this.IsLocalRegion = inNetworkRequest;
    }

    public Uri GetPrimaryUri(DocumentServiceRequest request, Protocol protocol) => this.perProtocolAddressInformation[(int) protocol].GetPrimaryAddressUri(request).Uri;

    public PerProtocolPartitionAddressInformation Get(Protocol protocol) => this.perProtocolAddressInformation[(int) protocol];

    public override int GetHashCode() => this.generateHashCode.Value;

    public bool Equals(PartitionAddressInformation other) => other != null && this.AllAddresses.Count == other.AllAddresses.Count && this.GetHashCode() == other.GetHashCode();
  }
}
