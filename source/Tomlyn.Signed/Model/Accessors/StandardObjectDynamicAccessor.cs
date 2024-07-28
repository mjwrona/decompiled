// Decompiled with JetBrains decompiler
// Type: Tomlyn.Model.Accessors.StandardObjectDynamicAccessor
// Assembly: Tomlyn.Signed, Version=0.16.0.0, Culture=neutral, PublicKeyToken=925c35bbd70fa682
// MVID: 8155B7FF-9444-4ADB-B8A6-687FA556DA39
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Tomlyn.Signed.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Tomlyn.Syntax;


#nullable enable
namespace Tomlyn.Model.Accessors
{
  internal class StandardObjectDynamicAccessor : ObjectDynamicAccessor
  {
    private readonly Dictionary<string, PropertyInfo> _props;
    private readonly Dictionary<string, FieldInfo> _fields;
    private readonly List<KeyValuePair<string, PropertyInfo>> _orderedProps;
    private readonly List<KeyValuePair<string, FieldInfo>> _orderedFields;

    public StandardObjectDynamicAccessor(
      DynamicModelReadContext context,
      Type type,
      ReflectionObjectKind kind)
      : base(context, type, kind)
    {
      this._props = new Dictionary<string, PropertyInfo>();
      this._fields = new Dictionary<string, FieldInfo>();
      this._orderedProps = new List<KeyValuePair<string, PropertyInfo>>();
      this._orderedFields = new List<KeyValuePair<string, FieldInfo>>();
      this.Initialize();
    }

