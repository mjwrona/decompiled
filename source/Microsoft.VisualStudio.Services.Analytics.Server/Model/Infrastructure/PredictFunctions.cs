// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.Model.Infrastructure.PredictFunctions
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace Microsoft.VisualStudio.Services.Analytics.Model.Infrastructure
{
  public class PredictFunctions : IConvention, IStoreModelConvention<EntityContainer>
  {
    private static readonly PrimitiveType _resultType = PrimitiveType.GetEdmPrimitiveType(PrimitiveTypeKind.Double);
    private static readonly PrimitiveType _intType = PrimitiveType.GetEdmPrimitiveType(PrimitiveTypeKind.Int32);
    private static readonly PrimitiveType _stringType = PrimitiveType.GetEdmPrimitiveType(PrimitiveTypeKind.String);

    public void Apply(EntityContainer item, DbModel model)
    {
      EdmFunction edmFunction = EdmFunction.Create("PredictWorkItemCompletedTime", "AnalyticsModel", DataSpace.SSpace, new EdmFunctionPayload()
      {
        ParameterTypeSemantics = new ParameterTypeSemantics?(ParameterTypeSemantics.AllowImplicitConversion),
        IsComposable = new bool?(true),
        IsAggregate = new bool?(false),
        IsBuiltIn = new bool?(false),
        StoreFunctionName = "func_PredictWorkItemCompletedTime",
        ReturnParameters = (IList<FunctionParameter>) new FunctionParameter[1]
        {
          FunctionParameter.Create("ReturnType", (EdmType) PredictFunctions._resultType, ParameterMode.ReturnValue)
        },
        Parameters = (IList<FunctionParameter>) new FunctionParameter[5]
        {
          FunctionParameter.Create("partitionId", (EdmType) PredictFunctions._intType, ParameterMode.In),
          FunctionParameter.Create("workItemId", (EdmType) PredictFunctions._intType, ParameterMode.In),
          FunctionParameter.Create("revision", (EdmType) PredictFunctions._intType, ParameterMode.In),
          FunctionParameter.Create("state", (EdmType) PredictFunctions._stringType, ParameterMode.In),
          FunctionParameter.Create("workItemType", (EdmType) PredictFunctions._stringType, ParameterMode.In)
        }
      }, (IEnumerable<MetadataProperty>) null);
      model.StoreModel.AddItem(edmFunction);
    }
  }
}
