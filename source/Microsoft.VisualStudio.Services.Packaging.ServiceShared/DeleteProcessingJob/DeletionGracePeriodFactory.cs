// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.DeleteProcessingJob.DeletionGracePeriodFactory
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.DeleteProcessingJob
{
  public class DeletionGracePeriodFactory : IFactory<TimeSpan>
  {
    private IRegistryService registrySvc;

    public DeletionGracePeriodFactory(IRegistryService registrySvc) => this.registrySvc = registrySvc;

    public TimeSpan Get() => TimeSpan.FromHours(this.registrySvc.GetValue<double>(new RegistryQuery("/Configuration/Packaging/DeletedPackagesProcessing/PackageGracePeriodHours"), PackagingServerConstants.DefaultPackageGracePeriod.TotalHours));
  }
}