    private void Initialize()
    {
      foreach (PropertyInfo property in this.TargetType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy))
      {
        string key = this.Context.GetPropertyName(property);
        if (key != null && property.CanRead && (!(property.PropertyType == typeof (string)) && !property.PropertyType.IsValueType || property.CanWrite) && !this._props.ContainsKey(key))
        {
          this._props[key] = property;
          this._orderedProps.Add(new KeyValuePair<string, PropertyInfo>(key, property));
        }
      }
      if (!this.Context.IncludeFields)
        return;
      foreach (FieldInfo field in this.TargetType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy))
      {
        string key = this.Context.GetFieldName(field);
        if (key != null && (!(field.FieldType == typeof (string)) && !field.FieldType.IsValueType || !field.IsInitOnly) && !this._fields.ContainsKey(key))
        {
          this._fields[key] = field;
          this._orderedFields.Add(new KeyValuePair<string, FieldInfo>(key, field));
        }
      }
    }

    public override IEnumerable<KeyValuePair<string, object?>> GetProperties(object obj)
    {
      StandardObjectDynamicAccessor objectDynamicAccessor = this;
      foreach (KeyValuePair<string, PropertyInfo> orderedProp in objectDynamicAccessor._orderedProps)
        yield return new KeyValuePair<string, object>(orderedProp.Key, orderedProp.Value.GetValue(obj));
      if (objectDynamicAccessor.Context.IncludeFields)
      {
        foreach (KeyValuePair<string, FieldInfo> orderedField in objectDynamicAccessor._orderedFields)
          yield return new KeyValuePair<string, object>(orderedField.Key, orderedField.Value.GetValue(obj));
      }
    }

    public override bool TryGetPropertyValue(
      SourceSpan span,
      object obj,
      string name,
      out object? value)
    {
      value = (object) null;
      PropertyInfo propertyInfo;
      if (this._props.TryGetValue(name, out propertyInfo))
      {
        value = propertyInfo.GetValue(obj);
        return true;
      }
      FieldInfo fieldInfo;
      if (!this.Context.IncludeFields || !this._fields.TryGetValue(name, out fieldInfo))
        return false;
      value = fieldInfo.GetValue(obj);
      return true;
    }

    public override bool TrySetPropertyValue(
      SourceSpan span,
      object obj,
      string name,
      object? value)
    {
      string text = "Unknown error";
      try
      {
        PropertyInfo propertyInfo;
        if (this._props.TryGetValue(name, out propertyInfo))
        {
          if (this.Context.GetAccessor(propertyInfo.PropertyType) is ListDynamicAccessor accessor1)
          {
            object list = propertyInfo.GetValue(obj);
            if (value != null && propertyInfo.PropertyType.IsInstanceOfType(value))
            {
              if ((object) propertyInfo.SetMethod != null)
              {
                if (this.Context.TryConvertValue(span, value, propertyInfo.PropertyType, out value))
                {
                  propertyInfo.SetValue(obj, value);
                  return true;
                }
                text = string.Format("The property value of type {0} couldn't be converted to {1} for the list property {2}/{3}", (object) value?.GetType().FullName, (object) propertyInfo.PropertyType, (object) this.TargetType.FullName, (object) name);
              }
              else if (list != null)
              {
                foreach (object obj1 in (IEnumerable) value)
                  accessor1.AddElement(list, obj1);
                return true;
              }
            }
            else
            {
              if (list == null)
              {
                list = this.Context.CreateInstance(accessor1.TargetType, ObjectKind.Array);
                propertyInfo.SetValue(obj, list);
              }
              if (this.Context.TryConvertValue(span, value, accessor1.ElementType, out value))
              {
                accessor1.AddElement(list, value);
                return true;
              }
              text = string.Format("The property value of type {0} couldn't be converted to {1} for the list property {2}/{3}", (object) value?.GetType().FullName, (object) accessor1.ElementType, (object) this.TargetType.FullName, (object) name);
            }
          }
          else
          {
            object outputValue;
            if (this.Context.TryConvertValue(span, value, propertyInfo.PropertyType, out outputValue))
            {
              propertyInfo.SetValue(obj, outputValue);
              return true;
            }
            text = string.Format("The property value of type {0} couldn't be converted to {1} for the property {2}/{3}", (object) value?.GetType().FullName, (object) propertyInfo.PropertyType, (object) this.TargetType.FullName, (object) name);
          }
        }
        else
          text = "The property " + name + " was not found on object type " + this.TargetType.FullName;
        if (this.Context.IncludeFields)
        {
          FieldInfo fieldInfo;
          if (this._fields.TryGetValue(name, out fieldInfo))
          {
            if (this.Context.GetAccessor(fieldInfo.FieldType) is ListDynamicAccessor accessor2)
            {
              object list = fieldInfo.GetValue(obj);
              if (value != null && fieldInfo.FieldType.IsInstanceOfType(value))
              {
                if (list != null)
                {
                  foreach (object obj2 in (IEnumerable) value)
                    accessor2.AddElement(list, obj2);
                  return true;
                }
                if (this.Context.TryConvertValue(span, value, fieldInfo.FieldType, out value))
                {
                  fieldInfo.SetValue(obj, value);
                  return true;
                }
                text = string.Format("The field value of type {0} couldn't be converted to {1} for the list property or field {2}/{3}", (object) value?.GetType().FullName, (object) fieldInfo.FieldType, (object) this.TargetType.FullName, (object) name);
              }
              else
              {
                if (list == null)
                {
                  list = this.Context.CreateInstance(accessor2.TargetType, ObjectKind.Array);
                  fieldInfo.SetValue(obj, list);
                }
                if (this.Context.TryConvertValue(span, value, accessor2.ElementType, out value))
                {
                  accessor2.AddElement(list, value);
                  return true;
                }
                text = string.Format("The field value of type {0} couldn't be converted to {1} for the list property or field {2}/{3}", (object) value?.GetType().FullName, (object) accessor2.ElementType, (object) this.TargetType.FullName, (object) name);
              }
            }
            else
            {
              object outputValue;
              if (this.Context.TryConvertValue(span, value, fieldInfo.FieldType, out outputValue))
              {
                fieldInfo.SetValue(obj, outputValue);
                return true;
              }
              text = string.Format("The field value of type {0} couldn't be converted to {1} for the field {2}/{3}", (object) value?.GetType().FullName, (object) fieldInfo.FieldHandle, (object) this.TargetType.FullName, (object) name);
            }
          }
          else
            text = "The field " + name + " was not found on object type " + this.TargetType.FullName;
        }
      }
      catch (Exception ex)
      {
        string str;
        if (!this.Context.IncludeFields)
          str = "Unexpected error while trying to set property " + name + " was not found on object type " + this.TargetType.FullName + ". Reason: " + ex.Message;
        else
          str = "Unexpected error while trying to set property or field " + name + " was not found on object type " + this.TargetType.FullName + ". Reason: " + ex.Message;
        text = str;
      }
      this.Context.Diagnostics.Error(span, text);
      return false;
    }

    public override bool TryGetPropertyType(SourceSpan span, string name, out Type? propertyType)
    {
      propertyType = (Type) null;
      PropertyInfo propertyInfo;
      if (this._props.TryGetValue(name, out propertyInfo))
      {
        propertyType = propertyInfo.PropertyType;
        return true;
      }
      FieldInfo fieldInfo1;
      if (this.Context.IncludeFields && this._fields.TryGetValue(name, out fieldInfo1))
      {
        propertyType = fieldInfo1.FieldType;
        return true;
      }
      string key1 = this.Context.ConvertPropertyName(name);
      if (key1 != name && this._props.TryGetValue(key1, out propertyInfo))
      {
        propertyType = propertyInfo.PropertyType;
        this.Context.Diagnostics.Warning(span, "The property `" + name + "` was not found, but `" + key1 + "` was. By default property names are lowered and split by _ by PascalCase letters. This behavior can be changed by passing a TomlModelOptions and specifying the TomlModelOptions.ConvertPropertyName delegate.");
        return true;
      }
      if (this.Context.IncludeFields)
      {
        string key2 = this.Context.ConvertFieldName(name);
        FieldInfo fieldInfo2;
        if (key2 != name && this._fields.TryGetValue(key2, out fieldInfo2))
        {
          propertyType = fieldInfo2.FieldType;
          this.Context.Diagnostics.Warning(span, "The field `" + name + "` was not found, but `" + key2 + "` was. By default field names are lowered and split by _ by PascalCase letters. This behavior can be changed by passing a TomlModelOptions and specifying the TomlModelOptions.ConvertFieldName delegate.");
          return true;
        }
      }
      if (!this.Context.IgnoreMissingProperties)
      {
        string text = this.Context.IncludeFields ? "The property or field `" + name + "` was not found on object type " + this.TargetType.FullName : "The property `" + name + "` was not found on object type " + this.TargetType.FullName;
        this.Context.Diagnostics.Error(span, text);
      }
      return false;
    }

    public override bool TryCreateAndSetDefaultPropertyValue(
      SourceSpan span,
      object obj,
      string name,
      ObjectKind kind,
      out object? instance)
    {
      instance = (object) null;
      string text;
      try
      {
        PropertyInfo propertyInfo;
        if (this._props.TryGetValue(name, out propertyInfo))
        {
          instance = this.Context.CreateInstance(propertyInfo.PropertyType, kind);
          if (instance != null)
          {
            propertyInfo.SetValue(obj, instance);
            return true;
          }
        }
        FieldInfo fieldInfo;
        if (this.Context.IncludeFields && this._fields.TryGetValue(name, out fieldInfo))
        {
          instance = this.Context.CreateInstance(fieldInfo.FieldType, kind);
          if (instance != null)
          {
            fieldInfo.SetValue(obj, instance);
            return true;
          }
        }
        string str;
        if (!this.Context.IncludeFields)
          str = "Unable to set the property " + name + " on object type " + this.TargetType.FullName + ".";
        else
          str = "Unable to set the property or field " + name + " on object type " + this.TargetType.FullName + ".";
        text = str;
      }
      catch (Exception ex)
      {
        string str;
        if (!this.Context.IncludeFields)
          str = "Unexpected error when creating object for property " + name + " on object type " + this.TargetType.FullName + ". Reason: " + ex.Message;
        else
          str = "Unexpected error when creating object for property or field " + name + " on object type " + this.TargetType.FullName + ". Reason: " + ex.Message;
        text = str;
      }
      if (!this.Context.IgnoreMissingProperties)
        this.Context.Diagnostics.Error(span, text);
      return false;
    }
  }
}
