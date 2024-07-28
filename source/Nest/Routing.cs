// Decompiled with JetBrains decompiler
// Type: Nest.Routing
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Utf8Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Nest
{
  [JsonFormatter(typeof (RoutingFormatter))]
  [DebuggerDisplay("{DebugDisplay,nq}")]
  public class Routing : IEquatable<Routing>, IUrlParameter
  {
    private static readonly char[] Separator = new char[1]
    {
      ','
    };

    internal Routing(Func<object> documentGetter)
    {
      this.Tag = 0;
      this.DocumentGetter = documentGetter;
    }

    public Routing(string routing)
    {
      this.Tag = 1;
      this.StringValue = routing;
    }

    public Routing(long routing)
    {
      this.Tag = 2;
      this.LongValue = new long?(routing);
    }

    public Routing(object document)
    {
      this.Tag = 4;
      this.Document = document;
    }

    internal object Document { get; }

    internal Func<object> DocumentGetter { get; }

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

    private string DebugDisplay => this.StringOrLongValue ?? "Routing from instance typeof: " + this.Document?.GetType().Name;

    public override string ToString() => this.DebugDisplay;

    private static int TypeHashCode { get; } = typeof (Routing).GetHashCode();

    public bool Equals(Routing other)
    {
      if (this.Tag == other.Tag)
      {
        switch (this.Tag)
        {
          case 0:
            object obj1 = this.DocumentGetter();
            object obj2 = other.DocumentGetter();
            return obj1 != null && obj1.Equals(obj2);
          case 4:
            object document = this.Document;
            return document != null && document.Equals(other.Document);
          default:
            return Routing.StringEquals(this.StringOrLongValue, other.StringOrLongValue);
        }
      }
      else
        return this.Tag + other.Tag == 3 && Routing.StringEquals(this.StringOrLongValue, other.StringOrLongValue);
    }

    string IUrlParameter.GetString(IConnectionConfigurationValues settings) => this.GetString(settings as IConnectionSettingsValues);

    public static implicit operator Routing(string routing) => !routing.IsNullOrEmptyCommaSeparatedList(out string[] _) ? new Routing(routing) : (Routing) null;

    public static implicit operator Routing(string[] routing) => !((IEnumerable<string>) routing).IsEmpty<string>() ? new Routing(string.Join(",", routing)) : (Routing) null;

    public static implicit operator Routing(long routing) => new Routing(routing);

    public static implicit operator Routing(Guid routing) => new Routing(routing.ToString("D"));

    public static Routing From<T>(T document) where T : class => new Routing((object) document);

    private string GetString(IConnectionSettingsValues nestSettings)
    {
      string str = (string) null;
      if (this.DocumentGetter != null)
      {
        object document = this.DocumentGetter();
        str = nestSettings.Inferrer.Routing<object>(document);
      }
      else if (this.Document != null)
        str = nestSettings.Inferrer.Routing<object>(this.Document);
      return str ?? this.StringOrLongValue;
    }

    public static bool operator ==(Routing left, Routing right) => object.Equals((object) left, (object) right);

    public static bool operator !=(Routing left, Routing right) => !object.Equals((object) left, (object) right);

    private static bool StringEquals(string left, string right)
    {
      if (left == null && right == null)
        return true;
      if (left == null || right == null)
        return false;
      if (!left.Contains(",") || !right.Contains(","))
        return left == right;
      List<string> list1 = ((IEnumerable<string>) left.Split(Routing.Separator, StringSplitOptions.RemoveEmptyEntries)).Select<string, string>((Func<string, string>) (v => v.Trim())).ToList<string>();
      List<string> list2 = ((IEnumerable<string>) right.Split(Routing.Separator, StringSplitOptions.RemoveEmptyEntries)).Select<string, string>((Func<string, string>) (v => v.Trim())).ToList<string>();
      return list1.Count == list2.Count && list1.Count == list2.Count && !list1.Except<string>((IEnumerable<string>) list2).Any<string>();
    }

    public override bool Equals(object obj)
    {
      Routing other1 = obj as Routing;
      if ((object) other1 != null)
        return this.Equals(other1);
      switch (obj)
      {
        case string other2:
          return this.Equals((Routing) other2);
        case int other3:
          return this.Equals((Routing) (long) other3);
        case long other4:
          return this.Equals((Routing) other4);
        case Guid other5:
          return this.Equals((Routing) other5);
        default:
          return this.Equals(new Routing(obj));
      }
    }

    public override int GetHashCode()
    {
      int num1 = Routing.TypeHashCode * 397;
      string stringValue = this.StringValue;
      int hashCode1 = stringValue != null ? stringValue.GetHashCode() : 0;
      int num2 = (num1 ^ hashCode1) * 397;
      long? longValue = this.LongValue;
      ref long? local = ref longValue;
      int hashCode2 = local.HasValue ? local.GetValueOrDefault().GetHashCode() : 0;
      int num3 = (num2 ^ hashCode2) * 397;
      Func<object> documentGetter = this.DocumentGetter;
      int hashCode3 = documentGetter != null ? documentGetter.GetHashCode() : 0;
      int num4 = (num3 ^ hashCode3) * 397;
      object document = this.Document;
      int hashCode4 = document != null ? document.GetHashCode() : 0;
      return num4 ^ hashCode4;
    }

    internal bool ShouldSerialize(IJsonFormatterResolver formatterResolver) => !formatterResolver.GetConnectionSettings().Inferrer.Resolve((IUrlParameter) this).IsNullOrEmpty();
  }
}
