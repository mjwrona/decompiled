// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.Internal.MsdnAdapterConfiguration
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

namespace Microsoft.VisualStudio.Services.Licensing.Internal
{
  public class MsdnAdapterConfiguration
  {
    public const int DefaultGetEntitlementsTimeoutSeconds = 8;
    public const int DefaultGetEntitlementsRetries = 2;
    public const int DefaultGetEntitlementsSlowRequestThresholdSeconds = 3;
    internal static CircuitBreakerSettings DefaultCircuitBreakerSettingsForGetMsdnEntitlements = CircuitBreakerSettings.Default;

    public int GetEntitlementsTimeoutSeconds { get; set; }

    public int GetEntitlementsRetries { get; set; }

    public int GetEntitlementsSlowRequestThresholdSeconds { get; set; }

    internal CircuitBreakerSettings CircuitBreakerSettingsForGetMsdnEntitlements { get; set; }
  }
}
