// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.PathParserModelUtils
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Validation;
using Microsoft.OData.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OData.UriParser
{
  internal static class PathParserModelUtils
  {
    internal static IEdmEntitySetBase GetTargetEntitySet(
      this IEdmOperationImport operationImport,
      IEdmEntitySetBase sourceEntitySet,
      IEdmModel model)
    {
      IEdmEntitySetBase entitySet;
      if (operationImport.TryGetStaticEntitySet(model, out entitySet))
        return entitySet;
      if (sourceEntitySet == null)
        return (IEdmEntitySetBase) null;
      if (operationImport.Operation.IsBound && operationImport.Operation.Parameters.Any<IEdmOperationParameter>())
      {
        Dictionary<IEdmNavigationProperty, IEdmPathExpression> relativeNavigations;
        IEnumerable<EdmError> edmErrors;
        if (operationImport.TryGetRelativeEntitySetPath(model, out IEdmOperationParameter _, out relativeNavigations, out edmErrors))
        {
          IEdmEntitySetBase targetEntitySet = sourceEntitySet;
          foreach (KeyValuePair<IEdmNavigationProperty, IEdmPathExpression> keyValuePair in relativeNavigations)
          {
            targetEntitySet = targetEntitySet.FindNavigationTarget(keyValuePair.Key, keyValuePair.Value) as IEdmEntitySetBase;
            if (targetEntitySet is IEdmUnknownEntitySet)
              return targetEntitySet;
          }
          return targetEntitySet;
        }
        if (edmErrors.Any<EdmError>((Func<EdmError, bool>) (e => e.ErrorCode == EdmErrorCode.InvalidPathFirstPathParameterNotMatchingFirstParameterName)))
          throw ExceptionUtil.CreateSyntaxError();
      }
      return (IEdmEntitySetBase) null;
    }

    internal static IEdmEntitySetBase GetTargetEntitySet(
      this IEdmOperation operation,
      IEdmNavigationSource source,
      IEdmModel model)
    {
      if (source == null)
        return (IEdmEntitySetBase) null;
      if (operation.IsBound && operation.Parameters.Any<IEdmOperationParameter>())
      {
        Dictionary<IEdmNavigationProperty, IEdmPathExpression> relativeNavigations;
        IEnumerable<EdmError> errors;
        if (operation.TryGetRelativeEntitySetPath(model, out IEdmOperationParameter _, out relativeNavigations, out IEdmEntityType _, out errors))
        {
          IEdmNavigationSource targetEntitySet = source;
          foreach (KeyValuePair<IEdmNavigationProperty, IEdmPathExpression> keyValuePair in relativeNavigations)
            targetEntitySet = targetEntitySet.FindNavigationTarget(keyValuePair.Key, keyValuePair.Value);
          return targetEntitySet as IEdmEntitySetBase;
        }
        if (errors.Any<EdmError>((Func<EdmError, bool>) (e => e.ErrorCode == EdmErrorCode.InvalidPathFirstPathParameterNotMatchingFirstParameterName)))
          throw ExceptionUtil.CreateSyntaxError();
      }
      return (IEdmEntitySetBase) null;
    }

    internal static RequestTargetKind GetTargetKindFromType(this IEdmType type)
    {
      switch (type.TypeKind)
      {
        case EdmTypeKind.Entity:
        case EdmTypeKind.Complex:
          return RequestTargetKind.Resource;
        case EdmTypeKind.Collection:
          return type.IsStructuredCollectionType() ? RequestTargetKind.Resource : RequestTargetKind.Collection;
        case EdmTypeKind.Enum:
          return RequestTargetKind.Enum;
        case EdmTypeKind.TypeDefinition:
          return RequestTargetKind.Primitive;
        default:
          return RequestTargetKind.Primitive;
      }
    }
  }
}
