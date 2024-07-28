// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.DeleteProcessingJob.ScheduledPermanentDeleteDateCalculatingHandler
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.DeleteProcessingJob
{
  public class ScheduledPermanentDeleteDateCalculatingHandler : 
    IAsyncHandler<DateTime, DateTime>,
    IHaveInputType<DateTime>,
    IHaveOutputType<DateTime>
  {
    private readonly IFactory<TimeSpan> deletionGracePeriodFactory;

    public ScheduledPermanentDeleteDateCalculatingHandler(
      IFactory<TimeSpan> deletionGracePeriodFactory)
    {
      this.deletionGracePeriodFactory = deletionGracePeriodFactory;
    }

    public Task<DateTime> Handle(DateTime deletedDate) => Task.FromResult<DateTime>(deletedDate + this.deletionGracePeriodFactory.Get());
  }
}
