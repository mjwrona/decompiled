// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.EdmNavigationPropertyInfo
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using System.Collections.Generic;

namespace Microsoft.OData.Edm
{
  public sealed class EdmNavigationPropertyInfo
  {
    public string Name { get; set; }

    public IEdmEntityType Target { get; set; }

    public EdmMultiplicity TargetMultiplicity { get; set; }

    public IEnumerable<IEdmStructuralProperty> DependentProperties { get; set; }

    public IEnumerable<IEdmStructuralProperty> PrincipalProperties { get; set; }

    public bool ContainsTarget { get; set; }

    public EdmOnDeleteAction OnDelete { get; set; }

    public EdmNavigationPropertyInfo Clone() => new EdmNavigationPropertyInfo()
    {
      Name = this.Name,
      Target = this.Target,
      TargetMultiplicity = this.TargetMultiplicity,
      DependentProperties = this.DependentProperties,
      PrincipalProperties = this.PrincipalProperties,
      ContainsTarget = this.ContainsTarget,
      OnDelete = this.OnDelete
    };
  }
}
