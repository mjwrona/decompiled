// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs.DeleteProcessingJob.DeletedPackageJobMaxPendingPermDeleteOpsFactoryBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs.DeleteProcessingJob
{
  public class DeletedPackageJobMaxPendingPermDeleteOpsFactoryBootstrapper : 
    IBootstrapper<IFactory<int>>
  {
    private readonly IVssRequestContext requestContext;

    public DeletedPackageJobMaxPendingPermDeleteOpsFactoryBootstrapper(
      IVssRequestContext requestContext)
    {
      this.requestContext = requestContext;
    }

    public IFactory<int> Bootstrap() => (IFactory<int>) new RegistryOverrideWithDefaultFactory<int>((IRegistryService) this.requestContext.GetRegistryFacade(), DeletedPackageJobConstants.MaxPendingPermDeleteOpsRegistryPath, 98);
  }
}
