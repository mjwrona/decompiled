// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.SelectPathSegmentTokenBinder
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Vocabularies;
using Microsoft.OData.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OData.UriParser
{
  internal static class SelectPathSegmentTokenBinder
  {
    public static ODataPathSegment ConvertNonTypeTokenToSegment(
      PathSegmentToken tokenIn,
      IEdmModel model,
      IEdmStructuredType edmType,
      ODataUriResolver resolver,
      BindingState state = null)
    {
      ExceptionUtils.CheckArgumentNotNull<ODataUriResolver>(resolver, nameof (resolver));
      if (UriParserHelper.IsAnnotation(tokenIn.Identifier))
      {
        ODataPathSegment segment;
        if (SelectPathSegmentTokenBinder.TryBindAsDeclaredTerm(tokenIn, model, resolver, out segment))
          return segment;
        string str1 = tokenIn.Identifier.Remove(0, 1);
        int length = str1.LastIndexOf(".", StringComparison.Ordinal);
        string str2 = str1.Substring(0, length);
        string name = str1.Substring(length == 0 ? 0 : length + 1);
        if (string.Compare(str2, "odata", StringComparison.OrdinalIgnoreCase) == 0)
          throw new ODataException(Microsoft.OData.Strings.UriSelectParser_TermIsNotValid((object) tokenIn.Identifier));
        return (ODataPathSegment) new AnnotationSegment((IEdmTerm) new EdmTerm(str2, name, (IEdmTypeReference) EdmCoreModel.Instance.GetUntyped()));
      }
      EndPathToken endPathToken = new EndPathToken(tokenIn.Identifier, (QueryToken) null);
      bool? nullable;
      if (state != null && state.IsCollapsed)
      {
        nullable = state?.AggregatedPropertyNames?.Contains(endPathToken);
        if (((int) nullable ?? 0) == 0)
          throw new ODataException(Microsoft.OData.Strings.ApplyBinder_GroupByPropertyNotPropertyAccessValue((object) tokenIn.Identifier));
      }
      ODataPathSegment segment1;
      if (SelectPathSegmentTokenBinder.TryBindAsDeclaredProperty(tokenIn, edmType, resolver, out segment1))
        return segment1;
      if (tokenIn.IsNamespaceOrContainerQualified())
      {
        if (SelectPathSegmentTokenBinder.TryBindAsOperation(tokenIn, model, edmType, out segment1))
          return segment1;
        if (!edmType.IsOpen)
          return (ODataPathSegment) null;
      }
      if (!edmType.IsOpen)
      {
        nullable = state?.AggregatedPropertyNames?.Contains(endPathToken);
        if (((int) nullable ?? 0) == 0)
          throw ExceptionUtil.CreatePropertyNotFoundException(tokenIn.Identifier, edmType.FullTypeName());
      }
      return (ODataPathSegment) new DynamicPathSegment(tokenIn.Identifier);
    }

    public static bool TryBindAsWildcard(
      PathSegmentToken tokenIn,
      IEdmModel model,
      out SelectItem item)
    {
      if (tokenIn.IsNamespaceOrContainerQualified() & tokenIn.Identifier.EndsWith("*", StringComparison.Ordinal))
      {
        string namespaceName = tokenIn.Identifier.Substring(0, tokenIn.Identifier.Length - 2);
        if (model.DeclaredNamespaces.Any<string>((Func<string, bool>) (declaredNamespace => declaredNamespace.Equals(namespaceName, StringComparison.Ordinal))))
        {
          item = (SelectItem) new NamespaceQualifiedWildcardSelectItem(namespaceName);
          return true;
        }
      }
      if (tokenIn.Identifier == "*")
      {
        item = (SelectItem) new WildcardSelectItem();
        return true;
      }
      item = (SelectItem) null;
      return false;
    }

    internal static bool TryBindAsOperation(
      PathSegmentToken pathToken,
      IEdmModel model,
      IEdmStructuredType entityType,
      out ODataPathSegment segment)
    {
      IEnumerable<IEdmOperation> edmOperations = Enumerable.Empty<IEdmOperation>();
      IList<string> parameters = (IList<string>) new List<string>();
      try
      {
        int num = pathToken.Identifier.IndexOf("*", StringComparison.Ordinal);
        if (num > -1)
        {
          string namespaceName = pathToken.Identifier.Substring(0, num - 1);
          edmOperations = model.FindBoundOperations((IEdmType) entityType).Where<IEdmOperation>((Func<IEdmOperation, bool>) (o => o.Namespace == namespaceName));
        }
        else
        {
          if (pathToken is NonSystemToken nonSystemToken && nonSystemToken.NamedValues != null)
            parameters = (IList<string>) nonSystemToken.NamedValues.Select<NamedValue, string>((Func<NamedValue, string>) (s => s.Name)).ToList<string>();
          edmOperations = parameters.Count <= 0 ? model.FindBoundOperations((IEdmType) entityType).FilterByName(true, pathToken.Identifier) : model.FindBoundOperations((IEdmType) entityType).FilterByName(true, pathToken.Identifier).FilterOperationsByParameterNames((IEnumerable<string>) parameters, false);
        }
      }
      catch (Exception ex)
      {
        if (!ExceptionUtils.IsCatchableExceptionType(ex))
          throw;
      }
      if (edmOperations.Count<IEdmOperation>() > 1)
        edmOperations = edmOperations.FilterBoundOperationsWithSameTypeHierarchyToTypeClosestToBindingType((IEdmType) entityType);
      if (edmOperations.Count<IEdmOperation>() > 1 && parameters.Count > 0)
        edmOperations = edmOperations.FindBestOverloadBasedOnParameters((IEnumerable<string>) parameters);
      if (!edmOperations.HasAny<IEdmOperation>())
      {
        segment = (ODataPathSegment) null;
        return false;
      }
      edmOperations.EnsureOperationsBoundWithBindingParameter();
      segment = (ODataPathSegment) new OperationSegment(edmOperations, (IEdmEntitySetBase) null);
      return true;
    }

    private static bool TryBindAsDeclaredProperty(
      PathSegmentToken tokenIn,
      IEdmStructuredType edmType,
      ODataUriResolver resolver,
      out ODataPathSegment segment)
    {
      IEdmProperty property = resolver.ResolveProperty(edmType, tokenIn.Identifier);
      if (property == null)
      {
        segment = (ODataPathSegment) null;
        return false;
      }
      if (property.PropertyKind == EdmPropertyKind.Structural)
      {
        segment = (ODataPathSegment) new PropertySegment((IEdmStructuralProperty) property);
        return true;
      }
      segment = property.PropertyKind == EdmPropertyKind.Navigation ? (ODataPathSegment) new NavigationPropertySegment((IEdmNavigationProperty) property, (IEdmNavigationSource) null) : throw new ODataException(Microsoft.OData.Strings.SelectExpandBinder_UnknownPropertyType((object) property.Name));
      return true;
    }

    private static bool TryBindAsDeclaredTerm(
      PathSegmentToken tokenIn,
      IEdmModel model,
      ODataUriResolver resolver,
      out ODataPathSegment segment)
    {
      if (!UriParserHelper.IsAnnotation(tokenIn.Identifier))
      {
        segment = (ODataPathSegment) null;
        return false;
      }
      IEdmTerm term = resolver.ResolveTerm(model, tokenIn.Identifier.Remove(0, 1));
      if (term == null)
      {
        segment = (ODataPathSegment) null;
        return false;
      }
      segment = (ODataPathSegment) new AnnotationSegment(term);
      return true;
    }
  }
}
