// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Checks.Server.ICheckSuiteService
// Assembly: Microsoft.Azure.Pipelines.Checks.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D08C7285-654E-4A4D-BA46-748F0D1E96AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Checks.Server.dll

using Microsoft.Azure.Pipelines.Checks.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Pipelines.Checks.Server
{
  [DefaultServiceImplementation(typeof (CheckSuiteService))]
  public interface ICheckSuiteService : IVssFrameworkService
  {
    CheckSuite GetCheckSuite(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid checkSuiteId,
      CheckSuiteExpandParameter expand = CheckSuiteExpandParameter.None);

    IList<CheckRun> FilterCheckRuns(
      IVssRequestContext requestContext,
      Guid projectId,
      CheckRunFilter checkRunFilter);

    IList<CheckSuite> GetCheckSuitesByIds(
      IVssRequestContext requestContext,
      Guid projectId,
      List<Guid> checkSuiteIds,
      CheckSuiteExpandParameter expand = CheckSuiteExpandParameter.None);

    IList<CheckSuite> GetCheckSuiteByRequestIds(
      IVssRequestContext requestContext,
      Guid projectId,
      List<Guid> requestIds,
      CheckSuiteExpandParameter expand = CheckSuiteExpandParameter.None);

    CheckRun UpdateCheckRun(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid runId,
      CheckRunResult result,
      CheckSuiteExpandParameter expand = CheckSuiteExpandParameter.None);

    CheckSuite Evaluate(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid checkSuiteId,
      List<Resource> resources,
      JObject evaluationContext,
      CheckSuiteExpandParameter expand = CheckSuiteExpandParameter.None);

    CheckSuite RerunCheckRun(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid checkSuiteId,
      Guid checkToRerun);

    IList<CheckSuite> UpdateCheckSuite(
      IVssRequestContext requestContext,
      Guid projectId,
      IReadOnlyDictionary<Guid, CheckRunResult> updateParameters,
      CheckSuiteExpandParameter expand = CheckSuiteExpandParameter.None,
      bool ignoreCompletedCheckSuites = false);

    CheckSuite CancelCheckSuite(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid checkSuiteId,
      string cancelMessage);

    void ReEvaluate(
      IVssRequestContext requestContext,
      Guid projectId,
      JObject evaluationContext,
      CheckConfiguration checkConfiguration,
      List<Resource> resources,
      Guid checkSuiteId,
      Guid checkRunId);

    void TimeoutCheckRun(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid checkSuiteId,
      Guid checkRunId);

    void DeleteCheckSuites(
      IVssRequestContext requestContext,
      Guid projectId,
      IList<Guid> checkSuiteIds);

    CheckSuite BypassCheckRun(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid checkSuiteId,
      Guid checkRunId,
      CheckSuiteExpandParameter expand = CheckSuiteExpandParameter.None);
  }
}
