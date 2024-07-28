// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.HealthAgentServiceSettings
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class HealthAgentServiceSettings
  {
    public static readonly RegistryQuery RegistryPath = (RegistryQuery) "HealthAgentService/**";

    public TimeSpan HeartbeatInterval { get; private set; }

    public TimeSpan CircuitBreakerTimeout { get; private set; }

    public TimeSpan CircuitBreakerMinBackoff { get; private set; }

    public TimeSpan CircuitBreakerMaxBackoff { get; private set; }

    public TimeSpan CircuitBreakerStatisticalWindow { get; private set; }

    public int CircuitBreakerErrorThresholdPercentage { get; private set; }

    public static HealthAgentServiceSettings Load(IVssRequestContext requestContext)
    {
      requestContext.CheckDeploymentRequestContext();
      RegistryEntryCollection registryEntryCollection = requestContext.GetService<IVssRegistryService>().ReadEntries(requestContext, HealthAgentServiceSettings.RegistryPath);
      return new HealthAgentServiceSettings()
      {
        HeartbeatInterval = registryEntryCollection.GetValueFromPath<TimeSpan>("HeartbeatInterval", TimeSpan.FromSeconds(15.0)),
        CircuitBreakerTimeout = registryEntryCollection.GetValueFromPath<TimeSpan>("CircuitBreakerTimeout", TimeSpan.FromSeconds(2.0)),
        CircuitBreakerMinBackoff = registryEntryCollection.GetValueFromPath<TimeSpan>("CircuitBreakerMinBackoff", TimeSpan.FromSeconds(1.0)),
        CircuitBreakerMaxBackoff = registryEntryCollection.GetValueFromPath<TimeSpan>("CircuitBreakerMaxBackoff", TimeSpan.FromSeconds(30.0)),
        CircuitBreakerStatisticalWindow = registryEntryCollection.GetValueFromPath<TimeSpan>("CircuitBreakerStatisticalWindow", TimeSpan.FromSeconds(120.0)),
        CircuitBreakerErrorThresholdPercentage = registryEntryCollection.GetValueFromPath<int>("CircuitBreakerErrorThresholdPercentage", 50)
      };
    }
  }
}
