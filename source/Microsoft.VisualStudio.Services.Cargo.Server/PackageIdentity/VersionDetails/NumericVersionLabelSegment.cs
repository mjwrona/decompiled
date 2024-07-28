// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.PackageIdentity.VersionDetails.NumericVersionLabelSegment
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using System;
using System.Globalization;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cargo.Server.PackageIdentity.VersionDetails
{
  public sealed class NumericVersionLabelSegment : 
    IVersionLabelSegment,
    IComparable<IVersionLabelSegment>
  {
    public NumericVersionLabelSegment(ulong value, int leadingZeroes, int startPos)
    {
      this.Value = value;
      this.LeadingZeroes = leadingZeroes;
      this.StartPos = startPos;
    }

    public ulong Value { get; }

    public int LeadingZeroes { get; }

    public int StartPos { get; }

    public string StringValue
    {
      get
      {
        string str = this.Value.ToString("D", (IFormatProvider) CultureInfo.InvariantCulture);
        return this.LeadingZeroes <= 0 ? str : new string('0', this.LeadingZeroes) + str;
      }
    }

    public int CompareTo(IVersionLabelSegment? other)
    {
      if (other == null)
        return 1;
      if (this == other)
        return 0;
      switch (other)
      {
        case NumericVersionLabelSegment versionLabelSegment:
          return (this.Value, this.LeadingZeroes).CompareTo((versionLabelSegment.Value, versionLabelSegment.LeadingZeroes));
        case StringVersionLabelSegment _:
          return -1;
        default:
          throw new InvalidOperationException("Unknown segment type " + other.GetType().FullName);
      }
    }

    public override string ToString() => "NumericVersionLabelSegment(" + this.StringValue + ")";
  }
}
