// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Common.Constants.CoreFieldRefNames
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.WorkItemTracking.Common.Constants
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  [GenerateAllConstants(null)]
  public static class CoreFieldRefNames
  {
    public const string AreaId = "System.AreaId";
    public const string AreaPath = "System.AreaPath";
    public const string AssignedTo = "System.AssignedTo";
    public const string AttachedFileCount = "System.AttachedFileCount";
    public const string AuthorizedAs = "System.AuthorizedAs";
    public const string BoardColumn = "System.BoardColumn";
    public const string BoardColumnDone = "System.BoardColumnDone";
    public const string BoardLane = "System.BoardLane";
    public const string ChangedBy = "System.ChangedBy";
    public const string ChangedDate = "System.ChangedDate";
    public const string CreatedBy = "System.CreatedBy";
    public const string CreatedDate = "System.CreatedDate";
    public const string Description = "System.Description";
    public const string CommentCount = "System.CommentCount";
    public const string ExternalLinkCount = "System.ExternalLinkCount";
    public const string History = "System.History";
    public const string HyperLinkCount = "System.HyperLinkCount";
    public const string RemoteLinkCount = "System.RemoteLinkCount";
    public const string Id = "System.Id";
    public const string IterationId = "System.IterationId";
    public const string IterationPath = "System.IterationPath";
    public const string LinkType = "System.Links.LinkType";
    public const string NodeName = "System.NodeName";
    public const string Parent = "System.Parent";
    public const string Reason = "System.Reason";
    public const string RelatedLinkCount = "System.RelatedLinkCount";
    public const string Rev = "System.Rev";
    public const string RevisedDate = "System.RevisedDate";
    public const string State = "System.State";
    public const string AuthorizedDate = "System.AuthorizedDate";
    public const string TeamProject = "System.TeamProject";
    public const string Tags = "System.Tags";
    public const string Title = "System.Title";
    public const string WorkItemType = "System.WorkItemType";
    public const string Watermark = "System.Watermark";
    public const string IsDeleted = "System.IsDeleted";
    private static string[] m_all = new string[31]
    {
      "System.AuthorizedDate",
      "System.AreaId",
      "System.AreaPath",
      "System.ExternalLinkCount",
      "System.AssignedTo",
      "System.AttachedFileCount",
      "System.AuthorizedAs",
      "System.ChangedBy",
      "System.ChangedDate",
      "System.CreatedBy",
      "System.CreatedDate",
      "System.Description",
      "System.CommentCount",
      "System.History",
      "System.HyperLinkCount",
      "System.Id",
      "System.IterationId",
      "System.IterationPath",
      "System.Links.LinkType",
      "System.NodeName",
      "System.Reason",
      "System.RelatedLinkCount",
      "System.RemoteLinkCount",
      "System.Rev",
      "System.RevisedDate",
      "System.State",
      "System.TeamProject",
      "System.Title",
      "System.Watermark",
      "System.WorkItemType",
      "System.IsDeleted"
    };

    public static IEnumerable<string> All => (IEnumerable<string>) CoreFieldRefNames.m_all;
  }
}
