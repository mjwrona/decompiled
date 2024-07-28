// Decompiled with JetBrains decompiler
// Type: Tomlyn.TomlModelOptions
// Assembly: Tomlyn.Signed, Version=0.16.0.0, Culture=neutral, PublicKeyToken=925c35bbd70fa682
// MVID: 8155B7FF-9444-4ADB-B8A6-687FA556DA39
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Tomlyn.Signed.dll

using System;
using System.Collections.Generic;
using System.Reflection;
using Tomlyn.Helpers;
using Tomlyn.Model;


#nullable enable
namespace Tomlyn
{
  public class TomlModelOptions
  {
    public static readonly Func<Type, ObjectKind, object> DefaultCreateInstance = new Func<Type, ObjectKind, object>(TomlModelOptions.DefaultCreateInstanceImpl);
    public static readonly Func<string, string> DefaultConvertPropertyName = new Func<string, string>(TomlNamingHelper.PascalToSnakeCase);
    public static readonly Func<string, string> DefaultConvertFieldName = new Func<string, string>(TomlNamingHelper.PascalToSnakeCase);

    public TomlModelOptions()
    {
      this.GetPropertyName = new Func<PropertyInfo, string>(this.DefaultGetPropertyNameImpl);
      this.GetFieldName = new Func<FieldInfo, string>(this.DefaultGetFieldNameImpl);
      this.CreateInstance = TomlModelOptions.DefaultCreateInstance;
      this.ConvertPropertyName = TomlModelOptions.DefaultConvertPropertyName;
      this.ConvertFieldName = TomlModelOptions.DefaultConvertFieldName;
      this.IgnoreMissingProperties = false;
      this.IncludeFields = false;
      this.AttributeListForIgnore = new List<string>()
      {
        "System.Runtime.Serialization.IgnoreDataMemberAttribute",
        "System.Text.Json.Serialization.JsonIgnoreAttribute"
      };
      this.AttributeListForGetName = new List<string>()
      {
        "System.Runtime.Serialization.DataMemberAttribute",
        "System.Text.Json.Serialization.JsonPropertyNameAttribute"
      };
    }

    public Func<PropertyInfo, string?> GetPropertyName { get; set; }

    public Func<FieldInfo, string?> GetFieldName { get; set; }

    public Func<string, string> ConvertPropertyName { get; set; }

    public Func<string, string> ConvertFieldName { get; set; }

    public Func<Type, ObjectKind, object> CreateInstance { get; set; }

    [Obsolete("Use ConvertToModel instead")]
    public Func<object, Type, object?>? ConvertTo
    {
      get => this.ConvertToModel;
      set => this.ConvertToModel = value;
    }

    public Func<object, Type, object?>? ConvertToModel { get; set; }

    public Func<object, object?>? ConvertToToml { get; set; }

    public List<string> AttributeListForIgnore { get; }

    public List<string> AttributeListForGetName { get; }

    public bool IgnoreMissingProperties { get; set; }

    public bool IncludeFields { get; set; }

    private string? DefaultGetPropertyNameImpl(PropertyInfo prop)
    {
      str2 = (string) null;
      foreach (Attribute customAttribute in prop.GetCustomAttributes())
      {
        string fullName = customAttribute.GetType().FullName;
        foreach (string str in this.AttributeListForIgnore)
        {
          if (fullName == str)
            return (string) null;
        }
        foreach (string str1 in this.AttributeListForGetName)
        {
          if (fullName == str1)
          {
            PropertyInfo property = customAttribute.GetType().GetProperty("Name");
            if (property != (PropertyInfo) null && property.PropertyType == typeof (string))
            {
              if (property.GetValue((object) customAttribute) is string str2)
                break;
            }
          }
        }
      }
      return this.ConvertPropertyName(str2 ?? prop.Name);
    }

    private string? DefaultGetFieldNameImpl(FieldInfo field)
    {
      str2 = (string) null;
      foreach (Attribute customAttribute in field.GetCustomAttributes())
      {
        string fullName = customAttribute.GetType().FullName;
        foreach (string str in this.AttributeListForIgnore)
        {
          if (fullName == str)
            return (string) null;
        }
        foreach (string str1 in this.AttributeListForGetName)
        {
          if (fullName == str1)
          {
            PropertyInfo property = customAttribute.GetType().GetProperty("Name");
            if (property != (PropertyInfo) null && property.PropertyType == typeof (string))
            {
              if (property.GetValue((object) customAttribute) is string str2)
                break;
            }
          }
        }
      }
      return this.ConvertFieldName(str2 ?? field.Name);
    }

    private static object DefaultCreateInstanceImpl(Type type, ObjectKind kind)
    {
      if (type == typeof (object))
      {
        switch (kind)
        {
          case ObjectKind.InlineTable:
          case ObjectKind.Table:
            return (object) new TomlTable(kind == ObjectKind.InlineTable);
          case ObjectKind.TableArray:
            return (object) new TomlTableArray();
          default:
            return (object) new TomlArray();
        }
      }
      else
        return Activator.CreateInstance(type) ?? throw new InvalidOperationException("Failed to create an instance of type '" + type.FullName + "'");
    }
  }
}
