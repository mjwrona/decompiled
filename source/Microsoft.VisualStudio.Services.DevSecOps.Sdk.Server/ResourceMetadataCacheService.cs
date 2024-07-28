// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server.ResourceMetadataCacheService
// Assembly: Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AEA81E2B-AB47-44C0-8043-66C0E1018997
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server
{
  internal sealed class ResourceMetadataCacheService : 
    VssMemoryCacheService<ResourceMetadataCacheKey, IResourceMetadataCacheValue>,
    IResourceMetadataCacheService,
    IVssFrameworkService
  {
    protected override void ServiceStart(IVssRequestContext context)
    {
      base.ServiceStart(context);
      this.ExpiryInterval.Value = TimeSpan.FromHours(1.0);
      this.MaxCacheLength.Value = 1024;
      this.MaxCacheSize.Value = 40960L;
    }

    protected override void ServiceEnd(IVssRequestContext context) => base.ServiceEnd(context);
  }
}
