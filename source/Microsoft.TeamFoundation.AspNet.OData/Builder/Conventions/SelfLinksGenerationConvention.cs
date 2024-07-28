// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Builder.Conventions.SelfLinksGenerationConvention
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using System;
using System.Linq;

namespace Microsoft.AspNet.OData.Builder.Conventions
{
  internal class SelfLinksGenerationConvention : INavigationSourceConvention, IConvention
  {
    public void Apply(NavigationSourceConfiguration configuration, ODataModelBuilder model)
    {
      if (configuration == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (configuration));
      if (configuration is EntitySetConfiguration setConfiguration && setConfiguration.GetFeedSelfLink() == null)
        setConfiguration.HasFeedSelfLink((Func<ResourceSetContext, Uri>) (feedContext =>
        {
          string odataLink = feedContext.InternalUrlHelper.CreateODataLink((ODataPathSegment) new EntitySetSegment(feedContext.EntitySetBase as IEdmEntitySet));
          return odataLink == null ? (Uri) null : new Uri(odataLink);
        }));
      if (configuration.GetIdLink() == null)
        configuration.HasIdLink(new SelfLinkBuilder<Uri>((Func<ResourceContext, Uri>) (entityContext => entityContext.GenerateSelfLink(false)), true));
      if (configuration.GetEditLink() != null)
        return;
      if (EdmTypeConfigurationExtensions.DerivedTypes(model, configuration.EntityType).OfType<EntityTypeConfiguration>().Any<EntityTypeConfiguration>((Func<EntityTypeConfiguration, bool>) (e => e.NavigationProperties.Any<NavigationPropertyConfiguration>())))
        configuration.HasEditLink(new SelfLinkBuilder<Uri>((Func<ResourceContext, Uri>) (entityContext => entityContext.GenerateSelfLink(true)), true));
      else
        configuration.HasEditLink(new SelfLinkBuilder<Uri>((Func<ResourceContext, Uri>) (entityContext => entityContext.GenerateSelfLink(false)), true));
    }
  }
}
