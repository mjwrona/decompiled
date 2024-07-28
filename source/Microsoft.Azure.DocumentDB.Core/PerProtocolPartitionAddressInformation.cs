// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.PerProtocolPartitionAddressInformation
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

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
      this.NonPrimaryReplicaUris = (IReadOnlyList<Uri>) this.ReplicaAddresses.Where<AddressInformation>((Func<AddressInformation, bool>) (e => !e.IsPrimary)).Select<AddressInformation, Uri>((Func<AddressInformation, Uri>) (e => new Uri(e.PhysicalUri))).ToArray<Uri>();
      AddressInformation addressInformation = this.ReplicaAddresses.SingleOrDefault<AddressInformation>((Func<AddressInformation, bool>) (address => address.IsPrimary && !address.PhysicalUri.Contains<char>('[')));
      if (addressInformation != null)
        this.PrimaryReplicaUri = new Uri(addressInformation.PhysicalUri);
      this.Protocol = protocol;
    }

    public Uri GetPrimaryUri(DocumentServiceRequest request)
    {
      Uri uri = (Uri) null;
      if (!request.DefaultReplicaIndex.HasValue || request.DefaultReplicaIndex.Value == 0U)
        uri = this.PrimaryReplicaUri;
      else if (request.DefaultReplicaIndex.Value > 0U && (long) request.DefaultReplicaIndex.Value < (long) this.ReplicaUris.Count)
        uri = this.ReplicaUris[(int) request.DefaultReplicaIndex.Value];
      return !(uri == (Uri) null) ? uri : throw new GoneException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, "The requested resource is no longer available at the server. Returned addresses are {0}", (object) string.Join(",", (IEnumerable<string>) this.ReplicaAddresses.Select<AddressInformation, string>((Func<AddressInformation, string>) (address => address.PhysicalUri)).ToList<string>())));
    }

    public Protocol Protocol { get; }

    public IReadOnlyList<Uri> NonPrimaryReplicaUris { get; }

    public IReadOnlyList<Uri> ReplicaUris { get; }

    public Uri PrimaryReplicaUri { get; }

    public IReadOnlyList<AddressInformation> ReplicaAddresses { get; }
  }
}
