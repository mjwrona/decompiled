// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Builder.LinkGenerationHelpers
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Builder.Conventions;
using Microsoft.AspNet.OData.Common;
using Microsoft.AspNet.OData.Formatter;
using Microsoft.AspNet.OData.Formatter.Deserialization;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace Microsoft.AspNet.OData.Builder
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class LinkGenerationHelpers
  {
    public static Uri GenerateSelfLink(this ResourceContext resourceContext, bool includeCast)
    {
      if (resourceContext == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (resourceContext));
      IList<ODataPathSegment> segments = resourceContext.InternalUrlHelper != null ? resourceContext.GenerateBaseODataPathSegments() : throw Microsoft.AspNet.OData.Common.Error.Argument(nameof (resourceContext), SRResources.UrlHelperNull, (object) typeof (ResourceContext).Name);
      bool flag = resourceContext.StructuredType == resourceContext.NavigationSource.EntityType();
      if (includeCast && !flag)
        segments.Add((ODataPathSegment) new TypeSegment((IEdmType) resourceContext.StructuredType, (IEdmNavigationSource) null));
      string odataLink = resourceContext.InternalUrlHelper.CreateODataLink(segments);
      return odataLink == null ? (Uri) null : new Uri(odataLink);
    }

    public static Uri GenerateNavigationPropertyLink(
      this ResourceContext resourceContext,
      IEdmNavigationProperty navigationProperty,
      bool includeCast)
    {
      if (resourceContext == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (resourceContext));
      IList<ODataPathSegment> segments = resourceContext.InternalUrlHelper != null ? resourceContext.GenerateBaseODataPathSegments() : throw Microsoft.AspNet.OData.Common.Error.Argument(nameof (resourceContext), SRResources.UrlHelperNull, (object) typeof (ResourceContext).Name);
      if (includeCast)
        segments.Add((ODataPathSegment) new TypeSegment((IEdmType) resourceContext.StructuredType, (IEdmNavigationSource) null));
      segments.Add((ODataPathSegment) new NavigationPropertySegment(navigationProperty, (IEdmNavigationSource) null));
      string odataLink = resourceContext.InternalUrlHelper.CreateODataLink(segments);
      return odataLink == null ? (Uri) null : new Uri(odataLink);
    }

    public static Uri GenerateActionLink(
      this ResourceSetContext resourceSetContext,
      IEdmOperation action)
    {
      if (resourceSetContext == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (resourceSetContext));
      IEdmOperationParameter operationParameter = action != null ? action.Parameters.FirstOrDefault<IEdmOperationParameter>() : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (action));
      if (operationParameter == null || !operationParameter.Type.IsCollection() || !((IEdmCollectionType) operationParameter.Type.Definition).ElementType.IsEntity())
        throw Microsoft.AspNet.OData.Common.Error.Argument(nameof (action), SRResources.ActionNotBoundToCollectionOfEntity, (object) action.Name);
      return resourceSetContext.GenerateActionLink(operationParameter.Type, action);
    }

    internal static Uri GenerateActionLink(
      this ResourceSetContext feedContext,
      string bindingParameterType,
      string actionName)
    {
      if (feedContext.EntitySetBase is IEdmContainedEntitySet)
        return (Uri) null;
      if (feedContext.EdmModel == null)
        return (Uri) null;
      IEdmModel edmModel = feedContext.EdmModel;
      IEdmTypeReference bindingParameterType1 = (IEdmTypeReference) new EdmCollectionTypeReference((IEdmCollectionType) new EdmCollectionType(edmModel.FindDeclaredType(DeserializationHelpers.GetCollectionElementTypeName(bindingParameterType, false)).ToEdmTypeReference(true)));
      IEdmOperation action = edmModel.FindDeclaredOperations(actionName).First<IEdmOperation>();
      return feedContext.GenerateActionLink(bindingParameterType1, action);
    }

    internal static Uri GenerateActionLink(
      this ResourceSetContext resourceSetContext,
      IEdmTypeReference bindingParameterType,
      IEdmOperation action)
    {
      if (resourceSetContext.EntitySetBase is IEdmContainedEntitySet)
        return (Uri) null;
      IList<ODataPathSegment> odataPathSegmentList = (IList<ODataPathSegment>) new List<ODataPathSegment>();
      resourceSetContext.GenerateBaseODataPathSegmentsForFeed(odataPathSegmentList);
      if (resourceSetContext.EntitySetBase.Type.FullTypeName() != bindingParameterType.FullName())
        odataPathSegmentList.Add((ODataPathSegment) new TypeSegment(bindingParameterType.Definition, (IEdmNavigationSource) resourceSetContext.EntitySetBase));
      OperationSegment operationSegment = new OperationSegment(action, (IEdmEntitySetBase) null);
      odataPathSegmentList.Add((ODataPathSegment) operationSegment);
      string odataLink = resourceSetContext.InternalUrlHelper.CreateODataLink(odataPathSegmentList);
      return odataLink != null ? new Uri(odataLink) : (Uri) null;
    }

    public static Uri GenerateFunctionLink(
      this ResourceSetContext resourceSetContext,
      IEdmOperation function)
    {
      if (resourceSetContext == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (resourceSetContext));
      IEdmOperationParameter operationParameter = function != null ? function.Parameters.FirstOrDefault<IEdmOperationParameter>() : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (function));
      if (operationParameter == null || !operationParameter.Type.IsCollection() || !((IEdmCollectionType) operationParameter.Type.Definition).ElementType.IsEntity())
        throw Microsoft.AspNet.OData.Common.Error.Argument(nameof (function), SRResources.FunctionNotBoundToCollectionOfEntity, (object) function.Name);
      return resourceSetContext.GenerateFunctionLink(operationParameter.Type, function, function.Parameters.Select<IEdmOperationParameter, string>((Func<IEdmOperationParameter, string>) (p => p.Name)));
    }

    internal static Uri GenerateFunctionLink(
      this ResourceSetContext resourceSetContext,
      IEdmTypeReference bindingParameterType,
      IEdmOperation functionImport,
      IEnumerable<string> parameterNames)
    {
      if (resourceSetContext.EntitySetBase is IEdmContainedEntitySet)
        return (Uri) null;
      IList<ODataPathSegment> odataPathSegmentList = (IList<ODataPathSegment>) new List<ODataPathSegment>();
      resourceSetContext.GenerateBaseODataPathSegmentsForFeed(odataPathSegmentList);
      if (resourceSetContext.EntitySetBase.Type.FullTypeName() != bindingParameterType.Definition.FullTypeName())
        odataPathSegmentList.Add((ODataPathSegment) new TypeSegment(bindingParameterType.Definition, (IEdmNavigationSource) null));
      IList<OperationSegmentParameter> parameters = (IList<OperationSegmentParameter>) new List<OperationSegmentParameter>();
      foreach (string name in parameterNames.Skip<string>(1))
      {
        string str = "@" + name;
        parameters.Add(new OperationSegmentParameter(name, (object) new ConstantNode((object) str, str)));
      }
      OperationSegment operationSegment = new OperationSegment((IEnumerable<IEdmOperation>) new IEdmOperation[1]
      {
        functionImport
      }, (IEnumerable<OperationSegmentParameter>) parameters, (IEdmEntitySetBase) null);
      odataPathSegmentList.Add((ODataPathSegment) operationSegment);
      string odataLink = resourceSetContext.InternalUrlHelper.CreateODataLink(odataPathSegmentList);
      return odataLink != null ? new Uri(odataLink) : (Uri) null;
    }

    internal static Uri GenerateFunctionLink(
      this ResourceSetContext feedContext,
      string bindingParameterType,
      string functionName,
      IEnumerable<string> parameterNames)
    {
      if (feedContext.EntitySetBase is IEdmContainedEntitySet)
        return (Uri) null;
      if (feedContext.EdmModel == null)
        return (Uri) null;
      IEdmModel edmModel = feedContext.EdmModel;
      IEdmTypeReference bindingParameterType1 = (IEdmTypeReference) new EdmCollectionTypeReference((IEdmCollectionType) new EdmCollectionType(edmModel.FindDeclaredType(DeserializationHelpers.GetCollectionElementTypeName(bindingParameterType, false)).ToEdmTypeReference(true)));
      IEdmOperation functionImport = edmModel.FindDeclaredOperations(functionName).First<IEdmOperation>();
      return feedContext.GenerateFunctionLink(bindingParameterType1, functionImport, parameterNames);
    }

    public static Uri GenerateActionLink(this ResourceContext resourceContext, IEdmOperation action)
    {
      if (resourceContext == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (resourceContext));
      IEdmOperationParameter operationParameter = action != null ? action.Parameters.FirstOrDefault<IEdmOperationParameter>() : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (action));
      if (operationParameter == null || !operationParameter.Type.IsEntity())
        throw Microsoft.AspNet.OData.Common.Error.Argument(nameof (action), SRResources.ActionNotBoundToEntity, (object) action.Name);
      return resourceContext.GenerateActionLink(operationParameter.Type, action);
    }

    internal static Uri GenerateActionLink(
      this ResourceContext resourceContext,
      IEdmTypeReference bindingParameterType,
      IEdmOperation action)
    {
      if (resourceContext.NavigationSource is IEdmContainedEntitySet)
        return (Uri) null;
      IList<ODataPathSegment> odataPathSegments = resourceContext.GenerateBaseODataPathSegments();
      if (resourceContext.NavigationSource.EntityType() != bindingParameterType.Definition)
        odataPathSegments.Add((ODataPathSegment) new TypeSegment(bindingParameterType.Definition, (IEdmNavigationSource) null));
      OperationSegment operationSegment = new OperationSegment((IEnumerable<IEdmOperation>) new IEdmOperation[1]
      {
        action
      }, (IEdmEntitySetBase) null);
      odataPathSegments.Add((ODataPathSegment) operationSegment);
      string odataLink = resourceContext.InternalUrlHelper.CreateODataLink(odataPathSegments);
      return odataLink != null ? new Uri(odataLink) : (Uri) null;
    }

    internal static Uri GenerateActionLink(
      this ResourceContext resourceContext,
      string bindingParameterType,
      string actionName)
    {
      if (resourceContext.NavigationSource is IEdmContainedEntitySet)
        return (Uri) null;
      if (resourceContext.EdmModel == null)
        return (Uri) null;
      IEdmModel edmModel = resourceContext.EdmModel;
      IEdmTypeReference edmTypeReference = edmModel.FindDeclaredType(bindingParameterType).ToEdmTypeReference(true);
      IEdmOperation action = edmModel.FindDeclaredOperations(actionName).First<IEdmOperation>();
      return resourceContext.GenerateActionLink(edmTypeReference, action);
    }

    public static Uri GenerateFunctionLink(
      this ResourceContext resourceContext,
      IEdmOperation function)
    {
      if (resourceContext == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (resourceContext));
      IEdmOperationParameter operationParameter = function != null ? function.Parameters.FirstOrDefault<IEdmOperationParameter>() : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (function));
      if (operationParameter == null || !operationParameter.Type.IsEntity())
        throw Microsoft.AspNet.OData.Common.Error.Argument(nameof (function), SRResources.FunctionNotBoundToEntity, (object) function.Name);
      return resourceContext.GenerateFunctionLink(operationParameter.Type.FullName(), function.FullName(), function.Parameters.Select<IEdmOperationParameter, string>((Func<IEdmOperationParameter, string>) (p => p.Name)));
    }

    internal static Uri GenerateFunctionLink(
      this ResourceContext resourceContext,
      IEdmTypeReference bindingParameterType,
      IEdmOperation function,
      IEnumerable<string> parameterNames)
    {
      IList<ODataPathSegment> odataPathSegments = resourceContext.GenerateBaseODataPathSegments();
      if (resourceContext.NavigationSource.EntityType() != bindingParameterType.Definition)
        odataPathSegments.Add((ODataPathSegment) new TypeSegment(bindingParameterType.Definition, (IEdmNavigationSource) null));
      IList<OperationSegmentParameter> parameters = (IList<OperationSegmentParameter>) new List<OperationSegmentParameter>();
      foreach (string name in parameterNames.Skip<string>(1))
      {
        string str = "@" + name;
        parameters.Add(new OperationSegmentParameter(name, (object) new ConstantNode((object) str, str)));
      }
      OperationSegment operationSegment = new OperationSegment((IEnumerable<IEdmOperation>) new IEdmOperation[1]
      {
        function
      }, (IEnumerable<OperationSegmentParameter>) parameters, (IEdmEntitySetBase) null);
      odataPathSegments.Add((ODataPathSegment) operationSegment);
      string odataLink = resourceContext.InternalUrlHelper.CreateODataLink(odataPathSegments);
      return odataLink != null ? new Uri(odataLink) : (Uri) null;
    }

    internal static Uri GenerateFunctionLink(
      this ResourceContext resourceContext,
      string bindingParameterType,
      string functionName,
      IEnumerable<string> parameterNames)
    {
      if (resourceContext.EdmModel == null)
        return (Uri) null;
      IEdmModel edmModel = resourceContext.EdmModel;
      IEdmTypeReference edmTypeReference = edmModel.FindDeclaredType(bindingParameterType).ToEdmTypeReference(true);
      IEdmOperation function = edmModel.FindDeclaredOperations(functionName).First<IEdmOperation>();
      return resourceContext.GenerateFunctionLink(edmTypeReference, function, parameterNames);
    }

    internal static IList<ODataPathSegment> GenerateBaseODataPathSegments(
      this ResourceContext resourceContext)
    {
      IList<ODataPathSegment> odataPath = (IList<ODataPathSegment>) new List<ODataPathSegment>();
      if (resourceContext.NavigationSource.NavigationSourceKind() == EdmNavigationSourceKind.Singleton)
        odataPath.Add((ODataPathSegment) new SingletonSegment((IEdmSingleton) resourceContext.NavigationSource));
      else
        resourceContext.GenerateBaseODataPathSegmentsForEntity(odataPath);
      return odataPath;
    }

    private static void GenerateBaseODataPathSegmentsForNonSingletons(
      Microsoft.AspNet.OData.Routing.ODataPath path,
      IEdmNavigationSource navigationSource,
      IList<ODataPathSegment> odataPath)
    {
      bool flag1 = false;
      bool flag2 = false;
      if (path != null)
      {
        ReadOnlyCollection<ODataPathSegment> segments = path.Segments;
        int count = segments.Count;
        int num = -1;
        for (int index1 = 0; index1 < count; ++index1)
        {
          ODataPathSegment odataPathSegment = segments[index1];
          IEdmNavigationSource navigationSource1 = (IEdmNavigationSource) null;
          if (odataPathSegment is EntitySetSegment entitySetSegment)
            navigationSource1 = (IEdmNavigationSource) entitySetSegment.EntitySet;
          if (odataPathSegment is NavigationPropertySegment navigationPropertySegment)
            navigationSource1 = navigationPropertySegment.NavigationSource;
          if (flag2)
            odataPath.Add(odataPathSegment);
          else if (navigationPropertySegment != null && navigationPropertySegment.NavigationProperty.ContainsTarget)
          {
            flag2 = true;
            if (num != -1)
            {
              for (int index2 = num; index2 <= index1; ++index2)
                odataPath.Add(segments[index2]);
            }
          }
          if (navigationSource1 != null)
          {
            num = index1;
            if (navigationSource1 == navigationSource)
            {
              flag1 = true;
              break;
            }
          }
        }
      }
      if (flag1 && flag2)
        return;
      odataPath.Clear();
      if (navigationSource is IEdmContainedEntitySet)
      {
        IEdmEntitySet entitySet = (IEdmEntitySet) new EdmEntitySet((IEdmEntityContainer) new EdmEntityContainer("NS", "Default"), navigationSource.Name, navigationSource.EntityType());
        odataPath.Add((ODataPathSegment) new EntitySetSegment(entitySet));
      }
      else
        odataPath.Add((ODataPathSegment) new EntitySetSegment((IEdmEntitySet) navigationSource));
    }

    private static void GenerateBaseODataPathSegmentsForEntity(
      this ResourceContext resourceContext,
      IList<ODataPathSegment> odataPath)
    {
      LinkGenerationHelpers.GenerateBaseODataPathSegmentsForNonSingletons(resourceContext.SerializerContext.Path, resourceContext.NavigationSource, odataPath);
      odataPath.Add((ODataPathSegment) new KeySegment(ConventionsHelpers.GetEntityKey(resourceContext), resourceContext.StructuredType as IEdmEntityType, (IEdmNavigationSource) null));
    }

    private static void GenerateBaseODataPathSegmentsForFeed(
      this ResourceSetContext feedContext,
      IList<ODataPathSegment> odataPath)
    {
      LinkGenerationHelpers.GenerateBaseODataPathSegmentsForNonSingletons(feedContext.InternalRequest.Context.Path, (IEdmNavigationSource) feedContext.EntitySetBase, odataPath);
    }
  }
}
