// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.EdmStructuredType
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using System;
using System.Collections.Generic;

namespace Microsoft.OData.Edm
{
  public abstract class EdmStructuredType : EdmType, IEdmStructuredType, IEdmType, IEdmElement
  {
    private readonly IEdmStructuredType baseStructuredType;
    private readonly List<IEdmProperty> declaredProperties = new List<IEdmProperty>();
    private readonly bool isAbstract;
    private readonly bool isOpen;
    private readonly Cache<EdmStructuredType, IDictionary<string, IEdmProperty>> propertiesDictionary = new Cache<EdmStructuredType, IDictionary<string, IEdmProperty>>();
    private static readonly Func<EdmStructuredType, IDictionary<string, IEdmProperty>> ComputePropertiesDictionaryFunc = (Func<EdmStructuredType, IDictionary<string, IEdmProperty>>) (me => me.ComputePropertiesDictionary());

    protected EdmStructuredType(
      bool isAbstract,
      bool isOpen,
      IEdmStructuredType baseStructuredType)
    {
      this.isAbstract = isAbstract;
      this.isOpen = isOpen;
      this.baseStructuredType = baseStructuredType;
    }

    public bool IsAbstract => this.isAbstract;

    public bool IsOpen => this.isOpen;

    public virtual IEnumerable<IEdmProperty> DeclaredProperties => (IEnumerable<IEdmProperty>) this.declaredProperties;

    public IEdmStructuredType BaseType => this.baseStructuredType;

    protected IDictionary<string, IEdmProperty> PropertiesDictionary => this.propertiesDictionary.GetValue(this, EdmStructuredType.ComputePropertiesDictionaryFunc, (Func<EdmStructuredType, IDictionary<string, IEdmProperty>>) null);

    public void AddProperty(IEdmProperty property)
    {
      EdmUtil.CheckArgumentNull<IEdmProperty>(property, nameof (property));
      if (this != property.DeclaringType)
        throw new InvalidOperationException(Strings.EdmModel_Validator_Semantic_DeclaringTypeMustBeCorrect((object) property.Name));
      this.declaredProperties.Add(property);
      this.propertiesDictionary.Clear((Func<EdmStructuredType, IDictionary<string, IEdmProperty>>) null);
    }

    public EdmStructuralProperty AddStructuralProperty(string name, EdmPrimitiveTypeKind type)
    {
      EdmStructuralProperty property = new EdmStructuralProperty((IEdmStructuredType) this, name, (IEdmTypeReference) EdmCoreModel.Instance.GetPrimitive(type, true));
      this.AddProperty((IEdmProperty) property);
      return property;
    }

    public EdmStructuralProperty AddStructuralProperty(
      string name,
      EdmPrimitiveTypeKind type,
      bool isNullable)
    {
      EdmStructuralProperty property = new EdmStructuralProperty((IEdmStructuredType) this, name, (IEdmTypeReference) EdmCoreModel.Instance.GetPrimitive(type, isNullable));
      this.AddProperty((IEdmProperty) property);
      return property;
    }

    public EdmStructuralProperty AddStructuralProperty(string name, IEdmTypeReference type)
    {
      EdmStructuralProperty property = new EdmStructuralProperty((IEdmStructuredType) this, name, type);
      this.AddProperty((IEdmProperty) property);
      return property;
    }

    public EdmStructuralProperty AddStructuralProperty(
      string name,
      IEdmTypeReference type,
      string defaultValue)
    {
      EdmStructuralProperty property = new EdmStructuralProperty((IEdmStructuredType) this, name, type, defaultValue);
      this.AddProperty((IEdmProperty) property);
      return property;
    }

    public EdmNavigationProperty AddUnidirectionalNavigation(EdmNavigationPropertyInfo propertyInfo)
    {
      EdmUtil.CheckArgumentNull<EdmNavigationPropertyInfo>(propertyInfo, nameof (propertyInfo));
      EdmNavigationProperty navigationProperty = EdmNavigationProperty.CreateNavigationProperty((IEdmStructuredType) this, propertyInfo);
      this.AddProperty((IEdmProperty) navigationProperty);
      return navigationProperty;
    }

    public IEdmProperty FindProperty(string name)
    {
      IEdmProperty edmProperty;
      return !this.PropertiesDictionary.TryGetValue(name, out edmProperty) ? (IEdmProperty) null : edmProperty;
    }

    private IDictionary<string, IEdmProperty> ComputePropertiesDictionary()
    {
      Dictionary<string, IEdmProperty> dictionary = new Dictionary<string, IEdmProperty>();
      foreach (IEdmProperty property in this.Properties())
        RegistrationHelper.RegisterProperty(property, property.Name, dictionary);
      return (IDictionary<string, IEdmProperty>) dictionary;
    }
  }
}
