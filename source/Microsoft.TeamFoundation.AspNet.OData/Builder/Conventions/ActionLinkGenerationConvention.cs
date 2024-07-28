// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Builder.Conventions.ActionLinkGenerationConvention
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.OData.Edm;
using System;

namespace Microsoft.AspNet.OData.Builder.Conventions
{
  internal class ActionLinkGenerationConvention : IOperationConvention, IConvention
  {
    public void Apply(OperationConfiguration configuration, ODataModelBuilder model)
    {
      ActionConfiguration action = configuration as ActionConfiguration;
      if (action == null || !action.IsBindable)
        return;
      if (action.BindingParameter.TypeConfiguration.Kind == EdmTypeKind.Entity && action.GetActionLink() == null)
      {
        if (action.BindingParameter.TypeConfiguration.Kind != EdmTypeKind.Entity || action.GetActionLink() != null)
          return;
        string bindingParameterType = action.BindingParameter.TypeConfiguration.FullName;
        action.HasActionLink((Func<ResourceContext, Uri>) (entityContext => entityContext.GenerateActionLink(bindingParameterType, action.FullyQualifiedName)), true);
      }
      else
      {
        if (action.BindingParameter.TypeConfiguration.Kind != EdmTypeKind.Collection || action.GetFeedActionLink() != null || ((CollectionTypeConfiguration) action.BindingParameter.TypeConfiguration).ElementType.Kind != EdmTypeKind.Entity)
          return;
        string bindingParameterType = action.BindingParameter.TypeConfiguration.FullName;
        action.HasFeedActionLink((Func<ResourceSetContext, Uri>) (feedContext => feedContext.GenerateActionLink(bindingParameterType, action.FullyQualifiedName)), true);
      }
    }
  }
}
