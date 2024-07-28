// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.DemandExtensionService
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3FE9DD3E-5758-4D1B-8056-ECAF8A4B7A77
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Licensing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server
{
  internal class DemandExtensionService : IDemandExtensionService, IVssFrameworkService
  {
    private const string s_area = "DemandExtensionService";
    private const string s_layer = "Extensions";

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void Demand(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      bool checkLicenseOnFallback = false,
      bool alwaysAllowSystemContexts = false,
      bool alwaysAllowDeploymentServiceIdentities = false)
    {
      if (alwaysAllowSystemContexts && requestContext.IsSystemContext)
        requestContext.Trace(100136266, TraceLevel.Info, nameof (DemandExtensionService), "Extensions", "Skipping extension check because alwaysAllowSystemContexts is true and running in a system context");
      else if (alwaysAllowDeploymentServiceIdentities && this.IsRunningAsDeploymentServiceIdentity(requestContext))
        requestContext.Trace(100136267, TraceLevel.Info, nameof (DemandExtensionService), "Extensions", "Skipping extension check because alwaysAllowDeploymentServiceIdentities is true and authenticated user is a service identity");
      else if (requestContext.ExecutionEnvironment.IsDevFabricDeployment && requestContext.IsFeatureEnabled("VisualStudio.Services.ExtensionsDisableDemandCheck"))
      {
        requestContext.Trace(100136270, TraceLevel.Info, nameof (DemandExtensionService), "Extensions", "Skipping extension check because DevFabric and 'VisualStudio.Services.ExtensionsDisableDemandCheck' is enabled");
      }
      else
      {
        IInstalledExtensionManager service = requestContext.GetService<IInstalledExtensionManager>();
        string extensionId = GalleryUtil.CreateFullyQualifiedName(publisherName, extensionName);
        requestContext.TraceConditionally(10013600, TraceLevel.Info, nameof (DemandExtensionService), "Extensions", (Func<string>) (() => string.Format("Demanding extension: {0}", (object) extensionId)));
        if (!service.IsExtensionActive(requestContext, publisherName, extensionName, checkLicenseOnFallback))
        {
          InstalledExtension installedExtension = service.GetInstalledExtension(requestContext, publisherName, extensionName, true);
          if (installedExtension != null && installedExtension.InstallState != null && (installedExtension.InstallState.Flags & ExtensionStateFlags.Disabled) == ExtensionStateFlags.Disabled)
          {
            requestContext.Trace(10013569, TraceLevel.Info, nameof (DemandExtensionService), "Extensions", "Extension {0}.{1} is installed but is disabled.", (object) publisherName, (object) extensionName);
            throw new ExtensionNotAvailableException(ExtMgmtResources.ExtensionIsDisabled((object) extensionId));
          }
          requestContext.TraceConditionally(10013605, TraceLevel.Info, nameof (DemandExtensionService), "Extensions", (Func<string>) (() => string.Format("Extension is not active: {0}", (object) extensionId)));
          bool licenseSuccessfullyAssigned = false;
          this.CheckForExtensionRights(requestContext, extensionId, out licenseSuccessfullyAssigned);
          if (!licenseSuccessfullyAssigned)
            throw new ExtensionNotAvailableException(ExtMgmtResources.ExtensionNotInstalled((object) extensionId));
        }
        else
        {
          requestContext.TraceConditionally(10013615, TraceLevel.Info, nameof (DemandExtensionService), "Extensions", (Func<string>) (() => string.Format("Extension is active.  Now check license data: {0}", (object) extensionId)));
          bool inFallbackMode = true;
          if (requestContext.IsFeatureEnabled("VisualStudio.Services.ExtensionsLicenseCheck") && requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection) && !requestContext.RootContext.TryGetItem<bool>("InExtensionFallbackMode", out inFallbackMode) && !inFallbackMode)
          {
            bool licenseSuccessfullyAssigned = false;
            this.CheckForExtensionRights(requestContext, extensionId, out licenseSuccessfullyAssigned, true);
          }
          else
            requestContext.TraceConditionally(10013620, TraceLevel.Info, nameof (DemandExtensionService), "Extensions", (Func<string>) (() => string.Format("Skipping license check for {0}.  ServiceHost: {1} FallbackKeyPreset: {2} FallbackMode: {3}", (object) extensionId, (object) requestContext.ServiceHost.HostType, (object) requestContext.Items.ContainsKey("InExtensionFallbackMode"), (object) inFallbackMode)));
        }
      }
    }

    private void CheckForExtensionRights(
      IVssRequestContext requestContext,
      string extensionId,
      out bool licenseSuccessfullyAssigned,
      bool throwLicensingExceptionOnMissingRights = false)
    {
      licenseSuccessfullyAssigned = false;
      requestContext.TraceConditionally(10013621, TraceLevel.Info, nameof (DemandExtensionService), "Extensions", (Func<string>) (() => string.Format("CheckForExtensionRights for {0}.", (object) extensionId)));
      if (!requestContext.Items.ContainsKey("extension-rights"))
      {
        requestContext.TraceConditionally(10013625, TraceLevel.Info, nameof (DemandExtensionService), "Extensions", (Func<string>) (() => string.Format("CheckForExtensionRights: Missing extension rights key. {0}", (object) extensionId)));
        if (throwLicensingExceptionOnMissingRights)
          throw new ExtensionNotLicensedException(ExtMgmtResources.ExtensionNotLicensed((object) extensionId));
        throw new ExtensionNotAvailableException(ExtMgmtResources.ExtensionNotInstalled((object) extensionId));
      }
      IDictionary<string, bool> extensionRights = (IDictionary<string, bool>) requestContext.Items["extension-rights"];
      if (extensionRights == null)
        return;
      bool flag;
      if (extensionRights.TryGetValue(extensionId, out flag) && !flag)
      {
        requestContext.TraceConditionally(100136230, TraceLevel.Info, nameof (DemandExtensionService), "Extensions", (Func<string>) (() => string.Format("Extension not licensed. {0}. Attempting on demand licensing.", (object) extensionId)));
        if (!ExtensionLicensingUtil.TryApplyingOnDemandLicense(requestContext, extensionId))
          throw new ExtensionNotLicensedException(ExtMgmtResources.ExtensionNotLicensed((object) extensionId));
        licenseSuccessfullyAssigned = true;
      }
      else
        requestContext.TraceConditionally(100136240, TraceLevel.Info, nameof (DemandExtensionService), "Extensions", (Func<string>) (() => string.Join(" | ", extensionRights.Select<KeyValuePair<string, bool>, string>((Func<KeyValuePair<string, bool>, string>) (ext => string.Format("{0}:{1}", (object) ext.Key, (object) ext.Value))))));
    }

    private bool IsRunningAsDeploymentServiceIdentity(IVssRequestContext requestContext)
    {
      Microsoft.VisualStudio.Services.Identity.Identity authenticatedIdentity = requestContext.GetAuthenticatedIdentity();
      return authenticatedIdentity != null && !(authenticatedIdentity.Id == Guid.Empty) && IdentityHelper.IsServiceIdentity(requestContext.To(TeamFoundationHostType.Deployment).Elevate(), (IReadOnlyVssIdentity) authenticatedIdentity);
    }
  }
}
