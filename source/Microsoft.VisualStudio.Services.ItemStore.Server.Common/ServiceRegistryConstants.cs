// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ItemStore.Server.Common.ServiceRegistryConstants
// Assembly: Microsoft.VisualStudio.Services.ItemStore.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8190F04D-5888-4DB5-A838-8C98A67C6E45
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ItemStore.Server.Common.dll

namespace Microsoft.VisualStudio.Services.ItemStore.Server.Common
{
  public static class ServiceRegistryConstants
  {
    public const string AzureTable = "AzureTable";
    public const string SQLTable = "SQLTable";
    public const string MemoryTable = "MemoryTable";
    public const string ItemProviderImplementationKey = "ItemProviderImplementation";
    public static readonly string[] ItemProviders = new string[3]
    {
      nameof (AzureTable),
      nameof (MemoryTable),
      nameof (SQLTable)
    };
    private const string WorkerThreadsPerCoreKey = "WorkerThreadsPerCore";
    private const string CompletionThreadsPerCoreKey = "CompletionThreadsPerCore";
    private const string ConcurrentGetItemRequestCountKey = "ConcurrentGetItemRequestCount";
    private const string MaxKeepUntilSpanKey = "MaxKeepUntilSpan";
    private const string MaxWindowForGroupingReferenceDeletesInBatchKey = "MaxWindowForGroupingReferenceDeletesInBatch";

    public static string GetRegistryRootPath(string experienceName) => "/Configuration/ItemStore/" + experienceName;

    public static string GetItemProviderImplementationRegistryPath(string experienceName) => ServiceRegistryConstants.GetRegistryRootPath(experienceName) + "/ItemProviderImplementation";

    public static string GetShardingStrategyRegistryPath(string experienceName) => ServiceRegistryConstants.GetRegistryRootPath(experienceName) + "/ShardingStrategy";

    public static string GetWorkerThreadsPerCoreRegistryPath(string experienceName) => ServiceRegistryConstants.GetRegistryRootPath(experienceName) + "/WorkerThreadsPerCore";

    public static string GetCompletionThreadsPerCoreRegistryPath(string experienceName) => ServiceRegistryConstants.GetRegistryRootPath(experienceName) + "/CompletionThreadsPerCore";

    public static string GetConcurrentGetItemRequestCountRegistryPath(string experienceName) => ServiceRegistryConstants.GetRegistryRootPath(experienceName) + "/ConcurrentGetItemRequestCount";

    public static string GetMaxKeepUntilSpanRegistryPath(string experienceName) => ServiceRegistryConstants.GetRegistryRootPath(experienceName) + "/MaxKeepUntilSpan";

    public static string GetMaxWindowForGroupingReferenceDeletesInBatchRegistryPath(
      string experienceName)
    {
      return ServiceRegistryConstants.GetRegistryRootPath(experienceName) + "/MaxWindowForGroupingReferenceDeletesInBatch";
    }
  }
}
