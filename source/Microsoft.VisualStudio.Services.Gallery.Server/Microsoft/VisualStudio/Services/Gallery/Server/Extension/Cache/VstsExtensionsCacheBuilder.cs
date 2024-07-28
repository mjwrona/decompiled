// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Extension.Cache.VstsExtensionsCacheBuilder
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Extension.Cache
{
  internal class VstsExtensionsCacheBuilder : ProductExtensionsCacheBuilder
  {
    private const int m_VstsExtensionCacheCircuitBreakerExceptionID = 160013;
    private const int m_VstsExtensionCacheCircuitBreakerThrottlingExceptionID = 160014;
    private int m_VstsExtensionsCacheTimeoutInSeconds = 300;

    public override int CacheTimeoutInSeconds
    {
      get => this.m_VstsExtensionsCacheTimeoutInSeconds;
      set => this.m_VstsExtensionsCacheTimeoutInSeconds = value;
    }

    protected override string ProductType
    {
      get => "vsts";
      set
      {
      }
    }

    protected override int CiruitBreakerExceptionId
    {
      get => 160013;
      set
      {
      }
    }

    protected override int CircuitBreakerThrottlingExceptionID
    {
      get => 160014;
      set
      {
      }
    }

    protected override List<PublishedExtension> GetExtensionPage(
      IVssRequestContext requestContext,
      int pageNumber)
    {
      List<PublishedExtension> extensionPage = new List<PublishedExtension>();
      IPublishedExtensionService extensionService = this.m_PublishedExtensionService ?? requestContext.GetService<IPublishedExtensionService>();
      List<string> targetsForProduct = GalleryUtil.GetInstallationTargetsForProduct(this.ProductType);
      List<InstallationTarget> targets = new List<InstallationTarget>();
      foreach (string str in targetsForProduct)
        targets.Add(new InstallationTarget()
        {
          Target = str
        });
      ExtensionsByInstallationTargetsResult installationTargetsResult = extensionService.QueryExtensionsForCacheBuilding(requestContext, targets, ExtensionQueryFlags.AllAttributes, pageNumber, 2000);
      if (installationTargetsResult?.Extensions != null)
        extensionPage = installationTargetsResult.Extensions;
      return extensionPage;
    }
  }
}
