// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.ClientCircuitBreakerSettingsAttribute
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;

namespace Microsoft.VisualStudio.Services.WebApi
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
  public sealed class ClientCircuitBreakerSettingsAttribute : Attribute
  {
    public ClientCircuitBreakerSettingsAttribute(int timeoutSeconds, int failurePercentage)
    {
      this.Timeout = TimeSpan.FromSeconds((double) timeoutSeconds);
      this.ErrorPercentage = failurePercentage;
    }

    public TimeSpan Timeout { get; private set; }

    public int ErrorPercentage { get; private set; }

    public int MaxConcurrentRequests { get; set; }
  }
}
