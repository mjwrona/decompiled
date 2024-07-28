// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Server.PlatformExtensionService
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 57F50803-C5C4-41A9-A26F-AD293D563111
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ExtensionManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Server
{
  internal class PlatformExtensionService : IExtensionService, IVssFrameworkService
  {
    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public List<ExtensionState> GetExtensionStates(
      IVssRequestContext requestContext,
      bool includeDisabledExtensions,
      bool includeErrors,
      bool forceRefresh)
    {
      return requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) ? new List<ExtensionState>() : requestContext.GetService<IInstalledExtensionService>().GetInstalledExtensionStates(requestContext, includeDisabledExtensions, includeErrors, forceRefresh);
    }

    public List<InstalledExtension> GetInstalledExtensions(
      IVssRequestContext requestContext,
      InstalledExtensionQuery extensionQuery)
    {
      return requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) ? new List<InstalledExtension>() : requestContext.GetService<IInstalledExtensionService>().GetInstalledExtensions(requestContext, extensionQuery);
    }

    public List<InstalledExtension> GetInstalledExtensions(
      IVssRequestContext requestContext,
      bool? includeDisabledExtensions = null,
      bool? includeErrors = null,
      IEnumerable<string> assetTypes = null,
      bool? includeInstallationIssues = null)
    {
      return requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) ? new List<InstalledExtension>() : requestContext.GetService<IInstalledExtensionService>().GetInstalledExtensions(requestContext, includeDisabledExtensions.HasValue && includeInstallationIssues.Value, includeErrors.HasValue && includeErrors.Value, assetTypes, includeInstallationIssues.HasValue && includeInstallationIssues.Value);
    }
  }
}
