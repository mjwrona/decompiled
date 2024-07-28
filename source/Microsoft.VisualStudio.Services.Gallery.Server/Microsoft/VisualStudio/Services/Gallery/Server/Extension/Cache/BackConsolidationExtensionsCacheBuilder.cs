// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Extension.Cache.BackConsolidationExtensionsCacheBuilder
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

namespace Microsoft.VisualStudio.Services.Gallery.Server.Extension.Cache
{
  internal class BackConsolidationExtensionsCacheBuilder : BackConsolidatedExtensionsCacheBuilder
  {
    private const int m_BCExtensionCacheCircuitBreakerExceptionID = 161013;
    private const int m_BCExtensionCacheCircuitBreakerThrottlingExceptionID = 161014;
    private int m_BCExtensionsCacheTimeoutInSeconds = 300;

    public override int CacheTimeoutInSeconds
    {
      get => this.m_BCExtensionsCacheTimeoutInSeconds;
      set => this.m_BCExtensionsCacheTimeoutInSeconds = value;
    }

    protected override int CiruitBreakerExceptionId
    {
      get => 161013;
      set
      {
      }
    }

    protected override int CircuitBreakerThrottlingExceptionID
    {
      get => 161014;
      set
      {
      }
    }
  }
}
