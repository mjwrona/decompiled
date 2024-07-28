// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities.PropertySelectorsHelper
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities
{
  public static class PropertySelectorsHelper
  {
    private static readonly ConcurrentDictionary<Type, Dictionary<PropertyInfo, string>> Type2ProperyInfo2SerializedName = new ConcurrentDictionary<Type, Dictionary<PropertyInfo, string>>();
    private static string releaseManagementClientAssembly = typeof (ReleaseDefinitionEnvironment).Assembly.FullName;

    public static void HandlePropertySelection(
      object targetValue,
      IList<PropertySelector> propertySelectors)
    {
      if (propertySelectors == null || propertySelectors.Count == 0)
        return;
      foreach (PropertySelector propertySelector in (IEnumerable<PropertySelector>) propertySelectors)
      {
        if (PropertySelectorsHelper.IsObjectAnIEnumerable(targetValue))
          PropertySelectorsHelper.SelectTargetProperties((IEnumerable) targetValue, propertySelector.SelectorType, propertySelector.Properties);
        else
          PropertySelectorsHelper.SelectTargetProperties((IEnumerable) new List<object>()
          {
            targetValue
          }, propertySelector.SelectorType, propertySelector.Properties);
      }
    }

    public static void RemoveNullFromLists(object value)
    {
      if (value == null)
        return;
      foreach (PropertyInfo propertyInfo in ((IEnumerable<PropertyInfo>) value.GetType().GetProperties()).Where<PropertyInfo>((Func<PropertyInfo, bool>) (p => p.GetIndexParameters().Length == 0)).ToArray<PropertyInfo>())
      {
        if (typeof (IEnumerable).IsAssignableFrom(propertyInfo.PropertyType) && propertyInfo.GetType() != typeof (string))
        {
          IList list = propertyInfo.GetValue(value, (object[]) null) as IList;
          if (propertyInfo.GetValue(value, (object[]) null) is IDictionary<string, ConfigurationVariableValue> dictionary)
          {
            List<string> stringList = new List<string>();
            foreach (KeyValuePair<string, ConfigurationVariableValue> keyValuePair in (IEnumerable<KeyValuePair<string, ConfigurationVariableValue>>) dictionary)
            {
              if (keyValuePair.Key != null && keyValuePair.Value == null)
                stringList.Add(keyValuePair.Key);
            }
            foreach (string key in stringList)
              dictionary.Remove(key);
          }
          if (list != null)
          {
            for (int index = list.Count - 1; index >= 0; --index)
            {
              if (list[index] == null)
                list.RemoveAt(index);
              else if (PropertySelectorsHelper.IsValidReleaseManagementType(list[index].GetType()))
                PropertySelectorsHelper.RemoveNullFromLists(list[index]);
            }
          }
        }
        else if (PropertySelectorsHelper.IsValidReleaseManagementType(propertyInfo.PropertyType))
        {
          object obj = propertyInfo.GetValue(value, (object[]) null);
          if (obj != null)
            PropertySelectorsHelper.RemoveNullFromLists(obj);
        }
      }
    }

    private static bool IsValidReleaseManagementType(Type type) => !type.IsEnum && type.Assembly.FullName.Equals(PropertySelectorsHelper.releaseManagementClientAssembly, StringComparison.OrdinalIgnoreCase);

    private static bool IsObjectAnIEnumerable(object obj)
    {
      switch (obj)
      {
        case null:
        case string _:
          return false;
        default:
          return obj.GetType().GetInterface(typeof (IEnumerable).Name) != (Type) null;
      }
    }

    private static void SelectTargetProperties(
      IEnumerable objects,
      PropertySelectorType selectorType,
      IList<string> targetPropertyNames)
    {
      if (targetPropertyNames == null)
        return;
      Type elementType = PropertySelectorsHelper.GetElementType(objects);
      if (elementType == (Type) null)
        return;
      Dictionary<PropertyInfo, string> propertyInfoDictionary = PropertySelectorsHelper.GetPropertyInfoDictionary(elementType);
      HashSet<string> stringSet = new HashSet<string>();
      Dictionary<string, HashSet<string>> dictionary = new Dictionary<string, HashSet<string>>();
      foreach (string targetPropertyName in (IEnumerable<string>) targetPropertyNames)
      {
        if (!targetPropertyName.Contains("."))
        {
          stringSet.Add(targetPropertyName);
        }
        else
        {
          int length = targetPropertyName.IndexOf(".", StringComparison.OrdinalIgnoreCase);
          string key = targetPropertyName.Substring(0, length);
          if (selectorType == PropertySelectorType.Inclusion)
            stringSet.Add(key);
          if (!dictionary.ContainsKey(key))
            dictionary[key] = new HashSet<string>();
          string str = targetPropertyName.Substring(length + 1);
          dictionary[key].Add(str);
        }
      }
      foreach (PropertyInfo key in propertyInfoDictionary.Keys)
      {
        if (stringSet.Contains(propertyInfoDictionary[key]))
        {
          if (selectorType != PropertySelectorType.Inclusion)
            PropertySelectorsHelper.SetObjectPropertyValueToDefault(objects, key);
        }
        else if (selectorType == PropertySelectorType.Inclusion)
          PropertySelectorsHelper.SetObjectPropertyValueToDefault(objects, key);
      }
      foreach (string key1 in dictionary.Keys)
      {
        string propertyNameForNextLevelObject = key1;
        PropertyInfo key2 = propertyInfoDictionary.SingleOrDefault<KeyValuePair<PropertyInfo, string>>((Func<KeyValuePair<PropertyInfo, string>, bool>) (p => p.Value == propertyNameForNextLevelObject)).Key;
        if (!(key2 == (PropertyInfo) null))
        {
          List<object> objects1 = new List<object>();
          foreach (object obj1 in objects)
          {
            object obj2 = key2.GetValue(obj1);
            if (PropertySelectorsHelper.IsObjectAnIEnumerable(obj2))
            {
              foreach (object obj3 in (IEnumerable) obj2)
                objects1.Add(obj3);
            }
            else
              objects1.Add(obj2);
          }
          PropertySelectorsHelper.SelectTargetProperties((IEnumerable) objects1, selectorType, (IList<string>) dictionary[propertyNameForNextLevelObject].ToList<string>());
        }
      }
    }

    private static Dictionary<PropertyInfo, string> GetPropertyInfoDictionary(Type elementType) => PropertySelectorsHelper.Type2ProperyInfo2SerializedName.GetOrAdd(elementType, PropertySelectorsHelper.ExtractPropertiesFromType(elementType));

    private static Dictionary<PropertyInfo, string> ExtractPropertiesFromType(Type elementType)
    {
      Dictionary<PropertyInfo, string> propertiesFromType = new Dictionary<PropertyInfo, string>();
      foreach (PropertyInfo property in elementType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
      {
        if (((IEnumerable<object>) property.GetCustomAttributes(true)).FirstOrDefault<object>((Func<object, bool>) (attr => attr is DataMemberAttribute)) is DataMemberAttribute dataMemberAttribute)
          propertiesFromType.Add(property, dataMemberAttribute == null || dataMemberAttribute.Name == null ? property.Name : dataMemberAttribute.Name);
      }
      return propertiesFromType;
    }

    private static Type GetElementType(IEnumerable objects)
    {
      foreach (object obj in objects)
      {
        if (obj != null)
          return obj.GetType();
      }
      return (Type) null;
    }

    private static void SetObjectPropertyValueToDefault(
      IEnumerable objects,
      PropertyInfo propertyInfo)
    {
      Type propertyType = propertyInfo.PropertyType;
      object instance = propertyType.IsValueType ? Activator.CreateInstance(propertyType) : (object) null;
      foreach (object obj in objects)
      {
        if (obj != null && propertyInfo.CanWrite)
          propertyInfo.SetValue(obj, instance);
      }
    }
  }
}
