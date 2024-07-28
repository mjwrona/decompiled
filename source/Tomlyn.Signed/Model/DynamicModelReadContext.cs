// Decompiled with JetBrains decompiler
// Type: Tomlyn.Model.DynamicModelReadContext
// Assembly: Tomlyn.Signed, Version=0.16.0.0, Culture=neutral, PublicKeyToken=925c35bbd70fa682
// MVID: 8155B7FF-9444-4ADB-B8A6-687FA556DA39
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Tomlyn.Signed.dll

using System;
using System.Collections.Generic;
using System.Reflection;
using Tomlyn.Model.Accessors;
using Tomlyn.Syntax;


#nullable enable
namespace Tomlyn.Model
{
  internal class DynamicModelReadContext
  {
    private readonly Dictionary<Type, DynamicAccessor> _accessors;

    public DynamicModelReadContext(TomlModelOptions options)
    {
      this.GetPropertyName = options.GetPropertyName;
      this.GetFieldName = options.GetFieldName;
      this.ConvertPropertyName = options.ConvertPropertyName;
      this.ConvertFieldName = options.ConvertFieldName;
      this.CreateInstance = options.CreateInstance;
      this.ConvertToModel = options.ConvertToModel;
      this.IgnoreMissingProperties = options.IgnoreMissingProperties;
      this.IncludeFields = options.IncludeFields;
      this.Diagnostics = new DiagnosticsBag();
      this._accessors = new Dictionary<Type, DynamicAccessor>();
    }

    public Func<PropertyInfo, string?> GetPropertyName { get; set; }

    public Func<FieldInfo, string?> GetFieldName { get; set; }

    public Func<string, string> ConvertPropertyName { get; set; }

    public Func<string, string> ConvertFieldName { get; set; }

    public Func<Type, ObjectKind, object> CreateInstance { get; set; }

    public Func<object, Type, object?>? ConvertToModel { get; set; }

    public DiagnosticsBag Diagnostics { get; }

    public bool IgnoreMissingProperties { get; set; }

    public bool IncludeFields { get; set; }

    public DynamicAccessor GetAccessor(Type type)
    {
      DynamicAccessor accessor;
      if (!this._accessors.TryGetValue(type, out accessor))
      {
        ReflectionObjectInfo reflectionObjectInfo = ReflectionObjectInfo.Get(type);
        switch (reflectionObjectInfo.Kind)
        {
          case ReflectionObjectKind.Primitive:
            accessor = (DynamicAccessor) new PrimitiveDynamicAccessor(this, type, false);
            break;
          case ReflectionObjectKind.NullablePrimitive:
            accessor = (DynamicAccessor) new PrimitiveDynamicAccessor(this, reflectionObjectInfo.GenericArgument1, true);
            break;
          case ReflectionObjectKind.Struct:
          case ReflectionObjectKind.Object:
            accessor = (DynamicAccessor) new StandardObjectDynamicAccessor(this, type, reflectionObjectInfo.Kind);
            break;
          case ReflectionObjectKind.NullableStruct:
            accessor = (DynamicAccessor) new StandardObjectDynamicAccessor(this, reflectionObjectInfo.GenericArgument1, ReflectionObjectKind.Struct);
            break;
          case ReflectionObjectKind.Collection:
            accessor = (DynamicAccessor) new ListDynamicAccessor(this, type, reflectionObjectInfo.GenericArgument1);
            break;
          case ReflectionObjectKind.Dictionary:
            accessor = (DynamicAccessor) new DictionaryDynamicAccessor(this, type, reflectionObjectInfo.GenericArgument1, reflectionObjectInfo.GenericArgument2);
            break;
          default:
            throw new ArgumentOutOfRangeException(string.Format("Unsupported {0}", (object) reflectionObjectInfo.Kind));
        }
        this._accessors.Add(type, accessor);
      }
      return accessor;
    }

    public bool TryConvertValue(
      SourceSpan span,
      object? value,
      Type changeType,
      out object? outputValue)
    {
      if (value == null || changeType.IsInstanceOfType(value))
      {
        outputValue = value;
        return true;
      }
      Type underlyingType = Nullable.GetUnderlyingType(changeType);
      if (underlyingType != (Type) null)
        return this.TryConvertValue(span, value, underlyingType, out outputValue);
      string text;
      try
      {
        if (typeof (Enum).IsAssignableFrom(changeType) && value is string str)
        {
          value = Enum.Parse(changeType, str, true);
          outputValue = value;
          return true;
        }
        if (value is IConvertible)
        {
          if (value.GetType().IsPrimitive && changeType.IsPrimitive)
          {
            switch (value)
            {
              case sbyte num1:
                if (changeType == typeof (byte))
                {
                  outputValue = (object) (byte) num1;
                  return true;
                }
                break;
              case short num2:
                if (changeType == typeof (ushort))
                {
                  outputValue = (object) (ushort) num2;
                  return true;
                }
                break;
              case int num3:
                if (changeType == typeof (uint))
                {
                  outputValue = (object) (uint) num3;
                  return true;
                }
                break;
              case long num4:
                if (changeType == typeof (ulong))
                {
                  outputValue = (object) (ulong) num4;
                  return true;
                }
                break;
              case byte num5:
                if (changeType == typeof (sbyte))
                {
                  outputValue = (object) (sbyte) num5;
                  return true;
                }
                break;
              case ushort num6:
                if (changeType == typeof (short))
                {
                  outputValue = (object) (short) num6;
                  return true;
                }
                break;
              case uint num7:
                if (changeType == typeof (int))
                {
                  outputValue = (object) (int) num7;
                  return true;
                }
                break;
              case ulong num8:
                if (changeType == typeof (long))
                {
                  outputValue = (object) (long) num8;
                  return true;
                }
                break;
            }
          }
          try
          {
            outputValue = Convert.ChangeType(value, changeType);
            return true;
          }
          catch (Exception ex) when (this.ConvertToModel != null)
          {
          }
        }
        if (this.ConvertToModel != null)
        {
          object obj = this.ConvertToModel(value, changeType);
          outputValue = obj;
          if (obj != null)
            return true;
        }
        text = "Unsupported type to convert " + value.GetType().FullName + " to type " + changeType.FullName + ".";
      }
      catch (Exception ex)
      {
        text = "Exception while trying to convert " + value.GetType().FullName + " to type " + changeType.FullName + ". Reason: " + ex.Message;
      }
      outputValue = (object) null;
      this.Diagnostics.Error(span, text);
      return false;
    }
  }
}
