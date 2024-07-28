// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Client.CoreFieldReferenceNames
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.ClientSlim, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 946B9068-2299-475E-A3F8-BCA3E57420E0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.ClientSlim.dll

using Microsoft.TeamFoundation.WorkItemTracking.Common.Constants;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Client
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
    public static readonly string CommentCount = "System.CommentCount";
    public static readonly string CreatedBy = "System.CreatedBy";
    public static readonly string CreatedDate = "System.CreatedDate";
    public static readonly string Description = "System.Description";
    public static readonly string ExternalLinkCount = "System.ExternalLinkCount";
    public static readonly string History = "System.History";
    public static readonly string HyperLinkCount = "System.HyperLinkCount";
    public static readonly string Id = "System.Id";
    public static readonly string IterationId = "System.IterationId";
    public static readonly string IterationPath = "System.IterationPath";
    public static readonly string LinkType = "System.Links.LinkType";
    public static readonly string NodeName = "System.NodeName";
    public static readonly string Parent = "System.Parent";
    public static readonly string Reason = "System.Reason";
    public static readonly string RelatedLinkCount = "System.RelatedLinkCount";
    public static readonly string Rev = "System.Rev";
    public static readonly string RevisedDate = "System.RevisedDate";
    public static readonly string State = "System.State";
    public static readonly string AuthorizedDate = "System.AuthorizedDate";
    public static readonly string TeamProject = "System.TeamProject";
    public static readonly string Tags = "System.Tags";
    public static readonly string Title = "System.Title";
    public static readonly string WorkItemType = "System.WorkItemType";
    public static readonly string Watermark = "System.Watermark";
    public static readonly string IsDeleted = "System.IsDeleted";

    public static string[] All => CoreFieldRefNames.All.ToArray<string>();

    public static int Count => CoreFieldReferenceNames.All.Length;
  }
}
