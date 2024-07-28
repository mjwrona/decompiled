// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Analytics.Server.IAnalyticsStagingJobService
// Assembly: Microsoft.VisualStudio.Services.Analytics.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E5A0742E-601C-4AD5-8902-781963AA7C5A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Analytics.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Analytics.Server
{
  [DefaultServiceImplementation(typeof (AnalyticsStagingJobService))]
  public interface IAnalyticsStagingJobService : IVssFrameworkService
  {
    void UpdateFeatureFlaggedStagingJobSchedules(
      IVssRequestContext requestContext,
      Action<string> logger);

    HashSet<Guid> GetFeatureFlaggedJobs(IVssRequestContext requestContext);
  }
}
