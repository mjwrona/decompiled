// Decompiled with JetBrains decompiler
// Type: Nest.IndexUuid
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System;

namespace Nest
{
  public class IndexUuid : IUrlParameter, IEquatable<IndexUuid>
  {
    public string Value { get; }

    public IndexUuid(string value) => this.Value = value ?? throw new ArgumentNullException(nameof (value));

    public string GetString(IConnectionConfigurationValues settings) => this.Value;

    public bool Equals(IndexUuid other)
    {
      if ((object) other == null)
        return false;
      return (object) this == (object) other || this.Value == other.Value;
    }

    public override bool Equals(object obj)
    {
      if (obj == null)
        return false;
      if ((object) this == obj)
        return true;
      return !(obj.GetType() != this.GetType()) && this.Equals((IndexUuid) obj);
    }

    public override int GetHashCode() => this.Value == null ? 0 : this.Value.GetHashCode();

    public static bool operator ==(IndexUuid left, IndexUuid right) => object.Equals((object) left, (object) right);

    public static bool operator !=(IndexUuid left, IndexUuid right) => !object.Equals((object) left, (object) right);

    public static implicit operator IndexUuid(string value) => !string.IsNullOrEmpty(value) ? new IndexUuid(value) : (IndexUuid) null;
  }
}
