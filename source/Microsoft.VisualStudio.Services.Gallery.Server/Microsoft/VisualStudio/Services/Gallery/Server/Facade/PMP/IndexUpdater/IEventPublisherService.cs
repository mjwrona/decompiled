// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Facade.PMP.IndexUpdater.IEventPublisherService
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Facade.PMP.IndexUpdater
{
  [DefaultServiceImplementation(typeof (VSCodeEventPublisherService))]
  public interface IEventPublisherService : IVssFrameworkService
  {
    void PublishPackageAggregateDeletedEvent(
      IVssRequestContext requestContext,
      string extensionName);

    void PublishPackageAggregateLockedEvent(IVssRequestContext requestContext, string extensionName);

    void PublishPackageAggregateUnlockedEvent(
      IVssRequestContext requestContext,
      string extensionName);

    void PublishPackageAggregateArchivedEvent(
      IVssRequestContext requestContext,
      string extensionName);

    void PublishPackageAggregateUnarchivedEvent(
      IVssRequestContext requestContext,
      string extensionName);

    void PublishPackageAggregateForceIndexEvent(
      IVssRequestContext requestContext,
      string extensionName);

    void PublishPackageDeletedEvent(
      IVssRequestContext requestContext,
      string extensionName,
      string version);

    void PublishArtifactFilePublishedEvent(
      IVssRequestContext requestContext,
      string extensionName,
      string version,
      string targetPlatform = null);

    void PublishPublisherUpdatedEvent(IVssRequestContext requestContext, string publisherName);
  }
}
