// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Ingestion.CentralFeedServices.SpecificResourceAppTokenProvider
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Settings;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Ingestion.CentralFeedServices
{
  public class SpecificResourceAppTokenProvider : IFactory<string>
  {
    private readonly ITracerService tracerService;
    private readonly IInjectableAadTokenService tokenService;
    private readonly IOrgLevelPackagingSetting<string> resourceIdFactory;
    private readonly IOrgLevelPackagingSetting<string> tenantIdFactory;

    public SpecificResourceAppTokenProvider(
      ITracerService tracerService,
      IInjectableAadTokenService tokenService,
      IOrgLevelPackagingSetting<string> resourceIdFactory,
      IOrgLevelPackagingSetting<string> tenantIdFactory)
    {
      this.tracerService = tracerService;
      this.tokenService = tokenService;
      this.resourceIdFactory = resourceIdFactory;
      this.tenantIdFactory = tenantIdFactory;
    }

    public string Get()
    {
      using (ITracerBlock tracerBlock = this.tracerService.Enter((object) this, nameof (Get)))
      {
        string resource = this.resourceIdFactory.Get();
        if (string.IsNullOrWhiteSpace(resource))
        {
          tracerBlock.TraceError("Resource ID factory returned null or whitespace");
          return (string) null;
        }
        string tenantId = this.tenantIdFactory.Get();
        if (!string.IsNullOrWhiteSpace(tenantId))
          return this.tokenService.AcquireAppToken(resource, tenantId).RawData;
        tracerBlock.TraceError("Tenant ID factory returned null or whitespace");
        return (string) null;
      }
    }
  }
}
