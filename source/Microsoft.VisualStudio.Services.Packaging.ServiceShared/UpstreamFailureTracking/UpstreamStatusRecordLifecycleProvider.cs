// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamFailureTracking.UpstreamStatusRecordLifecycleProvider
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Common;
using System;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamFailureTracking
{
  public class UpstreamStatusRecordLifecycleProvider : IUpstreamStatusRecordLifecycleProvider
  {
    private readonly ITimeProvider timeProvider;

    public UpstreamStatusRecordLifecycleProvider(ITimeProvider timeProvider) => this.timeProvider = timeProvider;

    public DateTime GetLatestDateTimeToRetrieve() => this.timeProvider.Now;

    public DateTime GetEarliestDateTimeToRetrieve() => this.GetLatestDateTimeToRetrieve().AddDays(-1.0);

    public DateTime GetLatestDateTimeToDelete() => this.GetEarliestDateTimeToRetrieve().AddDays(-1.0);
  }
}
