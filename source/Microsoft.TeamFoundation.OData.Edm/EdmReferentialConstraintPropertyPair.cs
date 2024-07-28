// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.EdmReferentialConstraintPropertyPair
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

namespace Microsoft.OData.Edm
{
  public class EdmReferentialConstraintPropertyPair
  {
    public EdmReferentialConstraintPropertyPair(
      IEdmStructuralProperty dependentProperty,
      IEdmStructuralProperty principalProperty)
    {
      EdmUtil.CheckArgumentNull<IEdmStructuralProperty>(dependentProperty, nameof (dependentProperty));
      EdmUtil.CheckArgumentNull<IEdmStructuralProperty>(principalProperty, nameof (principalProperty));
      this.DependentProperty = dependentProperty;
      this.PrincipalProperty = principalProperty;
    }

    public IEdmStructuralProperty DependentProperty { get; private set; }

    public IEdmStructuralProperty PrincipalProperty { get; private set; }
  }
}
