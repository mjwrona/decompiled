// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CollectionServicingTokens
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.TeamFoundation
{
  public static class CollectionServicingTokens
  {
    public const string ProvisionCollectionCreate = "ProvisionCollectionCreate";
    public const string NoAction = "None";
    public const string ReportingActionToken = "ReportingAction";
    public const string ReportServerToken = "ReportServer";
    public const string ReportFolderToken = "ReportFolder";
    public const string CreateFolderAction = "CreateFolder";
    public const string UseExistingFolderAction = "UseExistingFolder";
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static readonly ICollection<string> ReportingTokens = (ICollection<string>) Array.AsReadOnly<string>(new string[3]
    {
      "ReportingAction",
      "ReportServer",
      "ReportFolder"
    });
    public const string SharePointActionToken = "SharePointAction";
    public const string SharePointSitePathToken = "SharePointSitePath";
    public const string SharePointServerToken = "SharePointServer";
    public const string SharePointSiteOwnerToken = "SharePointSiteOwner";
    public const string CreateSiteAction = "CreateSite";
    public const string UseExistingSiteAction = "UseExistingSite";
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static readonly ICollection<string> SharePointTokens = (ICollection<string>) Array.AsReadOnly<string>(new string[4]
    {
      "SharePointAction",
      "SharePointServer",
      "SharePointSitePath",
      "SharePointSiteOwner"
    });
    public const string WarehouseActionToken = "WarehouseAction";
    public const string IsHostedCollection = "IsHostedCollection";
    public const string HostedIdentityMappings = "HostedIdentityMappings";
  }
}
