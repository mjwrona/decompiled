// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.Queryable.ClientConvert
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;

namespace Microsoft.Azure.Cosmos.Table.Queryable
{
  internal static class ClientConvert
  {
    private const string SystemDataLinq = "System.Data.Linq, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
    private static readonly Type[] KnownTypes = ClientConvert.CreateKnownPrimitives();
    private static readonly Dictionary<string, Type> NamedTypesMap = ClientConvert.CreateKnownNamesMap();
    private static bool needSystemDataLinqBinary = true;

    internal static object ChangeType(string propertyValue, Type propertyType)
    {
      try
      {
        switch (ClientConvert.IndexOfStorage(propertyType))
        {
          case 0:
            return (object) XmlConvert.ToBoolean(propertyValue);
          case 1:
            return (object) XmlConvert.ToByte(propertyValue);
          case 2:
            return (object) Convert.FromBase64String(propertyValue);
          case 3:
            return (object) XmlConvert.ToChar(propertyValue);
          case 4:
            return (object) propertyValue.ToCharArray();
          case 5:
            return (object) XmlConvert.ToDateTime(propertyValue, XmlDateTimeSerializationMode.RoundtripKind);
          case 6:
            return (object) XmlConvert.ToDateTimeOffset(propertyValue);
          case 7:
            return (object) XmlConvert.ToDecimal(propertyValue);
          case 8:
            return (object) XmlConvert.ToDouble(propertyValue);
          case 9:
            return (object) new Guid(propertyValue);
          case 10:
            return (object) XmlConvert.ToInt16(propertyValue);
          case 11:
            return (object) XmlConvert.ToInt32(propertyValue);
          case 12:
            return (object) XmlConvert.ToInt64(propertyValue);
          case 13:
            return (object) XmlConvert.ToSingle(propertyValue);
          case 14:
            return (object) propertyValue;
          case 15:
            return (object) XmlConvert.ToSByte(propertyValue);
          case 16:
            return (object) XmlConvert.ToTimeSpan(propertyValue);
          case 17:
            return (object) Type.GetType(propertyValue, true);
          case 18:
            return (object) XmlConvert.ToUInt16(propertyValue);
          case 19:
            return (object) XmlConvert.ToUInt32(propertyValue);
          case 20:
            return (object) XmlConvert.ToUInt64(propertyValue);
          case 21:
            return (object) ClientConvert.CreateUri(propertyValue, UriKind.RelativeOrAbsolute);
          case 22:
            return 0 < propertyValue.Length ? (object) XDocument.Parse(propertyValue) : (object) new XDocument();
          case 23:
            return (object) XElement.Parse(propertyValue);
          case 24:
            return Activator.CreateInstance(ClientConvert.KnownTypes[24], (object) Convert.FromBase64String(propertyValue));
          default:
            return (object) propertyValue;
        }
      }
      catch (FormatException ex)
      {
        propertyValue = propertyValue.Length == 0 ? "String.Empty" : "String";
        throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "The current value '{1}' type is not compatible with the expected '{0}' type.", (object) propertyType.ToString(), (object) propertyValue), (Exception) ex);
      }
      catch (OverflowException ex)
      {
        propertyValue = propertyValue.Length == 0 ? "String.Empty" : "String";
        throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "The current value '{1}' type is not compatible with the expected '{0}' type.", (object) propertyType.ToString(), (object) propertyValue), (Exception) ex);
      }
    }

    internal static bool IsBinaryValue(object value) => 24 == ClientConvert.IndexOfStorage(value.GetType());

    internal static bool TryKeyBinaryToString(object binaryValue, out string result) => WebConvert.TryKeyPrimitiveToString((object) (byte[]) binaryValue.GetType().InvokeMember("ToArray", BindingFlags.Instance | BindingFlags.Public | BindingFlags.InvokeMethod, (Binder) null, binaryValue, (object[]) null, (ParameterModifier[]) null, CultureInfo.InvariantCulture, (string[]) null), out result);

    internal static bool TryKeyPrimitiveToString(object value, out string result)
    {
      if (ClientConvert.IsBinaryValue(value))
        return ClientConvert.TryKeyBinaryToString(value, out result);
      switch (value)
      {
        case DateTimeOffset dateTimeOffset:
          value = (object) dateTimeOffset.UtcDateTime;
          break;
        case DateTimeOffset? nullable:
          value = (object) nullable.Value.UtcDateTime;
          break;
      }
      return WebConvert.TryKeyPrimitiveToString(value, out result);
    }

    internal static bool ToNamedType(string typeName, out Type type)
    {
      type = typeof (string);
      return string.IsNullOrEmpty(typeName) || ClientConvert.NamedTypesMap.TryGetValue(typeName, out type);
    }

    internal static string ToTypeName(Type type)
    {
      foreach (KeyValuePair<string, Type> namedTypes in ClientConvert.NamedTypesMap)
      {
        if (namedTypes.Value == type)
          return namedTypes.Key;
      }
      return type.FullName;
    }

    internal static string ToString(object propertyValue, bool atomDateConstruct)
    {
      switch (ClientConvert.IndexOfStorage(propertyValue.GetType()))
      {
        case 0:
          return XmlConvert.ToString((bool) propertyValue);
        case 1:
          return XmlConvert.ToString((byte) propertyValue);
        case 2:
          return Convert.ToBase64String((byte[]) propertyValue);
        case 3:
          return XmlConvert.ToString((char) propertyValue);
        case 4:
          return new string((char[]) propertyValue);
        case 5:
          DateTime dateTime = (DateTime) propertyValue;
          return XmlConvert.ToString(dateTime.Kind == DateTimeKind.Unspecified & atomDateConstruct ? new DateTime(dateTime.Ticks, DateTimeKind.Utc) : dateTime, XmlDateTimeSerializationMode.RoundtripKind);
        case 6:
          return XmlConvert.ToString((DateTimeOffset) propertyValue);
        case 7:
          return XmlConvert.ToString((Decimal) propertyValue);
        case 8:
          return XmlConvert.ToString((double) propertyValue);
        case 9:
          return ((Guid) propertyValue).ToString();
        case 10:
          return XmlConvert.ToString((short) propertyValue);
        case 11:
          return XmlConvert.ToString((int) propertyValue);
        case 12:
          return XmlConvert.ToString((long) propertyValue);
        case 13:
          return XmlConvert.ToString((float) propertyValue);
        case 14:
          return (string) propertyValue;
        case 15:
          return XmlConvert.ToString((sbyte) propertyValue);
        case 16:
          return XmlConvert.ToString((TimeSpan) propertyValue);
        case 17:
          return ((Type) propertyValue).AssemblyQualifiedName;
        case 18:
          return XmlConvert.ToString((ushort) propertyValue);
        case 19:
          return XmlConvert.ToString((uint) propertyValue);
        case 20:
          return XmlConvert.ToString((ulong) propertyValue);
        case 21:
          return ((Uri) propertyValue).ToString();
        case 22:
          return ((XDocument) propertyValue).ToString();
        case 23:
          return ((XElement) propertyValue).ToString();
        case 24:
          return propertyValue.ToString();
        default:
          return propertyValue.ToString();
      }
    }

    internal static bool IsKnownType(Type type) => 0 <= ClientConvert.IndexOfStorage(type);

    internal static bool IsKnownNullableType(Type type)
    {
      Type type1 = Nullable.GetUnderlyingType(type);
      if ((object) type1 == null)
        type1 = type;
      return ClientConvert.IsKnownType(type1);
    }

    internal static bool IsSupportedPrimitiveTypeForUri(Type type) => ClientConvert.ContainsReference<Type>(ClientConvert.NamedTypesMap.Values.ToArray<Type>(), type);

    internal static bool ContainsReference<T>(T[] array, T value) where T : class => 0 <= ClientConvert.IndexOfReference<T>(array, value);

    internal static string GetEdmType(Type propertyType)
    {
      switch (ClientConvert.IndexOfStorage(propertyType))
      {
        case 0:
          return "Edm.Boolean";
        case 1:
          return "Edm.Byte";
        case 2:
        case 24:
          return "Edm.Binary";
        case 3:
        case 4:
        case 14:
        case 17:
        case 21:
        case 22:
        case 23:
          return (string) null;
        case 5:
          return "Edm.DateTime";
        case 6:
        case 16:
        case 18:
        case 19:
        case 20:
          throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Can't cast to unsupported type '{0}'", (object) propertyType.Name));
        case 7:
          return "Edm.Decimal";
        case 8:
          return "Edm.Double";
        case 9:
          return "Edm.Guid";
        case 10:
          return "Edm.Int16";
        case 11:
          return "Edm.Int32";
        case 12:
          return "Edm.Int64";
        case 13:
          return "Edm.Single";
        case 15:
          return "Edm.SByte";
        default:
          return (string) null;
      }
    }

    private static Type[] CreateKnownPrimitives() => new Type[25]
    {
      typeof (bool),
      typeof (byte),
      typeof (byte[]),
      typeof (char),
      typeof (char[]),
      typeof (DateTime),
      typeof (DateTimeOffset),
      typeof (Decimal),
      typeof (double),
      typeof (Guid),
      typeof (short),
      typeof (int),
      typeof (long),
      typeof (float),
      typeof (string),
      typeof (sbyte),
      typeof (TimeSpan),
      typeof (Type),
      typeof (ushort),
      typeof (uint),
      typeof (ulong),
      typeof (Uri),
      typeof (XDocument),
      typeof (XElement),
      (Type) null
    };

    private static Dictionary<string, Type> CreateKnownNamesMap() => new Dictionary<string, Type>((IEqualityComparer<string>) EqualityComparer<string>.Default)
    {
      {
        "Edm.String",
        typeof (string)
      },
      {
        "Edm.Boolean",
        typeof (bool)
      },
      {
        "Edm.Byte",
        typeof (byte)
      },
      {
        "Edm.DateTime",
        typeof (DateTime)
      },
      {
        "Edm.Decimal",
        typeof (Decimal)
      },
      {
        "Edm.Double",
        typeof (double)
      },
      {
        "Edm.Guid",
        typeof (Guid)
      },
      {
        "Edm.Int16",
        typeof (short)
      },
      {
        "Edm.Int32",
        typeof (int)
      },
      {
        "Edm.Int64",
        typeof (long)
      },
      {
        "Edm.SByte",
        typeof (sbyte)
      },
      {
        "Edm.Single",
        typeof (float)
      },
      {
        "Edm.Binary",
        typeof (byte[])
      }
    };

    private static int IndexOfStorage(Type type)
    {
      int num = ClientConvert.IndexOfReference<Type>(ClientConvert.KnownTypes, type);
      return num < 0 && ClientConvert.needSystemDataLinqBinary && type.Name == "Binary" ? ClientConvert.LoadSystemDataLinqBinary(type) : num;
    }

    internal static int IndexOfReference<T>(T[] array, T value) where T : class
    {
      for (int index = 0; index < array.Length; ++index)
      {
        if ((object) array[index] == (object) value)
          return index;
      }
      return -1;
    }

    internal static Uri CreateUri(string value, UriKind kind) => value != null ? new Uri(value, kind) : (Uri) null;

    private static int LoadSystemDataLinqBinary(Type type)
    {
      if (!(type.Namespace == "System.Data.Linq") || !AssemblyName.ReferenceMatchesDefinition(type.Assembly.GetName(), new AssemblyName("System.Data.Linq, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")))
        return -1;
      ClientConvert.KnownTypes[24] = type;
      ClientConvert.needSystemDataLinqBinary = false;
      return 24;
    }

    internal enum StorageType
    {
      Boolean,
      Byte,
      ByteArray,
      Char,
      CharArray,
      DateTime,
      DateTimeOffset,
      Decimal,
      Double,
      Guid,
      Int16,
      Int32,
      Int64,
      Single,
      String,
      SByte,
      TimeSpan,
      Type,
      UInt16,
      UInt32,
      UInt64,
      Uri,
      XDocument,
      XElement,
      Binary,
    }
  }
}
