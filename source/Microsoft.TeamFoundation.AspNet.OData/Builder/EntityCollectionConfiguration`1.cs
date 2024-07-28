// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Builder.EntityCollectionConfiguration`1
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using System.Collections.Generic;

namespace Microsoft.AspNet.OData.Builder
{
  public class EntityCollectionConfiguration<TEntityType> : CollectionTypeConfiguration
  {
    internal EntityCollectionConfiguration(EntityTypeConfiguration elementType)
      : base((IEdmTypeConfiguration) elementType, typeof (IEnumerable<TEntityType>))
    {
    }

    public ActionConfiguration Action(string name)
    {
      ActionConfiguration actionConfiguration = this.ModelBuilder.Action(name);
      actionConfiguration.SetBindingParameter("bindingParameter", (IEdmTypeConfiguration) this);
      return actionConfiguration;
    }

    public FunctionConfiguration Function(string name)
    {
      FunctionConfiguration functionConfiguration = this.ModelBuilder.Function(name);
      functionConfiguration.SetBindingParameter("bindingParameter", (IEdmTypeConfiguration) this);
      return functionConfiguration;
    }
  }
}
