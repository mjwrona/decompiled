// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.ShardableLocator
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Content.Common
{
  public struct ShardableLocator : 
    IEqualityComparer<ShardableLocator>,
    IEquatable<ShardableLocator>,
    IComparable,
    IComparable<ShardableLocator>
  {
    public Locator Locator;
    public string ShardHint;

    public ShardableLocator(Locator locator, string shardHint)
    {
      this.Locator = locator;
      this.ShardHint = shardHint;
    }

    public static bool operator ==(ShardableLocator x, ShardableLocator y) => x.Equals(y);

    public static bool operator !=(ShardableLocator x, ShardableLocator y) => !(x == y);

    public static ShardableLocator Parse(string path, string shardKey) => new ShardableLocator()
    {
      Locator = Locator.Parse(path),
      ShardHint = shardKey
    };

    public bool Equals(ShardableLocator other) => this.Equals(this, other);

    public bool Equals(ShardableLocator x, ShardableLocator y) => x.Locator == y.Locator && x.ShardHint == y.ShardHint;

    public int GetHashCode(ShardableLocator obj) => obj.GetHashCode();

    public override bool Equals(object obj) => obj is ShardableLocator other && this.Equals(other);

    public override int GetHashCode() => this.Locator.GetHashCode() ^ this.ShardHint.GetHashCode();

    public int CompareTo(object obj) => !(obj is ShardableLocator other) ? -1 : this.CompareTo(other);

    public int CompareTo(ShardableLocator other)
    {
      int num = this.Locator.CompareTo(other.Locator);
      return num != 0 ? num : this.ShardHint.CompareTo(other.ShardHint);
    }

    public override string ToString() => this.Locator.Value + "@" + this.ShardHint;
  }
}
