// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.AddressInformation
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using Microsoft.Azure.Documents.Client;
using System;

namespace Microsoft.Azure.Documents
{
  internal sealed class AddressInformation : 
    IEquatable<AddressInformation>,
    IComparable<AddressInformation>
  {
    private readonly Lazy<int> lazyHashCode;

    public AddressInformation(
      string physicalUri,
      bool isPublic,
      bool isPrimary,
      Protocol protocol)
    {
      this.IsPublic = isPublic;
      this.IsPrimary = isPrimary;
      this.Protocol = protocol;
      this.PhysicalUri = physicalUri;
      this.lazyHashCode = new Lazy<int>((Func<int>) (() =>
      {
        int num = ((17 * 397 ^ this.Protocol.GetHashCode()) * 397 ^ this.IsPublic.GetHashCode()) * 397 ^ this.IsPrimary.GetHashCode();
        if (this.PhysicalUri != null)
          num = num * 397 ^ this.PhysicalUri.GetHashCode();
        return num;
      }));
    }

    public bool IsPublic { get; }

    public bool IsPrimary { get; }

    public Protocol Protocol { get; }

    public string PhysicalUri { get; }

    public int CompareTo(AddressInformation other)
    {
      if (other == null)
        return -1;
      bool flag = this.IsPrimary;
      int num1 = flag.CompareTo(other.IsPrimary);
      if (num1 != 0)
        return -1 * num1;
      flag = this.IsPublic;
      int num2 = flag.CompareTo(other.IsPublic);
      if (num2 != 0)
        return num2;
      int num3 = this.Protocol.CompareTo((object) other.Protocol);
      return num3 != 0 ? num3 : string.Compare(this.PhysicalUri, other.PhysicalUri, StringComparison.OrdinalIgnoreCase);
    }

    public bool Equals(AddressInformation other) => this.CompareTo(other) == 0;

    public override int GetHashCode() => this.lazyHashCode.Value;
  }
}
