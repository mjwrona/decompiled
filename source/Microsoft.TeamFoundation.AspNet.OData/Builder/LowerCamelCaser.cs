// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Builder.LowerCamelCaser
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using System;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace Microsoft.AspNet.OData.Builder
{
  public class LowerCamelCaser
  {
    private readonly NameResolverOptions _options;

    public LowerCamelCaser()
      : this(NameResolverOptions.ProcessReflectedPropertyNames | NameResolverOptions.ProcessDataMemberAttributePropertyNames | NameResolverOptions.ProcessExplicitPropertyNames)
    {
    }

    public LowerCamelCaser(NameResolverOptions options) => this._options = options;

    public void ApplyLowerCamelCase(ODataConventionModelBuilder builder)
    {
      foreach (StructuralTypeConfiguration structuralType in builder.StructuralTypes)
      {
        foreach (PropertyConfiguration property in structuralType.Properties)
        {
          if (this.ShouldApplyLowerCamelCase(property))
            property.Name = this.ToLowerCamelCase(property.Name);
        }
      }
    }

    public virtual string ToLowerCamelCase(string name)
    {
      if (string.IsNullOrEmpty(name) || !char.IsUpper(name[0]))
        return name;
      StringBuilder stringBuilder = new StringBuilder();
      for (int index = 0; index < name.Length; ++index)
      {
        if (index != 0 && index + 1 < name.Length && !char.IsUpper(name[index + 1]))
        {
          stringBuilder.Append(name.Substring(index));
          break;
        }
        stringBuilder.Append(char.ToLowerInvariant(name[index]));
      }
      return stringBuilder.ToString();
    }

    private bool ShouldApplyLowerCamelCase(PropertyConfiguration property)
    {
      if (property.AddedExplicitly)
        return this._options.HasFlag((Enum) NameResolverOptions.ProcessExplicitPropertyNames);
      DataMemberAttribute customAttribute = property.PropertyInfo.GetCustomAttribute<DataMemberAttribute>(false);
      return customAttribute != null && !string.IsNullOrWhiteSpace(customAttribute.Name) ? this._options.HasFlag((Enum) NameResolverOptions.ProcessDataMemberAttributePropertyNames) : this._options.HasFlag((Enum) NameResolverOptions.ProcessReflectedPropertyNames);
    }
  }
}
