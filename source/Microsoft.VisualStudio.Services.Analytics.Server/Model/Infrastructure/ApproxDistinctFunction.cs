// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.Model.Infrastructure.ApproxDistinctFunction
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace Microsoft.VisualStudio.Services.Analytics.Model.Infrastructure
{
  public class ApproxDistinctFunction : IConvention, IStoreModelConvention<EntityContainer>
  {
    private static readonly PrimitiveType _resultType = PrimitiveType.GetEdmPrimitiveType(PrimitiveTypeKind.Int64);

    public void Apply(EntityContainer item, DbModel model)
    {
      ApproxDistinctFunction.AddFunctionForType(model, PrimitiveTypeKind.Boolean);
      ApproxDistinctFunction.AddFunctionForType(model, PrimitiveTypeKind.Byte);
      ApproxDistinctFunction.AddFunctionForType(model, PrimitiveTypeKind.Int16);
      ApproxDistinctFunction.AddFunctionForType(model, PrimitiveTypeKind.Int32);
      ApproxDistinctFunction.AddFunctionForType(model, PrimitiveTypeKind.Int64);
      ApproxDistinctFunction.AddFunctionForType(model, PrimitiveTypeKind.Guid);
      ApproxDistinctFunction.AddFunctionForType(model, PrimitiveTypeKind.String);
      ApproxDistinctFunction.AddFunctionForType(model, PrimitiveTypeKind.DateTimeOffset);
      ApproxDistinctFunction.AddFunctionForType(model, PrimitiveTypeKind.Double);
      ApproxDistinctFunction.AddFunctionForType(model, PrimitiveTypeKind.Decimal);
    }

    private static void AddFunctionForType(DbModel model, PrimitiveTypeKind kind)
    {
      CollectionType collectionType = PrimitiveType.GetEdmPrimitiveType(kind).GetCollectionType();
      EdmFunction edmFunction = EdmFunction.Create(string.Format("APPROX_COUNT_DISTINCT_{0}", (object) kind), "AX", DataSpace.SSpace, new EdmFunctionPayload()
      {
        ParameterTypeSemantics = new ParameterTypeSemantics?(ParameterTypeSemantics.AllowImplicitConversion),
        IsComposable = new bool?(true),
        IsAggregate = new bool?(true),
        IsBuiltIn = new bool?(true),
        StoreFunctionName = "APPROX_COUNT_DISTINCT",
        ReturnParameters = (IList<FunctionParameter>) new FunctionParameter[1]
        {
          FunctionParameter.Create("ReturnType", (EdmType) ApproxDistinctFunction._resultType, ParameterMode.ReturnValue)
        },
        Parameters = (IList<FunctionParameter>) new FunctionParameter[1]
        {
          FunctionParameter.Create("input", (EdmType) collectionType, ParameterMode.In)
        }
      }, (IEnumerable<MetadataProperty>) null);
      model.StoreModel.AddItem(edmFunction);
    }
  }
}
