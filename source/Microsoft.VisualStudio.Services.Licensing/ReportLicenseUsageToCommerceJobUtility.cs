// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.ReportLicenseUsageToCommerceJobUtility
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Licensing
{
  public static class ReportLicenseUsageToCommerceJobUtility
  {
    private static readonly Guid JobId = new Guid("DBAA0219-7180-4CC7-AB6F-5061D8EE30ED");
    private static readonly string MaxDelaySecondsRegistryKey = "/Service/Licensing/ReportLicenseUsageToCommerceJobUtility/MaxDelaySeconds";
    private static readonly int MaxDelaySecondsDefaultValue = 43200;

    public static void QueueReportLicenseUsageToCommerceJob(IVssRequestContext requestContext)
    {
      int maxDelaySeconds = requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) ReportLicenseUsageToCommerceJobUtility.MaxDelaySecondsRegistryKey, true, ReportLicenseUsageToCommerceJobUtility.MaxDelaySecondsDefaultValue);
      ReportLicenseUsageToCommerceJobUtility.QueueReportLicenseUsageToCommerceJob(requestContext, maxDelaySeconds);
    }

    public static void QueueReportLicenseUsageToCommerceJob(
      IVssRequestContext requestContext,
      int maxDelaySeconds)
    {
      requestContext.CheckProjectCollectionRequestContext();
      requestContext.GetService<ITeamFoundationJobService>().QueueDelayedJobs(requestContext, (IEnumerable<Guid>) new Guid[1]
      {
        ReportLicenseUsageToCommerceJobUtility.JobId
      }, maxDelaySeconds);
    }
  }
}
