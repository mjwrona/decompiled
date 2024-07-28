// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Common.VersionControlIntegrationConstants
// Assembly: Microsoft.TeamFoundation.VersionControl.Common.Integration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2A9D748-4CEE-4498-9785-584B91A44F85
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Common.Integration.dll

namespace Microsoft.TeamFoundation.VersionControl.Common
{
  public static class VersionControlIntegrationConstants
  {
    public const string WorkItemToolIdentifier = "WorkItemTracking";
    public const string ServerPathExtendedAttributeString = "ServerPath";
    public const string RepositoryUrlExtendedAttributeString = "RepositoryUrl";
    public const string ChangesetArtifact = "changeset.aspx";
    public const string ChangesetUrlSuffix = "/VersionControl/changeset.aspx";
    public const string ChangesetTransformUrlSuffix = "/V1.0/Transforms/{0}/changeset.xsl";
    public const string VersionedItemArtifact = "versionedItem.aspx";
    public const string VersionedItemUrlSuffix = "/VersionControl/versionedItem.aspx";
    public const string VersionedItemTransformUrlSuffix = "/V1.0/Transforms/VersionedItem.xsl";
    public const string LatestItemVersionArtifact = "latestItemVersion.aspx";
    public const string LatestItemVersionUrlSuffix = "/VersionControl/latestItemVersion.aspx";
    public const string LatestItemVersionTransformUrlSuffix = "/V1.0/Transforms/LatestItemVersion.xsl";
    public const string LabelArtifact = "label.aspx";
    public const string LabelUrlSuffix = "/VersionControl/label.aspx";
    public const string LabelTransformUrlSuffix = "/V1.0/Transforms/{0}/Label.xsl";
    public const string CheckinNoteArtifact = "checkinNote.aspx";
    public const string CheckinNoteUrlSuffix = "/V1.0/checkinNote.aspx";
    public const string ShelvesetArtifact = "shelveset.aspx";
    public const string ShelvesetUrlSuffix = "/VersionControl/shelveset.aspx";
    public const string ShelvesetTransformUrlSuffix = "/V1.0/Transforms/{0}/shelveset.xsl";
    public const string ShelvedItemArtifact = "shelvedItem.aspx";
    public const string ShelvedItemUrlSuffix = "/VersionControl/shelvedItem.aspx";
    public const string ShelvedItemTransformUrlSuffix = "/V1.0/Transforms/shelvedItem.xsl";
    public const string ChangesetVersionQueryString = "changesetVersion";
    public const string ChangesetVersionDelimiter = "&";
    public const string ChangesetVersionQueryUrlString = "&changesetVersion=";
    public const string DeletionIdString = "deletionId";
    public const string DeletionIdDelimiter = "&";
    public const string DeletionIdUrlString = "&deletionId=";
    public const string ShelvesetNameQueryString = "shelvesetName";
    public const string ShelvesetNameDelimiter = "&";
    public const string ShelvesetNameQueryUrlString = "&shelvesetName=";
    public const string ShelvesetOwnerQueryString = "shelvesetOwner";
    public const string ShelvesetOwnerDelimiter = "&";
    public const string ShelvesetOwnerQueryUrlString = "&shelvesetOwner=";
    public const string WebViewQueryString = "webView";
    public const string VersionedItemExtendedUrl = "&webView=true";
    public const string ChangesetExtendedUrl = "&webView=true";
    public const string ChangesetNormalUrl = "&webView=false";
    public const string ShelvedItemExtendedUrl = "&webView=true";
    public const string ShelvesetExtendedUrl = "&webView=true";
    public const string ShelvesetNormalUrl = "&webView=false";
  }
}
