// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Checks.Server.ICheckType
// Assembly: Microsoft.Azure.Pipelines.Checks.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D08C7285-654E-4A4D-BA46-748F0D1E96AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Checks.Server.dll

using Microsoft.Azure.Pipelines.Checks.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Microsoft.Azure.Pipelines.Checks.Server
{
  [InheritedExport]
  public interface ICheckType
  {
    Guid CheckTypeId { get; }

    string CheckTypeName { get; }

    CheckEvaluationOrder EvaluationOrder(CheckConfiguration checkConfigurations = null);

    string UIContributionType { get; }

    string[] UIContributionDependencies { get; }

    CheckConfiguration ValidateAndGetCheckConfiguration(
      IVssRequestContext requestContext,
      Guid projectId,
      CheckConfiguration checkConfiguration);

    IReadOnlyDictionary<Guid, CheckRunResult> Evaluate(
      IVssRequestContext requestContext,
      Guid projectId,
      JObject evaluationContext,
      IReadOnlyDictionary<Guid, CheckConfiguration> requests,
      List<Resource> resources);

    bool Cancel(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid checkSuiteId,
      IList<CheckRun> checkRuns);

    CheckRunStatus Timeout(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid checkSuiteId,
      Guid requestId);

    void Delete(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid checkSuiteId,
      IList<CheckRun> checkRuns);

    void Rerun(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid checkSuiteId,
      string stageName,
      CheckRun checkRun);

    Dictionary<string, string> GetTelemetryData(CheckConfiguration checkConfiguration);

    Dictionary<string, object> GetAuditLogData(
      IVssRequestContext requestContext,
      Guid projectId,
      JObject context,
      IList<CheckRun> checkRuns);

    IList<CheckDefinitionData> GetCheckDefinitions(IVssRequestContext requestContext);

    IList<CheckConfigurationData> GetCheckConfigurationDataList(
      IVssRequestContext requestContext,
      IList<CheckConfiguration> checkConfigurations);

    IList<CheckConfiguration> UpdateAndGetNonCompliantCheckConfigurations(
      IVssRequestContext requestContext,
      IList<CheckConfiguration> checkConfigurations);

    bool Bypass(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid checkSuiteId,
      CheckRun bypassedCheckRun,
      IdentityRef bypassedBy);

    CheckDefinitionData GetCheckDefinition(
      IVssRequestContext requestContext,
      CheckConfiguration newCheckConfiguration);

    bool ShouldEvaluationFailUponCheckFailure(
      IVssRequestContext requestContext,
      IEnumerable<CheckRun> failedCheckRuns,
      IDictionary<Guid, CheckConfiguration> checkConfigurations);
  }
}
