// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.InstalledExtenionsController
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3FE9DD3E-5758-4D1B-8056-ECAF8A4B7A77
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server
{
  [VersionedApiControllerCustomName(Area = "Contribution", ResourceName = "InstalledApps")]
  public class InstalledExtenionsController : TfsApiController
  {
    public override string TraceArea => "Contributions";

    public override string ActivityLogArea => "Extensions";

    [HttpGet]
    [ClientLocationId("3E2F6668-0798-4DCB-B592-BFE2FA57FDE2")]
    public InstalledExtension GetInstalledExtensionByName(
      string publisherName,
      string extensionName,
      [ClientParameterAsIEnumerable(typeof (string), ':')] string assetTypes = null)
    {
      HashSet<string> assetTypes1 = (HashSet<string>) null;
      ArgumentUtility.CheckStringForNullOrEmpty(publisherName, nameof (publisherName));
      ArgumentUtility.CheckStringForNullOrEmpty(extensionName, nameof (extensionName));
      InstalledExtension installedExtension = this.TfsRequestContext.GetService<IInstalledExtensionManager>().GetInstalledExtension(this.TfsRequestContext, publisherName, extensionName);
      if (assetTypes != null)
        assetTypes1 = new HashSet<string>((IEnumerable<string>) assetTypes.Split(new char[1]
        {
          ':'
        }, StringSplitOptions.RemoveEmptyEntries), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      return this.PrepareExtensionForClient(this.TfsRequestContext, installedExtension, assetTypes1);
    }

    [HttpGet]
    [ClientLocationId("2648442B-FD63-4B9A-902F-0C913510F139")]
    public List<InstalledExtension> GetInstalledExtensions(
      [ClientParameterAsIEnumerable(typeof (string), ';')] string contributionIds = null,
      bool includeDisabledApps = true,
      [ClientParameterAsIEnumerable(typeof (string), ':')] string assetTypes = null)
    {
      IInstalledExtensionManager service = this.TfsRequestContext.GetService<IInstalledExtensionManager>();
      HashSet<string> assetTypes1 = (HashSet<string>) null;
      List<InstalledExtension> installedExtensions;
      if (!string.IsNullOrEmpty(contributionIds))
      {
        string[] targetContributionIds = contributionIds.Split(new char[1]
        {
          ';'
        }, StringSplitOptions.RemoveEmptyEntries);
        installedExtensions = service.GetInstalledExtensions(this.TfsRequestContext, (IEnumerable<string>) targetContributionIds);
      }
      else
        installedExtensions = service.GetInstalledExtensions(this.TfsRequestContext, includeDisabledApps);
      if (assetTypes != null)
        assetTypes1 = new HashSet<string>((IEnumerable<string>) assetTypes.Split(new char[1]
        {
          ':'
        }, StringSplitOptions.RemoveEmptyEntries), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      for (int index = 0; index < installedExtensions.Count; ++index)
        installedExtensions[index] = this.PrepareExtensionForClient(this.TfsRequestContext, installedExtensions[index], assetTypes1);
      return installedExtensions;
    }

    private InstalledExtension PrepareExtensionForClient(
      IVssRequestContext requestContext,
      InstalledExtension installedExtension,
      HashSet<string> assetTypes)
    {
      if (installedExtension == null)
        return (InstalledExtension) null;
      InstalledExtension installedExtension1;
      if (assetTypes != null && assetTypes.Contains("*"))
      {
        installedExtension1 = installedExtension;
      }
      else
      {
        List<ExtensionFile> extensionFileList = (List<ExtensionFile>) null;
        if (assetTypes != null)
        {
          foreach (ExtensionFile file in installedExtension.Files)
          {
            if (assetTypes.Contains(file.AssetType))
            {
              if (extensionFileList == null)
                extensionFileList = new List<ExtensionFile>();
              extensionFileList.Add(file);
            }
          }
        }
        installedExtension1 = new InstalledExtension(installedExtension);
        installedExtension1.Files = (IEnumerable<ExtensionFile>) extensionFileList;
      }
      return installedExtension1;
    }
  }
}
