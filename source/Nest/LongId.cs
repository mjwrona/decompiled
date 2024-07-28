// Decompiled with JetBrains decompiler
// Type: Nest.LongId
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System;
using System.Globalization;

namespace Nest
{
  public class LongId : IUrlParameter, IEquatable<LongId>
  {
    internal readonly long Value;

    public LongId(long value) => this.Value = value;

    public bool Equals(LongId other) => this.Value == other.Value;

    public string GetString(IConnectionConfigurationValues settings) => this.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture);

    public static implicit operator LongId(long value) => new LongId(value);

    public static implicit operator long(LongId value) => value.Value;

    public override bool Equals(object obj)
    {
      switch (obj)
      {
        case int num1:
          return this.Value == (long) num1;
        case long num2:
          return this.Value == num2;
        default:
          LongId longId = obj as LongId;
          return (object) longId != null && this.Value == longId.Value;
      }
    }

    public override int GetHashCode() => this.Value.GetHashCode();

    public static bool operator ==(LongId left, LongId right) => object.Equals((object) left, (object) right);

    public static bool operator !=(LongId left, LongId right) => !object.Equals((object) left, (object) right);
  }
}
