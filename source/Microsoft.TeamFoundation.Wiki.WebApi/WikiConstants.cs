// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Wiki.WebApi.WikiConstants
// Assembly: Microsoft.TeamFoundation.Wiki.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4A8C8A50-70A8-447A-B2AD-300BEAACF074
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Wiki.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.Wiki.WebApi
{
  [GenerateAllConstants(null)]
  public static class WikiConstants
  {
    public const string AreaId = "BF7D82A0-8AA5-4613-94EF-6172A5EA01F3";
    public const string AreaName = "wiki";
    public const string WikiPathSeparator = "/";
    public const string WikisResourceName = "wikis";
    public const string PagesResourceName = "pages";
    public const string PageMovesResourceName = "pageMoves";
    public const string PageViewStatsResourceName = "pageViewStats";
    public const string AttachmentsResourceName = "attachments";
    public const string PageCommentsResourceName = "pageComments";
    public const string PageCommentAttachmentsResourceName = "pageCommentAttachments";
    public const string PageCommentReactionsResourceName = "pageCommentReactions";
    public const string PageCommentReactionsEngagedUsersResourceName = "pageCommentReactionsEngagedUsers";
    public const string PagesBatchResourceName = "pagesBatch";
    public const string PageStatsResourceName = "pageStats";
    public static string WikiDefaultBranch = "wikiMaster";
    public const string Expand = "$expand";
    public const string Top = "$top";
    public const string Skip = "$skip";
    public const string Order = "order";
    public const string WikisLocationIdString = "288D122C-DBD4-451D-AA5F-7DBBBA070728";
    public static readonly Guid WikisLocationId = new Guid("288D122C-DBD4-451D-AA5F-7DBBBA070728");
    public const string WikiPagesLocationIdString = "25D3FBC7-FE3D-46CB-B5A5-0B6F79CAF27B";
    public static readonly Guid WikiPagesLocationId = new Guid("25D3FBC7-FE3D-46CB-B5A5-0B6F79CAF27B");
    public const string WikiPagesByIdLocationIdString = "CEDDCF75-1068-452D-8B13-2D4D76E1F970";
    public static readonly Guid WikiPagesByIdLocationId = new Guid("CEDDCF75-1068-452D-8B13-2D4D76E1F970");
    public const string WikiPageMovesLocationIdString = "E37BBE71-CBAE-49E5-9A4E-949143B9D910";
    public static readonly Guid WikiPageMovesLocationId = new Guid("E37BBE71-CBAE-49E5-9A4E-949143B9D910");
    public const string WikiAttachmentsLocationIdString = "C4382D8D-FEFC-40E0-92C5-49852E9E17C0";
    public static readonly Guid WikiAttachmentsLocationId = new Guid("C4382D8D-FEFC-40E0-92C5-49852E9E17C0");
    public const string WikiPageViewStatsLocationIdString = "1087b746-5d15-41b9-bea6-14e325e7f880";
    public static readonly Guid WikiPageViewStatsLocationId = new Guid("1087b746-5d15-41b9-bea6-14e325e7f880");
    public const string WikiPageCommentsLocationIdString = "9B394E93-7DB5-46CB-9C26-09A36AA5C895";
    public static readonly Guid WikiPageCommentsLocationId = new Guid("9B394E93-7DB5-46CB-9C26-09A36AA5C895");
    public const string WikiPageCommentAttachmentsLocationIdString = "5100D976-363D-42E7-A19D-4171ECB44782";
    public static readonly Guid WikiPageCommentAttachmentsLocationId = new Guid("5100D976-363D-42E7-A19D-4171ECB44782");
    public const string WikiPageCommentReactionsLocationIdString = "7A5BC693-AAB7-4D48-8F34-36F373022063";
    public static readonly Guid WikiPageCommentReactionsLocationId = new Guid("7A5BC693-AAB7-4D48-8F34-36F373022063");
    public const string WikiPageCommentReactionsEngagedUsersLocationIdString = "598A5268-41A7-4162-B7DC-344131E4D1FA";
    public static readonly Guid WikiPageCommentReactionsEngagedUsersLocationId = new Guid("598A5268-41A7-4162-B7DC-344131E4D1FA");
    public const string WikiPagesBatchLocationIdString = "71323C46-2592-4398-8771-CED73DD87207";
    public static readonly Guid WikiPagesBatchLocationId = new Guid("71323C46-2592-4398-8771-CED73DD87207");
    public const string WikiPageStatsLocationIdString = "81C4E0FE-7663-4D62-AD46-6AB78459F274";
    public static readonly Guid WikiPageStatsLocationId = new Guid("81C4E0FE-7663-4D62-AD46-6AB78459F274");
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static readonly Guid WikiPageArtifactKind = new Guid("C38A95A3-8516-40C1-BC31-247A5008586B");
  }
}
