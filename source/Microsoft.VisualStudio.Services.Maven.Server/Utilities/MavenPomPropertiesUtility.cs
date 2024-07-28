// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Utilities.MavenPomPropertiesUtility
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.VisualStudio.Services.Maven.WebApi.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Microsoft.VisualStudio.Services.Maven.Server.Utilities
{
  public static class MavenPomPropertiesUtility
  {
    public static MavenPomMetadata GetPatchedObject(this MavenPomMetadata metadata)
    {
      if (metadata == null)
        return (MavenPomMetadata) null;
      MavenPomMetadata metadata1 = (MavenPomMetadata) metadata.Clone();
      if (metadata1.Parent != null)
      {
        if (string.IsNullOrWhiteSpace(metadata1.GroupId))
          metadata1.GroupId = metadata1.Parent.GroupId;
        if (string.IsNullOrWhiteSpace(metadata1.Version))
          metadata1.Version = metadata1.Parent.Version;
      }
      Dictionary<string, string> allProperties = metadata1.GetAllProperties();
      foreach (PropertyInfo stringProperty in MavenPomPropertiesUtility.GetStringProperties<MavenPomMetadata>())
      {
        string str1 = stringProperty.GetValue((object) metadata1) as string;
        string str2 = MavenPomPropertiesUtility.ReplaceReference(str1, allProperties);
        if (str1 != str2)
          stringProperty.SetValue((object) metadata1, (object) str2);
      }
      metadata1.Properties.PatchDictionaryReferences(allProperties);
      metadata1.Prerequisites.PatchDictionaryReferences(allProperties);
      metadata1.Dependencies.PatchDependencyReferences<MavenPomDependency>(allProperties);
      MavenPomDependencyManagement dependencyManagement = metadata1.DependencyManagement;
      if (dependencyManagement != null)
        dependencyManagement.Dependencies.PatchDependencyReferences<MavenPomDependency>(allProperties);
      return metadata1;
    }

    private static Dictionary<string, string> GetAllProperties(this MavenPomMetadata metadata)
    {
      Dictionary<string, string> dictionary = metadata.ToPropertiesDictionary<MavenPomMetadata>().ToDictionary<KeyValuePair<string, string>, string, string>((Func<KeyValuePair<string, string>, string>) (x => "project." + x.Key), (Func<KeyValuePair<string, string>, string>) (x => x.Value));
      List<string> stringList = new List<string>((IEnumerable<string>) dictionary.Keys);
      foreach (KeyValuePair<string, string> property in metadata.Properties)
      {
        if (!dictionary.ContainsKey(property.Key))
          dictionary.Add(property.Key, MavenPomPropertiesUtility.ReplaceReference(property.Value, dictionary));
      }
      foreach (KeyValuePair<string, string> prerequisite in metadata.Prerequisites)
      {
        if (!dictionary.ContainsKey(prerequisite.Key))
          dictionary.Add(prerequisite.Key, MavenPomPropertiesUtility.ReplaceReference(prerequisite.Value, dictionary));
      }
      foreach (string key in stringList)
        dictionary[key] = MavenPomPropertiesUtility.ReplaceReference(dictionary[key], dictionary);
      return dictionary;
    }

    private static void PatchDictionaryReferences(
      this Dictionary<string, string> items,
      Dictionary<string, string> all)
    {
      if (items == null || !items.Any<KeyValuePair<string, string>>())
        return;
      foreach (string key in new List<string>((IEnumerable<string>) items.Keys))
        items[key] = MavenPomPropertiesUtility.ReplaceReference(items[key], all);
    }

    private static void PatchDependencyReferences<T>(
      this List<T> dependencies,
      Dictionary<string, string> properties)
      where T : MavenPomDependency
    {
      if (dependencies == null || properties == null || !properties.Any<KeyValuePair<string, string>>())
        return;
      List<PropertyInfo> stringProperties = MavenPomPropertiesUtility.GetStringProperties<T>();
      foreach (T dependency in dependencies)
      {
        foreach (PropertyInfo propertyInfo in stringProperties)
          propertyInfo.SetValue((object) dependency, (object) MavenPomPropertiesUtility.ReplaceReference((string) propertyInfo.GetValue((object) dependency), properties));
      }
    }

    private static string ReplaceReference(string value, Dictionary<string, string> properties)
    {
      if (!string.IsNullOrWhiteSpace(value) && properties != null && properties.Any<KeyValuePair<string, string>>())
      {
        foreach (KeyValuePair<string, string> property in properties)
        {
          string match = "${" + property.Key + "}";
          if (property.Value != null && (value.ToLowerInvariant().Contains(match) || value.Equals(match, StringComparison.OrdinalIgnoreCase)))
            value = value.ReplaceCaseInsensitive(match, property.Value);
        }
      }
      return value;
    }

    private static string ReplaceCaseInsensitive(
      this string main,
      string match,
      string replace,
      StringComparison defaultComparison = StringComparison.OrdinalIgnoreCase)
    {
      string str = main;
      int startIndex1;
      for (int startIndex2 = str.IndexOf(match, defaultComparison); startIndex2 > -1; startIndex2 = str.IndexOf(match, startIndex1, defaultComparison))
      {
        str = str.Remove(startIndex2, match.Length).Insert(startIndex2, replace);
        startIndex1 = startIndex2 + replace.Length;
      }
      return str;
    }

    private static Dictionary<string, string> ToPropertiesDictionary<T>(this T data) => MavenPomPropertiesUtility.GetStringProperties<T>().ToDictionary<PropertyInfo, string, string>((Func<PropertyInfo, string>) (prop => prop.Name.ToLowerInvariant()), (Func<PropertyInfo, string>) (prop => (string) prop.GetValue((object) (T) data)));

    private static List<PropertyInfo> GetStringProperties<T>() => ((IEnumerable<PropertyInfo>) typeof (T).GetProperties()).Where<PropertyInfo>((Func<PropertyInfo, bool>) (x => x.PropertyType == typeof (string))).ToList<PropertyInfo>();
  }
}
