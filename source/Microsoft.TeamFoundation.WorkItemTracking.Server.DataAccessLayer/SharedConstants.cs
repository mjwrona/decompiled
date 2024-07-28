// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.SharedConstants
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal sealed class SharedConstants
  {
    public const int SqlParametersLimit = 2098;
    public const int FileCommentLength = 255;
    public const int NumberOfMetadataTables = 13;
    public const int RevBatchReadLatest = -1;
    public const string SuccessValue = "0";
    public const string ParameterPrefix = "@P";
    public const string TempIdsXpath = ".//*[@TempID!='']";
    public const string BypassRulesXpath = ".//*[@BypassRules!='']";
    public const int MaxItemsInUpdatePackage = 200;
    public const string ClientCapabilitiesXpath = "//@ClientCapabilities";
    public const string InsertUpdatePackageXPath = "//InsertWorkItem|//UpdateWorkItem";
    public const string UpdateIssueWorkitemIdsXpath = "//UpdateWorkItem[@WorkItemID!='']";
    public const string ObjectIdVariable = "@O";
    public const string VerboseVariable = "@fVerbose";
    public const string RollbackVariable = "@fRollback";
    public const string ATRollbackVariable = "@fATRollback";
    public const string AdminVariable = "@fAdmin";
    public const string ForceRollbackError = "@ForceRollbackError";
    public const string PersonIdVariable = "@PersonId";
    public const string UserSidVariable = "@userSID";
    public const string UserVsidVariable = "@userVSID";
    public const string NowUtcVariable = "@NowUtc";
    public const string RowVersionVariable = "@RowVer";
    public const string CollationVariable = "@Collation";
    public const string FileIdVariable = "@FileId";
    public const string ProjectID = "@projectId";
    public const string SyncTime = "@SyncTime";
    public const string IsBulkUpdateVariable = "@isBulkUpdate";
    public const string BypassRulesVariable = "@bypassRules";
    public const string ConstantsVariable = "@constants";
    public const string IdentityConstantsVariable = "@identityConstants";
    public const string RulesVariable = "@rules";
    public const string IdentityRulesVariable = "@identityRules";
    public const string SetsVariable = "@sets";
    public const string TempIdMapVariable = "@tempIdMap";
    public const string PersonNamesVariable = "@personNames";
    public const string PersonIdMapVariable = "@personIdMap";
    public const string WorkItemTypeId = "@workItemTypeId";
    public const string WorkItemTypeNameConstId = "@workItemTypeNameConstId";
    public const string PartitionIdVariable = "@partitionId";
    public const string StatusVariable = "@status";
    public const string CustomFieldValues = "@customFieldValues";
    public const string WorkItemTable1 = "WorkItemInfo";
    public const string WorkItemTable2 = "Revisions";
    public const string WorkItemTable3 = "Keywords";
    public const string WorkItemTable4 = "Texts";
    public const string WorkItemTable5 = "Files";
    public const string WorkItemTable6 = "Relations";
    public const string WorkItemTable7 = "RelationRevisions";
    public const string WorkItemXmlTable1 = "WorkItemInfo";
    public const string WorkItemXmlTable2 = "Texts";
    public const string WorkItemXmlTable3 = "History";
    public const string WorkItemXmlTable4 = "Files";
    public const string WorkItemXmlTable5 = "Hyperlinks";
    public const string WorkItemXmlTable6 = "RelatedLinks";
    public const string WorkItemXmlTable7 = "Fields";
    public const string WorkItemXmlTable8 = "ExternalLinks";
    public const int MaxSyncBatchSize = 250;
    public const string ClientUser = "clientUser";
    public const string TablesRequested = "tablesRequested";
    public const string RowVersions = "rowVersions";
    public const string MetadataPayload = "metadataPayload";
    public const string QueriesPayload = "queriesPayload";
    public const string QueryXml = "queryXml";
    public const string QueryItemId = "queryItemId";
    public const string UpdateElement = "updateElement";
    public const string ProjectURI = "projectURI";
    public const string BuildName = "BuildName";
    public const string Project = "Project";
    public const string Uri = "Uri";
    public const string IdentitySid = "identitySid";
    public const string SourceReader = "sourceReader";
    public const string Writer = "writer";
    public const string Index = "index";
    public const string Name = "name";
    public const string ColumnName = "ColumnName";
    public const string TableName = "TableName";
    public const string Reader = "reader";
    public const string Table = "table";
    public const int InvalidAreaID = 0;
    public const int InvalidIssueID = 0;
    public const string DateTimeFormat = "yyyy-MM-ddTHH:mm:ss.fff";
    public const int QueryItemNameMaxLength = 255;
    public const char QueryItemPathLeftDelimiter = '«';
    public const char QueryItemPathRightDelimiter = '»';
    public const char QueryItemPathSlash = '⁄';
    public const string TempTableRuleIds = "#ruleIds";
    public const string TempTableWorkItemTypeUsageIds = "#workItemTypeUsageIds";
    public const string TempTableActionIds = "#actionIds";
    public const string TempTableCategoryIds = "#categoryIds";
    public const string TempTableCategoryMemberIds = "#categoryMembers";
    public const string TempTableSets = "#sets";
    public const string WorkItemTypeIdColumnName = "WorkItemTypeID";
    public const string IsContextFromPromotion = "isContextFromPromotion";

    private SharedConstants()
    {
    }
  }
}
