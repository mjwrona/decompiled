// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Builder.EntityTypeConfiguration
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Microsoft.AspNet.OData.Builder
{
  public class EntityTypeConfiguration : StructuralTypeConfiguration
  {
    private List<PrimitivePropertyConfiguration> _keys = new List<PrimitivePropertyConfiguration>();
    private List<EnumPropertyConfiguration> _enumKeys = new List<EnumPropertyConfiguration>();

    public EntityTypeConfiguration()
    {
    }

    public EntityTypeConfiguration(ODataModelBuilder modelBuilder, Type clrType)
      : base(modelBuilder, clrType)
    {
    }

    public override EdmTypeKind Kind => EdmTypeKind.Entity;

    public virtual bool HasStream { get; set; }

    public virtual IEnumerable<PrimitivePropertyConfiguration> Keys => (IEnumerable<PrimitivePropertyConfiguration>) this._keys;

    public virtual IEnumerable<EnumPropertyConfiguration> EnumKeys => (IEnumerable<EnumPropertyConfiguration>) this._enumKeys;

    public virtual EntityTypeConfiguration BaseType
    {
      get => this.BaseTypeInternal as EntityTypeConfiguration;
      set => this.DerivesFrom(value);
    }

    public virtual EntityTypeConfiguration Abstract()
    {
      this.AbstractImpl();
      return this;
    }

    public virtual EntityTypeConfiguration MediaType()
    {
      this.HasStream = true;
      return this;
    }

    public virtual EntityTypeConfiguration HasKey(PropertyInfo keyProperty)
    {
      if (this.BaseType != null && this.BaseType.Keys().Any<PropertyConfiguration>())
        throw Microsoft.AspNet.OData.Common.Error.InvalidOperation(SRResources.CannotDefineKeysOnDerivedTypes, (object) this.FullName, (object) this.BaseType.FullName);
      if (TypeHelper.IsEnum(keyProperty.PropertyType))
      {
        this.ModelBuilder.AddEnumType(keyProperty.PropertyType);
        EnumPropertyConfiguration propertyConfiguration = this.AddEnumProperty(keyProperty);
        propertyConfiguration.IsRequired();
        if (!this._enumKeys.Contains(propertyConfiguration))
          this._enumKeys.Add(propertyConfiguration);
      }
      else
      {
        PrimitivePropertyConfiguration propertyConfiguration = this.AddProperty(keyProperty);
        propertyConfiguration.IsRequired();
        if (!this._keys.Contains(propertyConfiguration))
          this._keys.Add(propertyConfiguration);
      }
      return this;
    }

    public virtual void RemoveKey(PrimitivePropertyConfiguration keyProperty)
    {
      if (keyProperty == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (keyProperty));
      this._keys.Remove(keyProperty);
    }

    public virtual void RemoveKey(EnumPropertyConfiguration enumKeyProperty)
    {
      if (enumKeyProperty == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (enumKeyProperty));
      this._enumKeys.Remove(enumKeyProperty);
    }

    public virtual EntityTypeConfiguration DerivesFromNothing()
    {
      this.DerivesFromNothingImpl();
      return this;
    }

    public virtual EntityTypeConfiguration DerivesFrom(EntityTypeConfiguration baseType)
    {
      if ((this.Keys.Any<PrimitivePropertyConfiguration>() || this.EnumKeys.Any<EnumPropertyConfiguration>()) && baseType.Keys().Any<PropertyConfiguration>())
        throw Microsoft.AspNet.OData.Common.Error.InvalidOperation(SRResources.CannotDefineKeysOnDerivedTypes, (object) this.FullName, (object) baseType.FullName);
      this.DerivesFromImpl((StructuralTypeConfiguration) baseType);
      return this;
    }

    public override void RemoveProperty(PropertyInfo propertyInfo)
    {
      base.RemoveProperty(propertyInfo);
      this._keys.RemoveAll((Predicate<PrimitivePropertyConfiguration>) (p => p.PropertyInfo == propertyInfo));
      this._enumKeys.RemoveAll((Predicate<EnumPropertyConfiguration>) (p => p.PropertyInfo == propertyInfo));
    }
  }
}
