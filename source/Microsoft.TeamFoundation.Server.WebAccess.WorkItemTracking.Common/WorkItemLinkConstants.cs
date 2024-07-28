// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.WorkItemLinkConstants
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  [GenerateAllConstants(null)]
  public static class WorkItemLinkConstants
  {
    public const string WORKITEMLINKUSAGE = "workItemLink";
    public const string RESOURCELINKUSAGE = "resourceLink";
    public const string ATTACHEDLINKTYPE = "AttachedFile";
    public const string HYPERLINKLINKTYPE = "Hyperlink";
    public const string ARTIFACTLINKTYPE = "ArtifactLink";
    public const string EXTERNALLINKTYPE = "ExternalLink";
    public static readonly string[] RESOURCELINKTYPES = new string[2]
    {
      "Hyperlink",
      "ArtifactLink"
    };
    public const string ATTRIBUTES_USAGE = "usage";
    public const string ATTRIBUTES_EDITABLE = "editable";
    public const string ATTRIBUTES_ENABLED = "enabled";
    public const string ATTRIBUTES_ACYCLIC = "acyclic";
    public const string ATTRIBUTES_DIRECTIONAL = "directional";
    public const string ATTRIBUTES_SINGLETARGET = "singleTarget";
    public const string ATTRIBUTES_TOPOLOGY = "topology";
    public const string ATTRIBUTES_ID = "id";
    public const string ATTRIBUTES_AUTHORIZEDDATE = "authorizedDate";
    public const string ATTRIBUTES_RESOURCECREATEDDATE = "resourceCreatedDate";
    public const string ATTRIBUTES_RESOURCEMODIFIEDDATE = "resourceModifiedDate";
    public const string ATTRIBUTES_REVISEDDATE = "revisedDate";
    public const string ATTRIBUTES_RESOURCESIZE = "resourceSize";
    public const string ATTRIBUTES_COMMENT = "comment";
    public const string ATTRIBUTES_NAME = "name";
    public const string ATTRIBUTES_ISLOCKED = "isLocked";
  }
}
