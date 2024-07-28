// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Common.StoredProcedureNames
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Microsoft.TeamFoundation.WorkItemTracking.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  [StructLayout(LayoutKind.Sequential, Size = 1)]
  public struct StoredProcedureNames
  {
    public const string ForceRollback = "ForceRollback";
    public const string GetForceRollbackErrorCode = "GetForceRollbackErrorCode";
    public const string GetAdminData = "GetAdminData";
    public const string AddFieldValues = "AddFieldValues";
    public const string UpdateTrendData = "prc_UpdateTrendData";
    public const string AddTexts = "AddTexts";
    public const string AddFiles = "AddFiles";
    public const string UpdateFiles = "UpdateFiles";
    public const string RemoveFiles = "RemoveFiles";
    public const string FetchTfsFileIdByFilePath = "FetchTfsFileIdByFilePath";
    public const string FetchTfsFileIdByExtId = "FetchTfsFileIdByExtId";
    public const string GetComputedColumns = "GetComputedColumns";
    public const string ProcessLinkChanges = "ProcessLinkChanges";
    public const string AddWorkItemLink = "AddLink";
    public const string UpdateWorkItemLink = "UpdateLink";
    public const string DeleteWorkItemLink = "DeleteLink";
    public const string AddWorkItemLinkType = "AddLinkType";
    public const string UpdateWorkItemLinkType = "UpdateLinkType";
    public const string DeleteWorkItemLinkType = "DeleteLinkType";
    public const string IssueOpen = "WorkItemOpen";
    public const string AuthorizeChanges = "AuthorizeChanges";
    public const string AuthorizeFieldChanges = "AuthorizeFieldChanges";
    public const string AuthorizeRuleChanges = "AuthorizeRuleChanges";
    public const string AuthorizeWorkItemTypeChanges = "AuthorizeWorkItemTypeChanges";
    public const string AuthorizeActionChanges = "AuthorizeActionChanges";
    public const string ApplyChanges = "ApplyChanges";
    public const string ApplyFieldChanges = "ApplyFieldChanges";
    public const string ApplyRuleChanges = "ApplyRuleChanges";
    public const string ApplyWorkItemTypeChanges = "ApplyWorkItemTypeChanges";
    public const string ApplyActionChanges = "ApplyActionChanges";
    public const string ChangeField = "ChangeField";
    public const string DeleteField = "DeleteField";
    public const string ChangeFieldUsage = "ChangeFieldUsage";
    public const string ChangeTreeProperty = "ChangeTreeProperty";
    public const string ChangeRuleSet = "ChangeRuleSet";
    public const string ChangeRule = "ChangeRule";
    public const string SaveRules = "SaveRules";
    public const string SaveConstantSets = "SaveConstantSets";
    public const string AddConstant = "AddConstant";
    public const string AddConstants = "AddConstants";
    public const string LookupIdentityConstants = "LookupIdentityConstants";
    public const string LookupRule = "LookupRule";
    public const string RebuildCallersViews = "RebuildCallersViews";
    public const string ExplodeSet = "ExplodeSet";
    public const string ExplodeSetIDToSids = "ExplodeSetIDToSIDs";
    public const string WordsContains = "WordsContains";
    public const string CreateIssuesTemplate = "Create{0}s";
    public const string CreateIssuesFromValuesTemplate = "Create{0}sFromValues";
    public const string UpdateIssuesTemplate = "Update{0}s";
    public const string UpdateIssuesFromValuesTemplate = "Update{0}sFromValues";
    public const string AddQueryItem = "AddQueryItem";
    public const string UpdateQueryItem = "UpdateQueryItem";
    public const string DeleteQueryItem = "DeleteQueryItem";
    public const string AuthorizeQueryItemChanges = "AuthorizeQueryItemChanges";
    public const string GetStoredQueries = "GetIncrementalStoredQueries";
    public const string GetStoredQueryItems = "GetQueryItems";
    public const string GetStoredQuery = "GetStoredQuery";
    public const string GetQueryItemFromPath = "GetQueryItemFromPath";
    public const string GetQueryItemAndChildren = "GetQueryItemAndChildren";
    public const string GetQueryItemSecurityInfo = "GetQueryItemSecurityInfo";
    public const string GetQueryItemSubtreeSecurityInfo = "GetQueryItemSubtreeSecurityInfo";
    public const string ChangeWorkItemType = "ChangeWorkItemType";
    public const string ChangeWorkItemTypeUsage = "ChangeWorkItemTypeUsage";
    public const string ChangeAction = "ChangeAction";
    public const string ProjectDelete = "ProjectDelete";
    public const string BoxCarAdminDataChanges = "BoxCarAdminDataChanges";
    public const string GetSequenceIds = "GetSequenceIds";
    public const string StampDb = "StampDb";
    public const string WorkitemServiceView = "WorkItemServiceView";
    public const string ProcessIdentities = "ProcessIdentities1";
    public const string DestroyWorkItems = "DestroyWorkItems";
    public const string DestroyWorkItemType = "DestroyWorkItemType";
    public const string RenameWorkItemType = "RenameWorkItemType";
    public const string DestroyGlobalList = "DestroyGlobalList";
    public const string ChangeWorkItemTypeCategory = "ChangeWorkItemTypeCategory";
    public const string CheckWorkItemTypeCategory = "CheckWorkItemTypeCategory";
    public const string DestroyWorkItemTypeCategory = "DestroyWorkItemTypeCategory";
    public const string AddWorkItemTypeCategoryMember = "AddWorkItemTypeCategoryMember";
    public const string DeleteWorkItemTypeCategoryMember = "DeleteWorkItemTypeCategoryMember";
    public const string EventChanges = "EventChange";
    public const string SubTreeFromId = "SubTreeFromID";
    public const string GetProjectID = "GetProjectID";
    public const string IsNamespaceAdminOrProjectAdmin = "IsNamespaceAdminOrProjectAdmin";
  }
}
