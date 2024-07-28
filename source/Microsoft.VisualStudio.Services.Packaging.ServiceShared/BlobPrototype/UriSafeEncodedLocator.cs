// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.UriSafeEncodedLocator
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Content.Common;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public class UriSafeEncodedLocator : IEquatable<UriSafeEncodedLocator>
  {
    public UriSafeEncodedLocator(IEnumerable<string> encodedSegments)
    {
      this.PathSegments = encodedSegments != null ? (IImmutableList<string>) encodedSegments.ToImmutableList<string>() : throw new ArgumentNullException(nameof (encodedSegments));
      this.Value = Locator.Separator + string.Join(Locator.Separator, (IEnumerable<string>) this.PathSegments);
    }

    public UriSafeEncodedLocator(UriSafeEncodedLocator l1, UriSafeEncodedLocator l2)
      : this((IEnumerable<string>) l1.PathSegments.AddRange((IEnumerable<string>) l2.PathSegments))
    {
    }

    public IImmutableList<string> PathSegments { get; }

    public string Value { get; }

    public int PathSegmentCount => this.PathSegments.Count;

    public static bool operator ==(UriSafeEncodedLocator left, UriSafeEncodedLocator right) => object.Equals((object) left, (object) right);

    public static bool operator !=(UriSafeEncodedLocator left, UriSafeEncodedLocator right) => !object.Equals((object) left, (object) right);

    public static UriSafeEncodedLocator Parse(string s) => new UriSafeEncodedLocator((IEnumerable<string>) s.Split(Locator.SeparatorCharArray, StringSplitOptions.RemoveEmptyEntries));

    public bool Equals(UriSafeEncodedLocator other)
    {
      if ((object) other == null)
        return false;
      return (object) this == (object) other || string.Equals(this.Value, other.Value);
    }

    public override bool Equals(object obj)
    {
      if (obj == null)
        return false;
      if ((object) this == obj)
        return true;
      return !(obj.GetType() != this.GetType()) && this.Equals((UriSafeEncodedLocator) obj);
    }

    public override int GetHashCode() => this.Value.GetHashCode();

    public override string ToString() => this.Value;
  }
}
