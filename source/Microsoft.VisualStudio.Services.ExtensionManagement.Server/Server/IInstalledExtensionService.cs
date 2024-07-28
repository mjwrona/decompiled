// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Server.IInstalledExtensionService
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 57F50803-C5C4-41A9-A26F-AD293D563111
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ExtensionManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Server
{
  [DefaultServiceImplementation(typeof (InstalledExtensionService))]
  public interface IInstalledExtensionService : IVssFrameworkService
  {
    InstalledExtension GetInstalledExtension(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      IEnumerable<string> assetTypes = null,
      bool includeInstallationIssues = false);

    List<ExtensionState> GetInstalledExtensionStates(
      IVssRequestContext requestContext,
      bool includeDisabledExtensions,
      bool includeErrors,
      bool includeInstallationIssues = false,
      bool forceRefresh = false);

    List<InstalledExtension> GetInstalledExtensions(
      IVssRequestContext requestContext,
      bool includeDisabledExtensions,
      bool includeErrors,
      IEnumerable<string> assetTypes = null,
      bool includeInstallationIssues = false);

    List<InstalledExtension> GetInstalledExtensions(
      IVssRequestContext requestContext,
      InstalledExtensionQuery query);

    InstalledExtension InstallExtension(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      string version = null);

    void UninstallExtension(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      string reason = null,
      string reasonCode = null);

    InstalledExtension UpdateExtension(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      ExtensionStateFlags flags,
      string version);

    ExtensionAuthorization AuthorizeExtension(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      Guid registrationId);

    void UpdateExtensionInstallCount(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName);
  }
}
