// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.SecurityAcesUpdaterBase
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.FaultManagement;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs
{
  public abstract class SecurityAcesUpdaterBase
  {
    protected const int GetAcesRetryCount = 4;
    protected const int GetAcesRetryDelayInSecs = 5;
    private const string s_traceArea = "Indexing Pipeline";
    private const string s_traceLayer = "SecurityChecks";

    protected string SecurityHashComputationKpiName { get; set; }

    [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    protected SecurityAcesUpdaterBase(ExecutionContext executionContext)
    {
      this.ExecutionContext = executionContext;
      this.LoadSecurityNamespace(executionContext);
    }

    protected abstract void LoadSecurityNamespace(ExecutionContext executionContext);

    public virtual byte[] GetSecurityHashCodeWithFaultCheck(
      Func<IEnumerable<IAccessControlEntry>> securityAcesFetcher,
      int permissionToBeChecked)
    {
      Stopwatch stopwatch = Stopwatch.StartNew();
      try
      {
        return SecurityChecksUtils.GetSecurityHashcode(ExponentialBackoffRetryInvoker.Instance.InvokeWithFaultCheck<IEnumerable<IAccessControlEntry>>((Func<object>) securityAcesFetcher, this.ExecutionContext.FaultService, 4, 5000, true, new TraceMetaData(1080291, "Indexing Pipeline", "SecurityChecks")), permissionToBeChecked);
      }
      finally
      {
        stopwatch.Stop();
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpi(this.SecurityHashComputationKpiName, "Indexing Pipeline", (double) stopwatch.ElapsedMilliseconds);
      }
    }

    protected ExecutionContext ExecutionContext { get; set; }
  }
}
