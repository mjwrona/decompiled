// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.CoreFieldReferenceNames
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  public static class CoreFieldReferenceNames
  {
    public static readonly string AreaId = "System.AreaId";
    public static readonly string AreaPath = "System.AreaPath";
    public static readonly string AssignedTo = "System.AssignedTo";
    public static readonly string AttachedFileCount = "System.AttachedFileCount";
    public static readonly string AuthorizedAs = "System.AuthorizedAs";
    public static readonly string BoardColumn = "System.BoardColumn";
    public static readonly string BoardColumnDone = "System.BoardColumnDone";
    public static readonly string BoardLane = "System.BoardLane";
    public static readonly string ChangedBy = "System.ChangedBy";
    public static readonly string ChangedDate = "System.ChangedDate";
    public static readonly string CreatedBy = "System.CreatedBy";
    public static readonly string CreatedDate = "System.CreatedDate";
    public static readonly string Description = "System.Description";
    public static readonly string CommentCount = "System.CommentCount";
    public static readonly string ExternalLinkCount = "System.ExternalLinkCount";
    public static readonly string History = "System.History";
    public static readonly string HyperLinkCount = "System.HyperLinkCount";
    public static readonly string Id = "System.Id";
    public static readonly string IsDeleted = "System.IsDeleted";
    public static readonly string IterationId = "System.IterationId";
    public static readonly string IterationPath = "System.IterationPath";
    public static readonly string LinkType = "System.Links.LinkType";
    public static readonly string NodeName = "System.NodeName";
    public static readonly string Parent = "System.Parent";
    public static readonly string Reason = "System.Reason";
    public static readonly string RelatedLinkCount = "System.RelatedLinkCount";
    public static readonly string RemoteLinkCount = "System.RemoteLinkCount";
    public static readonly string Rev = "System.Rev";
    public static readonly string RevisedDate = "System.RevisedDate";
    public static readonly string State = "System.State";
    public static readonly string Tags = "System.Tags";
    public static readonly string TeamProject = "System.TeamProject";
    public static readonly string Title = "System.Title";
    public static readonly string WorkItemType = "System.WorkItemType";
    public static readonly string SystemFieldPrefix = "System.";
    private static string[] m_all = new string[30]
    {
      CoreFieldReferenceNames.AreaId,
      CoreFieldReferenceNames.AreaPath,
      CoreFieldReferenceNames.ExternalLinkCount,
      CoreFieldReferenceNames.AssignedTo,
      CoreFieldReferenceNames.AttachedFileCount,
      CoreFieldReferenceNames.AuthorizedAs,
      CoreFieldReferenceNames.ChangedBy,
      CoreFieldReferenceNames.ChangedDate,
      CoreFieldReferenceNames.CreatedBy,
      CoreFieldReferenceNames.CreatedDate,
      CoreFieldReferenceNames.Description,
      CoreFieldReferenceNames.CommentCount,
      CoreFieldReferenceNames.History,
      CoreFieldReferenceNames.HyperLinkCount,
      CoreFieldReferenceNames.Id,
      CoreFieldReferenceNames.IsDeleted,
      CoreFieldReferenceNames.IterationId,
      CoreFieldReferenceNames.IterationPath,
      CoreFieldReferenceNames.NodeName,
      CoreFieldReferenceNames.Parent,
      CoreFieldReferenceNames.Reason,
      CoreFieldReferenceNames.RelatedLinkCount,
      CoreFieldReferenceNames.RemoteLinkCount,
      CoreFieldReferenceNames.Rev,
      CoreFieldReferenceNames.RevisedDate,
      CoreFieldReferenceNames.State,
      CoreFieldReferenceNames.TeamProject,
      CoreFieldReferenceNames.Title,
      CoreFieldReferenceNames.WorkItemType,
      CoreFieldReferenceNames.Tags
    };

    public static string[] All => CoreFieldReferenceNames.m_all;

    public static int Count => CoreFieldReferenceNames.m_all.Length;
  }
}
