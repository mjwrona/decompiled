// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.QueryMetricsConstants
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

namespace Microsoft.Azure.Documents
{
  internal static class QueryMetricsConstants
  {
    public const string RetrievedDocumentCount = "retrievedDocumentCount";
    public const string RetrievedDocumentSize = "retrievedDocumentSize";
    public const string OutputDocumentCount = "outputDocumentCount";
    public const string OutputDocumentSize = "outputDocumentSize";
    public const string IndexHitRatio = "indexUtilizationRatio";
    public const string IndexHitDocumentCount = "indexHitDocumentCount";
    public const string TotalQueryExecutionTimeInMs = "totalExecutionTimeInMs";
    public const string QueryCompileTimeInMs = "queryCompileTimeInMs";
    public const string LogicalPlanBuildTimeInMs = "queryLogicalPlanBuildTimeInMs";
    public const string PhysicalPlanBuildTimeInMs = "queryPhysicalPlanBuildTimeInMs";
    public const string QueryOptimizationTimeInMs = "queryOptimizationTimeInMs";
    public const string IndexLookupTimeInMs = "indexLookupTimeInMs";
    public const string DocumentLoadTimeInMs = "documentLoadTimeInMs";
    public const string VMExecutionTimeInMs = "VMExecutionTimeInMs";
    public const string DocumentWriteTimeInMs = "writeOutputTimeInMs";
    public const string QueryEngineTimes = "queryEngineTimes";
    public const string SystemFunctionExecuteTimeInMs = "systemFunctionExecuteTimeInMs";
    public const string UserDefinedFunctionExecutionTimeInMs = "userFunctionExecuteTimeInMs";
  }
}
