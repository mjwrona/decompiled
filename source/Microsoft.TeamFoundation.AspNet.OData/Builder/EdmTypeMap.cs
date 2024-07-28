// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Builder.EdmTypeMap
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Query;
using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.AspNet.OData.Builder
{
  internal class EdmTypeMap
  {
    public EdmTypeMap(
      Dictionary<Type, IEdmType> edmTypes,
      Dictionary<PropertyInfo, IEdmProperty> edmProperties,
      Dictionary<IEdmProperty, QueryableRestrictions> edmPropertiesRestrictions,
      Dictionary<IEdmProperty, ModelBoundQuerySettings> edmPropertiesQuerySettings,
      Dictionary<IEdmStructuredType, ModelBoundQuerySettings> edmStructuredTypeQuerySettings,
      Dictionary<Enum, IEdmEnumMember> enumMembers,
      Dictionary<IEdmStructuredType, PropertyInfo> openTypes)
    {
      this.EdmTypes = edmTypes;
      this.EdmProperties = edmProperties;
      this.EdmPropertiesRestrictions = edmPropertiesRestrictions;
      this.EdmPropertiesQuerySettings = edmPropertiesQuerySettings;
      this.EdmStructuredTypeQuerySettings = edmStructuredTypeQuerySettings;
      this.EnumMembers = enumMembers;
      this.OpenTypes = openTypes;
    }

    public Dictionary<Type, IEdmType> EdmTypes { get; private set; }

    public Dictionary<PropertyInfo, IEdmProperty> EdmProperties { get; private set; }

    public Dictionary<IEdmProperty, QueryableRestrictions> EdmPropertiesRestrictions { get; private set; }

    public Dictionary<IEdmProperty, ModelBoundQuerySettings> EdmPropertiesQuerySettings { get; private set; }

    public Dictionary<IEdmStructuredType, ModelBoundQuerySettings> EdmStructuredTypeQuerySettings { get; private set; }

    public Dictionary<Enum, IEdmEnumMember> EnumMembers { get; private set; }

    public Dictionary<IEdmStructuredType, PropertyInfo> OpenTypes { get; private set; }
  }
}
