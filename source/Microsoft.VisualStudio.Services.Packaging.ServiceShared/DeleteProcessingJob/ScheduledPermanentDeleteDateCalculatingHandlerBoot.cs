// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.DeleteProcessingJob.ScheduledPermanentDeleteDateCalculatingHandlerBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.DeleteProcessingJob
{
  public class ScheduledPermanentDeleteDateCalculatingHandlerBootstrapper : 
    IBootstrapper<IAsyncHandler<DateTime, DateTime>>
  {
    private readonly IVssRequestContext requestContext;

    public ScheduledPermanentDeleteDateCalculatingHandlerBootstrapper(
      IVssRequestContext requestContext)
    {
      this.requestContext = requestContext;
    }

    public IAsyncHandler<DateTime, DateTime> Bootstrap() => (IAsyncHandler<DateTime, DateTime>) new ScheduledPermanentDeleteDateCalculatingHandler((IFactory<TimeSpan>) new DeletionGracePeriodFactory((IRegistryService) new RegistryServiceFacade(this.requestContext)));
  }
}
