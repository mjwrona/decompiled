// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.EdmValueWriter
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using System;
using System.Xml;

namespace Microsoft.OData
{
  internal static class EdmValueWriter
  {
    private static char[] Hex = new char[16]
    {
      '0',
      '1',
      '2',
      '3',
      '4',
      '5',
      '6',
      '7',
      '8',
      '9',
      'A',
      'B',
      'C',
      'D',
      'E',
      'F'
    };

    internal static string StringAsXml(string s) => s;

    internal static string BinaryAsXml(byte[] binary)
    {
      char[] chArray = new char[binary.Length * 2];
      for (int index = 0; index < binary.Length; ++index)
      {
        chArray[index << 1] = EdmValueWriter.Hex[(int) binary[index] >> 4];
        chArray[index << 1 | 1] = EdmValueWriter.Hex[(int) binary[index] & 15];
      }
      return new string(chArray);
    }

    internal static string BooleanAsXml(bool b) => XmlConvert.ToString(b);

    internal static string BooleanAsXml(bool? b) => EdmValueWriter.BooleanAsXml(b.Value);

    internal static string IntAsXml(int i) => XmlConvert.ToString(i);

    internal static string IntAsXml(int? i) => EdmValueWriter.IntAsXml(i.Value);

    internal static string LongAsXml(long l) => XmlConvert.ToString(l);

    internal static string FloatAsXml(double f) => XmlConvert.ToString(f);

    internal static string DecimalAsXml(Decimal d) => XmlConvert.ToString(d);

    internal static string DurationAsXml(TimeSpan d) => XmlConvert.ToString(d);

    internal static string DateTimeOffsetAsXml(DateTimeOffset d) => XmlConvert.ToString(d);

    internal static string DateAsXml(Date d) => d.ToString();

    internal static string TimeOfDayAsXml(TimeOfDay time) => time.ToString();

    internal static string GuidAsXml(Guid g) => XmlConvert.ToString(g);

    internal static string UriAsXml(Uri uri) => uri.OriginalString;
  }
}
