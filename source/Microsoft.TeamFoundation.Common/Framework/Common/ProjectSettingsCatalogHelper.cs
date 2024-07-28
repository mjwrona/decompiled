// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Common.ProjectSettingsCatalogHelper
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using Microsoft.TeamFoundation.Framework.Common.Catalog.Objects;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class ProjectSettingsCatalogHelper
  {
    public static List<Guid> GetProjectSettingsResourceTypes<T>(bool includeReportingAndSharePoint = true) => includeReportingAndSharePoint ? ((IEnumerable<Guid>) new Guid[5]
    {
      CatalogResourceTypes.ReportingConfiguration,
      CatalogResourceTypes.ProjectCollection,
      CatalogResourceTypes.SharePointSiteCreationLocation,
      CatalogResourceTypes.ReportingFolder,
      CatalogResourceTypes.TeamProject
    }).Union<Guid>((IEnumerable<Guid>) CatalogObjectUtilities.GetKnownDescendantTypes<T>()).ToList<Guid>() : ((IEnumerable<Guid>) new Guid[2]
    {
      CatalogResourceTypes.ProjectCollection,
      CatalogResourceTypes.TeamProject
    }).ToList<Guid>();

    public static string[] GetProjectCollectionPathSpecs(string tpcFullPath) => new string[3]
    {
      tpcFullPath,
      tpcFullPath + CatalogConstants.FullRecurseStars,
      CatalogRoots.OrganizationalPath + CatalogConstants.SingleRecurseStar
    };
  }
}
