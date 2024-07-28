// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.CircuitBreakerDatabaseProperties
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Collections.Generic;
using System.Text;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class CircuitBreakerDatabaseProperties
  {
    private static readonly char[] registryBlockedchars = new char[14]
    {
      '<',
      ':',
      '>',
      '\\',
      '|',
      '#',
      '%',
      '_',
      '[',
      ']',
      '?',
      '"',
      ' ',
      '\''
    };
    private static readonly HashSet<char> badChars = new HashSet<char>((IEnumerable<char>) CircuitBreakerDatabaseProperties.registryBlockedchars);
    private static readonly CircuitBreakerDatabaseProperties s_bootstrapBreakerProperties = new CircuitBreakerDatabaseProperties(false, false, false, byte.MaxValue, -1, -1, -1, -1, -1);

    internal static CircuitBreakerDatabaseProperties BootstrapBreakerProperties => CircuitBreakerDatabaseProperties.s_bootstrapBreakerProperties;

    internal CircuitBreakerDatabaseProperties(
      ITeamFoundationDatabaseProperties databaseProperties)
      : this(databaseProperties.BreakerDisabled, databaseProperties.BreakerForceClosed, databaseProperties.BreakerForceOpen, databaseProperties.BreakerErrorThresholdPerc, databaseProperties.BreakerRequestVolumeThreshold, databaseProperties.BreakerMaxBackoff, databaseProperties.BreakerExecutionTimeout, databaseProperties.BreakerMaxExecConcurrentRequests, databaseProperties.BreakerMaxFallbackConcurrentRequests)
    {
    }

    internal CircuitBreakerDatabaseProperties(
      bool circuitBreakerDisabled,
      bool circuitBreakerForceClosed,
      bool circuitBreakerForceOpen,
      byte circuitBreakerErrorThresholdPercentage,
      int circuitBreakerRequestVolumeThreshold,
      int circuitBreakerMaxBackoff,
      int executionTimeout,
      int executionMaxConcurrentRequests,
      int fallbackMaxConcurrentRequests)
    {
      this.CircuitBreakerDisabled = circuitBreakerDisabled;
      this.CircuitBreakerForceClosed = circuitBreakerForceClosed;
      this.CircuitBreakerForceOpen = circuitBreakerForceOpen;
      this.CircuitBreakerErrorThresholdPercentage = circuitBreakerErrorThresholdPercentage;
      this.CircuitBreakerRequestVolumeThreshold = circuitBreakerRequestVolumeThreshold;
      this.CircuitBreakerMaxBackoff = circuitBreakerMaxBackoff;
      this.ExecutionTimeout = executionTimeout;
      this.ExecutionMaxConcurrentRequests = executionMaxConcurrentRequests;
      this.FallbackMaxConcurrentRequests = fallbackMaxConcurrentRequests;
    }

    internal static string SanitizeDatabaseName(string databaseName) => string.IsNullOrEmpty(databaseName) ? string.Empty : CircuitBreakerDatabaseProperties.SanitizeForRegistry(databaseName);

    private static string SanitizeForRegistry(string text)
    {
      StringBuilder stringBuilder = new StringBuilder();
      foreach (char ch in text)
      {
        if (!CircuitBreakerDatabaseProperties.badChars.Contains(ch))
          stringBuilder.Append(ch);
      }
      return stringBuilder.ToString();
    }

    internal bool CircuitBreakerDisabled { get; private set; }

    internal byte CircuitBreakerErrorThresholdPercentage { get; private set; }

    internal bool CircuitBreakerForceClosed { get; private set; }

    internal bool CircuitBreakerForceOpen { get; private set; }

    internal int CircuitBreakerRequestVolumeThreshold { get; private set; }

    internal int CircuitBreakerMaxBackoff { get; private set; }

    internal int ExecutionTimeout { get; private set; }

    internal int ExecutionMaxConcurrentRequests { get; private set; }

    internal int FallbackMaxConcurrentRequests { get; private set; }

    internal int ExecutionMaxRequests { get; private set; }

    internal int FallbackMaxRequests { get; private set; }
  }
}
