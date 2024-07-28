// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Routing.Conventions.PropertyRoutingConvention
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Adapters;
using Microsoft.AspNet.OData.Interfaces;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using System;
using System.Linq;
using System.Web.Http.Controllers;

namespace Microsoft.AspNet.OData.Routing.Conventions
{
  public class PropertyRoutingConvention : NavigationSourceRoutingConvention
  {
    public override string SelectAction(
      Microsoft.AspNet.OData.Routing.ODataPath odataPath,
      HttpControllerContext controllerContext,
      ILookup<string, HttpActionDescriptor> actionMap)
    {
      NavigationSourceRoutingConvention.ValidateSelectActionParameters(odataPath, controllerContext, actionMap);
      return PropertyRoutingConvention.SelectActionImpl(odataPath, (IWebApiControllerContext) new WebApiControllerContext(controllerContext, NavigationSourceRoutingConvention.GetControllerResult(controllerContext)), (IWebApiActionMap) new WebApiActionMap(actionMap));
    }

    internal static string SelectActionImpl(
      Microsoft.AspNet.OData.Routing.ODataPath odataPath,
      IWebApiControllerContext controllerContext,
      IWebApiActionMap actionMap)
    {
      string prefix;
      TypeSegment cast;
      IEdmProperty property = PropertyRoutingConvention.GetProperty(odataPath, controllerContext.Request.Method, out prefix, out cast);
      IEdmEntityType declaringType = property == null ? (IEdmEntityType) null : property.DeclaringType as IEdmEntityType;
      if (declaringType != null)
      {
        string matchingAction;
        if (cast == null)
        {
          matchingAction = actionMap.FindMatchingAction(prefix + property.Name + "From" + declaringType.Name, prefix + property.Name);
        }
        else
        {
          IEdmComplexType edmComplexType = cast.EdmType.TypeKind != EdmTypeKind.Collection ? (IEdmComplexType) cast.EdmType : ((IEdmCollectionType) cast.EdmType).ElementType.AsComplex().ComplexDefinition();
          matchingAction = actionMap.FindMatchingAction(prefix + property.Name + "Of" + edmComplexType.Name + "From" + declaringType.Name, prefix + property.Name + "Of" + edmComplexType.Name);
        }
        if (matchingAction != null)
        {
          if (odataPath.PathTemplate.StartsWith("~/entityset/key", StringComparison.Ordinal))
          {
            KeySegment segment = (KeySegment) odataPath.Segments[1];
            controllerContext.AddKeyValueToRouteData(segment);
          }
          return matchingAction;
        }
      }
      return (string) null;
    }

    private static IEdmProperty GetProperty(
      Microsoft.AspNet.OData.Routing.ODataPath odataPath,
      ODataRequestMethod method,
      out string prefix,
      out TypeSegment cast)
    {
      prefix = string.Empty;
      cast = (TypeSegment) null;
      PropertySegment propertySegment = (PropertySegment) null;
      if (odataPath.PathTemplate == "~/entityset/key/property" || odataPath.PathTemplate == "~/entityset/key/cast/property" || odataPath.PathTemplate == "~/singleton/property" || odataPath.PathTemplate == "~/singleton/cast/property")
      {
        PropertySegment segment = (PropertySegment) odataPath.Segments[odataPath.Segments.Count - 1];
        switch (method)
        {
          case ODataRequestMethod.Get:
            prefix = "Get";
            propertySegment = segment;
            break;
          case ODataRequestMethod.Delete:
            if (segment.Property.Type.IsNullable)
            {
              prefix = "DeleteTo";
              propertySegment = segment;
              break;
            }
            break;
          case ODataRequestMethod.Patch:
            if (!segment.Property.Type.IsCollection())
            {
              prefix = "PatchTo";
              propertySegment = segment;
              break;
            }
            break;
          case ODataRequestMethod.Post:
            if (segment.Property.Type.IsCollection())
            {
              prefix = "PostTo";
              propertySegment = segment;
              break;
            }
            break;
          case ODataRequestMethod.Put:
            prefix = "PutTo";
            propertySegment = segment;
            break;
        }
      }
      else if (odataPath.PathTemplate == "~/entityset/key/property/cast" || odataPath.PathTemplate == "~/entityset/key/cast/property/cast" || odataPath.PathTemplate == "~/singleton/property/cast" || odataPath.PathTemplate == "~/singleton/cast/property/cast")
      {
        PropertySegment segment = (PropertySegment) odataPath.Segments[odataPath.Segments.Count - 2];
        TypeSegment typeSegment = (TypeSegment) odataPath.Segments.Last<ODataPathSegment>();
        switch (method)
        {
          case ODataRequestMethod.Get:
            prefix = "Get";
            propertySegment = segment;
            cast = typeSegment;
            break;
          case ODataRequestMethod.Patch:
            if (!segment.Property.Type.IsCollection())
            {
              prefix = "PatchTo";
              propertySegment = segment;
              cast = typeSegment;
              break;
            }
            break;
          case ODataRequestMethod.Post:
            if (segment.Property.Type.IsCollection())
            {
              prefix = "PostTo";
              propertySegment = segment;
              cast = typeSegment;
              break;
            }
            break;
          case ODataRequestMethod.Put:
            prefix = "PutTo";
            propertySegment = segment;
            cast = typeSegment;
            break;
        }
      }
      else if (odataPath.PathTemplate == "~/entityset/key/property/$value" || odataPath.PathTemplate == "~/entityset/key/cast/property/$value" || odataPath.PathTemplate == "~/singleton/property/$value" || odataPath.PathTemplate == "~/singleton/cast/property/$value" || odataPath.PathTemplate == "~/entityset/key/property/$count" || odataPath.PathTemplate == "~/entityset/key/cast/property/$count" || odataPath.PathTemplate == "~/singleton/property/$count" || odataPath.PathTemplate == "~/singleton/cast/property/$count")
      {
        PropertySegment segment = (PropertySegment) odataPath.Segments[odataPath.Segments.Count - 2];
        if (method == ODataRequestMethod.Get)
        {
          prefix = "Get";
          propertySegment = segment;
        }
      }
      return propertySegment != null ? (IEdmProperty) propertySegment.Property : (IEdmProperty) null;
    }
  }
}
