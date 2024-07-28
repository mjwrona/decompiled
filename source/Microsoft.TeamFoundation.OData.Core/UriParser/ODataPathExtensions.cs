// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.ODataPathExtensions
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using Microsoft.OData.Metadata;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.OData.UriParser
{
  internal static class ODataPathExtensions
  {
    public static IEdmTypeReference EdmType(this ODataPath path) => path.LastSegment.EdmType.ToTypeReference();

    public static IEdmNavigationSource NavigationSource(this ODataPath path) => path.LastSegment.TranslateWith<IEdmNavigationSource>((PathSegmentTranslator<IEdmNavigationSource>) new DetermineNavigationSourceTranslator());

    public static bool IsCollection(this ODataPath path) => path.LastSegment.TranslateWith<bool>((PathSegmentTranslator<bool>) new IsCollectionTranslator());

    public static ODataPath AppendNavigationPropertySegment(
      this ODataPath path,
      IEdmNavigationProperty navigationProperty,
      IEdmNavigationSource navigationSource)
    {
      return new ODataPath((IEnumerable<ODataPathSegment>) path)
      {
        (ODataPathSegment) new NavigationPropertySegment(navigationProperty, navigationSource)
      };
    }

    public static ODataPath AppendPropertySegment(
      this ODataPath path,
      IEdmStructuralProperty property)
    {
      return new ODataPath((IEnumerable<ODataPathSegment>) path)
      {
        (ODataPathSegment) new PropertySegment(property)
      };
    }

    public static ODataPath AppendKeySegment(
      this ODataPath path,
      IEnumerable<KeyValuePair<string, object>> keys,
      IEdmEntityType edmType,
      IEdmNavigationSource navigationSource)
    {
      SplitEndingSegmentOfTypeHandler<TypeSegment> handler = new SplitEndingSegmentOfTypeHandler<TypeSegment>();
      path.WalkWith((PathSegmentHandler) handler);
      KeySegment newSegment1 = new KeySegment(keys, edmType, navigationSource);
      ODataPath firstPart = handler.FirstPart;
      firstPart.Add((ODataPathSegment) newSegment1);
      foreach (ODataPathSegment newSegment2 in handler.LastPart)
        firstPart.Add(newSegment2);
      return firstPart;
    }

    public static ODataPath TrimEndingKeySegment(this ODataPath path)
    {
      SplitEndingSegmentOfTypeHandler<TypeSegment> handler1 = new SplitEndingSegmentOfTypeHandler<TypeSegment>();
      SplitEndingSegmentOfTypeHandler<KeySegment> handler2 = new SplitEndingSegmentOfTypeHandler<KeySegment>();
      path.WalkWith((PathSegmentHandler) handler1);
      handler1.FirstPart.WalkWith((PathSegmentHandler) handler2);
      ODataPath firstPart = handler2.FirstPart;
      foreach (ODataPathSegment newSegment in handler1.LastPart)
        firstPart.Add(newSegment);
      return firstPart;
    }

    public static ODataPath TrimEndingTypeSegment(this ODataPath path)
    {
      SplitEndingSegmentOfTypeHandler<TypeSegment> handler = new SplitEndingSegmentOfTypeHandler<TypeSegment>();
      path.WalkWith((PathSegmentHandler) handler);
      return handler.FirstPart;
    }

    public static bool IsIndividualProperty(this ODataPath path)
    {
      ODataPathSegment lastSegment = path.TrimEndingTypeSegment().LastSegment;
      return lastSegment is PropertySegment || lastSegment is DynamicPathSegment;
    }

    public static bool IsUndeclared(this ODataPath path) => path.TrimEndingTypeSegment().LastSegment is DynamicPathSegment;

    public static string ToContextUrlPathString(this ODataPath path)
    {
      StringBuilder stringBuilder = new StringBuilder();
      PathSegmentToContextUrlPathTranslator defaultInstance = PathSegmentToContextUrlPathTranslator.DefaultInstance;
      ODataPathSegment odataPathSegment1 = (ODataPathSegment) null;
      bool flag = false;
      foreach (ODataPathSegment odataPathSegment2 in path)
      {
        OperationSegment operationSegment = odataPathSegment2 as OperationSegment;
        if (odataPathSegment2 is OperationImportSegment operationImportSegment)
        {
          IEdmOperationImport edmOperationImport = operationImportSegment.OperationImports.FirstOrDefault<IEdmOperationImport>();
          if (edmOperationImport.EntitySet is EdmPathExpression entitySet)
          {
            stringBuilder.Append(entitySet.Path);
          }
          else
          {
            stringBuilder = edmOperationImport.Operation.ReturnType != null ? new StringBuilder(edmOperationImport.Operation.ReturnType.FullName()) : new StringBuilder("Edm.Untyped");
            flag = true;
          }
        }
        else if (operationSegment != null)
        {
          IEdmOperation edmOperation = operationSegment.Operations.FirstOrDefault<IEdmOperation>();
          if (edmOperation.IsBound && odataPathSegment1 != null && edmOperation.Parameters.First<IEdmOperationParameter>().Type.Definition == odataPathSegment1.EdmType)
          {
            if (edmOperation.EntitySetPath != null)
            {
              foreach (string str in edmOperation.EntitySetPath.PathSegments.Skip<string>(1))
              {
                stringBuilder.Append('/');
                stringBuilder.Append(str);
              }
            }
            else if (operationSegment.EntitySet != null)
            {
              stringBuilder = new StringBuilder(operationSegment.EntitySet.Name);
            }
            else
            {
              stringBuilder = edmOperation.ReturnType != null ? new StringBuilder(edmOperation.ReturnType.FullName()) : new StringBuilder("Edm.Untyped");
              flag = true;
            }
          }
        }
        else if (flag)
        {
          stringBuilder = new StringBuilder(odataPathSegment2.EdmType.FullTypeName());
          flag = false;
        }
        else
          stringBuilder.Append(odataPathSegment2.TranslateWith<string>((PathSegmentTranslator<string>) defaultInstance));
        odataPathSegment1 = odataPathSegment2;
      }
      return stringBuilder.ToString().TrimStart('/');
    }

    public static string ToResourcePathString(
      this ODataPath path,
      ODataUrlKeyDelimiter urlKeyDelimiter)
    {
      return string.Concat(path.WalkWith<string>((PathSegmentTranslator<string>) new PathSegmentToResourcePathTranslator(urlKeyDelimiter)).ToArray<string>()).TrimStart('/');
    }

    public static ODataSelectPath ToSelectPath(this ODataExpandPath path) => new ODataSelectPath((IEnumerable<ODataPathSegment>) path);

    public static ODataExpandPath ToExpandPath(this ODataSelectPath path) => new ODataExpandPath((IEnumerable<ODataPathSegment>) path);

    internal static IEdmNavigationSource TargetNavigationSource(this ODataPath path) => path == null ? (IEdmNavigationSource) null : new ODataPathInfo(path).TargetNavigationSource;
  }
}
