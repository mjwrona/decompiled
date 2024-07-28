// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Common.SharedVariables
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Microsoft.TeamFoundation.WorkItemTracking.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  [StructLayout(LayoutKind.Sequential, Size = 1)]
  public struct SharedVariables
  {
    public const string GlobalListsSetName = "299f07ef-6201-41b3-90fc-03eeb3977587";
    public const int ConstValidUser = -2;
    public const int ConstEmptyValue = -10000;
    public const int ConstSameAsOldValue = -10001;
    public const int ConstCurrentUser = -10002;
    public const int ConstWasEmptyValue = -10006;
    public const int ConstOldValueInOtherField = -10012;
    public const int ConstServerDateTime = -10013;
    public const int ConstUtcDateTime = -10028;
    public const int ConstServerCurrentUser = -10026;
    public const int ConstLocalDateTime = -10027;
    public const int ConstValueInOtherField = -10025;
    public const int ConstWasEmptyOrSameAsOldValue = -10022;
    public const int ConstServerRandomGuid = -10031;
    public const int ConstBecameNonEmptyValue = -10014;
    public const int ConstRemainedNonEmptyValue = -10015;
    public const int FldIssue = -100;
    public const int FldState = 2;
    public const int FldReason = 22;
    public const int FldWorkItemType = 25;
    public const int FldForm = -14;
    public const int FldAttachedFiles = 50;
    public const int FldHiddenAttachedFiles = 49;
    public const int FldWorkItemLink = -101;
    public const int FldWorkItemTypeExtension = -99;
    public const int FldTree = -102;
    public const int IssueOpenNumberOfReturnedTables = 7;
    public const int WorkitemXmlNumberOfReturnedTables = 7;
    public const int FieldNameMaximum = 128;
    public const int FieldReferenceNameMaximum = 70;
    public const int ActionSizeMaximum = 255;
    public const int TreePropertyNameMaxLength = 120;
    public const int ConstantNameMaxLength = 256;
    public const int DomainNameMaxLength = 256;
    public const string FieldNameRegularExpression = "^[^\\.\\[\\]]+$";
    public const string FieldReferenceNameRegularExpression = "^[a-zA-Z_][a-zA-Z0-9_]*(\\.[a-zA-Z0-9_]+)+$";
    public const string WarehouseTableFields = "Fields";
    public const string WarehouseTableTreeNodes = "TreeNodes";
    public const string WarehouseTableChangedRevisions = "ChangedRevisions";
    public const string WarehouseTablePreviousRevisions = "PreviousRevisions";
    public const string WarehouseTableWorkItemFiles = "WorkItemFiles";
    public const string WarehouseTableRelatedWorkItems = "RelatedWorkItems";
    public const string WarehouseTablePeople = "People";
    public const string WarehouseTableBatchConstIds = "BatchConstIds";
    public const string WarehouseTableChangedDimensionRevisions = "ChangedDimensionRevisions";
    public const string WarehouseTableChangedFactRevisions = "ChangedFactRevisions";
    public const string WarehouseTableLargestChangedOrder = "LargestChangedOrder";
    public const string WarehouseTableChangesetLinks = "ChangesetLinks";
    public const string WarehouseTableTestResultLinks = "TestResultLinks";
    public const string WarehouseTableLinks = "Links";
    public const string WarehouseTableLargestLinkTimeStamp = "LargestLinkTimeStamp";
    public const string WarehouseTableLinkType = "LinkType";
    public const string WarehouseTableLargestLinkTypeCacheStamp = "LargestLinkTypeCacheStamp";
    public const string WarehouseTableDestroyedWorkItems = "WorkItems";
    public const string WarehouseTableDestroyedWorkItemStamp = "WIChangedOrder";
    public const string WarehouseTableRenamedWorkItemTypes = "WorkItemTypes";
    public const string WarehouseTableWorkItemTypeCategories = "WorkItemTypeCategories";
    public const string WarehouseTableWorkItemTypeCategoryMembers = "WorkItemTypeCategoryMembers";
    public const string BisRegBackendDatabaseName = "WIT DB";
    public const int StringFieldLength = 256;
    public const int AttachmentFileNameMaximum = 255;
    public const int AttachmentCommentMaximum = 255;
    public const int LinkStringMaxLength = 2083;
    public const int LinkStringMaxComment = 255;
    public const int MaxNewAttachments = 1000;
    public const string FileId = "FileID";
    public const string FileName = "FileName";
    public const string QueryIdsElementRoot = "QueryIds";
    public const string QueryIdsElementName = "id";
    public const string QueryIdsAttributeStart = "s";
    public const string QueryIdsAttributeEnd = "e";
    public const string QueryIdsAttributeId = "i";
    public const string QueryIdsAttributeRev = "r";
    public const int MaxAttachmentSizeLimit = 2000000000;
    public const int NamespaceAdministratorRoleID = -30;
    public const int SqlMinYear = 1753;
    public const int SqlMaxYear = 9999;
    public const int NewFieldIdBase = 10000;
    public const int DomainGroupsId = -10;
    public const string SqlFutureDate = "convert(datetime,'9999',126)";
    public static readonly DateTime FutureDateTimeValue = new DateTime(3155063616000000000L, DateTimeKind.Utc);
    public const string DateTimeToken = "$$DATETIME$$";
    public const int DomainAccounts = -1;
    public const int DomainUsers = -2;
    public const int GlobalProjectId = 0;
    public const int ValidationProjectId = -1;
    public const int GlobalPartitionId = 0;
    public const int TempIdOffset = -20000;
    public const string WorkItemTypeExtensionFieldPrefix = "WEF";
    public const string WorkItemTypeExtensionMarkerFieldName = "Extension Marker";
    public const string WorkItemTypeExtensionMarkerFieldRefName = "System.ExtensionMarker";
    public const int DefaultLinksLimit = 1000;
    public const int DefaultRemoteLinksLimit = 25;
  }
}
