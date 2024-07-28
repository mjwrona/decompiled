// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.PerProtocolPartitionAddressInformation
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.Azure.Documents
{
  internal sealed class PerProtocolPartitionAddressInformation
  {
    public PerProtocolPartitionAddressInformation(
      Protocol protocol,
      IReadOnlyList<AddressInformation> replicaAddresses)
    {
      if (replicaAddresses == null)
        throw new ArgumentNullException(nameof (replicaAddresses));
      IEnumerable<AddressInformation> source1 = replicaAddresses.Where<AddressInformation>((Func<AddressInformation, bool>) (address => !string.IsNullOrEmpty(address.PhysicalUri) && address.Protocol == protocol));
      IEnumerable<AddressInformation> source2 = source1.Where<AddressInformation>((Func<AddressInformation, bool>) (address => !address.IsPublic));
      this.ReplicaAddresses = source2.Any<AddressInformation>() ? (IReadOnlyList<AddressInformation>) source2.ToArray<AddressInformation>() : (IReadOnlyList<AddressInformation>) source1.Where<AddressInformation>((Func<AddressInformation, bool>) (address => address.IsPublic)).ToArray<AddressInformation>();
      this.ReplicaUris = (IReadOnlyList<Uri>) this.ReplicaAddresses.Select<AddressInformation, Uri>((Func<AddressInformation, Uri>) (e => new Uri(e.PhysicalUri))).ToArray<Uri>();
      this.ReplicaTransportAddressUris = (IReadOnlyList<TransportAddressUri>) this.ReplicaUris.Select<Uri, TransportAddressUri>((Func<Uri, TransportAddressUri>) (e => new TransportAddressUri(e))).ToArray<TransportAddressUri>();
      this.NonPrimaryReplicaTransportAddressUris = (IReadOnlyList<TransportAddressUri>) this.ReplicaAddresses.Where<AddressInformation>((Func<AddressInformation, bool>) (e => !e.IsPrimary)).Select<AddressInformation, Uri>((Func<AddressInformation, Uri>) (e => new Uri(e.PhysicalUri))).Select<Uri, TransportAddressUri>((Func<Uri, TransportAddressUri>) (e => new TransportAddressUri(e))).ToArray<TransportAddressUri>();
      AddressInformation addressInformation = this.ReplicaAddresses.SingleOrDefault<AddressInformation>((Func<AddressInformation, bool>) (address => address.IsPrimary && !address.PhysicalUri.Contains<char>('[')));
      if (addressInformation != null)
        this.PrimaryReplicaTransportAddressUri = new TransportAddressUri(new Uri(addressInformation.PhysicalUri));
      this.Protocol = protocol;
    }

    public TransportAddressUri GetPrimaryAddressUri(DocumentServiceRequest request)
    {
      TransportAddressUri transportAddressUri = (TransportAddressUri) null;
      if (!request.DefaultReplicaIndex.HasValue || request.DefaultReplicaIndex.Value == 0U)
        transportAddressUri = this.PrimaryReplicaTransportAddressUri;
      else if (request.DefaultReplicaIndex.Value > 0U && (long) request.DefaultReplicaIndex.Value < (long) this.ReplicaUris.Count)
        transportAddressUri = this.ReplicaTransportAddressUris[(int) request.DefaultReplicaIndex.Value];
      return transportAddressUri != null ? transportAddressUri : throw new GoneException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, "The requested resource is no longer available at the server. Returned addresses are {0}", (object) string.Join(",", (IEnumerable<string>) this.ReplicaAddresses.Select<AddressInformation, string>((Func<AddressInformation, string>) (address => address.PhysicalUri)).ToList<string>())), SubStatusCodes.ServerGenerated410);
    }

    public Protocol Protocol { get; }

    public IReadOnlyList<TransportAddressUri> NonPrimaryReplicaTransportAddressUris { get; }

    public IReadOnlyList<Uri> ReplicaUris { get; }

    public IReadOnlyList<TransportAddressUri> ReplicaTransportAddressUris { get; }

    public Uri PrimaryReplicaUri => this.PrimaryReplicaTransportAddressUri?.Uri;

    public TransportAddressUri PrimaryReplicaTransportAddressUri { get; }

    public IReadOnlyList<AddressInformation> ReplicaAddresses { get; }
  }
}
