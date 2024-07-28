// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ODataContextUrlInfo
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using Microsoft.OData.Metadata;
using Microsoft.OData.UriParser;
using Microsoft.OData.UriParser.Aggregation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OData
{
  internal sealed class ODataContextUrlInfo
  {
    private ODataContextUrlInfo() => this.DeltaKind = ODataDeltaKind.None;

    internal ODataDeltaKind DeltaKind { get; private set; }

    internal bool IsUnknownEntitySet { get; private set; }

    internal bool HasNavigationSourceInfo { get; private set; }

    internal string NavigationPath { get; private set; }

    internal string NavigationSource { get; private set; }

    internal string ResourcePath { get; private set; }

    internal bool? IsUndeclared { get; private set; }

    internal string QueryClause { get; private set; }

    internal string TypeName { get; private set; }

    internal string TypeCast { get; private set; }

    internal bool IncludeFragmentItemSelector { get; private set; }

    internal static ODataContextUrlInfo Create(
      ODataValue value,
      ODataVersion version,
      ODataUri odataUri = null,
      IEdmModel model = null)
    {
      return new ODataContextUrlInfo()
      {
        TypeName = ODataContextUrlInfo.GetTypeNameForValue(value, model),
        ResourcePath = ODataContextUrlInfo.ComputeResourcePath(odataUri),
        QueryClause = ODataContextUrlInfo.ComputeQueryClause(odataUri, version),
        IsUndeclared = ODataContextUrlInfo.ComputeIfIsUndeclared(odataUri)
      };
    }

    internal static ODataContextUrlInfo Create(
      ODataCollectionStartSerializationInfo info,
      IEdmTypeReference itemTypeReference)
    {
      string str = (string) null;
      if (info != null)
        str = info.CollectionTypeName;
      else if (itemTypeReference != null)
        str = EdmLibraryExtensions.GetCollectionTypeName(itemTypeReference.FullName());
      return new ODataContextUrlInfo() { TypeName = str };
    }

    internal static ODataContextUrlInfo Create(
      IEdmNavigationSource navigationSource,
      string expectedEntityTypeName,
      bool isSingle,
      ODataUri odataUri,
      ODataVersion version)
    {
      EdmNavigationSourceKind kind = navigationSource.NavigationSourceKind();
      string str = navigationSource.EntityType().FullName();
      return new ODataContextUrlInfo()
      {
        IsUnknownEntitySet = kind == EdmNavigationSourceKind.UnknownEntitySet,
        NavigationSource = navigationSource.Name,
        TypeCast = str == expectedEntityTypeName ? (string) null : expectedEntityTypeName,
        TypeName = str,
        IncludeFragmentItemSelector = isSingle && kind != EdmNavigationSourceKind.Singleton,
        NavigationPath = ODataContextUrlInfo.ComputeNavigationPath(kind, odataUri, navigationSource.Name),
        ResourcePath = ODataContextUrlInfo.ComputeResourcePath(odataUri),
        QueryClause = ODataContextUrlInfo.ComputeQueryClause(odataUri, version),
        IsUndeclared = ODataContextUrlInfo.ComputeIfIsUndeclared(odataUri)
      };
    }

    internal static ODataContextUrlInfo Create(
      ODataResourceTypeContext typeContext,
      ODataVersion version,
      bool isSingle,
      ODataUri odataUri = null)
    {
      bool flag = typeContext.NavigationSourceKind != EdmNavigationSourceKind.None || !string.IsNullOrEmpty(typeContext.NavigationSourceName);
      string str = flag ? typeContext.NavigationSourceFullTypeName : (typeContext.ExpectedResourceTypeName == null ? (string) null : (isSingle ? typeContext.ExpectedResourceTypeName : EdmLibraryExtensions.GetCollectionTypeName(typeContext.ExpectedResourceTypeName)));
      return new ODataContextUrlInfo()
      {
        HasNavigationSourceInfo = flag,
        IsUnknownEntitySet = typeContext.NavigationSourceKind == EdmNavigationSourceKind.UnknownEntitySet,
        NavigationSource = typeContext.NavigationSourceName,
        TypeCast = typeContext.NavigationSourceEntityTypeName == null || typeContext.ExpectedResourceTypeName == null || typeContext.ExpectedResourceType is IEdmComplexType || typeContext.NavigationSourceEntityTypeName == typeContext.ExpectedResourceTypeName ? (string) null : typeContext.ExpectedResourceTypeName,
        TypeName = str,
        IncludeFragmentItemSelector = isSingle && typeContext.NavigationSourceKind != EdmNavigationSourceKind.Singleton,
        NavigationPath = ODataContextUrlInfo.ComputeNavigationPath(typeContext.NavigationSourceKind, odataUri, typeContext.NavigationSourceName),
        ResourcePath = ODataContextUrlInfo.ComputeResourcePath(odataUri),
        QueryClause = ODataContextUrlInfo.ComputeQueryClause(odataUri, version),
        IsUndeclared = ODataContextUrlInfo.ComputeIfIsUndeclared(odataUri)
      };
    }

    internal static ODataContextUrlInfo Create(
      ODataResourceTypeContext typeContext,
      ODataVersion version,
      ODataDeltaKind kind,
      ODataUri odataUri = null)
    {
      ODataContextUrlInfo odataContextUrlInfo = new ODataContextUrlInfo()
      {
        IsUnknownEntitySet = typeContext.NavigationSourceKind == EdmNavigationSourceKind.UnknownEntitySet,
        NavigationSource = typeContext.NavigationSourceName,
        TypeCast = typeContext.NavigationSourceEntityTypeName == typeContext.ExpectedResourceTypeName ? (string) null : typeContext.ExpectedResourceTypeName,
        TypeName = typeContext.NavigationSourceEntityTypeName,
        IncludeFragmentItemSelector = kind == ODataDeltaKind.Resource && typeContext.NavigationSourceKind != EdmNavigationSourceKind.Singleton,
        DeltaKind = kind,
        NavigationPath = ODataContextUrlInfo.ComputeNavigationPath(typeContext.NavigationSourceKind, (ODataUri) null, typeContext.NavigationSourceName)
      };
      if (typeContext is ODataResourceTypeContext.ODataResourceTypeContextWithModel)
      {
        odataContextUrlInfo.NavigationPath = ODataContextUrlInfo.ComputeNavigationPath(typeContext.NavigationSourceKind, odataUri, typeContext.NavigationSourceName);
        odataContextUrlInfo.ResourcePath = ODataContextUrlInfo.ComputeResourcePath(odataUri);
        odataContextUrlInfo.QueryClause = ODataContextUrlInfo.ComputeQueryClause(odataUri, version);
        odataContextUrlInfo.IsUndeclared = ODataContextUrlInfo.ComputeIfIsUndeclared(odataUri);
      }
      return odataContextUrlInfo;
    }

    internal bool IsHiddenBy(ODataContextUrlInfo parentContextUrlInfo) => parentContextUrlInfo != null && parentContextUrlInfo.NavigationPath == this.NavigationPath && parentContextUrlInfo.DeltaKind == ODataDeltaKind.ResourceSet && this.DeltaKind == ODataDeltaKind.Resource;

    private static string ComputeNavigationPath(
      EdmNavigationSourceKind kind,
      ODataUri odataUri,
      string navigationSource)
    {
      bool flag = kind == EdmNavigationSourceKind.ContainedEntitySet;
      if (kind == EdmNavigationSourceKind.UnknownEntitySet)
        return (string) null;
      string str = (string) null;
      if (flag && odataUri != null && odataUri.Path != null)
      {
        ODataPath path = odataUri.Path.TrimEndingTypeSegment().TrimEndingKeySegment();
        str = path.LastSegment is NavigationPropertySegment || path.LastSegment is OperationSegment ? path.ToContextUrlPathString() : throw new ODataException(Strings.ODataContextUriBuilder_ODataPathInvalidForContainedElement((object) path.ToContextUrlPathString()));
      }
      return str ?? navigationSource;
    }

    private static string ComputeResourcePath(ODataUri odataUri) => odataUri != null && odataUri.Path != null && odataUri.Path.IsIndividualProperty() ? odataUri.Path.ToContextUrlPathString() : string.Empty;

    private static string ComputeQueryClause(ODataUri odataUri, ODataVersion version)
    {
      if (odataUri != null)
      {
        if (odataUri.SelectAndExpand != null)
          return ODataContextUrlInfo.CreateSelectExpandContextUriSegment(odataUri.SelectAndExpand, version);
        if (odataUri.Apply != null)
          return ODataContextUrlInfo.CreateApplyUriSegment(odataUri.Apply);
      }
      return (string) null;
    }

    private static bool? ComputeIfIsUndeclared(ODataUri odataUri) => odataUri != null && odataUri.Path != null ? new bool?(odataUri.Path.IsUndeclared()) : new bool?();

    private static string GetTypeNameForValue(ODataValue value, IEdmModel model)
    {
      if (value == null)
        return (string) null;
      if (value.IsNullValue)
        return "Edm.Null";
      if (value.TypeAnnotation != null && !string.IsNullOrEmpty(value.TypeAnnotation.TypeName))
        return value.TypeAnnotation.TypeName;
      switch (value)
      {
        case ODataCollectionValue odataCollectionValue:
          return EdmLibraryExtensions.GetCollectionTypeFullName(odataCollectionValue.TypeName);
        case ODataEnumValue odataEnumValue:
          return odataEnumValue.TypeName;
        case ODataResourceValue odataResourceValue:
          return odataResourceValue.TypeName;
        case ODataUntypedValue _:
          return "Edm.Untyped";
        case ODataPrimitiveValue odataPrimitiveValue:
          IEdmTypeDefinitionReference type = model.ResolveUIntTypeDefinition(odataPrimitiveValue.Value);
          if (type != null)
            return type.FullName();
          IEdmPrimitiveTypeReference primitiveTypeReference = EdmLibraryExtensions.GetPrimitiveTypeReference(odataPrimitiveValue.Value.GetType());
          return primitiveTypeReference != null ? primitiveTypeReference.FullName() : (string) null;
        default:
          throw new ODataException(Strings.ODataContextUriBuilder_StreamValueMustBePropertiesOfODataResource);
      }
    }

    private static string CreateApplyUriSegment(ApplyClause applyClause)
    {
      if (applyClause != null)
      {
        string contextUri = applyClause.GetContextUri();
        if (!string.IsNullOrEmpty(contextUri))
          return "(" + contextUri + ")";
      }
      return string.Empty;
    }

    private static string CreateSelectExpandContextUriSegment(
      SelectExpandClause selectExpandClause,
      ODataVersion version)
    {
      if (selectExpandClause != null)
      {
        string result;
        selectExpandClause.Traverse<string>(new Func<string, string, ODataVersion, string>(ODataContextUrlInfo.ProcessSubExpand), new Func<IList<string>, IList<string>, string>(ODataContextUrlInfo.CombineSelectAndExpandResult), new Func<ApplyClause, string>(ODataContextUrlInfo.ProcessApply), version, out result);
        if (!string.IsNullOrEmpty(result))
          return "(" + result + ")";
      }
      return string.Empty;
    }

    private static string ProcessApply(ApplyClause applyClause) => applyClause?.GetContextUri();

    private static string ProcessSubExpand(
      string expandNode,
      string subExpand,
      ODataVersion version)
    {
      return !string.IsNullOrEmpty(subExpand) || version > ODataVersion.V4 ? expandNode + "(" + subExpand + ")" : (string) null;
    }

    private static string CombineSelectAndExpandResult(
      IList<string> selectList,
      IList<string> expandList)
    {
      string empty = string.Empty;
      if (selectList.Any<string>())
      {
        foreach (string expand in (IEnumerable<string>) expandList)
        {
          string str = expand.Substring(0, expand.IndexOf('('));
          selectList.Remove(str);
        }
        empty += string.Join(",", selectList.ToArray<string>());
      }
      if (expandList.Any<string>())
      {
        if (!string.IsNullOrEmpty(empty))
          empty += ",";
        empty += string.Join(",", expandList.ToArray<string>());
      }
      return empty;
    }
  }
}
