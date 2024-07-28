// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.FunctionOverloadResolver
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using Microsoft.OData.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OData.UriParser
{
  internal static class FunctionOverloadResolver
  {
    internal static bool ResolveOperationImportFromList(
      string identifier,
      IList<string> parameterNames,
      IEdmModel model,
      out IEdmOperationImport matchingOperationImport,
      ODataUriResolver resolver)
    {
      IList<IEdmOperationImport> actionImportItems = (IList<IEdmOperationImport>) new List<IEdmOperationImport>();
      IEnumerable<IEdmOperationImport> edmOperationImports;
      try
      {
        edmOperationImports = parameterNames.Count <= 0 ? resolver.ResolveOperationImports(model, identifier) : resolver.ResolveOperationImports(model, identifier).RemoveActionImports(out actionImportItems).FilterOperationsByParameterNames((IEnumerable<string>) parameterNames, resolver.EnableCaseInsensitive);
      }
      catch (Exception ex)
      {
        if (ExceptionUtils.IsCatchableExceptionType(ex))
          throw new ODataException(Microsoft.OData.Strings.FunctionOverloadResolver_FoundInvalidOperationImport((object) identifier), ex);
        throw;
      }
      if (actionImportItems.Count > 0)
        throw ExceptionUtil.CreateBadRequestError(Microsoft.OData.Strings.RequestUriProcessor_SegmentDoesNotSupportKeyPredicates((object) identifier));
      if (edmOperationImports.Any<IEdmOperationImport>((Func<IEdmOperationImport, bool>) (f => f.IsActionImport())))
      {
        if (edmOperationImports.Count<IEdmOperationImport>() > 1)
        {
          if (edmOperationImports.Any<IEdmOperationImport>((Func<IEdmOperationImport, bool>) (o => o.IsFunctionImport())))
            throw new ODataException(Microsoft.OData.Strings.FunctionOverloadResolver_MultipleOperationImportOverloads((object) identifier));
          throw new ODataException(Microsoft.OData.Strings.FunctionOverloadResolver_MultipleActionImportOverloads((object) identifier));
        }
        if (parameterNames.Count<string>() != 0)
          throw ExceptionUtil.CreateBadRequestError(Microsoft.OData.Strings.RequestUriProcessor_SegmentDoesNotSupportKeyPredicates((object) identifier));
        matchingOperationImport = edmOperationImports.Single<IEdmOperationImport>();
        return true;
      }
      if (edmOperationImports.Count<IEdmOperationImport>() > 1 && parameterNames.Count == 0)
        edmOperationImports = edmOperationImports.Where<IEdmOperationImport>((Func<IEdmOperationImport, bool>) (operationImport => operationImport.Operation.Parameters.Count<IEdmOperationParameter>() == 0));
      if (!edmOperationImports.HasAny<IEdmOperationImport>())
      {
        matchingOperationImport = (IEdmOperationImport) null;
        return false;
      }
      if (edmOperationImports.Count<IEdmOperationImport>() > 1)
        edmOperationImports = edmOperationImports.FindBestOverloadBasedOnParameters((IEnumerable<string>) parameterNames);
      matchingOperationImport = edmOperationImports.Count<IEdmOperationImport>() <= 1 ? edmOperationImports.Single<IEdmOperationImport>() : throw new ODataException(Microsoft.OData.Strings.FunctionOverloadResolver_MultipleOperationImportOverloads((object) identifier));
      return matchingOperationImport != null;
    }

    internal static bool ResolveOperationFromList(
      string identifier,
      IEnumerable<string> parameterNames,
      IEdmType bindingType,
      IEdmModel model,
      out IEdmOperation matchingOperation,
      ODataUriResolver resolver)
    {
      if (bindingType != null && bindingType.IsOpen() && !identifier.Contains(".") && resolver.GetType() == typeof (ODataUriResolver))
      {
        matchingOperation = (IEdmOperation) null;
        return false;
      }
      IEnumerable<IEdmOperation> edmOperations1;
      try
      {
        edmOperations1 = bindingType == null ? resolver.ResolveUnboundOperations(model, identifier) : resolver.ResolveBoundOperations(model, identifier, bindingType);
      }
      catch (Exception ex)
      {
        if (ExceptionUtils.IsCatchableExceptionType(ex))
          throw new ODataException(Microsoft.OData.Strings.FunctionOverloadResolver_FoundInvalidOperation((object) identifier), ex);
        throw;
      }
      IList<IEdmOperation> actionItems = (IList<IEdmOperation>) new List<IEdmOperation>();
      bool flag = parameterNames.Count<string>() > 0;
      if (bindingType != null)
        edmOperations1.EnsureOperationsBoundWithBindingParameter();
      IEnumerable<IEdmOperation> edmOperations2 = !flag ? (bindingType == null ? edmOperations1.Where<IEdmOperation>((Func<IEdmOperation, bool>) (o => o.IsFunction() && !o.Parameters.Any<IEdmOperationParameter>() || o.IsAction())) : edmOperations1.Where<IEdmOperation>((Func<IEdmOperation, bool>) (o => o.IsFunction() && (o.Parameters.Count<IEdmOperationParameter>() == 1 || o.Parameters.Skip<IEdmOperationParameter>(1).All<IEdmOperationParameter>((Func<IEdmOperationParameter, bool>) (p => p is IEdmOptionalParameter))) || o.IsAction()))) : edmOperations1.RemoveActions(out actionItems).FilterOperationsByParameterNames(parameterNames, resolver.EnableCaseInsensitive);
      if (edmOperations2.Count<IEdmOperation>() > 1)
        edmOperations2 = edmOperations2.FilterBoundOperationsWithSameTypeHierarchyToTypeClosestToBindingType(bindingType);
      if (edmOperations2.Any<IEdmOperation>((Func<IEdmOperation, bool>) (f => f.IsAction())))
      {
        if (edmOperations2.Count<IEdmOperation>() > 1)
        {
          if (edmOperations2.Any<IEdmOperation>((Func<IEdmOperation, bool>) (o => o.IsFunction())))
            throw new ODataException(Microsoft.OData.Strings.FunctionOverloadResolver_MultipleOperationOverloads((object) identifier));
          throw new ODataException(Microsoft.OData.Strings.FunctionOverloadResolver_MultipleActionOverloads((object) identifier));
        }
        if (flag)
          throw ExceptionUtil.CreateBadRequestError(Microsoft.OData.Strings.RequestUriProcessor_SegmentDoesNotSupportKeyPredicates((object) identifier));
        matchingOperation = edmOperations2.Single<IEdmOperation>();
        return true;
      }
      if (actionItems.Count > 0)
        throw ExceptionUtil.CreateBadRequestError(Microsoft.OData.Strings.RequestUriProcessor_SegmentDoesNotSupportKeyPredicates((object) identifier));
      if (edmOperations2.Count<IEdmOperation>() > 1)
        edmOperations2 = edmOperations2.FindBestOverloadBasedOnParameters(parameterNames);
      matchingOperation = edmOperations2.Count<IEdmOperation>() <= 1 ? edmOperations2.SingleOrDefault<IEdmOperation>() : throw new ODataException(Microsoft.OData.Strings.FunctionOverloadResolver_NoSingleMatchFound((object) identifier, (object) string.Join(",", parameterNames.ToArray<string>())));
      return matchingOperation != null;
    }

    internal static bool HasAny<T>(this IEnumerable<T> enumerable) where T : class
    {
      switch (enumerable)
      {
        case IList<T> objList:
          return objList.Count > 0;
        case T[] objArray:
          return objArray.Length != 0;
        default:
          return (object) enumerable.FirstOrDefault<T>() != null;
      }
    }
  }
}
