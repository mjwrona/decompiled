// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.DeleteProcessingJob.ScheduledPermanentDeleteDateResolvingHandler
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.DeleteProcessingJob
{
  public class ScheduledPermanentDeleteDateResolvingHandler : 
    IAsyncHandler<IDeleteOperationData, DateTime>,
    IHaveInputType<IDeleteOperationData>,
    IHaveOutputType<DateTime>
  {
    private readonly IAsyncHandler<DateTime, DateTime> scheduledPermanentDeleteDateCalculatingHandler;

    public ScheduledPermanentDeleteDateResolvingHandler(
      IAsyncHandler<DateTime, DateTime> scheduledPermanentDeleteDateCalculatingHandler)
    {
      this.scheduledPermanentDeleteDateCalculatingHandler = scheduledPermanentDeleteDateCalculatingHandler;
    }

    public async Task<DateTime> Handle(IDeleteOperationData request)
    {
      DateTime? permanentDeleteDate = request.ScheduledPermanentDeleteDate;
      DateTime dateTime;
      if (permanentDeleteDate.HasValue)
        dateTime = permanentDeleteDate.GetValueOrDefault();
      else
        dateTime = await this.scheduledPermanentDeleteDateCalculatingHandler.Handle(request.DeletedDate);
      return dateTime;
    }
  }
}
