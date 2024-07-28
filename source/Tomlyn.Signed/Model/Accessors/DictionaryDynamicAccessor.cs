// Decompiled with JetBrains decompiler
// Type: Tomlyn.Model.Accessors.DictionaryDynamicAccessor
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
  internal class DictionaryDynamicAccessor : ObjectDynamicAccessor
  {
    private readonly DictionaryDynamicAccessor.DictionaryAccessor _dictionaryAccessor;

    public DictionaryDynamicAccessor(
      DynamicModelReadContext context,
      Type type,
      Type keyType,
      Type valueType)
      : base(context, type, ReflectionObjectKind.Dictionary)
    {
      if (this.TargetType == typeof (TomlTable))
        this._dictionaryAccessor = DictionaryDynamicAccessor.TomlTableAccessor.Instance;
      else if (keyType == typeof (string) && valueType == typeof (object))
        this._dictionaryAccessor = DictionaryDynamicAccessor.FastDictionaryAccessor.Instance;
      else
        this._dictionaryAccessor = (DictionaryDynamicAccessor.DictionaryAccessor) new DictionaryDynamicAccessor.SlowDictionaryAccessor(context, this.TargetType, keyType, valueType);
    }

    public override IEnumerable<KeyValuePair<string, object?>> GetProperties(object obj) => this._dictionaryAccessor.GetElements(obj);

    public override bool TryGetPropertyValue(
      SourceSpan span,
      object obj,
      string name,
      out object? value)
    {
      value = (object) null;
      object outputValue;
      if (this.Context.TryConvertValue(span, (object) name, this._dictionaryAccessor.KeyType, out outputValue))
        return this._dictionaryAccessor.TryGetValue(obj, outputValue, out value);
      this.Context.Diagnostics.Error(span, "Cannot convert string key " + name + " to dictionary key " + this._dictionaryAccessor.KeyType.FullName + " on object type " + this.TargetType.FullName);
      return false;
    }

    public override bool TrySetPropertyValue(
      SourceSpan span,
      object obj,
      string name,
      object? value)
    {
      string text;
      try
      {
        object outputValue1;
        if (this.Context.TryConvertValue(span, (object) name, this._dictionaryAccessor.KeyType, out outputValue1))
        {
          object outputValue2;
          if (this.Context.TryConvertValue(span, value, this._dictionaryAccessor.ValueType, out outputValue2))
          {
            this._dictionaryAccessor.SetKeyValue(obj, outputValue1, outputValue2);
            return true;
          }
          text = "Cannot convert string key " + name + " to dictionary key " + this._dictionaryAccessor.KeyType.FullName + " on object type " + this.TargetType.FullName;
        }
        else
          text = "Cannot convert string key " + name + " to dictionary key " + this._dictionaryAccessor.KeyType.FullName + " on object type " + this.TargetType.FullName;
      }
      catch (Exception ex)
      {
        text = "Unexpected error while trying to set property " + name + " was not found on object type " + this.TargetType.FullName + ". Reason: " + ex.Message;
      }
      this.Context.Diagnostics.Error(span, text);
      return false;
    }

    public override bool TryGetPropertyType(SourceSpan span, string name, out Type? propertyType)
    {
      propertyType = this._dictionaryAccessor.ValueType;
      return true;
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
        instance = this.Context.CreateInstance(this._dictionaryAccessor.ValueType, kind);
        object outputValue;
        if (instance != null && this.Context.TryConvertValue(span, (object) name, this._dictionaryAccessor.KeyType, out outputValue))
        {
          this._dictionaryAccessor.SetKeyValue(obj, outputValue, instance);
          return true;
        }
        text = "Unable to set the property " + name + " on object type " + this.TargetType.FullName + ".";
      }
      catch (Exception ex)
      {
        text = "Unexpected error when creating object for property " + name + " on object type " + this.TargetType.FullName + ". Reason: " + ex.Message;
      }
      if (!this.Context.IgnoreMissingProperties)
        this.Context.Diagnostics.Error(span, text);
      return false;
    }

    private abstract class DictionaryAccessor
    {
      public DictionaryAccessor(Type keyType, Type valueType)
      {
        this.KeyType = keyType;
        this.ValueType = valueType;
      }

      public Type KeyType { get; }

      public Type ValueType { get; }

      public abstract IEnumerable<KeyValuePair<string, object?>> GetElements(object dictionary);

      public abstract bool TryGetValue(object dictionary, object key, out object? value);

      public abstract void SetKeyValue(object dictionary, object key, object value);
    }

    private class TomlTableAccessor : DictionaryDynamicAccessor.DictionaryAccessor
    {
      public static readonly DictionaryDynamicAccessor.DictionaryAccessor Instance = (DictionaryDynamicAccessor.DictionaryAccessor) new DictionaryDynamicAccessor.TomlTableAccessor();

      private TomlTableAccessor()
        : base(typeof (string), typeof (object))
      {
      }

      public override IEnumerable<KeyValuePair<string, object?>> GetElements(object dictionary) => (IEnumerable<KeyValuePair<string, object>>) dictionary;

      public override bool TryGetValue(object dictionary, object key, out object? value) => ((TomlTable) dictionary).TryGetValue((string) key, out value);

      public override void SetKeyValue(object dictionary, object key, object value) => ((TomlTable) dictionary)[(string) key] = value;
    }

    private sealed class FastDictionaryAccessor : DictionaryDynamicAccessor.DictionaryAccessor
    {
      public static readonly DictionaryDynamicAccessor.DictionaryAccessor Instance = (DictionaryDynamicAccessor.DictionaryAccessor) new DictionaryDynamicAccessor.FastDictionaryAccessor();

      private FastDictionaryAccessor()
        : base(typeof (string), typeof (object))
      {
      }

      public override IEnumerable<KeyValuePair<string, object?>> GetElements(object dictionary) => (IEnumerable<KeyValuePair<string, object>>) dictionary;

      public override bool TryGetValue(object dictionary, object key, out object? value) => ((IDictionary<string, object>) dictionary).TryGetValue((string) key, out value);

      public override void SetKeyValue(object dictionary, object key, object value) => ((IDictionary<string, object>) dictionary)[(string) key] = value;
    }

    private sealed class SlowDictionaryAccessor : DictionaryDynamicAccessor.DictionaryAccessor
    {
      private readonly DynamicModelReadContext _context;
      private readonly PropertyInfo _propSetter;
      private readonly MethodInfo _methodTryGetValue;

      public SlowDictionaryAccessor(
        DynamicModelReadContext context,
        Type dictionaryType,
        Type keyType,
        Type valueType)
        : base(keyType, valueType)
      {
        this._context = context;
        PropertyInfo propertyInfo = (PropertyInfo) null;
        foreach (PropertyInfo property in dictionaryType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy))
        {
          ParameterInfo[] indexParameters = property.GetIndexParameters();
          if (indexParameters.Length == 1 && indexParameters[0].ParameterType == this.KeyType && property.PropertyType == valueType)
          {
            propertyInfo = property;
            break;
          }
        }
        this._propSetter = propertyInfo;
        MethodInfo methodInfo = (MethodInfo) null;
        Type type = valueType.MakeByRefType();
        foreach (MethodInfo method in dictionaryType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy))
        {
          ParameterInfo[] parameters = method.GetParameters();
          if (method.Name == "TryGetValue" && method.ReturnType == typeof (bool) && parameters.Length == 2 && parameters[0].ParameterType == this.KeyType && parameters[1].IsOut && parameters[1].ParameterType == type)
          {
            methodInfo = method;
            break;
          }
        }
        this._methodTryGetValue = methodInfo;
      }

      public override IEnumerable<KeyValuePair<string, object?>> GetElements(object dictionary)
      {
        IDictionaryEnumerator enumerator = (IDictionaryEnumerator) ((IEnumerable) dictionary).GetEnumerator();
        while (enumerator.MoveNext())
        {
          object outputValue;
          if (!this._context.TryConvertValue(new SourceSpan(), enumerator.Key, typeof (string), out outputValue) || !(outputValue is string key))
          {
            this._context.Diagnostics.Error(new SourceSpan(), string.Format("Unable to convert key {0} to a string", enumerator.Key));
            break;
          }
          yield return new KeyValuePair<string, object>(key, enumerator.Value);
        }
      }

      public override bool TryGetValue(object dictionary, object key, out object? value)
      {
        object[] parameters = new object[2]{ key, null };
        int num = (bool) this._methodTryGetValue.Invoke(dictionary, parameters) ? 1 : 0;
        value = parameters[1];
        return num != 0;
      }

      public override void SetKeyValue(object dictionary, object key, object value) => this._propSetter.SetValue(dictionary, value, new object[1]
      {
        key
      });
    }
  }
}
