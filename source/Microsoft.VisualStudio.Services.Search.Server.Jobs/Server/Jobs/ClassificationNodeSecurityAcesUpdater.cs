// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.ClassificationNodeSecurityAcesUpdater
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.FaultManagement;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs
{
  public class ClassificationNodeSecurityAcesUpdater : SecurityAcesUpdaterBase
  {
    public ClassificationNodeSecurityAcesUpdater(ExecutionContext executionContext)
      : base(executionContext)
    {
      this.SecurityHashComputationKpiName = "AreaSecurityHashComputationTime";
    }

    protected override void LoadSecurityNamespace(ExecutionContext executionContext) => SecurityChecksUtils.LoadRemoteSecurityNamespace(this.ExecutionContext.RequestContext, AuthorizationSecurityConstants.CommonStructureNodeSecurityGuid);

    public virtual List<ClassificationNode> UpdateClassificationNodesWithSecurityHashAndToken(
      List<ClassificationNode> nodes,
      string projectToken)
    {
      Stopwatch stopwatch = Stopwatch.StartNew();
      Func<List<ClassificationNode>> function = (Func<List<ClassificationNode>>) (() => SecurityChecksUtils.UpdateClassificationNodesSecurityHashAndToken(this.ExecutionContext.RequestContext, nodes, projectToken));
      try
      {
        TimeSpan timeSpan1 = TimeSpan.FromSeconds((double) this.ExecutionContext.RequestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/CssNodeSecurityAcesUpdateTimeoutInSeconds"));
        TimeSpan timeSpan2 = TimeSpan.FromSeconds(30.0);
        return GenericInvoker.Instance.InvokeWithFaultCheck<List<ClassificationNode>>(function, this.ExecutionContext.FaultService, (int) (timeSpan1.TotalSeconds / timeSpan2.TotalSeconds), (int) timeSpan2.TotalMilliseconds, new TraceMetaData(1080351, "Indexing Pipeline", "SecurityChecks"));
      }
      finally
      {
        stopwatch.Stop();
        Tracer.PublishKpi(this.SecurityHashComputationKpiName, "Indexing Pipeline", (double) stopwatch.ElapsedMilliseconds);
      }
    }
  }
}
