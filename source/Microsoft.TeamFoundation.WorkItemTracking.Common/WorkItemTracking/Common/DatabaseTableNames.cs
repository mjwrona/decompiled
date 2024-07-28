// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Common.DatabaseTableNames
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Microsoft.TeamFoundation.WorkItemTracking.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  [StructLayout(LayoutKind.Sequential, Size = 1)]
  public struct DatabaseTableNames
  {
    public const string Constants = "Constants";
    public const string StoredQueries = "StoredQueries";
    public const string PublicQueryItems = "PublicQueryItems";
    public const string PrivateQueryItems = "PrivateQueryItems";
    public const string TreeNodes = "TreeNodes";
    public const string Attachments = "Attachments";
    public const string Fields = "tbl_Field";
    public const string TreeProperties = "TreeProperties";
    public const string Rules = "Rules";
    public const string ConstantSets = "Sets";
    public const string FieldUsages = "tbl_FieldUsage";
    public const string WorkItemTypes = "WorkItemTypes";
    public const string WorkItemTypeUsages = "WorkItemTypeUsages";
    public const string Actions = "Actions";
    public const string LinkTypes = "LinkTypes";
    public const string WorkItemTypeCategories = "WorkItemTypeCategories";
    public const string WorkItemTypeCategoryMembers = "WorkItemTypeCategoryMembers";
    public const string TreeIDPairs = "TreeIDPairs";
  }
}
