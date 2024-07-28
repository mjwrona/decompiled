// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.PropertyRenameAndIgnoreContractResolver
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Reflection;


#nullable enable
namespace Microsoft.TeamFoundation.Framework.Server
{
  public class PropertyRenameAndIgnoreContractResolver : DefaultContractResolver
  {
    public readonly 
    #nullable disable
    Dictionary<Type, HashSet<string>> IgnoredProperties;
    public readonly Dictionary<Type, Dictionary<string, string>> RenamedProperties;

    public PropertyRenameAndIgnoreContractResolver()
    {
      this.IgnoredProperties = new Dictionary<Type, HashSet<string>>();
      this.RenamedProperties = new Dictionary<Type, Dictionary<string, string>>();
    }

    protected override JsonProperty CreateProperty(
      MemberInfo member,
      MemberSerialization memberSerialization)
    {
      JsonProperty property = base.CreateProperty(member, memberSerialization);
      if (this.IsIgnored(property.DeclaringType, property.PropertyName))
      {
        property.ShouldSerialize = (Predicate<object>) (_ => false);
        property.Ignored = true;
      }
      string newPropertyName;
      if (this.IsRenamed(property.DeclaringType, property.PropertyName, out newPropertyName))
        property.PropertyName = newPropertyName;
      return property;
    }

    private bool IsIgnored(Type type, string propertyName)
    {
      HashSet<string> stringSet;
      return this.IgnoredProperties.TryGetValue(type, out stringSet) && stringSet.Contains(propertyName);
    }

    private bool IsRenamed(Type type, string propertyName, out string newPropertyName)
    {
      newPropertyName = (string) null;
      Dictionary<string, string> dictionary;
      return this.RenamedProperties.TryGetValue(type, out dictionary) && dictionary.TryGetValue(propertyName, out newPropertyName);
    }
  }
}
