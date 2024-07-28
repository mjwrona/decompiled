// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.EdmValueWriter
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Vocabularies;
using System;
using System.Xml;

namespace Microsoft.OData.Edm.Csdl
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

    internal static string PrimitiveValueAsXml(IEdmPrimitiveValue v)
    {
      switch (v.ValueKind)
      {
        case EdmValueKind.Binary:
          return EdmValueWriter.BinaryAsXml(((IEdmBinaryValue) v).Value);
        case EdmValueKind.Boolean:
          return EdmValueWriter.BooleanAsXml(((IEdmBooleanValue) v).Value);
        case EdmValueKind.DateTimeOffset:
          return EdmValueWriter.DateTimeOffsetAsXml(((IEdmDateTimeOffsetValue) v).Value);
        case EdmValueKind.Decimal:
          return EdmValueWriter.DecimalAsXml(((IEdmDecimalValue) v).Value);
        case EdmValueKind.Floating:
          return EdmValueWriter.FloatAsXml(((IEdmFloatingValue) v).Value);
        case EdmValueKind.Guid:
          return EdmValueWriter.GuidAsXml(((IEdmGuidValue) v).Value);
        case EdmValueKind.Integer:
          return EdmValueWriter.LongAsXml(((IEdmIntegerValue) v).Value);
        case EdmValueKind.String:
          return EdmValueWriter.StringAsXml(((IEdmStringValue) v).Value);
        case EdmValueKind.Duration:
          return EdmValueWriter.DurationAsXml(((IEdmDurationValue) v).Value);
        case EdmValueKind.Date:
          return EdmValueWriter.DateAsXml(((IEdmDateValue) v).Value);
        case EdmValueKind.TimeOfDay:
          return EdmValueWriter.TimeOfDayAsXml(((IEdmTimeOfDayValue) v).Value);
        default:
          throw new NotSupportedException(Strings.ValueWriter_NonSerializableValue((object) v.ValueKind));
      }
    }

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
