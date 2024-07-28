// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Builder.StructuralTypeConfiguration
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.AspNet.OData.Formatter;
using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace Microsoft.AspNet.OData.Builder
{
  public abstract class StructuralTypeConfiguration : IEdmTypeConfiguration
  {
    private string _namespace;
    private string _name;
    private PropertyInfo _dynamicPropertyDictionary;
    private StructuralTypeConfiguration _baseType;
    private bool _baseTypeConfigured;
    private HashSet<string> explicitPropertyNames = new HashSet<string>();

    protected StructuralTypeConfiguration()
    {
      this.ExplicitProperties = (IDictionary<PropertyInfo, PropertyConfiguration>) new Dictionary<PropertyInfo, PropertyConfiguration>();
      this.RemovedProperties = (IList<PropertyInfo>) new List<PropertyInfo>();
      this.QueryConfiguration = new QueryConfiguration();
    }

    protected StructuralTypeConfiguration(ODataModelBuilder modelBuilder, Type clrType)
      : this()
    {
      if (modelBuilder == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (modelBuilder));
      this.ClrType = !(clrType == (Type) null) ? clrType : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (clrType));
      this.ModelBuilder = modelBuilder;
      this._name = clrType.EdmName();
      this._namespace = modelBuilder.HasAssignedNamespace ? modelBuilder.Namespace : clrType.Namespace ?? modelBuilder.Namespace;
    }

    public abstract EdmTypeKind Kind { get; }

    public virtual Type ClrType { get; private set; }

    public virtual string FullName => this.Namespace + "." + this.Name;

    public virtual string Namespace
    {
      get => this._namespace;
      set
      {
        this._namespace = value != null ? value : throw Microsoft.AspNet.OData.Common.Error.PropertyNull();
        this.AddedExplicitly = true;
      }
    }

    public virtual string Name
    {
      get => this._name;
      set
      {
        this._name = value != null ? value : throw Microsoft.AspNet.OData.Common.Error.PropertyNull();
        this.AddedExplicitly = true;
      }
    }

    public bool IsOpen => this._dynamicPropertyDictionary != (PropertyInfo) null;

    public PropertyInfo DynamicPropertyDictionary => this._dynamicPropertyDictionary;

    public virtual bool? IsAbstract { get; set; }

    public virtual bool BaseTypeConfigured => this._baseTypeConfigured;

    public IEnumerable<PropertyConfiguration> Properties => (IEnumerable<PropertyConfiguration>) this.ExplicitProperties.Values;

    protected internal bool HasProperty(string propertyName) => this.explicitPropertyNames.Contains(propertyName);

    public ReadOnlyCollection<PropertyInfo> IgnoredProperties => new ReadOnlyCollection<PropertyInfo>(this.RemovedProperties);

    public virtual IEnumerable<NavigationPropertyConfiguration> NavigationProperties => this.ExplicitProperties.Values.OfType<NavigationPropertyConfiguration>();

    public QueryConfiguration QueryConfiguration { get; set; }

    public bool AddedExplicitly { get; set; }

    public virtual ODataModelBuilder ModelBuilder { get; private set; }

    protected internal IList<PropertyInfo> RemovedProperties { get; private set; }

    protected internal IDictionary<PropertyInfo, PropertyConfiguration> ExplicitProperties { get; private set; }

    protected internal virtual StructuralTypeConfiguration BaseTypeInternal => this._baseType;

    internal virtual void AbstractImpl() => this.IsAbstract = new bool?(true);

    internal virtual void DerivesFromNothingImpl()
    {
      this._baseType = (StructuralTypeConfiguration) null;
      this._baseTypeConfigured = true;
    }

    internal virtual void DerivesFromImpl(StructuralTypeConfiguration baseType)
    {
      this._baseType = baseType != null ? baseType : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (baseType));
      this._baseTypeConfigured = true;
      if (!baseType.ClrType.IsAssignableFrom(this.ClrType) || baseType.ClrType == this.ClrType)
        throw Microsoft.AspNet.OData.Common.Error.Argument(nameof (baseType), SRResources.TypeDoesNotInheritFromBaseType, (object) this.ClrType.FullName, (object) baseType.ClrType.FullName);
      foreach (PropertyConfiguration property in this.Properties)
        this.ValidatePropertyNotAlreadyDefinedInBaseTypes(property.PropertyInfo);
      foreach (PropertyConfiguration derivedProperty in this.DerivedProperties())
        this.ValidatePropertyNotAlreadyDefinedInDerivedTypes(derivedProperty.PropertyInfo);
    }

    public virtual PrimitivePropertyConfiguration AddProperty(PropertyInfo propertyInfo)
    {
      if (propertyInfo == (PropertyInfo) null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (propertyInfo));
      if (!TypeHelper.GetReflectedType((MemberInfo) propertyInfo).IsAssignableFrom(this.ClrType))
        throw Microsoft.AspNet.OData.Common.Error.Argument(nameof (propertyInfo), SRResources.PropertyDoesNotBelongToType, (object) propertyInfo.Name, (object) this.ClrType.FullName);
      this.ValidatePropertyNotAlreadyDefinedInBaseTypes(propertyInfo);
      this.ValidatePropertyNotAlreadyDefinedInDerivedTypes(propertyInfo);
      if (this.RemovedProperties.Any<PropertyInfo>((Func<PropertyInfo, bool>) (prop => prop.Name.Equals(propertyInfo.Name))))
        this.RemovedProperties.Remove(this.RemovedProperties.First<PropertyInfo>((Func<PropertyInfo, bool>) (prop => prop.Name.Equals(propertyInfo.Name))));
      PrimitivePropertyConfiguration propertyConfiguration = this.ValidatePropertyNotAlreadyDefinedOtherTypes<PrimitivePropertyConfiguration>(propertyInfo, SRResources.MustBePrimitiveProperty);
      if (propertyConfiguration == null)
      {
        propertyConfiguration = new PrimitivePropertyConfiguration(propertyInfo, this);
        IEdmPrimitiveType primitiveTypeOrNull = EdmLibHelpers.GetEdmPrimitiveTypeOrNull(propertyInfo.PropertyType);
        if (primitiveTypeOrNull != null)
        {
          if (primitiveTypeOrNull.PrimitiveKind == EdmPrimitiveTypeKind.Decimal)
            propertyConfiguration = (PrimitivePropertyConfiguration) new DecimalPropertyConfiguration(propertyInfo, this);
          else if (EdmLibHelpers.HasLength(primitiveTypeOrNull.PrimitiveKind))
            propertyConfiguration = (PrimitivePropertyConfiguration) new LengthPropertyConfiguration(propertyInfo, this);
          else if (EdmLibHelpers.HasPrecision(primitiveTypeOrNull.PrimitiveKind))
            propertyConfiguration = (PrimitivePropertyConfiguration) new PrecisionPropertyConfiguration(propertyInfo, this);
        }
        this.AddExplicitProperty(propertyInfo, (PropertyConfiguration) propertyConfiguration);
      }
      return propertyConfiguration;
    }

    private void AddExplicitProperty(
      PropertyInfo propertyInfo,
      PropertyConfiguration propertyConfiguration)
    {
      this.ExplicitProperties[propertyInfo] = propertyConfiguration;
      this.explicitPropertyNames.Add(propertyConfiguration.Name);
    }

    private void RemoveExplicitProperty(PropertyInfo propertyInfo)
    {
      this.ExplicitProperties.Remove(this.ExplicitProperties.Keys.First<PropertyInfo>((Func<PropertyInfo, bool>) (key => key.Name.Equals(propertyInfo.Name))));
      this.explicitPropertyNames.Remove(propertyInfo.Name);
    }

    public virtual EnumPropertyConfiguration AddEnumProperty(PropertyInfo propertyInfo)
    {
      if (propertyInfo == (PropertyInfo) null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (propertyInfo));
      if (!TypeHelper.GetReflectedType((MemberInfo) propertyInfo).IsAssignableFrom(this.ClrType))
        throw Microsoft.AspNet.OData.Common.Error.Argument(nameof (propertyInfo), SRResources.PropertyDoesNotBelongToType, (object) propertyInfo.Name, (object) this.ClrType.FullName);
      if (!TypeHelper.IsEnum(propertyInfo.PropertyType))
        throw Microsoft.AspNet.OData.Common.Error.Argument(nameof (propertyInfo), SRResources.MustBeEnumProperty, (object) propertyInfo.Name, (object) this.ClrType.FullName);
      this.ValidatePropertyNotAlreadyDefinedInBaseTypes(propertyInfo);
      this.ValidatePropertyNotAlreadyDefinedInDerivedTypes(propertyInfo);
      if (this.RemovedProperties.Any<PropertyInfo>((Func<PropertyInfo, bool>) (prop => prop.Name.Equals(propertyInfo.Name))))
        this.RemovedProperties.Remove(this.RemovedProperties.First<PropertyInfo>((Func<PropertyInfo, bool>) (prop => prop.Name.Equals(propertyInfo.Name))));
      EnumPropertyConfiguration propertyConfiguration = this.ValidatePropertyNotAlreadyDefinedOtherTypes<EnumPropertyConfiguration>(propertyInfo, SRResources.MustBeEnumProperty);
      if (propertyConfiguration == null)
      {
        propertyConfiguration = new EnumPropertyConfiguration(propertyInfo, this);
        this.AddExplicitProperty(propertyInfo, (PropertyConfiguration) propertyConfiguration);
      }
      return propertyConfiguration;
    }

    public virtual ComplexPropertyConfiguration AddComplexProperty(PropertyInfo propertyInfo)
    {
      if (propertyInfo == (PropertyInfo) null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (propertyInfo));
      if (!TypeHelper.GetReflectedType((MemberInfo) propertyInfo).IsAssignableFrom(this.ClrType))
        throw Microsoft.AspNet.OData.Common.Error.Argument(nameof (propertyInfo), SRResources.PropertyDoesNotBelongToType, (object) propertyInfo.Name, (object) this.ClrType.FullName);
      this.ValidatePropertyNotAlreadyDefinedInBaseTypes(propertyInfo);
      this.ValidatePropertyNotAlreadyDefinedInDerivedTypes(propertyInfo);
      if (this.RemovedProperties.Any<PropertyInfo>((Func<PropertyInfo, bool>) (prop => prop.Name.Equals(propertyInfo.Name))))
        this.RemovedProperties.Remove(this.RemovedProperties.First<PropertyInfo>((Func<PropertyInfo, bool>) (prop => prop.Name.Equals(propertyInfo.Name))));
      ComplexPropertyConfiguration propertyConfiguration = this.ValidatePropertyNotAlreadyDefinedOtherTypes<ComplexPropertyConfiguration>(propertyInfo, SRResources.MustBeComplexProperty);
      if (propertyConfiguration == null)
      {
        propertyConfiguration = new ComplexPropertyConfiguration(propertyInfo, this);
        this.AddExplicitProperty(propertyInfo, (PropertyConfiguration) propertyConfiguration);
        this.ModelBuilder.AddComplexType(propertyInfo.PropertyType);
      }
      return propertyConfiguration;
    }

    public virtual CollectionPropertyConfiguration AddCollectionProperty(PropertyInfo propertyInfo)
    {
      if (propertyInfo == (PropertyInfo) null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (propertyInfo));
      if (!propertyInfo.DeclaringType.IsAssignableFrom(this.ClrType))
        throw Microsoft.AspNet.OData.Common.Error.Argument(nameof (propertyInfo), SRResources.PropertyDoesNotBelongToType);
      this.ValidatePropertyNotAlreadyDefinedInBaseTypes(propertyInfo);
      this.ValidatePropertyNotAlreadyDefinedInDerivedTypes(propertyInfo);
      if (this.RemovedProperties.Any<PropertyInfo>((Func<PropertyInfo, bool>) (prop => prop.Name.Equals(propertyInfo.Name))))
        this.RemovedProperties.Remove(this.RemovedProperties.First<PropertyInfo>((Func<PropertyInfo, bool>) (prop => prop.Name.Equals(propertyInfo.Name))));
      CollectionPropertyConfiguration propertyConfiguration = this.ValidatePropertyNotAlreadyDefinedOtherTypes<CollectionPropertyConfiguration>(propertyInfo, SRResources.MustBeCollectionProperty);
      if (propertyConfiguration == null)
      {
        propertyConfiguration = new CollectionPropertyConfiguration(propertyInfo, this);
        this.AddExplicitProperty(propertyInfo, (PropertyConfiguration) propertyConfiguration);
        if (EdmLibHelpers.GetEdmPrimitiveTypeReferenceOrNull(propertyConfiguration.ElementType) == null && !TypeHelper.IsEnum(propertyConfiguration.ElementType))
          this.ModelBuilder.AddComplexType(propertyConfiguration.ElementType);
      }
      return propertyConfiguration;
    }

    public virtual void AddDynamicPropertyDictionary(PropertyInfo propertyInfo)
    {
      if (propertyInfo == (PropertyInfo) null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (propertyInfo));
      if (!typeof (IDictionary<string, object>).IsAssignableFrom(propertyInfo.PropertyType))
        throw Microsoft.AspNet.OData.Common.Error.Argument(nameof (propertyInfo), SRResources.ArgumentMustBeOfType, (object) "IDictionary<string, object>");
      if (!propertyInfo.DeclaringType.IsAssignableFrom(this.ClrType))
        throw Microsoft.AspNet.OData.Common.Error.Argument(nameof (propertyInfo), SRResources.PropertyDoesNotBelongToType);
      if (this.IgnoredProperties.Contains(propertyInfo))
        this.RemovedProperties.Remove(propertyInfo);
      this._dynamicPropertyDictionary = !(this._dynamicPropertyDictionary != (PropertyInfo) null) ? propertyInfo : throw Microsoft.AspNet.OData.Common.Error.Argument(nameof (propertyInfo), SRResources.MoreThanOneDynamicPropertyContainerFound, (object) this.ClrType.Name);
    }

    public virtual void RemoveProperty(PropertyInfo propertyInfo)
    {
      if (propertyInfo == (PropertyInfo) null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (propertyInfo));
      if (!TypeHelper.GetReflectedType((MemberInfo) propertyInfo).IsAssignableFrom(this.ClrType))
        throw Microsoft.AspNet.OData.Common.Error.Argument(nameof (propertyInfo), SRResources.PropertyDoesNotBelongToType, (object) propertyInfo.Name, (object) this.ClrType.FullName);
      if (this.ExplicitProperties.Keys.Any<PropertyInfo>((Func<PropertyInfo, bool>) (key => key.Name.Equals(propertyInfo.Name))))
        this.RemoveExplicitProperty(propertyInfo);
      if (!this.RemovedProperties.Any<PropertyInfo>((Func<PropertyInfo, bool>) (prop => prop.Name.Equals(propertyInfo.Name))))
        this.RemovedProperties.Add(propertyInfo);
      if (!(this._dynamicPropertyDictionary == propertyInfo))
        return;
      this._dynamicPropertyDictionary = (PropertyInfo) null;
    }

    public virtual NavigationPropertyConfiguration AddNavigationProperty(
      PropertyInfo navigationProperty,
      EdmMultiplicity multiplicity)
    {
      return this.AddNavigationProperty(navigationProperty, multiplicity, false);
    }

    public virtual NavigationPropertyConfiguration AddContainedNavigationProperty(
      PropertyInfo navigationProperty,
      EdmMultiplicity multiplicity)
    {
      return this.AddNavigationProperty(navigationProperty, multiplicity, true);
    }

    private NavigationPropertyConfiguration AddNavigationProperty(
      PropertyInfo navigationProperty,
      EdmMultiplicity multiplicity,
      bool containsTarget)
    {
      if (navigationProperty == (PropertyInfo) null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (navigationProperty));
      if (!TypeHelper.GetReflectedType((MemberInfo) navigationProperty).IsAssignableFrom(this.ClrType))
        throw Microsoft.AspNet.OData.Common.Error.Argument(nameof (navigationProperty), SRResources.PropertyDoesNotBelongToType, (object) navigationProperty.Name, (object) this.ClrType.FullName);
      this.ValidatePropertyNotAlreadyDefinedInBaseTypes(navigationProperty);
      this.ValidatePropertyNotAlreadyDefinedInDerivedTypes(navigationProperty);
      NavigationPropertyConfiguration propertyConfiguration;
      if (this.ExplicitProperties.ContainsKey(navigationProperty))
      {
        PropertyConfiguration explicitProperty = this.ExplicitProperties[navigationProperty];
        propertyConfiguration = explicitProperty.Kind == PropertyKind.Navigation ? explicitProperty as NavigationPropertyConfiguration : throw Microsoft.AspNet.OData.Common.Error.Argument(nameof (navigationProperty), SRResources.MustBeNavigationProperty, (object) navigationProperty.Name, (object) this.ClrType.FullName);
        if (propertyConfiguration.Multiplicity != multiplicity)
          throw Microsoft.AspNet.OData.Common.Error.Argument(nameof (navigationProperty), SRResources.MustHaveMatchingMultiplicity, (object) navigationProperty.Name, (object) multiplicity);
      }
      else
      {
        propertyConfiguration = new NavigationPropertyConfiguration(navigationProperty, multiplicity, this);
        if (containsTarget)
          propertyConfiguration = propertyConfiguration.Contained();
        this.AddExplicitProperty(navigationProperty, (PropertyConfiguration) propertyConfiguration);
        this.ModelBuilder.AddEntityType(propertyConfiguration.RelatedClrType);
      }
      return propertyConfiguration;
    }

    internal T ValidatePropertyNotAlreadyDefinedOtherTypes<T>(
      PropertyInfo propertyInfo,
      string typeErrorMessage)
      where T : class
    {
      obj = default (T);
      PropertyInfo key1 = this.ExplicitProperties.Keys.FirstOrDefault<PropertyInfo>((Func<PropertyInfo, bool>) (key => key.Name.Equals(propertyInfo.Name)));
      if (key1 != (PropertyInfo) null && !(this.ExplicitProperties[key1] is T obj))
        throw Microsoft.AspNet.OData.Common.Error.Argument(nameof (propertyInfo), typeErrorMessage, (object) propertyInfo.Name, (object) this.ClrType.FullName);
      return obj;
    }

    internal void ValidatePropertyNotAlreadyDefinedInBaseTypes(PropertyInfo propertyInfo)
    {
      PropertyConfiguration propertyConfiguration = this.DerivedProperties().FirstOrDefault<PropertyConfiguration>((Func<PropertyConfiguration, bool>) (p => p.Name == propertyInfo.Name));
      if (propertyConfiguration != null)
        throw Microsoft.AspNet.OData.Common.Error.Argument(nameof (propertyInfo), SRResources.CannotRedefineBaseTypeProperty, (object) propertyInfo.Name, (object) TypeHelper.GetReflectedType((MemberInfo) propertyConfiguration.PropertyInfo).FullName);
    }

    internal void ValidatePropertyNotAlreadyDefinedInDerivedTypes(PropertyInfo propertyInfo)
    {
      foreach (StructuralTypeConfiguration derivedType in this.ModelBuilder.DerivedTypes(this))
      {
        if (derivedType.HasProperty(propertyInfo.Name))
          throw Microsoft.AspNet.OData.Common.Error.Argument(nameof (propertyInfo), SRResources.PropertyAlreadyDefinedInDerivedType, (object) propertyInfo.Name, (object) this.FullName, (object) derivedType.FullName);
      }
    }
  }
}
