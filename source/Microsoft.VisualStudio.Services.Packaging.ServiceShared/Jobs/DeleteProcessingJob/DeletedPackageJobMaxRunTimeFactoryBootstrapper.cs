// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs.DeleteProcessingJob.DeletedPackageJobMaxRunTimeFactoryBootstrapper
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
  public class DeletedPackageJobMaxRunTimeFactoryBootstrapper : IBootstrapper<IFactory<TimeSpan>>
  {
    private readonly IVssRequestContext requestContext;

    public DeletedPackageJobMaxRunTimeFactoryBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public IFactory<TimeSpan> Bootstrap() => new RegistryOverrideWithDefaultFactory<double>((IRegistryService) this.requestContext.GetRegistryFacade(), DeletedPackageJobConstants.MaxRunTimeRegistryPath, DeletedPackageJobConstants.MaxRunTimeDefault.TotalSeconds).ConvertBy<double, TimeSpan>(DeletedPackageJobMaxRunTimeFactoryBootstrapper.\u003C\u003EO.\u003C0\u003E__FromSeconds ?? (DeletedPackageJobMaxRunTimeFactoryBootstrapper.\u003C\u003EO.\u003C0\u003E__FromSeconds = new Func<double, TimeSpan>(TimeSpan.FromSeconds)));
  }
}
