// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ODataRawValueConverter
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using System;
using System.Xml;

namespace Microsoft.OData
{
  internal static class ODataRawValueConverter
  {
    private const string RawValueTrueLiteral = "true";
    private const string RawValueFalseLiteral = "false";

    internal static string ToString(bool b) => !b ? "false" : "true";

    internal static string ToString(byte b) => XmlConvert.ToString(b);

    internal static string ToString(Decimal d) => XmlConvert.ToString(d);

    internal static string ToString(DateTimeOffset dateTime) => XmlConvert.ToString(dateTime);

    internal static string ToString(this TimeSpan ts) => EdmValueWriter.DurationAsXml(ts);

    internal static string ToString(this double d) => XmlConvert.ToString(d);

    internal static string ToString(this short i) => XmlConvert.ToString(i);

    internal static string ToString(this int i) => XmlConvert.ToString(i);

    internal static string ToString(this long i) => XmlConvert.ToString(i);

    internal static string ToString(this sbyte sb) => XmlConvert.ToString(sb);

    internal static string ToString(this byte[] bytes) => Convert.ToBase64String(bytes);

    internal static string ToString(this float s) => XmlConvert.ToString(s);

    internal static string ToString(this Guid guid) => XmlConvert.ToString(guid);

    internal static string ToString(Date date) => date.ToString();

    internal static string ToString(TimeOfDay time) => time.ToString();
  }
}
