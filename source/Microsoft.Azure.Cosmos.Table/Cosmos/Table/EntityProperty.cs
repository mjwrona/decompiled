// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.EntityProperty
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.Azure.Cosmos.Table
{
  public sealed class EntityProperty : IEquatable<EntityProperty>
  {
    private object propertyAsObject;

    public object PropertyAsObject
    {
      get => this.propertyAsObject;
      internal set
      {
        this.IsNull = value == null;
        this.propertyAsObject = value;
      }
    }

    public static EntityProperty GeneratePropertyForDateTimeOffset(DateTimeOffset? input) => new EntityProperty(input);

    public static EntityProperty GeneratePropertyForByteArray(byte[] input) => new EntityProperty(input);

    public static EntityProperty GeneratePropertyForBool(bool? input) => new EntityProperty(input);

    public static EntityProperty GeneratePropertyForDouble(double? input) => new EntityProperty(input);

    public static EntityProperty GeneratePropertyForGuid(Guid? input) => new EntityProperty(input);

    public static EntityProperty GeneratePropertyForInt(int? input) => new EntityProperty(input);

    public static EntityProperty GeneratePropertyForLong(long? input) => new EntityProperty(input);

    public static EntityProperty GeneratePropertyForString(string input) => new EntityProperty(input);

    public EntityProperty(byte[] input)
      : this(EdmType.Binary)
    {
      this.PropertyAsObject = (object) input;
    }

    public EntityProperty(bool? input)
      : this(EdmType.Boolean)
    {
      this.IsNull = !input.HasValue;
      this.PropertyAsObject = (object) input;
    }

    public EntityProperty(DateTimeOffset? input)
      : this(EdmType.DateTime)
    {
      if (input.HasValue)
      {
        this.PropertyAsObject = (object) input.Value.UtcDateTime;
      }
      else
      {
        this.IsNull = true;
        this.PropertyAsObject = (object) null;
      }
    }

    public EntityProperty(System.DateTime? input)
      : this(EdmType.DateTime)
    {
      this.IsNull = !input.HasValue;
      this.PropertyAsObject = (object) input;
    }

    public EntityProperty(double? input)
      : this(EdmType.Double)
    {
      this.IsNull = !input.HasValue;
      this.PropertyAsObject = (object) input;
    }

    public EntityProperty(Guid? input)
      : this(EdmType.Guid)
    {
      this.IsNull = !input.HasValue;
      this.PropertyAsObject = (object) input;
    }

    public EntityProperty(int? input)
      : this(EdmType.Int32)
    {
      this.IsNull = !input.HasValue;
      this.PropertyAsObject = (object) input;
    }

    public EntityProperty(long? input)
      : this(EdmType.Int64)
    {
      this.IsNull = !input.HasValue;
      this.PropertyAsObject = (object) input;
    }

    public EntityProperty(string input)
      : this(EdmType.String)
    {
      this.PropertyAsObject = (object) input;
    }

    private EntityProperty(EdmType propertyType) => this.PropertyType = propertyType;

    public EdmType PropertyType { get; private set; }

    public byte[] BinaryValue
    {
      get
      {
        if (!this.IsNull)
          this.EnforceType(EdmType.Binary);
        return (byte[]) this.PropertyAsObject;
      }
      set
      {
        if (value != null)
          this.EnforceType(EdmType.Binary);
        this.PropertyAsObject = (object) value;
      }
    }

    public bool? BooleanValue
    {
      get
      {
        if (!this.IsNull)
          this.EnforceType(EdmType.Boolean);
        return (bool?) this.PropertyAsObject;
      }
      set
      {
        if (value.HasValue)
          this.EnforceType(EdmType.Boolean);
        this.PropertyAsObject = (object) value;
      }
    }

    public System.DateTime? DateTime
    {
      get
      {
        if (!this.IsNull)
          this.EnforceType(EdmType.DateTime);
        return (System.DateTime?) this.PropertyAsObject;
      }
      set
      {
        if (value.HasValue)
          this.EnforceType(EdmType.DateTime);
        this.PropertyAsObject = (object) value;
      }
    }

    public DateTimeOffset? DateTimeOffsetValue
    {
      get
      {
        if (!this.IsNull)
          this.EnforceType(EdmType.DateTime);
        return this.PropertyAsObject == null ? new DateTimeOffset?() : new DateTimeOffset?(new DateTimeOffset((System.DateTime) this.PropertyAsObject));
      }
      set
      {
        if (value.HasValue)
        {
          this.EnforceType(EdmType.DateTime);
          this.PropertyAsObject = (object) value.Value.UtcDateTime;
        }
        else
          this.PropertyAsObject = (object) null;
      }
    }

    public double? DoubleValue
    {
      get
      {
        if (!this.IsNull)
          this.EnforceType(EdmType.Double);
        return (double?) this.PropertyAsObject;
      }
      set
      {
        if (value.HasValue)
          this.EnforceType(EdmType.Double);
        this.PropertyAsObject = (object) value;
      }
    }

    public Guid? GuidValue
    {
      get
      {
        if (!this.IsNull)
          this.EnforceType(EdmType.Guid);
        return (Guid?) this.PropertyAsObject;
      }
      set
      {
        if (value.HasValue)
          this.EnforceType(EdmType.Guid);
        this.PropertyAsObject = (object) value;
      }
    }

    public int? Int32Value
    {
      get
      {
        if (!this.IsNull)
          this.EnforceType(EdmType.Int32);
        return (int?) this.PropertyAsObject;
      }
      set
      {
        if (value.HasValue)
          this.EnforceType(EdmType.Int32);
        this.PropertyAsObject = (object) value;
      }
    }

    public long? Int64Value
    {
      get
      {
        if (!this.IsNull)
          this.EnforceType(EdmType.Int64);
        return (long?) this.PropertyAsObject;
      }
      set
      {
        if (value.HasValue)
          this.EnforceType(EdmType.Int64);
        this.PropertyAsObject = (object) value;
      }
    }

    public string StringValue
    {
      get
      {
        if (!this.IsNull)
          this.EnforceType(EdmType.String);
        return (string) this.PropertyAsObject;
      }
      set
      {
        if (value != null)
          this.EnforceType(EdmType.String);
        this.PropertyAsObject = (object) value;
      }
    }

    public override bool Equals(object obj) => this.Equals(obj as EntityProperty);

    public bool Equals(EntityProperty other)
    {
      if (other == null || this.PropertyType != other.PropertyType)
        return false;
      if (this.IsNull && other.IsNull)
        return true;
      switch (this.PropertyType)
      {
        case EdmType.String:
          return string.Equals(this.StringValue, other.StringValue, StringComparison.Ordinal);
        case EdmType.Binary:
          return this.BinaryValue.Length == other.BinaryValue.Length && ((IEnumerable<byte>) this.BinaryValue).SequenceEqual<byte>((IEnumerable<byte>) other.BinaryValue);
        case EdmType.Boolean:
          bool? booleanValue1 = this.BooleanValue;
          bool? booleanValue2 = other.BooleanValue;
          return booleanValue1.GetValueOrDefault() == booleanValue2.GetValueOrDefault() & booleanValue1.HasValue == booleanValue2.HasValue;
        case EdmType.DateTime:
          System.DateTime? dateTime1 = this.DateTime;
          System.DateTime? dateTime2 = other.DateTime;
          if (dateTime1.HasValue != dateTime2.HasValue)
            return false;
          return !dateTime1.HasValue || dateTime1.GetValueOrDefault() == dateTime2.GetValueOrDefault();
        case EdmType.Double:
          double? doubleValue1 = this.DoubleValue;
          double? doubleValue2 = other.DoubleValue;
          return doubleValue1.GetValueOrDefault() == doubleValue2.GetValueOrDefault() & doubleValue1.HasValue == doubleValue2.HasValue;
        case EdmType.Guid:
          Guid? guidValue1 = this.GuidValue;
          Guid? guidValue2 = other.GuidValue;
          if (guidValue1.HasValue != guidValue2.HasValue)
            return false;
          return !guidValue1.HasValue || guidValue1.GetValueOrDefault() == guidValue2.GetValueOrDefault();
        case EdmType.Int32:
          int? int32Value1 = this.Int32Value;
          int? int32Value2 = other.Int32Value;
          return int32Value1.GetValueOrDefault() == int32Value2.GetValueOrDefault() & int32Value1.HasValue == int32Value2.HasValue;
        case EdmType.Int64:
          long? int64Value1 = this.Int64Value;
          long? int64Value2 = other.Int64Value;
          return int64Value1.GetValueOrDefault() == int64Value2.GetValueOrDefault() & int64Value1.HasValue == int64Value2.HasValue;
        default:
          return this.PropertyAsObject.Equals(other.PropertyAsObject);
      }
    }

    public override int GetHashCode()
    {
      int num;
      if (this.PropertyAsObject == null)
        num = 0;
      else if (this.PropertyType == EdmType.Binary)
      {
        num = 0;
        byte[] propertyAsObject = (byte[]) this.PropertyAsObject;
        if (propertyAsObject.Length != 0)
        {
          for (int startIndex = 0; startIndex < Math.Min(propertyAsObject.Length - 4, 1024); startIndex += 4)
            num ^= BitConverter.ToInt32(propertyAsObject, startIndex);
        }
      }
      else
        num = this.PropertyAsObject.GetHashCode();
      return num ^ this.PropertyType.GetHashCode() ^ this.IsNull.GetHashCode();
    }

    internal bool IsNull { get; set; }

    internal bool IsEncrypted { get; set; }

    public static EntityProperty CreateEntityPropertyFromObject(object entityValue) => EntityProperty.CreateEntityPropertyFromObject(entityValue, true);

    internal static EntityProperty CreateEntityPropertyFromObject(
      object value,
      bool allowUnknownTypes)
    {
      switch (value)
      {
        case string _:
          return new EntityProperty((string) value);
        case byte[] _:
          return new EntityProperty((byte[]) value);
        case bool flag:
          return new EntityProperty(new bool?(flag));
        case bool? input1:
          return new EntityProperty(input1);
        case System.DateTime dateTime:
          return new EntityProperty(new System.DateTime?(dateTime));
        case System.DateTime? input2:
          return new EntityProperty(input2);
        case DateTimeOffset dateTimeOffset:
          return new EntityProperty(new DateTimeOffset?(dateTimeOffset));
        case DateTimeOffset? input3:
          return new EntityProperty(input3);
        case double num1:
          return new EntityProperty(new double?(num1));
        case double? input4:
          return new EntityProperty(input4);
        case Guid? input5:
          return new EntityProperty(input5);
        case Guid guid:
          return new EntityProperty(new Guid?(guid));
        case int num2:
          return new EntityProperty(new int?(num2));
        case int? input6:
          return new EntityProperty(input6);
        case long num3:
          return new EntityProperty(new long?(num3));
        case long? input7:
          return new EntityProperty(input7);
        case null:
          return new EntityProperty((string) null);
        default:
          return allowUnknownTypes ? new EntityProperty(value.ToString()) : (EntityProperty) null;
      }
    }

    internal static EntityProperty CreateEntityPropertyFromObject(object value, Type type)
    {
      if (type == typeof (string))
        return new EntityProperty((string) value);
      if (type == typeof (byte[]))
        return new EntityProperty((byte[]) value);
      if (type == typeof (bool))
        return new EntityProperty(new bool?((bool) value));
      if (type == typeof (bool?))
        return new EntityProperty((bool?) value);
      if (type == typeof (System.DateTime))
        return new EntityProperty(new System.DateTime?((System.DateTime) value));
      if (type == typeof (System.DateTime?))
        return new EntityProperty((System.DateTime?) value);
      if (type == typeof (DateTimeOffset))
        return new EntityProperty(new DateTimeOffset?((DateTimeOffset) value));
      if (type == typeof (DateTimeOffset?))
        return new EntityProperty((DateTimeOffset?) value);
      if (type == typeof (double))
        return new EntityProperty(new double?((double) value));
      if (type == typeof (double?))
        return new EntityProperty((double?) value);
      if (type == typeof (Guid?))
        return new EntityProperty((Guid?) value);
      if (type == typeof (Guid))
        return new EntityProperty(new Guid?((Guid) value));
      if (type == typeof (int))
        return new EntityProperty(new int?((int) value));
      if (type == typeof (int?))
        return new EntityProperty((int?) value);
      if (type == typeof (long))
        return new EntityProperty(new long?((long) value));
      return type == typeof (long?) ? new EntityProperty((long?) value) : (EntityProperty) null;
    }

    internal static EntityProperty CreateEntityPropertyFromObject(object value, EdmType type)
    {
      switch (type)
      {
        case EdmType.String:
          return new EntityProperty((string) value);
        case EdmType.Binary:
          return new EntityProperty(Convert.FromBase64String((string) value));
        case EdmType.Boolean:
          return new EntityProperty(new bool?(bool.Parse((string) value)));
        case EdmType.DateTime:
          return new EntityProperty(new DateTimeOffset?(DateTimeOffset.Parse((string) value, (IFormatProvider) CultureInfo.InvariantCulture)));
        case EdmType.Double:
          return new EntityProperty(new double?(double.Parse((string) value, (IFormatProvider) CultureInfo.InvariantCulture)));
        case EdmType.Guid:
          return new EntityProperty(new Guid?(Guid.Parse((string) value)));
        case EdmType.Int32:
          return new EntityProperty(new int?(int.Parse((string) value, (IFormatProvider) CultureInfo.InvariantCulture)));
        case EdmType.Int64:
          return new EntityProperty(new long?(long.Parse((string) value, (IFormatProvider) CultureInfo.InvariantCulture)));
        default:
          return (EntityProperty) null;
      }
    }

    public override string ToString() => EntityProperty.GetStringValue(this);

    internal static string GetStringValue(EntityProperty entityProperty)
    {
      object objectValue = EntityProperty.GetObjectValue(entityProperty);
      if (objectValue == null)
        return (string) null;
      if (entityProperty.PropertyType == EdmType.Double)
        return entityProperty.DoubleValue.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      if (entityProperty.PropertyType == EdmType.DateTime)
        return entityProperty.DateTime.Value.ToString("o", (IFormatProvider) CultureInfo.InvariantCulture);
      return entityProperty.PropertyType == EdmType.Binary ? Convert.ToBase64String(entityProperty.BinaryValue) : objectValue.ToString();
    }

    internal static object GetObjectValue(EntityProperty entityProperty)
    {
      if (entityProperty.PropertyType == EdmType.String)
        return (object) entityProperty.StringValue;
      if (entityProperty.PropertyType == EdmType.Binary)
        return (object) entityProperty.BinaryValue;
      if (entityProperty.PropertyType == EdmType.Boolean)
        return (object) entityProperty.BooleanValue;
      if (entityProperty.PropertyType == EdmType.DateTime)
        return (object) entityProperty.DateTime;
      if (entityProperty.PropertyType == EdmType.Double)
        return (object) entityProperty.DoubleValue;
      if (entityProperty.PropertyType == EdmType.Guid)
        return (object) entityProperty.GuidValue;
      if (entityProperty.PropertyType == EdmType.Int32)
        return (object) entityProperty.Int32Value;
      return entityProperty.PropertyType == EdmType.Int64 ? (object) entityProperty.Int64Value : (object) null;
    }

    private void EnforceType(EdmType requestedType)
    {
      if (this.PropertyType != requestedType)
        throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Cannot return {0} type for a {1} typed property.", (object) requestedType, (object) this.PropertyType));
    }
  }
}
