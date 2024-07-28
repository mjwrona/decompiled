// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Routing.ODataParameterHelper
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.OData;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using System;
using System.Globalization;
using System.Linq;

namespace Microsoft.AspNet.OData.Routing
{
  public static class ODataParameterHelper
  {
    public static bool TryGetParameterValue(
      this OperationSegment segment,
      string parameterName,
      out object parameterValue)
    {
      if (segment == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (segment));
      if (string.IsNullOrEmpty(parameterName))
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNullOrEmpty(nameof (parameterName));
      parameterValue = (object) null;
      OperationSegmentParameter segmentParameter = segment.Parameters.FirstOrDefault<OperationSegmentParameter>((Func<OperationSegmentParameter, bool>) (p => p.Name == parameterName));
      if (segmentParameter == null)
        return false;
      parameterValue = ODataParameterHelper.TranslateNode(segmentParameter.Value);
      return true;
    }

    public static object GetParameterValue(this OperationSegment segment, string parameterName)
    {
      if (segment == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (segment));
      if (string.IsNullOrEmpty(parameterName))
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNullOrEmpty(nameof (parameterName));
      if (!segment.Operations.Any<IEdmOperation>() || !segment.Operations.First<IEdmOperation>().IsFunction())
        throw Microsoft.AspNet.OData.Common.Error.Argument(nameof (segment), SRResources.OperationSegmentMustBeFunction);
      object parameterValue = ODataParameterHelper.TranslateNode((segment.Parameters.FirstOrDefault<OperationSegmentParameter>((Func<OperationSegmentParameter, bool>) (p => p.Name == parameterName)) ?? throw Microsoft.AspNet.OData.Common.Error.Argument(nameof (parameterName), SRResources.FunctionParameterNotFound, (object) parameterName)).Value);
      switch (parameterValue)
      {
        case null:
        case ODataNullValue _:
          IEdmOperationParameter operationParameter = segment.Operations.First<IEdmOperation>().Parameters.First<IEdmOperationParameter>((Func<IEdmOperationParameter, bool>) (p => p.Name == parameterName));
          if (!operationParameter.Type.IsNullable)
            throw new ODataException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, SRResources.NullOnNonNullableFunctionParameter, new object[1]
            {
              (object) operationParameter.Type.FullName()
            }));
          break;
      }
      return parameterValue;
    }

    public static bool TryGetParameterValue(
      this OperationImportSegment segment,
      string parameterName,
      out object parameterValue)
    {
      if (segment == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (segment));
      if (string.IsNullOrEmpty(parameterName))
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNullOrEmpty(nameof (parameterName));
      parameterValue = (object) null;
      OperationSegmentParameter segmentParameter = segment.Parameters.FirstOrDefault<OperationSegmentParameter>((Func<OperationSegmentParameter, bool>) (p => p.Name == parameterName));
      if (segmentParameter == null)
        return false;
      parameterValue = ODataParameterHelper.TranslateNode(segmentParameter.Value);
      return true;
    }

    public static object GetParameterValue(
      this OperationImportSegment segment,
      string parameterName)
    {
      if (segment == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (segment));
      if (string.IsNullOrEmpty(parameterName))
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNullOrEmpty(nameof (parameterName));
      if (!segment.OperationImports.Any<IEdmOperationImport>() || !segment.OperationImports.First<IEdmOperationImport>().IsFunctionImport())
        throw Microsoft.AspNet.OData.Common.Error.Argument(nameof (segment), SRResources.OperationImportSegmentMustBeFunction);
      object parameterValue = ODataParameterHelper.TranslateNode((segment.Parameters.FirstOrDefault<OperationSegmentParameter>((Func<OperationSegmentParameter, bool>) (p => p.Name == parameterName)) ?? throw Microsoft.AspNet.OData.Common.Error.Argument(nameof (parameterName), SRResources.FunctionParameterNotFound, (object) parameterName)).Value);
      switch (parameterValue)
      {
        case null:
        case ODataNullValue _:
          IEdmOperationParameter operationParameter = segment.OperationImports.First<IEdmOperationImport>().Operation.Parameters.First<IEdmOperationParameter>((Func<IEdmOperationParameter, bool>) (p => p.Name == parameterName));
          if (!operationParameter.Type.IsNullable)
            throw new ODataException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, SRResources.NullOnNonNullableFunctionParameter, new object[1]
            {
              (object) operationParameter.Type.FullName()
            }));
          break;
      }
      return parameterValue;
    }

    internal static object TranslateNode(object value)
    {
      switch (value)
      {
        case null:
          return (object) null;
        case ConstantNode constantNode:
          return constantNode.Value;
        case ConvertNode convertNode:
          return ODataParameterHelper.TranslateNode((object) convertNode.Source);
        case ParameterAliasNode parameterAliasNode:
          return (object) parameterAliasNode.Alias;
        default:
          throw Microsoft.AspNet.OData.Common.Error.NotSupported(SRResources.CannotRecognizeNodeType, (object) typeof (ODataParameterHelper), (object) value.GetType().FullName);
      }
    }
  }
}
