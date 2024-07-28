// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement.JobRunSettings
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using System;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement
{
  public class JobRunSettings
  {
    public JobRunSettings()
    {
    }

    public JobRunSettings(TimeSpan firstRequeueMaxDelay, TimeSpan subsequentRequeueMaxDelay)
    {
      this.FirstRequeueMaxDelay = firstRequeueMaxDelay;
      this.SubsequentRequeueMaxDelay = subsequentRequeueMaxDelay;
    }

    public TimeSpan FirstRequeueMaxDelay { get; } = TimeSpan.FromMinutes(15.0);

    public TimeSpan SubsequentRequeueMaxDelay { get; } = TimeSpan.FromHours(2.0);
  }
}
