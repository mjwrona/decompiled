// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.EdmReferentialConstraint
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OData.Edm
{
  public class EdmReferentialConstraint : IEdmReferentialConstraint
  {
    private readonly IEnumerable<EdmReferentialConstraintPropertyPair> propertyPairs;

    public EdmReferentialConstraint(
      IEnumerable<EdmReferentialConstraintPropertyPair> propertyPairs)
    {
      EdmUtil.CheckArgumentNull<IEnumerable<EdmReferentialConstraintPropertyPair>>(propertyPairs, nameof (propertyPairs));
      this.propertyPairs = (IEnumerable<EdmReferentialConstraintPropertyPair>) propertyPairs.ToList<EdmReferentialConstraintPropertyPair>();
    }

    public IEnumerable<EdmReferentialConstraintPropertyPair> PropertyPairs => this.propertyPairs;

    public static EdmReferentialConstraint Create(
      IEnumerable<IEdmStructuralProperty> dependentProperties,
      IEnumerable<IEdmStructuralProperty> principalProperties)
    {
      EdmUtil.CheckArgumentNull<IEnumerable<IEdmStructuralProperty>>(dependentProperties, nameof (dependentProperties));
      EdmUtil.CheckArgumentNull<IEnumerable<IEdmStructuralProperty>>(principalProperties, nameof (principalProperties));
      List<IEdmStructuralProperty> source = new List<IEdmStructuralProperty>(dependentProperties);
      List<IEdmStructuralProperty> principalPropertyList = new List<IEdmStructuralProperty>(principalProperties);
      if (source.Count != principalPropertyList.Count)
        throw new ArgumentException(Strings.Constructable_DependentPropertyCountMustMatchNumberOfPropertiesOnPrincipalType((object) principalPropertyList.Count, (object) source.Count));
      return new EdmReferentialConstraint(source.Select<IEdmStructuralProperty, EdmReferentialConstraintPropertyPair>((Func<IEdmStructuralProperty, int, EdmReferentialConstraintPropertyPair>) ((d, i) => new EdmReferentialConstraintPropertyPair(d, principalPropertyList[i]))));
    }
  }
}
