// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CircuitBreaker.CommandPropertiesDatabase
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.VisualStudio.Services.CircuitBreaker
{
  internal class CommandPropertiesDatabase : CommandPropertiesDefault
  {
    internal static readonly byte s_defaultByteValue = byte.MaxValue;
    internal static readonly int s_defaultIntValue = -1;
    internal static readonly bool s_defaultBoolValue = false;

    internal CommandPropertiesDatabase(
      CircuitBreakerDatabaseProperties databaseProperties,
      CommandKey key,
      CommandPropertiesSetter setter)
      : base(setter)
    {
      this.CircuitBreakerDisabled = databaseProperties.CircuitBreakerDisabled;
      this.CircuitBreakerForceClosed = databaseProperties.CircuitBreakerForceClosed;
      this.CircuitBreakerForceOpen = databaseProperties.CircuitBreakerForceOpen;
      int num1 = this.GetValue(databaseProperties.CircuitBreakerErrorThresholdPercentage, this.CircuitBreakerErrorThresholdPercentage);
      if (num1 > 0 && num1 <= 100)
        this.CircuitBreakerErrorThresholdPercentage = num1;
      int num2 = this.GetValue(databaseProperties.CircuitBreakerRequestVolumeThreshold, this.CircuitBreakerRequestVolumeThreshold);
      if (num2 > 0)
        this.CircuitBreakerRequestVolumeThreshold = num2;
      TimeSpan timeSpan1 = this.GetValue(databaseProperties.CircuitBreakerMaxBackoff, this.CircuitBreakerMaxBackoff);
      if (timeSpan1 != TimeSpan.Zero)
        this.CircuitBreakerMaxBackoff = timeSpan1;
      TimeSpan timeSpan2 = this.GetValue(databaseProperties.ExecutionTimeout, this.ExecutionTimeout);
      if (timeSpan2 != TimeSpan.Zero)
        this.ExecutionTimeout = timeSpan2;
      int num3 = this.GetValue(databaseProperties.ExecutionMaxConcurrentRequests, this.ExecutionMaxConcurrentRequests);
      if (num3 > 0)
        this.ExecutionMaxConcurrentRequests = num3;
      int num4 = this.GetValue(databaseProperties.FallbackMaxConcurrentRequests, this.FallbackMaxConcurrentRequests);
      if (num4 > 0)
        this.FallbackMaxConcurrentRequests = num4;
      int num5 = this.GetValue(databaseProperties.ExecutionMaxRequests, this.ExecutionMaxRequests);
      if (num5 > 0)
        this.ExecutionMaxConcurrentRequests = num5;
      int num6 = this.GetValue(databaseProperties.FallbackMaxRequests, this.FallbackMaxRequests);
      if (num6 <= 0)
        return;
      this.FallbackMaxConcurrentRequests = num6;
    }

    private int GetValue(byte databaseValue, int defaultValue) => (int) databaseValue == (int) CommandPropertiesDatabase.s_defaultByteValue ? defaultValue : (int) databaseValue;

    private int GetValue(int databaseValue, int defaultValue) => databaseValue < 0 ? defaultValue : databaseValue;

    private TimeSpan GetValue(int databaseValue, TimeSpan defaultValue) => databaseValue < 0 ? defaultValue : TimeSpan.FromSeconds((double) databaseValue);
  }
}
