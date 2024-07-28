// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Redis.TracePoints
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

namespace Microsoft.VisualStudio.Services.Redis
{
  internal static class TracePoints
  {
    private const int RedisBase = 8000000;
    private const int RedisTracingBase = 8010000;
    internal const int RedisTracingEnterMethod = 8010001;
    internal const int RedisTracingExitMethod = 8010002;
    internal const int RedisTracingMethodException = 8010003;
    private const int RedisCacheBase = 8100000;
    private const int RedisCacheErrorBase = 8110000;
    internal const int RedisCallException = 8110001;
    internal const int RedisCallTimeout = 8110002;
    internal const int RedisConfigurationError = 8110003;
    internal const int RedisContainerSettingsError = 8110004;
    internal const int RedisOversizeMessage = 8110005;
    internal const int RedisConnectedMessage = 8110006;
    internal const int RedisReconnectedMessage = 8110007;
    internal const int RedisMuxEventMessage = 8110008;
    internal const int RedisWarmupMessage = 8110009;
    internal const int RedisSubscriptionError = 8110010;
    internal const int RedisConnectionProcessorError = 8110011;
    internal const int RedisConnectionProcessorMessage = 8110012;
    internal const int RedisConnectionConfigurationError = 8110013;
    internal const int RedisConnectionInitiationProgress = 8110014;
    private const int RedisMonitorBase = 8130000;
    internal const int RedisMonitoringEvent = 8130001;
    private const int RedisMonitoringJobBase = 8131000;
    internal const int RedisMonitoringJobEnterExit = 8131001;
    internal const int RedisMonitoringJobCompleted = 8131002;
    internal const int RedisMonitoringJobFailed = 8131003;
    internal const int RedisMonitoringJobScheduled = 8131004;
    internal const int RedisMonitoringServerInfo = 8131005;
    internal const int RedisMonitoringSlowLog = 8131006;
    private const int RedisMutableOperationBase = 8140000;
    internal const int RedisMutableGetOperation = 8140001;
    internal const int RedisMutableSetOperation = 8140002;
    internal const int RedisMutableHeavySetOperation = 8140012;
    internal const int RedisMutableInvalidateOperation = 8140003;
    internal const int RedisMutableTtlOperation = 8140004;
    internal const int RedisMutableIncrementOperation = 8140005;
    internal const int RedisBatchOperationInfo = 8140006;
    private const int RedisWindowedOperationBase = 8150000;
    internal const int RedisWindowedGetOperation = 8150001;
    internal const int RedisWindowedTtlOperation = 8150002;
    internal const int RedisWindowedIncrementOperation = 8150003;
    internal const string Area = "Redis";
    internal const string CacheLayer = "RedisCache";
    internal const string MonitorLayer = "RedisMonitor";
  }
}
