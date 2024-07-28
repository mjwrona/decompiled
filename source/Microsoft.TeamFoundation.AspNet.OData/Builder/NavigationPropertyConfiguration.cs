// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Builder.NavigationPropertyConfiguration
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.AspNet.OData.Formatter;
using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Microsoft.AspNet.OData.Builder
{
  public class NavigationPropertyConfiguration : PropertyConfiguration
  {
    private readonly Type _relatedType;
    private readonly IDictionary<PropertyInfo, PropertyInfo> _referentialConstraint = (IDictionary<PropertyInfo, PropertyInfo>) new Dictionary<PropertyInfo, PropertyInfo>();

    public NavigationPropertyConfiguration(
      PropertyInfo property,
      EdmMultiplicity multiplicity,
      StructuralTypeConfiguration declaringType)
      : base(property, declaringType)
    {
      if (property == (PropertyInfo) null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (property));
      this.Multiplicity = multiplicity;
      this._relatedType = property.PropertyType;
      if (multiplicity == EdmMultiplicity.Many)
      {
        Type elementType;
        this._relatedType = TypeHelper.IsCollection(this._relatedType, out elementType) ? elementType : throw Microsoft.AspNet.OData.Common.Error.Argument(nameof (property), SRResources.ManyToManyNavigationPropertyMustReturnCollection, (object) property.Name, (object) TypeHelper.GetReflectedType((MemberInfo) property).Name);
      }
      this.OnDeleteAction = EdmOnDeleteAction.None;
    }

    public NavigationPropertyConfiguration Partner { get; internal set; }

    public EdmMultiplicity Multiplicity { get; private set; }

    public bool ContainsTarget { get; private set; }

    public override Type RelatedClrType => this._relatedType;

    public override PropertyKind Kind => PropertyKind.Navigation;

    public EdmOnDeleteAction OnDeleteAction { get; set; }

    public IEnumerable<PropertyInfo> DependentProperties => (IEnumerable<PropertyInfo>) this._referentialConstraint.Keys;

    public IEnumerable<PropertyInfo> PrincipalProperties => (IEnumerable<PropertyInfo>) this._referentialConstraint.Values;

    public NavigationPropertyConfiguration Optional()
    {
      this.Multiplicity = this.Multiplicity != EdmMultiplicity.Many ? EdmMultiplicity.ZeroOrOne : throw Microsoft.AspNet.OData.Common.Error.InvalidOperation(SRResources.ManyNavigationPropertiesCannotBeChanged, (object) this.Name);
      return this;
    }

    public NavigationPropertyConfiguration Required()
    {
      this.Multiplicity = this.Multiplicity != EdmMultiplicity.Many ? EdmMultiplicity.One : throw Microsoft.AspNet.OData.Common.Error.InvalidOperation(SRResources.ManyNavigationPropertiesCannotBeChanged, (object) this.Name);
      return this;
    }

    public NavigationPropertyConfiguration Contained()
    {
      this.ContainsTarget = true;
      return this;
    }

    public NavigationPropertyConfiguration NonContained()
    {
      this.ContainsTarget = false;
      return this;
    }

    public NavigationPropertyConfiguration AutomaticallyExpand(bool disableWhenSelectIsPresent)
    {
      this.AutoExpand = true;
      this.DisableAutoExpandWhenSelectIsPresent = disableWhenSelectIsPresent;
      return this;
    }

    public NavigationPropertyConfiguration CascadeOnDelete()
    {
      this.CascadeOnDelete(true);
      return this;
    }

    public NavigationPropertyConfiguration CascadeOnDelete(bool cascade)
    {
      this.OnDeleteAction = cascade ? EdmOnDeleteAction.Cascade : EdmOnDeleteAction.None;
      return this;
    }

    public NavigationPropertyConfiguration HasConstraint(
      PropertyInfo dependentPropertyInfo,
      PropertyInfo principalPropertyInfo)
    {
      return this.HasConstraint(new KeyValuePair<PropertyInfo, PropertyInfo>(dependentPropertyInfo, principalPropertyInfo));
    }

    public NavigationPropertyConfiguration HasConstraint(
      KeyValuePair<PropertyInfo, PropertyInfo> constraint)
    {
      if (constraint.Key == (PropertyInfo) null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull("dependentPropertyInfo");
      if (constraint.Value == (PropertyInfo) null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull("principalPropertyInfo");
      if (this.Multiplicity == EdmMultiplicity.Many)
        throw Microsoft.AspNet.OData.Common.Error.NotSupported(SRResources.ReferentialConstraintOnManyNavigationPropertyNotSupported, (object) this.Name, (object) this.DeclaringType.ClrType.FullName);
      if (this.ValidateConstraint(constraint))
        return this;
      PrimitivePropertyConfiguration propertyConfiguration1 = this.DeclaringType.ModelBuilder.StructuralTypes.OfType<EntityTypeConfiguration>().FirstOrDefault<EntityTypeConfiguration>((Func<EntityTypeConfiguration, bool>) (e => e.ClrType == this.RelatedClrType)).AddProperty(constraint.Value);
      PrimitivePropertyConfiguration propertyConfiguration2 = this.DeclaringType.AddProperty(constraint.Key);
      if (this.Multiplicity == EdmMultiplicity.ZeroOrOne || propertyConfiguration1.OptionalProperty)
        propertyConfiguration2.OptionalProperty = true;
      if (this.Multiplicity == EdmMultiplicity.One && !propertyConfiguration1.OptionalProperty)
        propertyConfiguration2.OptionalProperty = false;
      this._referentialConstraint.Add(constraint);
      return this;
    }

    private bool ValidateConstraint(
      KeyValuePair<PropertyInfo, PropertyInfo> constraint)
    {
      if (this._referentialConstraint.Contains(constraint))
        return true;
      PropertyInfo propertyInfo;
      if (this._referentialConstraint.TryGetValue(constraint.Key, out propertyInfo))
        throw Microsoft.AspNet.OData.Common.Error.InvalidOperation(SRResources.ReferentialConstraintAlreadyConfigured, (object) "dependent", (object) constraint.Key.Name, (object) "principal", (object) propertyInfo.Name);
      if (this.PrincipalProperties.Any<PropertyInfo>((Func<PropertyInfo, bool>) (p => p == constraint.Value)))
      {
        PropertyInfo key = this._referentialConstraint.First<KeyValuePair<PropertyInfo, PropertyInfo>>((Func<KeyValuePair<PropertyInfo, PropertyInfo>, bool>) (r => r.Value == constraint.Value)).Key;
        throw Microsoft.AspNet.OData.Common.Error.InvalidOperation(SRResources.ReferentialConstraintAlreadyConfigured, (object) "principal", (object) constraint.Value.Name, (object) "dependent", (object) key.Name);
      }
      Type type1 = Nullable.GetUnderlyingType(constraint.Key.PropertyType);
      if ((object) type1 == null)
        type1 = constraint.Key.PropertyType;
      Type type2 = Nullable.GetUnderlyingType(constraint.Value.PropertyType);
      if ((object) type2 == null)
        type2 = constraint.Value.PropertyType;
      if (type1 != type2)
        throw Microsoft.AspNet.OData.Common.Error.InvalidOperation(SRResources.DependentAndPrincipalTypeNotMatch, (object) constraint.Key.PropertyType.FullName, (object) constraint.Value.PropertyType.FullName);
      if (EdmLibHelpers.GetEdmPrimitiveTypeOrNull(constraint.Key.PropertyType) == null)
        throw Microsoft.AspNet.OData.Common.Error.InvalidOperation(SRResources.ReferentialConstraintPropertyTypeNotValid, (object) constraint.Key.PropertyType.FullName);
      return false;
    }
  }
}
