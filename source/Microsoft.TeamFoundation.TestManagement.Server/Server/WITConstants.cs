// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.WITConstants
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal static class WITConstants
  {
    internal const int WorkItemBatchSizeLimit = 200;
    internal static int NewClonedEntityTempId = -1;
    public const string ColumnName_Relations_ID = "ID";
    public const string ColumnName_Relations_LinkType = "LinkType";
    public const string ColumnName_Relations_Comment = "Comment";
    public const string ColumnName_Texts_FldID = "FldID";
    public const string ColumnName_Texts_Words = "Words";
    public const string ColumnName_Texts_AddedDate = "AddedDate";
    public const string ColumnName_Files_OriginalName = "OriginalName";
    public const string ColumnName_Files_FilePath = "FilePath";
    public const string ColumnName_Files_CreationDate = "CreationDate";
    public const string ColumnName_Files_LastWriteDate = "LastWriteDate";
    public const string ColumnName_Files_Length = "Length";
    public const string ColumnName_Files_Comment = "Comment";
    public const string ColumnName_Files_FieldId = "FldID";
    public const string ColumnName_Files_AddedDate = "AddedDate";
    public const string ColumnName_Files_RemovedDate = "RemovedDate";
    public const string XPathToWitIdInUpdateResults = "InsertWorkItem[1]/@ID";
    public const string TestPlanProcessConfigCategoryName = "TestPlanWorkItems";
    public const string TestSuiteProcessConfigCategoryName = "TestSuiteWorkItems";
    public const string InboxFieldPrefix = "Microsoft.";
    public const int WorkItemImportHelpLinkId = 398535;
    public const int DataMigrationHelpLinkId = 399101;
    public const string LinkTypeRelated = "System.LinkTypes.Related";
    public const string CommentAttribute = "comment";
    public const string LinkTypeParent = "System.LinkTypes.Hierarchy-Forward";
  }
}
