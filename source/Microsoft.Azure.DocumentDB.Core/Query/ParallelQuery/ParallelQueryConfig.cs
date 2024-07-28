// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Query.ParallelQuery.ParallelQueryConfig
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

namespace Microsoft.Azure.Documents.Query.ParallelQuery
{
  internal sealed class ParallelQueryConfig
  {
    public readonly int ClientInternalPageSize;
    public readonly long DefaultMaximumBufferSize;
    public readonly int AutoModeTasksIncrementFactor;
    public readonly int ClientInternalMaxItemCount;
    public readonly int NumberOfNetworkCallsPerProcessor;
    private static readonly ParallelQueryConfig DefaultInstance = new ParallelQueryConfig(100, 100, 100, 2, 1);

    private ParallelQueryConfig(
      int clientInternalMaxItemCount,
      int defaultMaximumBufferSize,
      int clientInternalPageSize,
      int autoModeTasksIncrementFactor,
      int numberOfNetworkCallsPerProcessor)
    {
      this.ClientInternalMaxItemCount = clientInternalMaxItemCount;
      this.DefaultMaximumBufferSize = (long) defaultMaximumBufferSize;
      this.ClientInternalPageSize = clientInternalPageSize;
      this.AutoModeTasksIncrementFactor = autoModeTasksIncrementFactor;
      this.NumberOfNetworkCallsPerProcessor = numberOfNetworkCallsPerProcessor;
    }

    public static ParallelQueryConfig GetConfig() => ParallelQueryConfig.DefaultInstance;
  }
}
