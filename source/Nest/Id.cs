// Decompiled with JetBrains decompiler
// Type: Nest.Id
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Utf8Json;
using System;
using System.Diagnostics;
using System.Globalization;

namespace Nest
{
  [JsonFormatter(typeof (IdFormatter))]
  [DebuggerDisplay("{DebugDisplay,nq}")]
  public class Id : IEquatable<Id>, IUrlParameter
  {
    public Id(string id)
    {
      this.Tag = 0;
      this.StringValue = id;
    }

    public Id(long id)
    {
      this.Tag = 1;
      this.LongValue = new long?(id);
    }

    public Id(object document)
    {
      this.Tag = 2;
      this.Document = document;
    }

    internal object Document { get; }

    internal long? LongValue { get; }

    internal string StringOrLongValue
    {
      get
      {
        string stringValue = this.StringValue;
        if (stringValue != null)
          return stringValue;
        long? longValue = this.LongValue;
        ref long? local = ref longValue;
        return !local.HasValue ? (string) null : local.GetValueOrDefault().ToString((IFormatProvider) CultureInfo.InvariantCulture);
      }
    }

    internal string StringValue { get; }

    internal int Tag { get; }

    private string DebugDisplay => this.StringOrLongValue ?? "Id from instance typeof: " + this.Document?.GetType().Name;

    private static int TypeHashCode { get; } = typeof (Id).GetHashCode();

    public bool Equals(Id other)
    {
      if (this.Tag + other.Tag == 1)
        return this.StringOrLongValue == other.StringOrLongValue;
      if (this.Tag != other.Tag)
        return false;
      switch (this.Tag)
      {
        case 0:
        case 1:
          return this.StringOrLongValue == other.StringOrLongValue;
        default:
          object document = this.Document;
          return document != null && document.Equals(other.Document);
      }
    }

    string IUrlParameter.GetString(IConnectionConfigurationValues settings) => ((IConnectionSettingsValues) settings).Inferrer.Id<object>(this.Document) ?? this.StringOrLongValue;

    public static implicit operator Id(string id) => !id.IsNullOrEmpty() ? new Id(id) : (Id) null;

    public static implicit operator Id(long id) => new Id(id);

    public static implicit operator Id(Guid id) => new Id(id.ToString("D"));

    public static Id From<T>(T document) where T : class => new Id((object) document);

    public override string ToString() => this.DebugDisplay;

    public override bool Equals(object obj)
    {
      Id other1 = obj as Id;
      if ((object) other1 != null)
        return this.Equals(other1);
      switch (obj)
      {
        case string other2:
          return this.Equals((Id) other2);
        case int other3:
          return this.Equals((Id) (long) other3);
        case long other4:
          return this.Equals((Id) other4);
        case Guid other5:
          return this.Equals((Id) other5);
        default:
          return this.Equals(new Id(obj));
      }
    }

    public override int GetHashCode()
    {
      int num1 = Id.TypeHashCode * 397;
      string stringValue = this.StringValue;
      int hashCode1 = stringValue != null ? stringValue.GetHashCode() : 0;
      int num2 = (num1 ^ hashCode1) * 397;
      long? longValue = this.LongValue;
      ref long? local = ref longValue;
      int hashCode2 = local.HasValue ? local.GetValueOrDefault().GetHashCode() : 0;
      int num3 = (num2 ^ hashCode2) * 397;
      object document = this.Document;
      int hashCode3 = document != null ? document.GetHashCode() : 0;
      return num3 ^ hashCode3;
    }

    public static bool operator ==(Id left, Id right) => object.Equals((object) left, (object) right);

    public static bool operator !=(Id left, Id right) => !object.Equals((object) left, (object) right);
  }
}
