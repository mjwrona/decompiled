// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.JsonLight.ODataJsonLightReaderNestedResourceInfo
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using Microsoft.OData.Metadata;
using System.Collections.Generic;

namespace Microsoft.OData.JsonLight
{
  internal sealed class ODataJsonLightReaderNestedResourceInfo : ODataJsonLightReaderNestedInfo
  {
    private readonly ODataNestedResourceInfo nestedResourceInfo;
    private readonly bool hasValue;
    private ODataResourceSetBase resourceSet;
    private LinkedList<ODataEntityReferenceLink> entityReferenceLinks;

    private ODataJsonLightReaderNestedResourceInfo(
      ODataNestedResourceInfo nestedResourceInfo,
      IEdmProperty nestedProperty,
      bool isExpanded)
      : base(nestedProperty)
    {
      this.nestedResourceInfo = nestedResourceInfo;
      this.NestedResourceTypeReference = nestedProperty != null ? nestedProperty.Type.Definition.AsElementType().ToTypeReference(nestedProperty.Type.IsNullable) : (IEdmTypeReference) null;
      this.hasValue = isExpanded;
    }

    private ODataJsonLightReaderNestedResourceInfo(
      ODataNestedResourceInfo nestedResourceInfo,
      IEdmProperty nestedProperty,
      IEdmType nestedResourceType,
      bool isExpanded)
      : base(nestedProperty)
    {
      this.nestedResourceInfo = nestedResourceInfo;
      bool nullable = true;
      if (nestedProperty != null && nestedProperty.Type != null)
        nullable = nestedProperty.Type.IsNullable;
      IEdmType type = nestedResourceType;
      if (nestedProperty != null && type == null)
        type = nestedProperty.Type.Definition;
      this.NestedResourceTypeReference = type.ToTypeReference(nullable);
      this.hasValue = isExpanded;
    }

    internal ODataNestedResourceInfo NestedResourceInfo => this.nestedResourceInfo;

    internal IEdmNavigationProperty NavigationProperty => this.NestedProperty as IEdmNavigationProperty;

    internal IEdmStructuralProperty StructuralProperty => this.NestedProperty as IEdmStructuralProperty;

    internal bool HasValue => this.hasValue;

    internal ODataResourceSetBase NestedResourceSet => this.resourceSet;

    internal IEdmTypeReference NestedResourceTypeReference { get; private set; }

    internal bool HasEntityReferenceLink => this.entityReferenceLinks != null && this.entityReferenceLinks.First != null;

    internal static ODataJsonLightReaderNestedResourceInfo CreateDeferredLinkInfo(
      ODataNestedResourceInfo nestedResourceInfo,
      IEdmNavigationProperty navigationProperty)
    {
      return new ODataJsonLightReaderNestedResourceInfo(nestedResourceInfo, (IEdmProperty) navigationProperty, false);
    }

    internal static ODataJsonLightReaderNestedResourceInfo CreateResourceReaderNestedResourceInfo(
      ODataNestedResourceInfo nestedResourceInfo,
      IEdmProperty nestedProperty,
      IEdmStructuredType nestedResourceType)
    {
      return new ODataJsonLightReaderNestedResourceInfo(nestedResourceInfo, nestedProperty, (IEdmType) nestedResourceType, true);
    }

    internal static ODataJsonLightReaderNestedResourceInfo CreateResourceSetReaderNestedResourceInfo(
      ODataNestedResourceInfo nestedResourceInfo,
      IEdmProperty nestedProperty,
      IEdmType nestedResourceType,
      ODataResourceSetBase resourceSet)
    {
      return new ODataJsonLightReaderNestedResourceInfo(nestedResourceInfo, nestedProperty, nestedResourceType, true)
      {
        resourceSet = resourceSet
      };
    }

    internal static ODataJsonLightReaderNestedResourceInfo CreateSingletonEntityReferenceLinkInfo(
      ODataNestedResourceInfo nestedResourceInfo,
      IEdmNavigationProperty navigationProperty,
      ODataEntityReferenceLink entityReferenceLink,
      bool isExpanded)
    {
      ODataJsonLightReaderNestedResourceInfo referenceLinkInfo = new ODataJsonLightReaderNestedResourceInfo(nestedResourceInfo, (IEdmProperty) navigationProperty, isExpanded);
      if (entityReferenceLink != null)
      {
        referenceLinkInfo.entityReferenceLinks = new LinkedList<ODataEntityReferenceLink>();
        referenceLinkInfo.entityReferenceLinks.AddFirst(entityReferenceLink);
      }
      return referenceLinkInfo;
    }

    internal static ODataJsonLightReaderNestedResourceInfo CreateCollectionEntityReferenceLinksInfo(
      ODataNestedResourceInfo nestedResourceInfo,
      IEdmNavigationProperty navigationProperty,
      LinkedList<ODataEntityReferenceLink> entityReferenceLinks,
      bool isExpanded)
    {
      return new ODataJsonLightReaderNestedResourceInfo(nestedResourceInfo, (IEdmProperty) navigationProperty, isExpanded)
      {
        entityReferenceLinks = entityReferenceLinks
      };
    }

    internal static ODataJsonLightReaderNestedResourceInfo CreateProjectedNestedResourceInfo(
      IEdmNavigationProperty navigationProperty)
    {
      return new ODataJsonLightReaderNestedResourceInfo(new ODataNestedResourceInfo()
      {
        Name = navigationProperty.Name,
        IsCollection = new bool?(navigationProperty.Type.IsCollection())
      }, (IEdmProperty) navigationProperty, false);
    }

    internal ODataEntityReferenceLink ReportEntityReferenceLink()
    {
      if (this.entityReferenceLinks == null || this.entityReferenceLinks.First == null)
        return (ODataEntityReferenceLink) null;
      ODataEntityReferenceLink entityReferenceLink = this.entityReferenceLinks.First.Value;
      this.entityReferenceLinks.RemoveFirst();
      return entityReferenceLink;
    }
  }
}
