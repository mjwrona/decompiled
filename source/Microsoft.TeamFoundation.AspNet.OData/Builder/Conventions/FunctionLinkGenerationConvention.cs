// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Builder.Conventions.FunctionLinkGenerationConvention
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.OData.Edm;
using System;
using System.Linq;

namespace Microsoft.AspNet.OData.Builder.Conventions
{
  internal class FunctionLinkGenerationConvention : IOperationConvention, IConvention
  {
    public void Apply(OperationConfiguration configuration, ODataModelBuilder model)
    {
      FunctionConfiguration function = configuration as FunctionConfiguration;
      if (function == null || !function.IsBindable)
        return;
      if (function.BindingParameter.TypeConfiguration.Kind == EdmTypeKind.Entity && function.GetFunctionLink() == null)
      {
        string bindingParamterType = function.BindingParameter.TypeConfiguration.FullName;
        function.HasFunctionLink((Func<ResourceContext, Uri>) (entityContext => entityContext.GenerateFunctionLink(bindingParamterType, function.FullyQualifiedName, function.Parameters.Select<ParameterConfiguration, string>((Func<ParameterConfiguration, string>) (p => p.Name)))), true);
      }
      else
      {
        if (function.BindingParameter.TypeConfiguration.Kind != EdmTypeKind.Collection || function.GetFeedFunctionLink() != null || ((CollectionTypeConfiguration) function.BindingParameter.TypeConfiguration).ElementType.Kind != EdmTypeKind.Entity)
          return;
        string bindingParamterType = function.BindingParameter.TypeConfiguration.FullName;
        function.HasFeedFunctionLink((Func<ResourceSetContext, Uri>) (feedContext => feedContext.GenerateFunctionLink(bindingParamterType, function.FullyQualifiedName, function.Parameters.Select<ParameterConfiguration, string>((Func<ParameterConfiguration, string>) (p => p.Name)))), true);
      }
    }
  }
}
