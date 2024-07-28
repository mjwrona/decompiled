// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Builder.NavigationSourceLinkBuilderAnnotation
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Formatter;
using Microsoft.AspNet.OData.Formatter.Serialization;
using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.AspNet.OData.Builder
{
  public class NavigationSourceLinkBuilderAnnotation
  {
    private readonly SelfLinkBuilder<Uri> _idLinkBuilder;
    private readonly SelfLinkBuilder<Uri> _editLinkBuilder;
    private readonly SelfLinkBuilder<Uri> _readLinkBuilder;
    private readonly Dictionary<IEdmNavigationProperty, NavigationLinkBuilder> _navigationPropertyLinkBuilderLookup = new Dictionary<IEdmNavigationProperty, NavigationLinkBuilder>();

    public NavigationSourceLinkBuilderAnnotation()
    {
    }

    public NavigationSourceLinkBuilderAnnotation(
      IEdmNavigationSource navigationSource,
      IEdmModel model)
    {
      if (navigationSource == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (navigationSource));
      if (model == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (model));
      IEdmEntityType edmEntityType = navigationSource.EntityType();
      IEnumerable<IEdmEntityType> edmEntityTypes = model.FindAllDerivedTypes((IEdmStructuredType) edmEntityType).Cast<IEdmEntityType>();
      foreach (IEdmNavigationProperty navigationProperty in edmEntityType.NavigationProperties())
        this.AddNavigationPropertyLinkBuilder(navigationProperty, new NavigationLinkBuilder((Func<ResourceContext, IEdmNavigationProperty, Uri>) ((resourceContext, navProperty) => resourceContext.GenerateNavigationPropertyLink(navProperty, false)), true));
      bool derivedTypesDefineNavigationProperty = false;
      foreach (IEdmStructuredType type in edmEntityTypes)
      {
        foreach (IEdmNavigationProperty navigationProperty in type.DeclaredNavigationProperties())
        {
          derivedTypesDefineNavigationProperty = true;
          Func<ResourceContext, IEdmNavigationProperty, Uri> navigationLinkFactory = (Func<ResourceContext, IEdmNavigationProperty, Uri>) ((resourceContext, navProperty) => resourceContext.GenerateNavigationPropertyLink(navProperty, true));
          this.AddNavigationPropertyLinkBuilder(navigationProperty, new NavigationLinkBuilder(navigationLinkFactory, true));
        }
      }
      this._idLinkBuilder = new SelfLinkBuilder<Uri>((Func<ResourceContext, Uri>) (resourceContext => resourceContext.GenerateSelfLink(derivedTypesDefineNavigationProperty)), true);
    }

    public NavigationSourceLinkBuilderAnnotation(
      IEdmNavigationSource navigationSource,
      SelfLinkBuilder<Uri> idLinkBuilder,
      SelfLinkBuilder<Uri> editLinkBuilder,
      SelfLinkBuilder<Uri> readLinkBuilder)
    {
      if (navigationSource == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (navigationSource));
      this._idLinkBuilder = idLinkBuilder;
      this._editLinkBuilder = editLinkBuilder;
      this._readLinkBuilder = readLinkBuilder;
    }

    public NavigationSourceLinkBuilderAnnotation(NavigationSourceConfiguration navigationSource)
    {
      this._idLinkBuilder = navigationSource != null ? navigationSource.GetIdLink() : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (navigationSource));
      this._editLinkBuilder = navigationSource.GetEditLink();
      this._readLinkBuilder = navigationSource.GetReadLink();
    }

    public void AddNavigationPropertyLinkBuilder(
      IEdmNavigationProperty navigationProperty,
      NavigationLinkBuilder linkBuilder)
    {
      this._navigationPropertyLinkBuilderLookup[navigationProperty] = linkBuilder;
    }

    public virtual EntitySelfLinks BuildEntitySelfLinks(
      ResourceContext instanceContext,
      ODataMetadataLevel metadataLevel)
    {
      EntitySelfLinks entitySelfLinks = new EntitySelfLinks()
      {
        IdLink = this.BuildIdLink(instanceContext, metadataLevel)
      };
      entitySelfLinks.EditLink = this.BuildEditLink(instanceContext, metadataLevel, entitySelfLinks.IdLink);
      entitySelfLinks.ReadLink = this.BuildReadLink(instanceContext, metadataLevel, entitySelfLinks.EditLink);
      return entitySelfLinks;
    }

    public virtual Uri BuildIdLink(
      ResourceContext instanceContext,
      ODataMetadataLevel metadataLevel)
    {
      if (instanceContext == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (instanceContext));
      if (this._idLinkBuilder != null)
      {
        switch (metadataLevel)
        {
          case ODataMetadataLevel.MinimalMetadata:
            if (this._idLinkBuilder.FollowsConventions)
              break;
            goto case ODataMetadataLevel.FullMetadata;
          case ODataMetadataLevel.FullMetadata:
            return this._idLinkBuilder.Factory(instanceContext);
        }
      }
      return (Uri) null;
    }

    internal Uri BuildIdLink(ResourceContext instanceContext) => this.BuildIdLink(instanceContext, ODataMetadataLevel.FullMetadata);

    public virtual Uri BuildEditLink(
      ResourceContext instanceContext,
      ODataMetadataLevel metadataLevel,
      Uri idLink)
    {
      if (instanceContext == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (instanceContext));
      if (this._editLinkBuilder != null)
      {
        switch (metadataLevel)
        {
          case ODataMetadataLevel.MinimalMetadata:
            if (this._editLinkBuilder.FollowsConventions)
              break;
            goto case ODataMetadataLevel.FullMetadata;
          case ODataMetadataLevel.FullMetadata:
            return this._editLinkBuilder.Factory(instanceContext);
        }
      }
      return (Uri) null;
    }

    internal Uri BuildEditLink(ResourceContext instanceContext) => this.BuildEditLink(instanceContext, ODataMetadataLevel.FullMetadata, (Uri) null);

    public virtual Uri BuildReadLink(
      ResourceContext instanceContext,
      ODataMetadataLevel metadataLevel,
      Uri editLink)
    {
      if (instanceContext == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (instanceContext));
      if (this._readLinkBuilder != null)
      {
        switch (metadataLevel)
        {
          case ODataMetadataLevel.MinimalMetadata:
            if (this._readLinkBuilder.FollowsConventions)
              break;
            goto case ODataMetadataLevel.FullMetadata;
          case ODataMetadataLevel.FullMetadata:
            return this._readLinkBuilder.Factory(instanceContext);
        }
      }
      return (Uri) null;
    }

    internal Uri BuildReadLink(ResourceContext instanceContext) => this.BuildReadLink(instanceContext, ODataMetadataLevel.FullMetadata, (Uri) null);

    public virtual Uri BuildNavigationLink(
      ResourceContext instanceContext,
      IEdmNavigationProperty navigationProperty,
      ODataMetadataLevel metadataLevel)
    {
      if (instanceContext == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (instanceContext));
      if (navigationProperty == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (navigationProperty));
      NavigationLinkBuilder navigationLinkBuilder;
      return this._navigationPropertyLinkBuilderLookup.TryGetValue(navigationProperty, out navigationLinkBuilder) && !navigationLinkBuilder.FollowsConventions && (metadataLevel == ODataMetadataLevel.MinimalMetadata || metadataLevel == ODataMetadataLevel.FullMetadata) ? navigationLinkBuilder.Factory(instanceContext, navigationProperty) : (Uri) null;
    }

    internal Uri BuildNavigationLink(
      ResourceContext instanceContext,
      IEdmNavigationProperty navigationProperty)
    {
      if (instanceContext == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (instanceContext));
      if (navigationProperty == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (navigationProperty));
      NavigationLinkBuilder navigationLinkBuilder;
      return this._navigationPropertyLinkBuilderLookup.TryGetValue(navigationProperty, out navigationLinkBuilder) ? navigationLinkBuilder.Factory(instanceContext, navigationProperty) : (Uri) null;
    }
  }
}
