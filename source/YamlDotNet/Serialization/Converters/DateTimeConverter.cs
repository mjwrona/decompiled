// Decompiled with JetBrains decompiler
// Type: YamlDotNet.Serialization.Converters.DateTimeConverter
// Assembly: YamlDotNet, Version=5.0.0.0, Culture=neutral, PublicKeyToken=ec19458f3c15af5e
// MVID: 5F9DD5C4-A41D-46B2-A793-8157A0D55AB5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\YamlDotNet.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;

namespace YamlDotNet.Serialization.Converters
{
  public class DateTimeConverter : IYamlTypeConverter
  {
    private readonly DateTimeKind kind;
    private readonly IFormatProvider provider;
    private readonly string[] formats;

    public DateTimeConverter(DateTimeKind kind = DateTimeKind.Utc, IFormatProvider provider = null, params string[] formats)
    {
      this.kind = kind == DateTimeKind.Unspecified ? DateTimeKind.Utc : kind;
      this.provider = provider ?? (IFormatProvider) CultureInfo.InvariantCulture;
      this.formats = ((IEnumerable<string>) formats).DefaultIfEmpty<string>("G").ToArray<string>();
    }

    public bool Accepts(Type type) => (object) type == (object) typeof (DateTime);

    public object ReadYaml(IParser parser, Type type)
    {
      string s = ((Scalar) parser.Current).Value;
      DateTimeStyles dateTimeStyles = this.kind == DateTimeKind.Local ? DateTimeStyles.AssumeLocal : DateTimeStyles.AssumeUniversal;
      string[] formats = this.formats;
      IFormatProvider provider = this.provider;
      int style = (int) dateTimeStyles;
      DateTime dateTime = DateTimeConverter.EnsureDateTimeKind(DateTime.ParseExact(s, formats, provider, (DateTimeStyles) style), this.kind);
      parser.MoveNext();
      return (object) dateTime;
    }

    public void WriteYaml(IEmitter emitter, object value, Type type)
    {
      DateTime dateTime = (DateTime) value;
      string str = (this.kind == DateTimeKind.Local ? dateTime.ToLocalTime() : dateTime.ToUniversalTime()).ToString(((IEnumerable<string>) this.formats).First<string>(), this.provider);
      emitter.Emit((ParsingEvent) new Scalar((string) null, (string) null, str, ScalarStyle.Any, true, false));
    }

    private static DateTime EnsureDateTimeKind(DateTime dt, DateTimeKind kind)
    {
      if (dt.Kind == DateTimeKind.Local && kind == DateTimeKind.Utc)
        return dt.ToUniversalTime();
      return dt.Kind == DateTimeKind.Utc && kind == DateTimeKind.Local ? dt.ToLocalTime() : dt;
    }
  }
}
