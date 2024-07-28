// Decompiled with JetBrains decompiler
// Type: Nest.PropertyMappingProvider
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.Serialization;

namespace Nest
{
  public class PropertyMappingProvider : IPropertyMappingProvider
  {
    protected readonly ConcurrentDictionary<string, IPropertyMapping> Properties = new ConcurrentDictionary<string, IPropertyMapping>();

    public virtual IPropertyMapping CreatePropertyMapping(MemberInfo memberInfo)
    {
      string key = memberInfo.DeclaringType?.FullName + "." + memberInfo.Name;
      IPropertyMapping propertyMapping;
      if (this.Properties.TryGetValue(key, out propertyMapping))
        return propertyMapping;
      propertyMapping = PropertyMappingProvider.PropertyMappingFromAttributes(memberInfo);
      this.Properties.TryAdd(key, propertyMapping);
      return propertyMapping;
    }

    private static IPropertyMapping PropertyMappingFromAttributes(MemberInfo memberInfo)
    {
      DataMemberAttribute customAttribute1 = memberInfo.GetCustomAttribute<DataMemberAttribute>(true);
      PropertyNameAttribute customAttribute2 = memberInfo.GetCustomAttribute<PropertyNameAttribute>(true);
      IgnoreAttribute customAttribute3 = memberInfo.GetCustomAttribute<IgnoreAttribute>(true);
      if (customAttribute3 == null && customAttribute2 == null && customAttribute1 == null)
        return (IPropertyMapping) null;
      return (IPropertyMapping) new PropertyMapping()
      {
        Name = (customAttribute2?.Name ?? customAttribute1?.Name),
        Ignore = (customAttribute3 != null || customAttribute2 != null && customAttribute2.Ignore)
      };
    }
  }
}
