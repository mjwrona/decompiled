// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Query.QueryProcessor.QueryProcessorConstants
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Query.QueryProcessor
{
  internal static class QueryProcessorConstants
  {
    public const int c_maxStringLength = 4000;
    public const string c_asOfParameterAlias = "AsOfDates";
    public const string c_dataspaceProjectMapTempTableName = "DataspaceIdProjectMap";
    public const string c_workItemCustomLatest = "dbo.tbl_WorkItemCustomLatest";
    public const string c_workItemLongTexts = "dbo.WorkItemLongTexts";
    public const string c_workItemCustomLatestIndex = "PK_tbl_WorkItemCustomLatest";
    public const string c_workItemLongTextsIndex = "IX_WorkItemLongTexts_PartitionTextID";
    public const string c_leftJoin = "LEFT";
  }
}
