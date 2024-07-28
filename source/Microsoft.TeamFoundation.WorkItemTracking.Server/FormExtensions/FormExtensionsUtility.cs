// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.FormExtensions.FormExtensionsUtility
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.FormExtensions
{
  public class FormExtensionsUtility
  {
    public static IEnumerable<Contribution> GetFilteredContributions(
      IVssRequestContext requestContext,
      string type = null)
    {
      IEnumerable<Contribution> contributions = requestContext.GetService<IContributionService>().QueryContributionsForTarget(requestContext, "ms.vss-work-web.work-item-form");
      if (contributions == null || !contributions.Any<Contribution>())
        return Enumerable.Empty<Contribution>();
      return type != null ? FormExtensionsUtility.GetFilteredContributionsByType(contributions, type) : contributions;
    }

    public static string GetContributionName(Contribution contribution)
    {
      string str = (string) null;
      if (contribution.Properties["name"] != null)
        str = contribution.Properties["name"].ToString();
      return !string.IsNullOrWhiteSpace(str) ? str : Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.MissingContributionName((object) contribution.Id);
    }

    public static int GetContributionHeight(Contribution contribution, int defaultHeight)
    {
      int result;
      return contribution.Properties["height"] != null && int.TryParse(contribution.Properties["height"].ToString(), out result) ? result : defaultHeight;
    }

    public static bool GetShowOnDeletedWorkItem(Contribution contribution)
    {
      bool result = false;
      if (contribution.Properties["showOnDeletedWorkItem"] != null)
        bool.TryParse(contribution.Properties["showOnDeletedWorkItem"].ToString(), out result);
      return result;
    }

    public static IEnumerable<Contribution> GetFilteredContributionsByType(
      IEnumerable<Contribution> contributions,
      string type)
    {
      return (IEnumerable<Contribution>) contributions.Where<Contribution>((Func<Contribution, bool>) (c => VssStringComparer.ExtensionType.Equals(c.Type, type))).ToList<Contribution>();
    }

    public static IEnumerable<Contribution> GetFilteredContributionsByExtensionsAndType(
      IEnumerable<Contribution> contributions,
      IEnumerable<string> extensionIds,
      string type)
    {
      HashSet<string> extensionIdsLookup = new HashSet<string>(extensionIds, (IEqualityComparer<string>) VssStringComparer.ExtensionName);
      return (IEnumerable<Contribution>) contributions.Where<Contribution>((Func<Contribution, bool>) (contribution =>
      {
        ContributionIdentifier contributionIdentifier = new ContributionIdentifier(contribution.Id);
        return extensionIdsLookup.Contains(GalleryUtil.CreateFullyQualifiedName(contributionIdentifier.PublisherName, contributionIdentifier.ExtensionName)) && VssStringComparer.ExtensionType.Equals(contribution.Type, type);
      })).ToList<Contribution>();
    }

    public static IEnumerable<InstalledExtension> GetFormExtensions(
      IVssRequestContext requestContext)
    {
      return (IEnumerable<InstalledExtension>) requestContext.GetService<IInstalledExtensionManager>().GetInstalledExtensions(requestContext, (IEnumerable<string>) new string[1]
      {
        "ms.vss-work-web.work-item-form"
      }) ?? Enumerable.Empty<InstalledExtension>();
    }
  }
}
