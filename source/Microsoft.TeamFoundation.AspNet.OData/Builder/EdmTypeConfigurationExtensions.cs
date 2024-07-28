// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Builder.EdmTypeConfigurationExtensions
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.AspNet.OData.Builder
{
  internal static class EdmTypeConfigurationExtensions
  {
    public static IEnumerable<PropertyConfiguration> DerivedProperties(
      this StructuralTypeConfiguration structuralType)
    {
      if (structuralType == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (structuralType));
      if (structuralType.Kind == EdmTypeKind.Entity)
        return EdmTypeConfigurationExtensions.DerivedProperties((EntityTypeConfiguration) structuralType);
      return structuralType.Kind == EdmTypeKind.Complex ? EdmTypeConfigurationExtensions.DerivedProperties((ComplexTypeConfiguration) structuralType) : Enumerable.Empty<PropertyConfiguration>();
    }

    public static IEnumerable<PropertyConfiguration> DerivedProperties(
      this EntityTypeConfiguration entity)
    {
      if (entity == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (entity));
      for (EntityTypeConfiguration baseType = entity.BaseType; baseType != null; baseType = baseType.BaseType)
      {
        foreach (PropertyConfiguration property in baseType.Properties)
          yield return property;
      }
    }

    public static IEnumerable<PropertyConfiguration> DerivedProperties(
      this ComplexTypeConfiguration complex)
    {
      if (complex == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (complex));
      for (ComplexTypeConfiguration baseType = complex.BaseType; baseType != null; baseType = baseType.BaseType)
      {
        foreach (PropertyConfiguration property in baseType.Properties)
          yield return property;
      }
    }

    public static IEnumerable<PropertyConfiguration> Keys(this EntityTypeConfiguration entity)
    {
      if (entity.Keys.Any<PrimitivePropertyConfiguration>() || entity.EnumKeys.Any<EnumPropertyConfiguration>())
        return entity.Keys.OfType<PropertyConfiguration>().Concat<PropertyConfiguration>((IEnumerable<PropertyConfiguration>) entity.EnumKeys);
      return entity.BaseType == null ? Enumerable.Empty<PropertyConfiguration>() : entity.BaseType.Keys();
    }

    public static IEnumerable<StructuralTypeConfiguration> ThisAndBaseTypes(
      this StructuralTypeConfiguration structuralType)
    {
      return structuralType.BaseTypes().Concat<StructuralTypeConfiguration>((IEnumerable<StructuralTypeConfiguration>) new StructuralTypeConfiguration[1]
      {
        structuralType
      });
    }

    public static IEnumerable<StructuralTypeConfiguration> ThisAndBaseAndDerivedTypes(
      this ODataModelBuilder modelBuilder,
      StructuralTypeConfiguration structuralType)
    {
      return structuralType.BaseTypes().Concat<StructuralTypeConfiguration>((IEnumerable<StructuralTypeConfiguration>) new StructuralTypeConfiguration[1]
      {
        structuralType
      }).Concat<StructuralTypeConfiguration>(modelBuilder.DerivedTypes(structuralType));
    }

    public static IEnumerable<StructuralTypeConfiguration> BaseTypes(
      this StructuralTypeConfiguration structuralType)
    {
      if (structuralType.Kind == EdmTypeKind.Entity)
      {
        EntityTypeConfiguration entity = (EntityTypeConfiguration) structuralType;
        for (entity = entity.BaseType; entity != null; entity = entity.BaseType)
          yield return (StructuralTypeConfiguration) entity;
        entity = (EntityTypeConfiguration) null;
      }
      if (structuralType.Kind == EdmTypeKind.Complex)
      {
        ComplexTypeConfiguration complex = (ComplexTypeConfiguration) structuralType;
        for (complex = complex.BaseType; complex != null; complex = complex.BaseType)
          yield return (StructuralTypeConfiguration) complex;
        complex = (ComplexTypeConfiguration) null;
      }
    }

    public static IEnumerable<StructuralTypeConfiguration> DerivedTypes(
      this ODataModelBuilder modelBuilder,
      StructuralTypeConfiguration structuralType)
    {
      if (modelBuilder == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (modelBuilder));
      if (structuralType == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (structuralType));
      if (structuralType.Kind == EdmTypeKind.Entity)
        return (IEnumerable<StructuralTypeConfiguration>) EdmTypeConfigurationExtensions.DerivedTypes(modelBuilder, (EntityTypeConfiguration) structuralType);
      return structuralType.Kind == EdmTypeKind.Complex ? (IEnumerable<StructuralTypeConfiguration>) EdmTypeConfigurationExtensions.DerivedTypes(modelBuilder, (ComplexTypeConfiguration) structuralType) : Enumerable.Empty<StructuralTypeConfiguration>();
    }

    public static IEnumerable<EntityTypeConfiguration> DerivedTypes(
      this ODataModelBuilder modelBuilder,
      EntityTypeConfiguration entity)
    {
      if (modelBuilder == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (modelBuilder));
      if (entity == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (entity));
      foreach (EntityTypeConfiguration derivedType1 in modelBuilder.StructuralTypes.OfType<EntityTypeConfiguration>().Where<EntityTypeConfiguration>((Func<EntityTypeConfiguration, bool>) (e => e.BaseType == entity)))
      {
        yield return derivedType1;
        foreach (EntityTypeConfiguration derivedType2 in EdmTypeConfigurationExtensions.DerivedTypes(modelBuilder, derivedType1))
          yield return derivedType2;
      }
    }

    public static IEnumerable<ComplexTypeConfiguration> DerivedTypes(
      this ODataModelBuilder modelBuilder,
      ComplexTypeConfiguration complex)
    {
      if (modelBuilder == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (modelBuilder));
      if (complex == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (complex));
      foreach (ComplexTypeConfiguration derivedType1 in modelBuilder.StructuralTypes.OfType<ComplexTypeConfiguration>().Where<ComplexTypeConfiguration>((Func<ComplexTypeConfiguration, bool>) (e => e.BaseType == complex)))
      {
        yield return derivedType1;
        foreach (ComplexTypeConfiguration derivedType2 in EdmTypeConfigurationExtensions.DerivedTypes(modelBuilder, derivedType1))
          yield return derivedType2;
      }
    }

    public static bool IsAssignableFrom(
      this StructuralTypeConfiguration baseStructuralType,
      StructuralTypeConfiguration structuralType)
    {
      if (structuralType.Kind == EdmTypeKind.Entity && baseStructuralType.Kind == EdmTypeKind.Entity)
      {
        for (EntityTypeConfiguration typeConfiguration = (EntityTypeConfiguration) structuralType; typeConfiguration != null; typeConfiguration = typeConfiguration.BaseType)
        {
          if (baseStructuralType == typeConfiguration)
            return true;
        }
      }
      else if (structuralType.Kind == EdmTypeKind.Complex && baseStructuralType.Kind == EdmTypeKind.Complex)
      {
        for (ComplexTypeConfiguration typeConfiguration = (ComplexTypeConfiguration) structuralType; typeConfiguration != null; typeConfiguration = typeConfiguration.BaseType)
        {
          if (baseStructuralType == typeConfiguration)
            return true;
        }
      }
      return false;
    }
  }
}
