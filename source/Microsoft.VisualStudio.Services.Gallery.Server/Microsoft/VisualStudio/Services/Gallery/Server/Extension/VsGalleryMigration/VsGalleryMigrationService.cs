// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Extension.VsGalleryMigration.VsGalleryMigrationService
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.Server.Components;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Extension.VsGalleryMigration
{
  public class VsGalleryMigrationService : IVsGalleryMigrationService, IVssFrameworkService
  {
    internal Microsoft.VisualStudio.Services.Identity.Identity m_everyoneGroup;

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      IdentityService service = systemRequestContext.GetService<IdentityService>();
      IdentityDescriptor identityDescriptor = IdentityDomain.MapFromWellKnownIdentifier(service.GetScope(systemRequestContext, systemRequestContext.ServiceHost.InstanceId).Id, GroupWellKnownIdentityDescriptors.EveryoneGroup);
      this.m_everyoneGroup = service.ReadIdentities(systemRequestContext, (IList<IdentityDescriptor>) new IdentityDescriptor[1]
      {
        identityDescriptor
      }, QueryMembership.None, (IEnumerable<string>) null)[0];
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public PublishedExtension QueryExtensionById(
      IVssRequestContext requestContext,
      Guid extensionId,
      string version,
      ExtensionQueryFlags flags,
      Guid validationId)
    {
      PublishedExtension extension;
      using (PublishedExtensionComponent component = requestContext.CreateComponent<PublishedExtensionComponent>())
        extension = component.QueryExtensionById(extensionId, version, validationId, flags);
      if (!GallerySecurity.HasExtensionPermission(requestContext, extension, (string) null, PublisherPermissions.PrivateRead, true))
        GallerySecurity.CheckExtensionPermission(requestContext, extension, (string) null, PublisherPermissions.Read, true);
      return extension;
    }

    public void UpdateVsExtensionMetadata(
      IVssRequestContext requestContext,
      Guid extensionId,
      IList<KeyValuePair<string, string>> metadataItems)
    {
      using (VsGalleryMigrationComponent component = requestContext.CreateComponent<VsGalleryMigrationComponent>())
        component.UpdateExtensionMetadata(extensionId, metadataItems);
    }
  }
}
