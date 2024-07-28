// Decompiled with JetBrains decompiler
// Type: Tomlyn.Model.Accessors.ListDynamicAccessor
// Assembly: Tomlyn.Signed, Version=0.16.0.0, Culture=neutral, PublicKeyToken=925c35bbd70fa682
// MVID: 8155B7FF-9444-4ADB-B8A6-687FA556DA39
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Tomlyn.Signed.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;


#nullable enable
namespace Tomlyn.Model.Accessors
{
  internal class ListDynamicAccessor : DynamicAccessor
  {
    private readonly PropertyInfo? _propIndexer;
    private readonly PropertyInfo? _propCount;
    private readonly MethodInfo? _addMethod;

    public ListDynamicAccessor(DynamicModelReadContext context, Type type, Type elementType)
      : base(context, type, ReflectionObjectKind.Collection)
    {
      this.ElementType = elementType;
      if (ListDynamicAccessor.IsFastType(type))
        return;
      int num = 0;
      foreach (PropertyInfo property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy))
      {
        ParameterInfo[] indexParameters = property.GetIndexParameters();
        if (property.Name == "Count" && indexParameters.Length == 0 && property.PropertyType == typeof (int))
        {
          this._propCount = property;
          num |= 1;
        }
        else if (indexParameters.Length == 1)
        {
          if (indexParameters[0].ParameterType == typeof (int) && property.PropertyType == elementType)
          {
            this._propIndexer = property;
            num |= 2;
          }
          if (num == 3)
            break;
        }
      }
      foreach (MethodInfo method in type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy))
      {
        if (method.Name == "Add" && method.GetParameters().Length == 1 && method.ReturnType == typeof (void) && method.GetParameters()[0].ParameterType == elementType)
        {
          this._addMethod = method;
          break;
        }
      }
    }

    private static bool IsFastType(Type type) => typeof (IList).IsAssignableFrom(type) || typeof (TomlArray).IsAssignableFrom(type) || typeof (TomlTableArray).IsAssignableFrom(type);

    public Type ElementType { get; }

    public IEnumerable<object?> GetElements(object obj)
    {
      switch (obj)
      {
        case IEnumerable<object> elements:
          return elements;
        case IEnumerable it:
          return ListDynamicAccessor.EnumToEnumObject(it);
        default:
          return this.EnumFromCollection(obj);
      }
    }

    private IEnumerable<object?> EnumFromCollection(object obj)
    {
      int count = this.GetCount(obj);
      for (int i = 0; i < count; ++i)
        yield return this.GetElementAt(obj, i);
    }

    private static IEnumerable<object?> EnumToEnumObject(IEnumerable it)
    {
      foreach (object obj in it)
        yield return obj;
    }

    public int GetCount(object list)
    {
      switch (list)
      {
        case ICollection collection:
          return collection.Count;
        case TomlArray tomlArray:
          return tomlArray.Count;
        case TomlTableArray tomlTableArray:
          return tomlTableArray.Count;
        default:
          return (int) this._propCount.GetValue(list);
      }
    }

    public object? GetElementAt(object list, int index)
    {
      switch (list)
      {
        case IList list1:
          return list1[index];
        case TomlArray tomlArray:
          return tomlArray[index];
        case TomlTableArray tomlTableArray:
          return (object) tomlTableArray[index];
        default:
          return this._propIndexer.GetValue(list, new object[1]
          {
            (object) index
          });
      }
    }

    public object? GetLastElement(object list)
    {
      switch (list)
      {
        case IList list1:
          return list1[list1.Count - 1];
        case TomlArray tomlArray:
          return tomlArray[tomlArray.Count - 1];
        case TomlTableArray tomlTableArray:
          return (object) tomlTableArray[tomlTableArray.Count - 1];
        default:
          return this.GetElementAt(list, this.GetCount(list) - 1);
      }
    }

    public void AddElement(object list, object? value)
    {
      switch (list)
      {
        case IList list1:
          list1.Add(value);
          break;
        case TomlArray tomlArray:
          tomlArray.Add(value);
          break;
        case TomlTableArray tomlTableArray:
          tomlTableArray.Add((TomlTable) value);
          break;
        default:
          this._addMethod.Invoke(list, new object[1]
          {
            value
          });
          break;
      }
    }
  }
}
