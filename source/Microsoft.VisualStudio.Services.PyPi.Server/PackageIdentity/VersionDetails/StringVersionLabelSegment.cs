// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity.VersionDetails.StringVersionLabelSegment
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using System;


#nullable enable
namespace Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity.VersionDetails
{
  public sealed class StringVersionLabelSegment : 
    IVersionLabelSegment,
    IComparable<IVersionLabelSegment>
  {
    public StringVersionLabelSegment(string value) => this.StringValue = value;

    public string StringValue { get; }

    public int CompareTo(IVersionLabelSegment? other)
    {
      if (other == null)
        return 1;
      if (this == other)
        return 0;
      switch (other)
      {
        case NumericVersionLabelSegment _:
          return 1;
        case StringVersionLabelSegment versionLabelSegment:
          return StringComparer.OrdinalIgnoreCase.Compare(this.StringValue, versionLabelSegment.StringValue);
        default:
          throw new InvalidOperationException("Unknown segment type " + other.GetType().FullName);
      }
    }

    public override string ToString() => "StringVersionLabelSegment(" + this.StringValue + ")";
  }
}
