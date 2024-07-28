// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs.DeleteProcessingJob.DeletedPackageJobFlushIntervalFactoryBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using System;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs.DeleteProcessingJob
{
  public class DeletedPackageJobFlushIntervalFactoryBootstrapper : IBootstrapper<IFactory<TimeSpan>>
  {
    private readonly IVssRequestContext requestContext;

    public DeletedPackageJobFlushIntervalFactoryBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public IFactory<TimeSpan> Bootstrap() => new RegistryOverrideWithDefaultFactory<double>((IRegistryService) this.requestContext.GetRegistryFacade(), DeletedPackageJobConstants.FlushIntervalRegistryPath, DeletedPackageJobConstants.FlushIntervalDefault.TotalSeconds).ConvertBy<double, TimeSpan>(DeletedPackageJobFlushIntervalFactoryBootstrapper.\u003C\u003EO.\u003C0\u003E__FromSeconds ?? (DeletedPackageJobFlushIntervalFactoryBootstrapper.\u003C\u003EO.\u003C0\u003E__FromSeconds = new Func<double, TimeSpan>(TimeSpan.FromSeconds)));
  }
}
