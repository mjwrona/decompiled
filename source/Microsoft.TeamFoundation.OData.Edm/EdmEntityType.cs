// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.EdmEntityType
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Vocabularies;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.OData.Edm
{
  public class EdmEntityType : 
    EdmStructuredType,
    IEdmEntityType,
    IEdmStructuredType,
    IEdmType,
    IEdmElement,
    IEdmSchemaType,
    IEdmSchemaElement,
    IEdmNamedElement,
    IEdmVocabularyAnnotatable,
    IEdmFullNamedElement
  {
    private readonly string namespaceName;
    private readonly string name;
    private readonly string fullName;
    private readonly bool hasStream;
    private List<IEdmStructuralProperty> declaredKey;

    public EdmEntityType(string namespaceName, string name)
      : this(namespaceName, name, (IEdmEntityType) null, false, false)
    {
    }

    public EdmEntityType(string namespaceName, string name, IEdmEntityType baseType)
      : this(namespaceName, name, baseType, false, false)
    {
    }

    public EdmEntityType(
      string namespaceName,
      string name,
      IEdmEntityType baseType,
      bool isAbstract,
      bool isOpen)
      : this(namespaceName, name, baseType, isAbstract, isOpen, false)
    {
    }

    public EdmEntityType(
      string namespaceName,
      string name,
      IEdmEntityType baseType,
      bool isAbstract,
      bool isOpen,
      bool hasStream)
      : base(isAbstract, isOpen, (IEdmStructuredType) baseType)
    {
      EdmUtil.CheckArgumentNull<string>(namespaceName, nameof (namespaceName));
      EdmUtil.CheckArgumentNull<string>(name, nameof (name));
      this.namespaceName = namespaceName;
      this.name = name;
      this.hasStream = hasStream;
      this.fullName = EdmUtil.GetFullNameForSchemaElement(this.namespaceName, this.Name);
    }

    public virtual IEnumerable<IEdmStructuralProperty> DeclaredKey => (IEnumerable<IEdmStructuralProperty>) this.declaredKey;

    public EdmSchemaElementKind SchemaElementKind => EdmSchemaElementKind.TypeDefinition;

    public string Namespace => this.namespaceName;

    public string Name => this.name;

    public string FullName => this.fullName;

    public override EdmTypeKind TypeKind => EdmTypeKind.Entity;

    public bool HasStream
    {
      get
      {
        if (this.hasStream)
          return true;
        return this.BaseType != null && this.BaseEntityType().HasStream;
      }
    }

    public void AddKeys(params IEdmStructuralProperty[] keyProperties) => this.AddKeys((IEnumerable<IEdmStructuralProperty>) keyProperties);

    public void AddKeys(IEnumerable<IEdmStructuralProperty> keyProperties)
    {
      EdmUtil.CheckArgumentNull<IEnumerable<IEdmStructuralProperty>>(keyProperties, nameof (keyProperties));
      foreach (IEdmStructuralProperty keyProperty in keyProperties)
      {
        if (this.declaredKey == null)
          this.declaredKey = new List<IEdmStructuralProperty>();
        this.declaredKey.Add(keyProperty);
      }
    }

    public EdmNavigationProperty AddBidirectionalNavigation(
      EdmNavigationPropertyInfo propertyInfo,
      EdmNavigationPropertyInfo partnerInfo)
    {
      EdmUtil.CheckArgumentNull<EdmNavigationPropertyInfo>(propertyInfo, nameof (propertyInfo));
      EdmUtil.CheckArgumentNull<IEdmEntityType>(propertyInfo.Target, "propertyInfo.Target");
      if (!(propertyInfo.Target is EdmEntityType target))
        throw new ArgumentException("propertyInfo.Target", Strings.Constructable_TargetMustBeStock((object) typeof (EdmEntityType).FullName));
      EdmNavigationProperty propertyWithPartner = EdmNavigationProperty.CreateNavigationPropertyWithPartner(propertyInfo, this.FixUpDefaultPartnerInfo(propertyInfo, partnerInfo));
      this.AddProperty((IEdmProperty) propertyWithPartner);
      target.AddProperty((IEdmProperty) propertyWithPartner.Partner);
      return propertyWithPartner;
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    public void SetNavigationPropertyPartner(
      EdmNavigationProperty navigationProperty,
      IEdmPathExpression navigationPropertyPath,
      EdmNavigationProperty partnerNavigationProperty,
      IEdmPathExpression partnerNavigationPropertyPath)
    {
      navigationProperty.SetPartner((IEdmNavigationProperty) partnerNavigationProperty, partnerNavigationPropertyPath);
      if (!(partnerNavigationProperty.DeclaringType is IEdmEntityType))
        return;
      partnerNavigationProperty.SetPartner((IEdmNavigationProperty) navigationProperty, navigationPropertyPath);
    }

    private EdmNavigationPropertyInfo FixUpDefaultPartnerInfo(
      EdmNavigationPropertyInfo propertyInfo,
      EdmNavigationPropertyInfo partnerInfo)
    {
      EdmNavigationPropertyInfo navigationPropertyInfo = (EdmNavigationPropertyInfo) null;
      if (partnerInfo == null)
        partnerInfo = navigationPropertyInfo = new EdmNavigationPropertyInfo();
      if (partnerInfo.Name == null)
      {
        if (navigationPropertyInfo == null)
          navigationPropertyInfo = partnerInfo.Clone();
        navigationPropertyInfo.Name = (propertyInfo.Name ?? string.Empty) + "Partner";
      }
      if (partnerInfo.Target == null)
      {
        if (navigationPropertyInfo == null)
          navigationPropertyInfo = partnerInfo.Clone();
        navigationPropertyInfo.Target = (IEdmEntityType) this;
      }
      if (partnerInfo.TargetMultiplicity == EdmMultiplicity.Unknown)
      {
        if (navigationPropertyInfo == null)
          navigationPropertyInfo = partnerInfo.Clone();
        navigationPropertyInfo.TargetMultiplicity = EdmMultiplicity.ZeroOrOne;
      }
      return navigationPropertyInfo ?? partnerInfo;
    }
  }
}
