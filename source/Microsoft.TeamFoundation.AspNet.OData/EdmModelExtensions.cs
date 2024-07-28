// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.EdmModelExtensions
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Formatter;
using Microsoft.OData.Edm;
using System;
using System.ComponentModel;
using System.Linq;

namespace Microsoft.AspNet.OData
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class EdmModelExtensions
  {
    public static NavigationSourceLinkBuilderAnnotation GetNavigationSourceLinkBuilder(
      this IEdmModel model,
      IEdmNavigationSource navigationSource)
    {
      NavigationSourceLinkBuilderAnnotation navigationSourceLinkBuilder = model != null ? model.GetAnnotationValue<NavigationSourceLinkBuilderAnnotation>((IEdmElement) navigationSource) : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (model));
      if (navigationSourceLinkBuilder == null)
      {
        navigationSourceLinkBuilder = new NavigationSourceLinkBuilderAnnotation(navigationSource, model);
        model.SetNavigationSourceLinkBuilder(navigationSource, navigationSourceLinkBuilder);
      }
      return navigationSourceLinkBuilder;
    }

    public static void SetNavigationSourceLinkBuilder(
      this IEdmModel model,
      IEdmNavigationSource navigationSource,
      NavigationSourceLinkBuilderAnnotation navigationSourceLinkBuilder)
    {
      if (model == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (model));
      model.SetAnnotationValue<NavigationSourceLinkBuilderAnnotation>((IEdmElement) navigationSource, navigationSourceLinkBuilder);
    }

    public static OperationLinkBuilder GetOperationLinkBuilder(
      this IEdmModel model,
      IEdmOperation operation)
    {
      if (model == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (model));
      OperationLinkBuilder operationLinkBuilder = operation != null ? model.GetAnnotationValue<OperationLinkBuilder>((IEdmElement) operation) : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (operation));
      if (operationLinkBuilder == null)
      {
        operationLinkBuilder = EdmModelExtensions.GetDefaultOperationLinkBuilder(operation);
        model.SetOperationLinkBuilder(operation, operationLinkBuilder);
      }
      return operationLinkBuilder;
    }

    public static void SetOperationLinkBuilder(
      this IEdmModel model,
      IEdmOperation operation,
      OperationLinkBuilder operationLinkBuilder)
    {
      if (model == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (model));
      model.SetAnnotationValue<OperationLinkBuilder>((IEdmElement) operation, operationLinkBuilder);
    }

    internal static ClrTypeCache GetTypeMappingCache(this IEdmModel model)
    {
      ClrTypeCache typeMappingCache = model.GetAnnotationValue<ClrTypeCache>((IEdmElement) model);
      if (typeMappingCache == null)
      {
        typeMappingCache = new ClrTypeCache();
        model.SetAnnotationValue<ClrTypeCache>((IEdmElement) model, typeMappingCache);
      }
      return typeMappingCache;
    }

    internal static void SetOperationTitleAnnotation(
      this IEdmModel model,
      IEdmOperation action,
      OperationTitleAnnotation title)
    {
      model.SetAnnotationValue<OperationTitleAnnotation>((IEdmElement) action, title);
    }

    internal static OperationTitleAnnotation GetOperationTitleAnnotation(
      this IEdmModel model,
      IEdmOperation operation)
    {
      return model.GetAnnotationValue<OperationTitleAnnotation>((IEdmElement) operation);
    }

    private static OperationLinkBuilder GetDefaultOperationLinkBuilder(IEdmOperation operation)
    {
      OperationLinkBuilder operationLinkBuilder = (OperationLinkBuilder) null;
      if (operation.Parameters != null)
      {
        if (operation.Parameters.First<IEdmOperationParameter>().Type.IsEntity())
          operationLinkBuilder = !(operation is IEdmAction) ? new OperationLinkBuilder((Func<ResourceContext, Uri>) (resourceContext => resourceContext.GenerateFunctionLink(operation)), true) : new OperationLinkBuilder((Func<ResourceContext, Uri>) (resourceContext => resourceContext.GenerateActionLink(operation)), true);
        else if (operation.Parameters.First<IEdmOperationParameter>().Type.IsCollection())
          operationLinkBuilder = !(operation is IEdmAction) ? new OperationLinkBuilder((Func<ResourceSetContext, Uri>) (reseourceSetContext => reseourceSetContext.GenerateFunctionLink(operation)), true) : new OperationLinkBuilder((Func<ResourceSetContext, Uri>) (reseourceSetContext => reseourceSetContext.GenerateActionLink(operation)), true);
      }
      return operationLinkBuilder;
    }
  }
}
