// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Query.ItemTypeHelper
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Newtonsoft.Json.Linq;
using System;
using System.Globalization;

namespace Microsoft.Azure.Documents.Query
{
  internal static class ItemTypeHelper
  {
    public static ItemType GetItemType(object value)
    {
      switch (value)
      {
        case Undefined _:
          return ItemType.NoValue;
        case null:
          return ItemType.Null;
        case bool _:
          return ItemType.Bool;
        case string _:
          return ItemType.String;
        default:
          if (ItemTypeHelper.IsNumeric(value))
            return ItemType.Number;
          switch (value)
          {
            case JArray _:
              return ItemType.Array;
            case JObject _:
              return ItemType.Object;
            default:
              throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Unrecognized type {0}", (object) value.GetType().ToString()));
          }
      }
    }

    public static bool IsPrimitive(object value)
    {
      switch (value)
      {
        case null:
        case bool _:
        case string _:
          return true;
        default:
          return ItemTypeHelper.IsNumeric(value);
      }
    }

    public static bool IsNumeric(object value)
    {
      switch (value)
      {
        case sbyte _:
        case byte _:
        case short _:
        case ushort _:
        case int _:
        case uint _:
        case long _:
        case ulong _:
        case float _:
        case double _:
          return true;
        default:
          return value is Decimal;
      }
    }
  }
}
