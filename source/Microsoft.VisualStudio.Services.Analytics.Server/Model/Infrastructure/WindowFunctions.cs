// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.Model.Infrastructure.WindowFunctions
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Analytics.Model.Infrastructure
{
  public class WindowFunctions : IConvention, IStoreModelConvention<EntityContainer>
  {
    private static readonly List<PrimitiveTypeKind> _valueTypes = new List<PrimitiveTypeKind>()
    {
      PrimitiveTypeKind.Byte,
      PrimitiveTypeKind.Int16,
      PrimitiveTypeKind.Int32,
      PrimitiveTypeKind.Int64,
      PrimitiveTypeKind.Double,
      PrimitiveTypeKind.Decimal,
      PrimitiveTypeKind.String,
      PrimitiveTypeKind.DateTimeOffset,
      PrimitiveTypeKind.Guid
    };
    private static readonly List<PrimitiveTypeKind> _numericValueTypes = new List<PrimitiveTypeKind>()
    {
      PrimitiveTypeKind.Byte,
      PrimitiveTypeKind.Int16,
      PrimitiveTypeKind.Int32,
      PrimitiveTypeKind.Int64,
      PrimitiveTypeKind.Double,
      PrimitiveTypeKind.Decimal
    };
    private static readonly List<PrimitiveTypeKind> _groupByTypes = new List<PrimitiveTypeKind>()
    {
      PrimitiveTypeKind.Int32,
      PrimitiveTypeKind.String,
      PrimitiveTypeKind.Guid
    };
    private static readonly List<PrimitiveTypeKind> _orderByTypes = new List<PrimitiveTypeKind>()
    {
      PrimitiveTypeKind.Int32,
      PrimitiveTypeKind.DateTimeOffset
    };

    public void Apply(EntityContainer item, DbModel model)
    {
      foreach (PrimitiveTypeKind valueType in WindowFunctions._valueTypes)
      {
        foreach (PrimitiveTypeKind groupByType in WindowFunctions._groupByTypes)
        {
          foreach (PrimitiveTypeKind orderByType in WindowFunctions._orderByTypes)
          {
            WindowFunctions.AddFunctionForType(model, "LAG", valueType, valueType, orderByType, groupByType);
            WindowFunctions.AddFunctionForType(model, "LEAD", valueType, valueType, orderByType, groupByType);
          }
        }
      }
      foreach (PrimitiveTypeKind groupByType in WindowFunctions._groupByTypes)
      {
        foreach (PrimitiveTypeKind orderByType in WindowFunctions._orderByTypes)
          WindowFunctions.AddFunctionForType(model, "ROW_NUMBER", PrimitiveTypeKind.Int64, orderByType, groupByType);
      }
      foreach (PrimitiveTypeKind numericValueType in WindowFunctions._numericValueTypes)
      {
        WindowFunctions.AddFunctionForType(model, "PERCENTILE_CONT", PrimitiveTypeKind.Double, numericValueType, PrimitiveTypeKind.Decimal);
        WindowFunctions.AddFunctionForType(model, "PERCENTILE_DISC", numericValueType, numericValueType, PrimitiveTypeKind.Decimal);
        foreach (PrimitiveTypeKind groupByType1 in WindowFunctions._groupByTypes)
        {
          WindowFunctions.AddFunctionForType(model, "PERCENTILE_CONT", PrimitiveTypeKind.Double, numericValueType, PrimitiveTypeKind.Decimal, groupByType1);
          WindowFunctions.AddFunctionForType(model, "PERCENTILE_DISC", numericValueType, numericValueType, PrimitiveTypeKind.Decimal, groupByType1);
          foreach (PrimitiveTypeKind groupByType2 in WindowFunctions._groupByTypes)
          {
            WindowFunctions.AddFunctionForType(model, "PERCENTILE_CONT", PrimitiveTypeKind.Double, numericValueType, PrimitiveTypeKind.Decimal, groupByType1, groupByType2);
            WindowFunctions.AddFunctionForType(model, "PERCENTILE_DISC", numericValueType, numericValueType, PrimitiveTypeKind.Decimal, groupByType1, groupByType2);
          }
        }
      }
      WindowFunctions.AddFunctionForType(model, "PERCENTILE_CONT", PrimitiveTypeKind.Double, PrimitiveTypeKind.Decimal, PrimitiveTypeKind.Decimal, PrimitiveTypeKind.String, PrimitiveTypeKind.String, PrimitiveTypeKind.Int32);
      WindowFunctions.AddFunctionForType(model, "PERCENTILE_DISC", PrimitiveTypeKind.Decimal, PrimitiveTypeKind.Decimal, PrimitiveTypeKind.Decimal, PrimitiveTypeKind.String, PrimitiveTypeKind.String, PrimitiveTypeKind.Int32);
    }

    private static void AddFunctionForType(
      DbModel model,
      string function,
      PrimitiveTypeKind output,
      params PrimitiveTypeKind[] kinds)
    {
      PrimitiveType edmPrimitiveType = PrimitiveType.GetEdmPrimitiveType(output);
      edmPrimitiveType.GetCollectionType();
      IEnumerable<FunctionParameter> source = ((IEnumerable<PrimitiveTypeKind>) kinds).Select<PrimitiveTypeKind, FunctionParameter>((Func<PrimitiveTypeKind, int, FunctionParameter>) ((k, i) => FunctionParameter.Create(string.Format("grp{0}", (object) i), (EdmType) PrimitiveType.GetEdmPrimitiveType(k), ParameterMode.In)));
      EdmFunction edmFunction = EdmFunction.Create(function + "_" + string.Join<PrimitiveTypeKind>("_", (IEnumerable<PrimitiveTypeKind>) kinds), "AX", DataSpace.SSpace, new EdmFunctionPayload()
      {
        ParameterTypeSemantics = new ParameterTypeSemantics?(ParameterTypeSemantics.AllowImplicitConversion),
        IsComposable = new bool?(true),
        IsAggregate = new bool?(false),
        IsBuiltIn = new bool?(false),
        StoreFunctionName = function,
        ReturnParameters = (IList<FunctionParameter>) new FunctionParameter[1]
        {
          FunctionParameter.Create("ReturnType", (EdmType) edmPrimitiveType, ParameterMode.ReturnValue)
        },
        Parameters = (IList<FunctionParameter>) source.ToList<FunctionParameter>()
      }, (IEnumerable<MetadataProperty>) null);
      model.StoreModel.AddItem(edmFunction);
    }
  }
}
