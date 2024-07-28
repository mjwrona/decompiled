// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Checks.Server.CheckSuiteService
// Assembly: Microsoft.Azure.Pipelines.Checks.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D08C7285-654E-4A4D-BA46-748F0D1E96AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Checks.Server.dll

using Microsoft.Azure.Pipelines.Checks.Common;
using Microsoft.Azure.Pipelines.Checks.Server.DataAccess;
using Microsoft.Azure.Pipelines.Checks.WebApi;
using Microsoft.Azure.Pipelines.TaskCheck.WebApi;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Checkpoints;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace Microsoft.Azure.Pipelines.Checks.Server
{
  internal class CheckSuiteService : ICheckSuiteService, IVssFrameworkService
  {
    private readonly int c_defaultTimeout = 43200;
    private Guid[] m_whitelistedTaskCheckIds = new Guid[3]
    {
      TaskCheckConstants.ArtifactPolicyCheckTaskDefinitionId,
      TaskCheckConstants.BranchProtectionCheckTaskDefinitionId,
      TaskCheckConstants.BusinessHoursCheckTaskDefinitionId
    };
    private List<Guid> m_notRetriableTaskCheckIds = new List<Guid>()
    {
      TaskCheckConstants.ArtifactPolicyCheckTaskDefinitionId,
      TaskCheckConstants.BranchProtectionCheckTaskDefinitionId,
      TaskCheckConstants.BusinessHoursCheckTaskDefinitionId
    };

    public CheckSuite GetCheckSuite(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid checkSuiteId,
      CheckSuiteExpandParameter expand = CheckSuiteExpandParameter.None)
    {
      CheckSuiteService.ValidateBasicData(requestContext, projectId);
      using (new MethodScope(requestContext, nameof (CheckSuiteService), nameof (GetCheckSuite)))
      {
        ArgumentUtility.CheckForEmptyGuid(checkSuiteId, nameof (checkSuiteId), "Pipeline.Checks");
        requestContext.TraceInfo(34001913, nameof (CheckSuiteService), "CheckSuiteService::GetCheckSuite Get Check suite request with id: {0}", (object) checkSuiteId);
        CheckSuite checkSuite;
        using (CheckSuiteComponent component = requestContext.CreateComponent<CheckSuiteComponent>())
          checkSuite = component.GetCheckSuite(projectId, checkSuiteId);
        if (checkSuite == null)
        {
          requestContext.TraceInfo(34001924, nameof (CheckSuiteService), "CheckSuiteService::GetCheckSuite Suite not found: {0}.", (object) checkSuiteId);
          throw new CheckSuiteRequestNotFoundException(PipelineChecksResources.ChecksSuiteIdNotFound((object) checkSuiteId));
        }
        List<Resource> resources = new List<Resource>();
        if ((expand & CheckSuiteExpandParameter.Resources).Equals((object) CheckSuiteExpandParameter.Resources))
          resources = Utilities.GetPermissibleResourcesForConfigurations(requestContext, projectId, checkSuite.CheckRuns.Select<CheckRun, CheckConfigurationRef>((Func<CheckRun, CheckConfigurationRef>) (r => r.CheckConfigurationRef)).ToList<CheckConfigurationRef>());
        this.PopulateMissingDetails(requestContext, projectId, checkSuite, resources);
        requestContext.TraceInfo(34001913, nameof (CheckSuiteService), "CheckSuiteService::GetCheckSuite Returning {0} check runs with id: {1}", (object) checkSuite.CheckRuns.Count, (object) checkSuiteId);
        return checkSuite;
      }
    }

    public IList<CheckSuite> GetCheckSuitesByIds(
      IVssRequestContext requestContext,
      Guid projectId,
      List<Guid> checkSuiteIds,
      CheckSuiteExpandParameter expand = CheckSuiteExpandParameter.None)
    {
      CheckSuiteService.ValidateBasicData(requestContext, projectId);
      using (new MethodScope(requestContext, nameof (CheckSuiteService), nameof (GetCheckSuitesByIds)))
      {
        ArgumentUtility.CheckForNull<List<Guid>>(checkSuiteIds, nameof (checkSuiteIds), "Pipeline.Checks");
        checkSuiteIds.ForEach((Action<Guid>) (checkSuiteId => ArgumentUtility.CheckForEmptyGuid(checkSuiteId, nameof (checkSuiteId), "Pipeline.Checks")));
        requestContext.TraceInfo(34001913, nameof (CheckSuiteService), "CheckSuiteService::GetCheckSuitesByIds Get check suite with check suite ids: {0}", (object) string.Join(", ", checkSuiteIds.Select<Guid, string>((Func<Guid, string>) (id => id.ToString()))));
        List<CheckSuite> checkSuitesByIds;
        using (CheckSuiteComponent component = requestContext.CreateComponent<CheckSuiteComponent>())
          checkSuitesByIds = component.GetCheckSuitesByIds(projectId, checkSuiteIds);
        List<Resource> permissibleResources = new List<Resource>();
        if ((expand & CheckSuiteExpandParameter.Resources).Equals((object) CheckSuiteExpandParameter.Resources))
        {
          List<CheckConfigurationRef> checkConfigurationRefs = new List<CheckConfigurationRef>();
          checkSuitesByIds.ForEach((Action<CheckSuite>) (r => checkConfigurationRefs.AddRangeIfRangeNotNull<CheckConfigurationRef, List<CheckConfigurationRef>>((IEnumerable<CheckConfigurationRef>) r.CheckRuns.Select<CheckRun, CheckConfigurationRef>((Func<CheckRun, CheckConfigurationRef>) (cr => cr.CheckConfigurationRef)).ToList<CheckConfigurationRef>())));
          permissibleResources = Utilities.GetPermissibleResourcesForConfigurations(requestContext, projectId, checkConfigurationRefs);
        }
        checkSuitesByIds.ForEach((Action<CheckSuite>) (checkSuiteResponse => this.PopulateMissingDetails(requestContext, projectId, checkSuiteResponse, permissibleResources)));
        requestContext.TraceInfo(34001913, nameof (CheckSuiteService), "CheckSuiteService::GetCheckSuitesByIds Returning {0} check suites.", (object) checkSuitesByIds.Count<CheckSuite>());
        return (IList<CheckSuite>) checkSuitesByIds;
      }
    }

    public IList<CheckSuite> GetCheckSuiteByRequestIds(
      IVssRequestContext requestContext,
      Guid projectId,
      List<Guid> requestIds,
      CheckSuiteExpandParameter expand = CheckSuiteExpandParameter.None)
    {
      CheckSuiteService.ValidateBasicData(requestContext, projectId);
      using (new MethodScope(requestContext, nameof (CheckSuiteService), nameof (GetCheckSuiteByRequestIds)))
      {
        requestIds.ForEach((Action<Guid>) (requestId => ArgumentUtility.CheckForEmptyGuid(requestId, nameof (requestId), "Pipeline.Checks")));
        requestContext.TraceInfo(34001913, nameof (CheckSuiteService), "CheckSuiteService::GetCheckSuiteByRequestIds Get check suite with request ids: {0}", (object) string.Join(", ", requestIds.Select<Guid, string>((Func<Guid, string>) (id => id.ToString()))));
        List<CheckSuite> suitesByRequestIds;
        using (CheckSuiteComponent component = requestContext.CreateComponent<CheckSuiteComponent>())
          suitesByRequestIds = component.GetCheckSuitesByRequestIds(projectId, requestIds);
        List<Resource> permissibleResources = new List<Resource>();
        if ((expand & CheckSuiteExpandParameter.Resources).Equals((object) CheckSuiteExpandParameter.Resources))
        {
          List<CheckConfigurationRef> checkConfigurationRefs = new List<CheckConfigurationRef>();
          suitesByRequestIds.ForEach((Action<CheckSuite>) (r => checkConfigurationRefs.AddRangeIfRangeNotNull<CheckConfigurationRef, List<CheckConfigurationRef>>((IEnumerable<CheckConfigurationRef>) r.CheckRuns.Select<CheckRun, CheckConfigurationRef>((Func<CheckRun, CheckConfigurationRef>) (cr => cr.CheckConfigurationRef)).ToList<CheckConfigurationRef>())));
          permissibleResources = Utilities.GetPermissibleResourcesForConfigurations(requestContext, projectId, checkConfigurationRefs);
        }
        suitesByRequestIds.ForEach((Action<CheckSuite>) (checkSuiteResponse => this.PopulateMissingDetails(requestContext, projectId, checkSuiteResponse, permissibleResources)));
        requestContext.TraceInfo(34001913, nameof (CheckSuiteService), "CheckSuiteService::GetCheckSuiteByRequestIds Returning {0} check suites.", (object) suitesByRequestIds.Count<CheckSuite>());
        return (IList<CheckSuite>) suitesByRequestIds;
      }
    }

    public CheckRun UpdateCheckRun(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid runId,
      CheckRunResult result,
      CheckSuiteExpandParameter expand = CheckSuiteExpandParameter.None)
    {
      CheckSuiteService.ValidateCheckRunResultUpdateParametersData(runId, result);
      Dictionary<Guid, CheckRunResult> updateParameters = new Dictionary<Guid, CheckRunResult>()
      {
        {
          runId,
          result
        }
      };
      CheckSuite checkSuite = this.UpdateCheckSuite(requestContext, projectId, (IReadOnlyDictionary<Guid, CheckRunResult>) updateParameters, expand, false).FirstOrDefault<CheckSuite>();
      if (checkSuite == null)
      {
        requestContext.TraceInfo(34001927, nameof (CheckSuiteService), "CheckSuiteService::UpdateCheckRun Check run not found: {0}.", (object) runId);
        throw new CheckRunRequestNotFoundException(PipelineChecksResources.ChecksRunNotFound((object) runId));
      }
      CheckRun checkRun = checkSuite.CheckRuns.Where<CheckRun>((Func<CheckRun, bool>) (r => r.Id == runId)).FirstOrDefault<CheckRun>();
      if (checkRun != null && checkRun.Status == CheckRunStatus.Deferred)
      {
        CheckConfiguration checkConfiguration = CheckSuiteService.GetCheckConfiguration(requestContext, checkRun);
        IdentityRef modifiedBy = new IdentityRef()
        {
          Id = requestContext.GetUserIdentity().Id.ToString("D")
        };
        CheckSuiteService.PublishCheckRunUpdateEventToTelemetry(requestContext, CheckSuiteUpdateParameter.UpdateAction.Defer, checkRun, checkConfiguration, modifiedBy);
      }
      return checkRun;
    }

    public CheckSuite BypassCheckRun(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid checkSuiteId,
      Guid checkRunId,
      CheckSuiteExpandParameter expand = CheckSuiteExpandParameter.None)
    {
      if (!requestContext.IsBypassCheckFeatureEnabled())
        throw new InvalidCheckRunUpdateException("Bypass action is not available.");
      CheckSuiteService.ValidateCheckSuiteUpdateData(requestContext, projectId, checkSuiteId, checkRunId);
      using (new MethodScope(requestContext, nameof (CheckSuiteService), nameof (BypassCheckRun)))
      {
        requestContext.TraceInfo(34001939, nameof (CheckSuiteService), "CheckSuiteService::BypassCheckRun Bypass check suite id {0} with request id {1}.", (object) checkSuiteId, (object) checkRunId);
        IVssRequestContext requestContext1 = requestContext;
        Guid projectId1 = projectId;
        List<Guid> requestIds = new List<Guid>();
        requestIds.Add(checkRunId);
        int expand1 = (int) expand;
        IList<CheckSuite> suiteByRequestIds = this.GetCheckSuiteByRequestIds(requestContext1, projectId1, requestIds, (CheckSuiteExpandParameter) expand1);
        CheckSuite checkSuite1 = suiteByRequestIds != null ? suiteByRequestIds.FirstOrDefault<CheckSuite>((Func<CheckSuite, bool>) (checkSuite => checkSuite.Id.Equals(checkSuiteId))) : (CheckSuite) null;
        if (checkSuite1 == null)
          throw new CheckSuiteRequestNotFoundException(PipelineChecksResources.ChecksSuiteIdNotFound((object) checkSuiteId));
        CheckRun bypassedCheckRun = checkSuite1.CheckRuns.FirstOrDefault<CheckRun>((Func<CheckRun, bool>) (checkRun => checkRun.Id.Equals(checkRunId)));
        if (bypassedCheckRun == null)
          throw new CheckRunRequestNotFoundException(PipelineChecksResources.ChecksRunNotFound((object) checkRunId));
        if (Utilities.GetPermissibleResourcesForConfigurations(requestContext, projectId, new List<CheckConfigurationRef>()
        {
          bypassedCheckRun.CheckConfigurationRef
        }, ResourcePermission.Admin).Find((Predicate<Resource>) (r => r.Equals(bypassedCheckRun.CheckConfigurationRef.Resource))) == null)
          throw new AccessDeniedException(PipelineChecksResources.ResourceAccessDenied());
        if (checkSuite1.isCompleted())
          throw new CheckSuiteIsAlreadyCompleted(PipelineChecksResources.CheckSuiteIsAlreadyCompleted((object) checkSuite1.Id));
        CheckSuiteService.ValidateIfCheckRunIsAlreadyBypassed(requestContext, checkSuiteId, bypassedCheckRun);
        Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
        IdentityRef identityRef = new IdentityRef()
        {
          Id = userIdentity.Id.ToString("D")
        };
        if (!requestContext.GetService<IChecksExtensionService>().GetCheckInstance(bypassedCheckRun.CheckConfigurationRef.Type).Bypass(requestContext, projectId, checkSuiteId, bypassedCheckRun, identityRef))
          throw new CheckSuiteBypassException(PipelineChecksResources.CheckRunBypassGenericError());
        CheckConfiguration checkConfiguration = CheckSuiteService.GetCheckConfiguration(requestContext, bypassedCheckRun);
        CheckSuiteService.PublishCheckRunUpdateEventToTelemetry(requestContext, CheckSuiteUpdateParameter.UpdateAction.Bypass, bypassedCheckRun, checkConfiguration, identityRef);
        return checkSuite1;
      }
    }

    public IList<CheckSuite> UpdateCheckSuite(
      IVssRequestContext requestContext,
      Guid projectId,
      IReadOnlyDictionary<Guid, CheckRunResult> updateParameters,
      CheckSuiteExpandParameter expand = CheckSuiteExpandParameter.None,
      bool ignoreCompletedCheckSuites = false)
    {
      CheckSuiteService.ValidateBasicData(requestContext, projectId);
      using (new MethodScope(requestContext, nameof (CheckSuiteService), nameof (UpdateCheckSuite)))
      {
        updateParameters.ForEach<KeyValuePair<Guid, CheckRunResult>>((Action<KeyValuePair<Guid, CheckRunResult>>) (parameter => CheckSuiteService.ValidateCheckRunResultUpdateParametersData(parameter.Key, parameter.Value)));
        IEnumerable<Guid> keys = updateParameters.Keys;
        requestContext.TraceInfo(34001913, nameof (CheckSuiteService), "CheckSuiteService::UpdateCheckSuite Update check suite request for check runs with request ids: {0}.", (object) string.Join(", ", keys.Select<Guid, string>((Func<Guid, string>) (x => x.ToString()))));
        IList<CheckSuite> suiteByRequestIds = this.GetCheckSuiteByRequestIds(requestContext, projectId, updateParameters.Keys.ToList<Guid>(), expand);
        Dictionary<string, object> auditData = new Dictionary<string, object>();
        Dictionary<Guid, CheckRunResult> dictionary1 = new Dictionary<Guid, CheckRunResult>();
        Dictionary<Guid, CheckSuite> dictionary2 = new Dictionary<Guid, CheckSuite>();
        IChecksExtensionService service = requestContext.GetService<IChecksExtensionService>();
        foreach (KeyValuePair<Guid, CheckRunResult> updateParameter in (IEnumerable<KeyValuePair<Guid, CheckRunResult>>) updateParameters)
        {
          KeyValuePair<Guid, CheckRunResult> updateParam = updateParameter;
          foreach (CheckSuite checkSuite in (IEnumerable<CheckSuite>) suiteByRequestIds)
          {
            if (checkSuite.isCompleted())
            {
              requestContext.TraceAlways(34001931, nameof (CheckSuiteService), "CheckSuiteService::UpdateCheckSuite Check suite with id {0} is already in completed state. Ignore completed check suites flag is {1}.", (object) checkSuite.Id, (object) ignoreCompletedCheckSuites);
              if (!ignoreCompletedCheckSuites)
                throw new CheckSuiteIsAlreadyCompleted(PipelineChecksResources.CheckSuiteIsAlreadyCompleted((object) checkSuite.Id));
              dictionary2[checkSuite.Id] = checkSuite;
            }
            else
            {
              CheckRun check = checkSuite.CheckRuns.FirstOrDefault<CheckRun>((Func<CheckRun, bool>) (r => r.Id == updateParam.Key));
              if (check != null)
              {
                if (requestContext.IsBypassCheckFeatureEnabled() && check.Status.Equals((object) CheckRunStatus.Bypassed))
                  CheckSuiteService.ValidateIfCheckRunIsAlreadyBypassed(requestContext, checkSuite.Id, check);
                dictionary1[updateParam.Key] = updateParam.Value;
                service.GetCheckInstance(check.CheckConfigurationRef.Type);
                checkSuite.CheckRuns.Where<CheckRun>((Func<CheckRun, bool>) (r => r.Id == updateParam.Key)).ToList<CheckRun>().ForEach((Action<CheckRun>) (x => x.Status = updateParam.Value.Status));
                CheckRunStatus checkSuiteStatus = CheckSuiteResult.GetCheckSuiteStatus(checkSuite.CheckRuns, out string _);
                if (CheckSuiteResult.isCheckSuiteCompleted(checkSuiteStatus))
                {
                  try
                  {
                    auditData = CheckSuiteService.GetSuiteAuditLogData(checkSuite, checkSuiteStatus);
                    List<object> objectList = new List<object>();
                    foreach (IGrouping<Guid, CheckRun> source in checkSuite.CheckRuns.GroupBy<CheckRun, Guid>((Func<CheckRun, Guid>) (x => x.CheckConfigurationRef.Type.Id)))
                    {
                      ICheckType checkInstance = service.GetCheckInstance(source.First<CheckRun>().CheckConfigurationRef.Type);
                      List<CheckRun> list = source.ToList<CheckRun>();
                      IVssRequestContext requestContext1 = requestContext;
                      Guid projectId1 = projectId;
                      JObject context = checkSuite.Context;
                      List<CheckRun> checkRuns = list;
                      Dictionary<string, object> auditLogData = checkInstance.GetAuditLogData(requestContext1, projectId1, context, (IList<CheckRun>) checkRuns);
                      if (auditLogData != null)
                      {
                        object obj1;
                        object obj2;
                        object obj3;
                        if (auditLogData.TryGetValue("RunName", out obj1) && auditLogData.TryGetValue("StageName", out obj2) && auditLogData.TryGetValue("PipelineName", out obj3))
                        {
                          auditData["RunName"] = obj1;
                          auditData["StageName"] = obj2;
                          auditData["PipelineName"] = obj3;
                        }
                        object collection;
                        if (auditLogData.TryGetValue("ChecksAuditData", out collection))
                          objectList.AddRange((IEnumerable<object>) new List<object>((IEnumerable<object>) collection));
                      }
                    }
                    auditData.Add("CheckRuns", (object) objectList);
                  }
                  catch (Exception ex)
                  {
                    requestContext.TraceException(34001933, nameof (CheckSuiteService), ex);
                  }
                }
              }
            }
          }
        }
        List<CheckSuite> checkSuiteList1 = new List<CheckSuite>();
        if (dictionary1 != null && dictionary1.Any<KeyValuePair<Guid, CheckRunResult>>())
        {
          requestContext.TraceInfo(34001913, nameof (CheckSuiteService), "CheckSuiteService::UpdateCheckSuite Updating check runs with request ids: {0}.", (object) string.Join(", ", dictionary1.Keys.Select<Guid, string>((Func<Guid, string>) (x => x.ToString()))));
          if (auditData.Serialize<Dictionary<string, object>>().Length > 4000)
            auditData.Remove("CheckRuns");
          List<CheckSuite> checkSuiteList2;
          using (CheckSuiteComponent component = requestContext.CreateComponent<CheckSuiteComponent>())
            checkSuiteList2 = component.UpdateCheckSuite(projectId, (IReadOnlyDictionary<Guid, CheckRunResult>) dictionary1, auditData);
          List<Resource> resources1 = new List<Resource>();
          if ((expand & CheckSuiteExpandParameter.Resources).Equals((object) CheckSuiteExpandParameter.Resources))
          {
            List<CheckConfigurationRef> checkConfigurationRefs = new List<CheckConfigurationRef>();
            checkSuiteList2.ForEach((Action<CheckSuite>) (r => checkConfigurationRefs.AddRangeIfRangeNotNull<CheckConfigurationRef, List<CheckConfigurationRef>>((IEnumerable<CheckConfigurationRef>) r.CheckRuns.Select<CheckRun, CheckConfigurationRef>((Func<CheckRun, CheckConfigurationRef>) (cr => cr.CheckConfigurationRef)).ToList<CheckConfigurationRef>())));
            resources1 = Utilities.GetPermissibleResourcesForConfigurations(requestContext, projectId, checkConfigurationRefs);
          }
          bool flag = requestContext.IsFeatureEnabled("Pipelines.Checks.AuthChecksAsyncFlow");
          foreach (CheckSuite checkSuite1 in checkSuiteList2)
          {
            CheckSuite checkSuite2 = checkSuite1;
            try
            {
              if (checkSuite1.isDeclined())
                checkSuite2 = this.CancelCheckSuite(requestContext, projectId, checkSuite1.Id);
              if (checkSuite1.isCompleted())
                this.CancelCheckRunPendingTimeoutJobs(requestContext, (IList<CheckRun>) checkSuite1.CheckRuns);
              this.PopulateMissingDetails(requestContext, projectId, checkSuite2, resources1);
              if (flag)
              {
                List<Resource> resources2 = checkSuite2.Resources;
                if (resources2 != null && resources2.Any<Resource>())
                  checkSuite2 = this.EvaluateChecksOnResources(requestContext, projectId, checkSuite2, resources2, checkSuite2.Context, true);
              }
              checkSuiteList1.Add(checkSuite2);
              this.SendCheckSuiteUpdatedNotificationEvent(requestContext, projectId, checkSuite2);
            }
            catch (Exception ex)
            {
              requestContext.TraceException(34001909, nameof (CheckSuiteService), ex);
              checkSuiteList1.Add(checkSuite2);
            }
          }
          requestContext.TraceInfo(34001921, nameof (CheckSuiteService), "CheckSuiteService::UpdateCheckSuite Updated {0} check suites", (object) checkSuiteList1.Count<CheckSuite>());
        }
        checkSuiteList1.AddRangeIfRangeNotNull<CheckSuite, List<CheckSuite>>((IEnumerable<CheckSuite>) dictionary2?.Values);
        return (IList<CheckSuite>) checkSuiteList1;
      }
    }

    public CheckSuite Evaluate(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid checkSuiteId,
      List<Resource> resources,
      JObject evaluationContext,
      CheckSuiteExpandParameter expand = CheckSuiteExpandParameter.None)
    {
      CheckSuiteService.ValidateBasicData(requestContext, projectId);
      using (new MethodScope(requestContext, nameof (CheckSuiteService), nameof (Evaluate)))
      {
        CheckSuite checkSuite = CheckSuiteResult.GetCheckSuiteResponse(checkSuiteId, (List<CheckRun>) null, (CheckSuiteContext) null);
        try
        {
          if (resources != null && resources.Any<Resource>())
            checkSuite = this.EvaluateChecksOnResources(requestContext, projectId, checkSuite, resources, evaluationContext);
          else
            this.AddCheckSuite(requestContext, projectId, checkSuiteId, new List<CheckRun>(), new List<Resource>(), evaluationContext);
        }
        catch (Exception ex)
        {
          requestContext.TraceError(34001913, nameof (CheckSuiteService), "CheckSuiteService::EvaluateCheckSuite Exception received: {0}", (object) ex);
          throw;
        }
        return checkSuite;
      }
    }

    public CheckSuite RerunCheckRun(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid checkSuiteId,
      Guid taskToRerun)
    {
      if (!requestContext.IsFeatureEnabled("Pipelines.Checks.EnableRerunChecks"))
        throw new InvalidCheckRunUpdateException("The provided action is not yet available.");
      CheckSuiteService.ValidateCheckSuiteUpdateData(requestContext, projectId, checkSuiteId, taskToRerun);
      using (new MethodScope(requestContext, nameof (CheckSuiteService), nameof (RerunCheckRun)))
      {
        requestContext.TraceInfo(34001938, nameof (CheckSuiteService), string.Format("CheckSuiteService::RerunCheckRequest Rerun request with id {0} from check suite with id {1}.", (object) 0, (object) 1), (object) taskToRerun, (object) checkSuiteId);
        CheckSuite checkSuite = this.GetCheckSuitesByIds(requestContext, projectId, new List<Guid>()
        {
          checkSuiteId
        }, CheckSuiteExpandParameter.None).FirstOrDefault<CheckSuite>();
        if (checkSuite == null)
          throw new CheckSuiteRequestNotFoundException(PipelineChecksResources.ChecksSuiteIdNotFound((object) checkSuiteId));
        if (checkSuite.isCompleted())
          throw new CheckSuiteIsAlreadyCompleted(PipelineChecksResources.CheckSuiteIsAlreadyCompleted((object) checkSuite.Id));
        CheckRun check = checkSuite.CheckRuns.Where<CheckRun>((Func<CheckRun, bool>) (c => c.Id == taskToRerun)).FirstOrDefault<CheckRun>();
        if (check == null)
          throw new CheckRunRequestNotFoundException(PipelineChecksResources.ChecksRunNotFound((object) taskToRerun));
        if (Utilities.GetPermissibleResourcesForConfigurations(requestContext, projectId, new List<CheckConfigurationRef>()
        {
          check.CheckConfigurationRef
        }, ResourcePermission.Admin).Find((Predicate<Resource>) (r => r.Equals(check.CheckConfigurationRef.Resource))) == null)
          throw new AccessDeniedException(PipelineChecksResources.ResourceAccessDenied());
        if (check.CheckConfigurationRef.Type.Id != TaskCheckConstants.TaskCheckTypeId)
          throw new CheckCannotBeRerunException(PipelineChecksResources.CheckCannotBeRerun((object) taskToRerun));
        if (check.Status != CheckRunStatus.Approved)
          throw new CheckCannotBeRerunException(PipelineChecksResources.CheckCannotBeRerun((object) taskToRerun));
        CheckSuiteService.ValidateIfCheckRunIsAlreadyBypassed(requestContext, checkSuiteId, check);
        ICheckType checkInstance = requestContext.GetService<IChecksExtensionService>().GetCheckInstance(check.CheckConfigurationRef.Type);
        if (checkInstance == null)
        {
          requestContext.TraceError("CheckSuiteService::RerunCheckRequest CheckTypeInstance is null for check id {0}", taskToRerun.ToString());
          throw new CheckCannotBeRerunException(PipelineChecksResources.CheckCannotBeRerun((object) taskToRerun));
        }
        CheckConfiguration checkConfiguration1 = CheckSuiteService.GetCheckConfiguration(requestContext, check);
        TaskCheckConfiguration checkConfiguration2 = checkConfiguration1 != null && checkConfiguration1 is TaskCheckConfiguration ? (TaskCheckConfiguration) checkConfiguration1 : throw new CheckCannotBeRerunException(PipelineChecksResources.CheckCannotBeRerun((object) taskToRerun));
        TaskCheckConfig settings1 = checkConfiguration2.Settings;
        Guid guid = settings1 != null ? settings1.DefinitionRef.Id : Guid.Empty;
        TaskCheckConfig settings2 = checkConfiguration2.Settings;
        int num1;
        if (settings2 == null)
        {
          num1 = 1;
        }
        else
        {
          int? retryInterval = settings2.RetryInterval;
          int num2 = 0;
          num1 = !(retryInterval.GetValueOrDefault() == num2 & retryInterval.HasValue) ? 1 : 0;
        }
        if (num1 != 0 || this.m_notRetriableTaskCheckIds.Contains(guid))
          throw new CheckCannotBeRerunException(PipelineChecksResources.CheckCannotBeRerun((object) taskToRerun));
        new Dictionary<Guid, CheckConfiguration>()
        {
          {
            taskToRerun,
            checkConfiguration1
          }
        };
        HashSet<Resource> resourceSet = new HashSet<Resource>();
        try
        {
          CheckpointContext checkpointContext = checkSuite.Context.ToObject<CheckpointContext>();
          checkInstance.Rerun(requestContext, projectId, checkSuiteId, checkpointContext.GraphNode.Name, check);
          requestContext.TraceInfo(34001913, nameof (CheckSuiteService), "CheckSuiteService::RerunCheck Rerunning check runs with requestId: {0}", (object) check.Id.ToString());
          CheckSuiteService.PublishCheckRunUpdateEventToTelemetry(requestContext, CheckSuiteUpdateParameter.UpdateAction.Rerun, check, checkConfiguration1);
        }
        catch (AccessDeniedException ex)
        {
          requestContext.TraceError(34001905, nameof (CheckSuiteService), "CheckSuiteService::RerunCheck Rerunning is unauthorized, reason: {0}", (object) ex.Message);
        }
        catch (Exception ex)
        {
          requestContext.TraceException(34001914, nameof (CheckSuiteService), ex);
        }
        return checkSuite;
      }
    }

    private static CheckConfiguration GetCheckConfiguration(
      IVssRequestContext requestContext,
      CheckRun checkRun)
    {
      using (CheckConfigurationComponent component = requestContext.CreateComponent<CheckConfigurationComponent>())
        return requestContext.IsFeatureEnabled("Pipelines.Checks.EnableOrderingChecks") ? component.GetCheckConfigurationByIdVersion(checkRun.CheckConfigurationRef.Id, checkRun.CheckConfigurationRef.Version, true) : component.GetCheckConfiguration(checkRun.CheckConfigurationRef.Id, true);
    }

    private static void ValidateIfCheckRunIsAlreadyBypassed(
      IVssRequestContext requestContext,
      Guid checkSuiteId,
      CheckRun check)
    {
      ArgumentUtility.CheckForNull<CheckRun>(check, nameof (check), "Pipeline.Checks");
      if (check.Status.Equals((object) CheckRunStatus.Bypassed))
      {
        IdentityService service = requestContext.GetService<IdentityService>();
        CheckRunUpdate checkRunUpdate = check.ResultUpdates.OrderByDescending<CheckRunUpdate, DateTime>((Func<CheckRunUpdate, DateTime>) (ru => ru.ModifiedOn)).FirstOrDefault<CheckRunUpdate>((Func<CheckRunUpdate, bool>) (ru => ru.Status.Equals((object) CheckRunStatus.Bypassed)));
        if (checkRunUpdate != null)
        {
          IdentityRef identityRef;
          if (service.QueryIdentities(requestContext, (IList<string>) new List<string>()
          {
            checkRunUpdate.ModifiedBy.Id
          }).TryGetValue(checkRunUpdate.ModifiedBy.Id, out identityRef))
            throw new CheckSuiteUpdateException(PipelineChecksResources.CheckRunAlreadyBypassedBy((object) check.Id, (object) checkSuiteId, (object) identityRef.DisplayName));
        }
        throw new CheckSuiteUpdateException(PipelineChecksResources.CheckRunAlreadyBypassed((object) check.Id, (object) checkSuiteId));
      }
    }

    public IList<CheckRun> FilterCheckRuns(
      IVssRequestContext requestContext,
      Guid projectId,
      CheckRunFilter checkRunFilter)
    {
      CheckSuiteService.ValidateBasicData(requestContext, projectId);
      using (new MethodScope(requestContext, nameof (CheckSuiteService), nameof (FilterCheckRuns)))
      {
        ArgumentValidation.ValidateCheckRunFilter(checkRunFilter);
        requestContext.TraceInfo(34001913, nameof (CheckSuiteService), "CheckSuiteService::FilterCheckRuns Get Filtered Check Runs with resource id : {0}, resource Type : {1}, checkType : {2}, Status : {3}", (object) checkRunFilter.Resource.Id, (object) checkRunFilter.Resource.Type, (object) checkRunFilter.Type, (object) checkRunFilter.Status);
        IList<CheckRun> checkRuns;
        using (CheckSuiteComponent component = requestContext.CreateComponent<CheckSuiteComponent>())
          checkRuns = component.GetCheckRuns(projectId, checkRunFilter);
        requestContext.TraceInfo(34001913, nameof (CheckSuiteService), "CheckSuiteService::FilterCheckRuns Returning {0} check runs.", (object) checkRuns.Count<CheckRun>());
        return checkRuns;
      }
    }

    public CheckSuite CancelCheckSuite(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid checkSuiteId,
      string cancelMessage = null)
    {
      CheckSuiteService.ValidateBasicData(requestContext, projectId);
      using (new MethodScope(requestContext, nameof (CheckSuiteService), nameof (CancelCheckSuite)))
      {
        ArgumentUtility.CheckForEmptyGuid(checkSuiteId, nameof (checkSuiteId), "Pipeline.Checks");
        requestContext.TraceInfo(34001913, nameof (CheckSuiteService), "CheckSuiteService::CancelCheckSuite Cancelation request for check suite id: {0}", (object) checkSuiteId);
        CheckSuite checkSuite;
        using (CheckSuiteComponent component = requestContext.CreateComponent<CheckSuiteComponent>())
        {
          checkSuite = component.CancelCheckSuit(projectId, checkSuiteId, cancelMessage);
          requestContext.TraceInfo(34001922, nameof (CheckSuiteService), "CheckSuiteService::CancelCheckSuite Cancelled check evaluation with check suite id: {0}, {1} records canceled", (object) checkSuiteId, (object) checkSuite.CheckRuns.Count<CheckRun>());
        }
        List<CheckRun> checkRuns = checkSuite.CheckRuns;
        CheckSuiteService.CancelPluginCheckRuns(requestContext, projectId, checkSuiteId, checkRuns);
        this.CancelCheckRunPendingTimeoutJobs(requestContext, (IList<CheckRun>) checkRuns);
        return checkSuite;
      }
    }

    public void TimeoutCheckRun(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid checkSuiteId,
      Guid checkRunId)
    {
      try
      {
        CheckSuiteService.ValidateBasicData(requestContext, projectId);
        ArgumentUtility.CheckForEmptyGuid(checkSuiteId, nameof (checkSuiteId), "Pipeline.Checks");
        ArgumentUtility.CheckForEmptyGuid(checkRunId, nameof (checkRunId), "Pipeline.Checks");
        CheckRun checkRun = this.GetCheckRun(requestContext, projectId, checkSuiteId, checkRunId);
        if (!checkRun.IsEvaluationInProgress)
          return;
        CheckRunStatus checkRunStatus = CheckRunStatus.Rejected;
        try
        {
          checkRunStatus = requestContext.GetService<IChecksExtensionService>().GetCheckInstance(checkRun.CheckConfigurationRef.Type).Timeout(requestContext, projectId, checkSuiteId, checkRun.Id);
        }
        catch (Exception ex)
        {
          requestContext.TraceError(34001925, nameof (CheckSuiteService), "CheckSuiteService::TimeoutCheckRun Exception received from plugin: {0}", (object) ex);
        }
        this.UpdateCheckRun(requestContext, projectId, checkRun.Id, new CheckRunResult()
        {
          Status = checkRunStatus
        }, CheckSuiteExpandParameter.None);
      }
      catch (Exception ex)
      {
        requestContext.TraceError(34001925, nameof (CheckSuiteService), "CheckSuiteService::TimeoutCheckRun Exception received: {0}", (object) ex);
      }
    }

    internal CheckSuite AddCheckSuite(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid checkSuiteId,
      List<CheckRun> records,
      List<Resource> resources,
      JObject evaluationContext,
      CheckSuiteExpandParameter expand = CheckSuiteExpandParameter.None)
    {
      CheckSuiteService.ValidateBasicData(requestContext, projectId);
      using (new MethodScope(requestContext, nameof (CheckSuiteService), nameof (AddCheckSuite)))
      {
        try
        {
          ArgumentUtility.CheckForEmptyGuid(checkSuiteId, nameof (checkSuiteId), "Pipeline.Checks");
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          records.ForEach(CheckSuiteService.\u003C\u003EO.\u003C0\u003E__ValidateCheckRunData ?? (CheckSuiteService.\u003C\u003EO.\u003C0\u003E__ValidateCheckRunData = new Action<CheckRun>(CheckSuiteService.ValidateCheckRunData)));
          requestContext.TraceInfo(34001913, nameof (CheckSuiteService), "CheckSuiteService::AddCheckSuite Check suite request with id: {0} and records: {1}", (object) checkSuiteId, (object) records.Count<CheckRun>());
          CheckSuite checkSuiteResponse;
          using (CheckSuiteComponent component = requestContext.CreateComponent<CheckSuiteComponent>())
          {
            checkSuiteResponse = component.AddCheckSuite(projectId, checkSuiteId, records, resources, evaluationContext);
            requestContext.TraceInfo(34001920, nameof (CheckSuiteService), "CheckSuiteService::AddCheckSuite Added check suite with id: {0}", (object) checkSuiteId);
          }
          if (checkSuiteResponse == null)
          {
            requestContext.TraceInfo(34001924, nameof (CheckSuiteService), "CheckSuiteService::AddCheckSuite Suite not found: {0}.", (object) checkSuiteId);
            throw new CheckSuiteRequestNotFoundException(PipelineChecksResources.ChecksSuiteIdNotFound((object) checkSuiteId));
          }
          List<Resource> resources1 = new List<Resource>();
          if ((expand & CheckSuiteExpandParameter.Resources).Equals((object) CheckSuiteExpandParameter.Resources))
            resources1 = Utilities.GetPermissibleResourcesForConfigurations(requestContext, projectId, records.Select<CheckRun, CheckConfigurationRef>((Func<CheckRun, CheckConfigurationRef>) (r => r.CheckConfigurationRef)).ToList<CheckConfigurationRef>());
          this.PopulateMissingDetails(requestContext, projectId, checkSuiteResponse, resources1);
          return checkSuiteResponse;
        }
        catch (Exception ex)
        {
          string str1 = records == null || !records.Any<CheckRun>() ? string.Empty : JsonConvert.SerializeObject((object) records);
          string str2 = resources == null || !resources.Any<Resource>() ? string.Empty : JsonConvert.SerializeObject((object) resources);
          string str3 = evaluationContext != null ? evaluationContext.ToString() : string.Empty;
          requestContext.TraceException(34001930, nameof (CheckSuiteService), ex);
          requestContext.TraceError(34001930, nameof (CheckSuiteService), "CheckSuiteService::AddCheckSuite failed. Parameters: ProjectId: {0}, CheckSuiteId: {1}, Records: {2}, Resources: {3}, EvaluationContext: {4}", (object) projectId, (object) checkSuiteId, (object) str1, (object) str2, (object) str3);
          throw;
        }
      }
    }

    internal CheckSuite AddCheckRuns(
      IVssRequestContext requestContext,
      Guid projectId,
      CheckSuite checkSuite,
      IList<CheckRun> records,
      CheckSuiteExpandParameter expand,
      bool isCheckOrderingEnabled)
    {
      CheckSuiteService.ValidateBasicData(requestContext, projectId);
      using (new MethodScope(requestContext, nameof (CheckSuiteService), nameof (AddCheckRuns)))
      {
        ArgumentUtility.CheckForEmptyGuid(checkSuite.Id, "Id", "Pipeline.Checks");
        ArgumentUtility.CheckForNull<IList<CheckRun>>(records, nameof (records), "Pipeline.Checks");
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        records.ForEach<CheckRun>(CheckSuiteService.\u003C\u003EO.\u003C0\u003E__ValidateCheckRunData ?? (CheckSuiteService.\u003C\u003EO.\u003C0\u003E__ValidateCheckRunData = new Action<CheckRun>(CheckSuiteService.ValidateCheckRunData)));
        requestContext.TraceInfo(34001913, nameof (CheckSuiteService), "CheckSuiteService::AddCheckRuns Check run request with suite id: {0} and records: {1}", (object) checkSuite.Id, (object) records.Count<CheckRun>());
        CheckSuite checkSuiteResponse;
        using (CheckSuiteComponent component = requestContext.CreateComponent<CheckSuiteComponent>())
        {
          if (isCheckOrderingEnabled)
          {
            Dictionary<Guid, CheckRunResult> existingRecords = records.Where<CheckRun>((Func<CheckRun, bool>) (checkRun => checkSuite.CheckRuns.Exists((Predicate<CheckRun>) (cr => cr.Id == checkRun.Id)))).ToDictionary<CheckRun, Guid, CheckRunResult>((Func<CheckRun, Guid>) (checkRun => checkRun.Id), (Func<CheckRun, CheckRunResult>) (checkRun => (CheckRunResult) checkRun));
            if (!records.All<CheckRun>((Func<CheckRun, bool>) (checkRun => existingRecords.ContainsKey(checkRun.Id))))
              throw new InvalidCheckRunUpdateException("Not all of the check runs are present in checksuite");
            if (!existingRecords.IsNullOrEmpty<KeyValuePair<Guid, CheckRunResult>>())
            {
              component.UpdateCheckSuite(projectId, (IReadOnlyDictionary<Guid, CheckRunResult>) existingRecords, new Dictionary<string, object>()).FirstOrDefault<CheckSuite>();
              requestContext.TraceInfo(34001921, nameof (CheckSuiteService), "CheckSuiteService::UpdateCheckSuite Updated check runs for suite id : {0}", (object) checkSuite.Id);
            }
            checkSuiteResponse = component.GetCheckSuite(projectId, checkSuite.Id);
          }
          else
          {
            checkSuiteResponse = component.AddCheckRuns(projectId, checkSuite.Id, records);
            requestContext.TraceInfo(34001920, nameof (CheckSuiteService), "CheckSuiteService::AddCheckRuns Added check runs for suite id : {0}", (object) checkSuite.Id);
          }
        }
        if (checkSuiteResponse == null)
        {
          requestContext.TraceInfo(34001924, nameof (CheckSuiteService), "CheckSuiteService::AddCheckRuns Suite not found: {0}.", (object) checkSuite.Id);
          throw new CheckSuiteRequestNotFoundException(PipelineChecksResources.ChecksSuiteIdNotFound((object) checkSuite.Id));
        }
        List<Resource> resources = new List<Resource>();
        if ((expand & CheckSuiteExpandParameter.Resources).Equals((object) CheckSuiteExpandParameter.Resources))
          resources = Utilities.GetPermissibleResourcesForConfigurations(requestContext, projectId, records.Select<CheckRun, CheckConfigurationRef>((Func<CheckRun, CheckConfigurationRef>) (r => r.CheckConfigurationRef)).ToList<CheckConfigurationRef>());
        this.PopulateMissingDetails(requestContext, projectId, checkSuiteResponse, resources);
        return checkSuiteResponse;
      }
    }

    private static Dictionary<string, object> GetSuiteAuditLogData(
      CheckSuite checkSuite,
      CheckRunStatus status)
    {
      Dictionary<string, object> suiteAuditLogData = new Dictionary<string, object>();
      if (checkSuite != null)
      {
        suiteAuditLogData.Add("CheckSuiteId", (object) checkSuite.Id);
        suiteAuditLogData.Add("CheckSuiteStatus", (object) status);
      }
      return suiteAuditLogData;
    }

    private void ScheduleTimeoutJobs(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid checkSuiteId,
      IList<CheckRun> checkRuns,
      Dictionary<int, int> checkConfigIdToTimeoutMap,
      DateTime evaluationStartTime)
    {
      foreach (CheckRun checkRun in (IEnumerable<CheckRun>) checkRuns)
      {
        int num;
        if (checkConfigIdToTimeoutMap.TryGetValue(checkRun.CheckConfigurationRef.Id, out num))
        {
          XmlNode xml = TeamFoundationSerializationUtility.SerializeToXml((object) CheckTimeoutJobData.GetJobData(projectId, checkSuiteId, checkRun.Id));
          DateTime scheduleTime = evaluationStartTime.AddMinutes((double) num);
          Guid jobId = Guid.NewGuid();
          checkRun.TimeoutJobId = new Guid?(jobId);
          try
          {
            JobHelper.ScheduleJob(requestContext, jobId, JobConstants.TimeoutJobName, JobConstants.CheckTimeoutJobExtension, xml, scheduleTime);
          }
          catch (Exception ex)
          {
            requestContext.TraceError(34001913, nameof (CheckSuiteService), "CheckSuiteService::EvaluateCheckSuite Exception received: {0}", (object) ex);
          }
        }
      }
    }

    private void PopulateMapsForCheckConfigurations(
      IVssRequestContext requestContext,
      IList<CheckConfiguration> checkConfigurations,
      Dictionary<CheckEvaluationOrder, Dictionary<ICheckType, Dictionary<Guid, CheckConfiguration>>> orderedCheckTypeToEvaluationMap,
      Dictionary<int, int> checkConfigIdToTimeoutMap,
      Dictionary<int, CheckEvaluationOrder> checkConfigIdToOrder,
      List<CheckRun> existingRuns,
      bool isCheckOrderingEnabled)
    {
      IChecksExtensionService service = requestContext.GetService<IChecksExtensionService>();
      foreach (CheckConfiguration checkConfiguration in (IEnumerable<CheckConfiguration>) checkConfigurations)
      {
        int assignmentId = checkConfiguration.Id;
        int assignmentVersion = checkConfiguration.Version;
        CheckType type = checkConfiguration.Type;
        if (assignmentId > 0 && type != null)
        {
          ICheckType checkInstance = service.GetCheckInstance(type);
          if (checkInstance != null)
          {
            CheckRun checkRun = existingRuns.FirstOrDefault<CheckRun>((Func<CheckRun, bool>) (r => r.CheckConfigurationRef.Id == assignmentId && r.CheckConfigurationRef.Version == assignmentVersion));
            Guid requestId = checkRun != null & isCheckOrderingEnabled ? checkRun.Id : Guid.NewGuid();
            CheckEvaluationOrder key = isCheckOrderingEnabled ? checkInstance.EvaluationOrder(checkConfiguration) : checkInstance.EvaluationOrder();
            Dictionary<ICheckType, Dictionary<Guid, CheckConfiguration>> checkTypeToEvaluationRequestsMap;
            if (!orderedCheckTypeToEvaluationMap.TryGetValue(key, out checkTypeToEvaluationRequestsMap))
            {
              checkTypeToEvaluationRequestsMap = new Dictionary<ICheckType, Dictionary<Guid, CheckConfiguration>>();
              orderedCheckTypeToEvaluationMap.Add(key, checkTypeToEvaluationRequestsMap);
            }
            CheckSuiteService.PopulateCheckTypeToEvaluationRequestsMap(checkTypeToEvaluationRequestsMap, checkInstance, requestId, checkConfiguration);
            checkConfigIdToOrder[checkConfiguration.Id] = key;
          }
          bool flag = true;
          if ((type.Id == TaskCheckConstants.TaskCheckTypeId || type.Name == "Task Check") && !((IEnumerable<Guid>) this.m_whitelistedTaskCheckIds).Contains<Guid>((checkConfiguration.GetCheckConfigurationSettings() as TaskCheckConfig).DefinitionRef.Id))
            flag = false;
          if (flag)
          {
            int? timeout = checkConfiguration.Timeout;
            if (timeout.HasValue)
            {
              timeout = checkConfiguration.Timeout;
              if (timeout.Value > 0)
              {
                Dictionary<int, int> dictionary = checkConfigIdToTimeoutMap;
                int id = checkConfiguration.Id;
                timeout = checkConfiguration.Timeout;
                int num = timeout.Value;
                dictionary[id] = num;
                continue;
              }
            }
            checkConfigIdToTimeoutMap[checkConfiguration.Id] = this.c_defaultTimeout;
          }
        }
      }
    }

    private void PublishCheckSuiteEvaluateEventToTelemetry(
      IVssRequestContext requestContext,
      CheckSuite response,
      DateTime startTime)
    {
      try
      {
        string feature = string.Format("{0}.{1}", (object) nameof (CheckSuiteService), (object) "Evaluate");
        CustomerIntelligenceData properties = new CustomerIntelligenceData();
        foreach (KeyValuePair<string, JToken> keyValuePair in response.Context)
        {
          if (keyValuePair.Value != null)
            properties.Add(keyValuePair.Key, (object) keyValuePair.Value);
        }
        properties.Add("No. of Checks", (double) response.CheckRuns.Count<CheckRun>());
        properties.Add("Result", (object) response.Status);
        DateTime valueOrDefault = response.CompletedDate.GetValueOrDefault();
        if (response.CompletedDate.HasValue)
          properties.Add("Duration", this.GetReadableDuration(startTime, valueOrDefault));
        requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "Pipeline.Checks", feature, properties);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(34001908, nameof (CheckSuiteService), ex);
      }
    }

    private static void PublishCheckRunUpdateEventToTelemetry(
      IVssRequestContext requestContext,
      CheckSuiteUpdateParameter.UpdateAction action,
      CheckRun checkRun,
      CheckConfiguration checkConfiguration,
      IdentityRef modifiedBy = null)
    {
      try
      {
        ArgumentUtility.CheckGenericForNull((object) action, nameof (action));
        ArgumentUtility.CheckForNull<CheckRun>(checkRun, nameof (checkRun));
        ArgumentUtility.CheckForNull<CheckConfiguration>(checkConfiguration, nameof (checkConfiguration));
        string feature = string.Format("{0}.{1}", (object) nameof (CheckSuiteService), (object) "Update");
        CustomerIntelligenceData properties = new CustomerIntelligenceData();
        properties.Add("CheckSuiteUpdateAction", action.ToString());
        properties.Add("CheckRun", checkRun.ToStringExtended());
        properties.Add("CheckConfiguration", checkConfiguration.ToStringEx());
        if (modifiedBy != null && modifiedBy.Id != null)
          properties.Add("ModifiedBy", modifiedBy.Id);
        requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "Pipeline.Checks", feature, properties);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(34001908, nameof (CheckSuiteService), ex);
      }
    }

    private string GetReadableDuration(DateTime startTime, DateTime endTime)
    {
      TimeSpan timeSpan = endTime - startTime;
      return string.Format("{0:D2} hrs, {1:D2} mins, {2:D2} secs", (object) timeSpan.Hours, (object) timeSpan.Minutes, (object) timeSpan.Seconds);
    }

    private static void PopulateCheckTypeToEvaluationRequestsMap(
      Dictionary<ICheckType, Dictionary<Guid, CheckConfiguration>> checkTypeToEvaluationRequestsMap,
      ICheckType type,
      Guid requestId,
      CheckConfiguration checkConfiguration)
    {
      Dictionary<Guid, CheckConfiguration> dictionary;
      if (checkTypeToEvaluationRequestsMap.TryGetValue(type, out dictionary))
        dictionary[requestId] = checkConfiguration;
      else
        checkTypeToEvaluationRequestsMap[type] = new Dictionary<Guid, CheckConfiguration>()
        {
          [requestId] = checkConfiguration
        };
    }

    private static List<CheckRun> EvaluateChecks(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid checkSuiteId,
      JObject evaluationContext,
      List<Resource> resources,
      Dictionary<ICheckType, Dictionary<Guid, CheckConfiguration>> checkTypeToEvaluationRequestsMap,
      out bool evaluationFailed)
    {
      Dictionary<ICheckType, bool> dictionary = new Dictionary<ICheckType, bool>();
      List<CheckRun> checkRuns1 = new List<CheckRun>();
      evaluationFailed = false;
      foreach (KeyValuePair<ICheckType, Dictionary<Guid, CheckConfiguration>> evaluationRequests1 in checkTypeToEvaluationRequestsMap)
      {
        ICheckType key = evaluationRequests1.Key;
        IReadOnlyDictionary<Guid, CheckRunResult> readOnlyDictionary = (IReadOnlyDictionary<Guid, CheckRunResult>) new Dictionary<Guid, CheckRunResult>();
        try
        {
          Dictionary<Guid, CheckConfiguration> evaluationRequests2 = checkTypeToEvaluationRequestsMap[key];
          requestContext.TraceInfo(34001913, nameof (CheckSuiteService), "CheckSuiteService:EvaluateChecks Starting check evaluation for type {0} total request count {1}", (object) key.CheckTypeName, (object) evaluationRequests2.Count);
          IReadOnlyDictionary<Guid, CheckRunResult> checkRunResults = key.Evaluate(requestContext, projectId, evaluationContext, (IReadOnlyDictionary<Guid, CheckConfiguration>) evaluationRequests2, resources);
          if (checkRunResults != null)
          {
            List<CheckRun> checkRuns2 = CheckSuiteService.GetCheckRuns(checkSuiteId, checkRunResults, (IReadOnlyDictionary<Guid, CheckConfiguration>) evaluationRequests2, key);
            checkRuns1.AddRange((IEnumerable<CheckRun>) checkRuns2);
            dictionary[key] = true;
            requestContext.TraceInfo(34001913, nameof (CheckSuiteService), "CheckSuiteService::EvaluateChecks Evaluation completed for {0} check runs for type: {1}", (object) checkRuns2.Count<CheckRun>(), (object) key.CheckTypeName);
            IEnumerable<CheckRun> checkRuns3 = checkRuns2.Where<CheckRun>((Func<CheckRun, bool>) (run => (run.Status & CheckRunStatus.Failed) > CheckRunStatus.None));
            if (!evaluationFailed)
            {
              if (checkRuns3.Any<CheckRun>())
                evaluationFailed = key.ShouldEvaluationFailUponCheckFailure(requestContext, checkRuns3, (IDictionary<Guid, CheckConfiguration>) evaluationRequests2);
            }
          }
        }
        catch (Exception ex)
        {
          evaluationFailed = true;
          requestContext.TraceException(34001910, nameof (CheckSuiteService), ex);
          break;
        }
      }
      if (evaluationFailed)
      {
        requestContext.TraceInfo(34001923, nameof (CheckSuiteService), "CheckSuiteService::EvaluateChecks Evaluation failed for check suite id: {0}, canceling the in-progress evaluations", (object) checkSuiteId);
        checkRuns1.ForEach((Action<CheckRun>) (record =>
        {
          if (!record.IsEvaluationInProgress)
            return;
          record.Status = CheckRunStatus.Canceled;
        }));
        CheckSuiteService.CancelPluginCheckRuns(requestContext, projectId, checkSuiteId, checkRuns1);
        foreach (KeyValuePair<ICheckType, Dictionary<Guid, CheckConfiguration>> evaluationRequests3 in checkTypeToEvaluationRequestsMap)
        {
          ICheckType key = evaluationRequests3.Key;
          if (!dictionary.ContainsKey(key))
          {
            Dictionary<Guid, CheckConfiguration> evaluationRequests4 = checkTypeToEvaluationRequestsMap[key];
            List<CheckRun> cancellationCheckRuns = CheckSuiteService.GetCancellationCheckRuns(checkSuiteId, (IReadOnlyDictionary<Guid, CheckConfiguration>) evaluationRequests4, key);
            checkRuns1.AddRange((IEnumerable<CheckRun>) cancellationCheckRuns);
          }
        }
      }
      return checkRuns1;
    }

    internal CheckSuite EvaluateChecksOnResources(
      IVssRequestContext requestContext,
      Guid projectId,
      CheckSuite checkSuite,
      List<Resource> resources,
      JObject evaluationContext,
      bool checkSuiteExists = false,
      CheckSuiteExpandParameter expand = CheckSuiteExpandParameter.None)
    {
      DateTime utcNow = DateTime.UtcNow;
      Guid id = checkSuite.Id;
      ArgumentUtility.CheckForEmptyGuid(id, "checkSuiteId", "Pipeline.Checks");
      ArgumentUtility.CheckForNull<List<Resource>>(resources, nameof (resources), "Pipeline.Checks");
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      resources.ForEach(CheckSuiteService.\u003C\u003EO.\u003C1\u003E__ValidateResource ?? (CheckSuiteService.\u003C\u003EO.\u003C1\u003E__ValidateResource = new Action<Resource>(ArgumentValidation.ValidateResource)));
      requestContext.TraceInfo(34001913, nameof (CheckSuiteService), "CheckSuiteService::EvaluateChecksOnResources Evaluation request with check suite id: {0} on {1} resources", (object) id, (object) resources.Count<Resource>());
      bool flag = requestContext.IsFeatureEnabled("Pipelines.Checks.EnableDisabledChecksFeature");
      bool isCheckOrderingEnabled = requestContext.IsFeatureEnabled("Pipelines.Checks.EnableOrderingChecks");
      List<CheckConfiguration> checkConfigurationList;
      using (CheckConfigurationComponent component = requestContext.CreateComponent<CheckConfigurationComponent>())
      {
        if (isCheckOrderingEnabled & checkSuiteExists && checkSuite.CheckRuns.Count > 0)
        {
          IEnumerable<CheckConfigurationRef> checkParams = checkSuite.CheckRuns.Select<CheckRun, CheckConfigurationRef>((Func<CheckRun, CheckConfigurationRef>) (cr => cr.CheckConfigurationRef));
          checkConfigurationList = component.GetCheckConfigurationsByIdVersion(checkParams, true, true);
        }
        else
          checkConfigurationList = component.GetCheckConfigurationsOnResources(resources, !flag, true);
      }
      if (checkConfigurationList == null || checkConfigurationList.Count == 0)
      {
        requestContext.TraceAlways(34001911, nameof (CheckSuiteService), "CheckSuiteService:EvaluateChecksOnResources There are no configured checks on referenced resources");
        if (!checkSuiteExists)
          checkSuite = this.AddCheckSuite(requestContext, projectId, id, new List<CheckRun>(), resources, evaluationContext);
        return checkSuite;
      }
      if (requestContext.IsFeatureEnabled("Pipelines.Checks.LogDetailsAboutConfiguredChecks"))
      {
        requestContext.TraceAlways(34001912, nameof (CheckSuiteService), "CheckSuiteService:EvaluateChecksOnResources Total {0} checks configured on referenced resources, starting evaluation. {1}", (object) checkConfigurationList.Count, (object) checkConfigurationList.Select<CheckConfiguration, string>((Func<CheckConfiguration, string>) (cc => cc.ToStringEx())).Serialize<IEnumerable<string>>());
        List<CheckRun> checkRuns = checkSuite.CheckRuns;
        // ISSUE: explicit non-virtual call
        if ((checkRuns != null ? (__nonvirtual (checkRuns.Count) > 0 ? 1 : 0) : 0) != 0)
          requestContext.TraceAlways(34001934, nameof (CheckSuiteService), "CheckSuiteService:EvaluateChecksOnResources Check runs details: {0}", (object) checkSuite.CheckRuns.Select<CheckRun, string>((Func<CheckRun, string>) (checkRun => checkRun.ToStringWithCheckConfigurationExtended())).Serialize<IEnumerable<string>>());
      }
      else
        requestContext.TraceAlways(34001912, nameof (CheckSuiteService), "CheckSuiteService:EvaluateChecksOnResources Total {0} checks configured on referenced resources, starting evaluation", (object) checkConfigurationList.Count);
      Dictionary<CheckEvaluationOrder, Dictionary<ICheckType, Dictionary<Guid, CheckConfiguration>>> dictionary = new Dictionary<CheckEvaluationOrder, Dictionary<ICheckType, Dictionary<Guid, CheckConfiguration>>>();
      Dictionary<int, int> checkConfigIdToTimeoutMap = new Dictionary<int, int>();
      Dictionary<int, CheckEvaluationOrder> checkConfigIdToOrder = new Dictionary<int, CheckEvaluationOrder>();
      this.PopulateMapsForCheckConfigurations(requestContext, (IList<CheckConfiguration>) checkConfigurationList, dictionary, checkConfigIdToTimeoutMap, checkConfigIdToOrder, checkSuite.CheckRuns, isCheckOrderingEnabled);
      Dictionary<Guid, CheckEvaluationOrder> checkOrderCache = new Dictionary<Guid, CheckEvaluationOrder>();
      foreach (CheckEvaluationOrder checkEvaluationOrder1 in Enum.GetValues(typeof (CheckEvaluationOrder)))
      {
        CheckEvaluationOrder ordering = checkEvaluationOrder1;
        IEnumerable<CheckRun> source = checkSuite.CheckRuns.Where<CheckRun>(closure_0 ?? (closure_0 = (Func<CheckRun, bool>) (checkRun => !isCheckOrderingEnabled || checkRun.Status != CheckRunStatus.Queued))).Where<CheckRun>((Func<CheckRun, bool>) (checkRun =>
        {
          CheckEvaluationOrder? evaluationOrder = this.GetEvaluationOrder(requestContext, checkRun.CheckConfigurationRef, checkOrderCache, isCheckOrderingEnabled, checkConfigIdToOrder);
          CheckEvaluationOrder checkEvaluationOrder2 = ordering;
          return evaluationOrder.GetValueOrDefault() == checkEvaluationOrder2 & evaluationOrder.HasValue;
        }));
        int num1 = source.Count<CheckRun>();
        int num2 = source.Where<CheckRun>((Func<CheckRun, bool>) (x => x.Status == CheckRunStatus.Approved || x.Status == CheckRunStatus.Bypassed)).Count<CheckRun>();
        int num3 = checkConfigurationList.Where<CheckConfiguration>((Func<CheckConfiguration, bool>) (config =>
        {
          CheckEvaluationOrder? evaluationOrder = this.GetEvaluationOrder(requestContext, (CheckConfigurationRef) config, checkOrderCache, isCheckOrderingEnabled, checkConfigIdToOrder);
          CheckEvaluationOrder checkEvaluationOrder3 = ordering;
          return evaluationOrder.GetValueOrDefault() == checkEvaluationOrder3 & evaluationOrder.HasValue;
        })).Count<CheckConfiguration>();
        if (num1 > 0 && requestContext.IsFeatureEnabled("Pipelines.Checks.LogDetailsAboutConfiguredChecks"))
          requestContext.TraceAlways(34001934, nameof (CheckSuiteService), "CheckSuiteService:EvaluateChecksOnResources CheckEvaluationOrder = {0}: checksStartedOrCompleted = {1}, checkRunsApproved = {2}, totalChecks = {3}.", (object) ordering.ToString(), (object) num1, (object) num2, (object) num3);
        if (num1 != 0 && num2 < num1)
          return checkSuite;
        if (num2 >= num1 && (num3 == 0 || num1 > 0))
        {
          if (num2 > num1)
            requestContext.TraceError(34001932, nameof (CheckSuiteService), "More check runs {0} exist than checks configured {1} for CheckSuite {2}", (object) num2, (object) num3, (object) id);
          dictionary.Remove(ordering);
        }
      }
      if (!dictionary.SelectMany<KeyValuePair<CheckEvaluationOrder, Dictionary<ICheckType, Dictionary<Guid, CheckConfiguration>>>, KeyValuePair<ICheckType, Dictionary<Guid, CheckConfiguration>>>((Func<KeyValuePair<CheckEvaluationOrder, Dictionary<ICheckType, Dictionary<Guid, CheckConfiguration>>>, IEnumerable<KeyValuePair<ICheckType, Dictionary<Guid, CheckConfiguration>>>>) (x => (IEnumerable<KeyValuePair<ICheckType, Dictionary<Guid, CheckConfiguration>>>) x.Value)).Any<KeyValuePair<ICheckType, Dictionary<Guid, CheckConfiguration>>>())
        return checkSuite;
      List<Resource> forConfigurations = Utilities.GetPermissibleResourcesForConfigurations(requestContext, projectId, new List<CheckConfigurationRef>((IEnumerable<CheckConfigurationRef>) checkConfigurationList));
      this.PopulateResourceDataInCheckConfigurations(dictionary, forConfigurations);
      if (!requestContext.IsFeatureEnabled("Pipelines.Checks.AuthChecksAsyncFlow"))
        dictionary.Remove(CheckEvaluationOrder.System);
      List<CheckRun> checksInOrder = this.EvaluateChecksInOrder(requestContext, projectId, id, resources, evaluationContext, dictionary);
      this.ScheduleTimeoutJobs(requestContext, projectId, id, (IList<CheckRun>) checksInOrder, checkConfigIdToTimeoutMap, utcNow);
      checkSuite = !checkSuiteExists ? this.AddCheckSuite(requestContext, projectId, id, checksInOrder, resources, evaluationContext) : this.AddCheckRuns(requestContext, projectId, checkSuite, (IList<CheckRun>) checksInOrder, CheckSuiteExpandParameter.None, isCheckOrderingEnabled);
      this.PublishCheckSuiteEvaluateEventToTelemetry(requestContext, checkSuite, utcNow);
      requestContext.TraceInfo(34001913, nameof (CheckSuiteService), "CheckSuiteService::EvaluateChecksOnResources Evaluation response for check suite id: {0}, status: {1}", (object) id, (object) checkSuite.Status);
      if (checkSuite.Status == CheckRunStatus.Running)
        this.ScheduleBatchExpiry(requestContext, projectId, id, resources, evaluationContext, checkSuite.CheckRuns, dictionary.GetValueOrDefault<CheckEvaluationOrder, Dictionary<ICheckType, Dictionary<Guid, CheckConfiguration>>>(CheckEvaluationOrder.Main, (Dictionary<ICheckType, Dictionary<Guid, CheckConfiguration>>) null));
      return checkSuite;
    }

    private List<CheckRun> EvaluateChecksInOrder(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid checkSuiteId,
      List<Resource> resources,
      JObject evaluationContext,
      Dictionary<CheckEvaluationOrder, Dictionary<ICheckType, Dictionary<Guid, CheckConfiguration>>> orderedCheckTypeToEvaluationRequestMap)
    {
      List<CheckRun> checkRuns = new List<CheckRun>();
      bool flag1 = requestContext.IsFeatureEnabled("Pipelines.Checks.EnableOrderingChecks");
      bool flag2 = true;
      bool evaluationFailed = false;
      foreach (CheckEvaluationOrder checkEvaluationOrder in Enum.GetValues(typeof (CheckEvaluationOrder)))
      {
        CheckEvaluationOrder ordering = checkEvaluationOrder;
        List<CheckRun> collection = new List<CheckRun>();
        Dictionary<ICheckType, Dictionary<Guid, CheckConfiguration>> dictionary;
        string message;
        if (orderedCheckTypeToEvaluationRequestMap.TryGetValue(ordering, out dictionary) && dictionary.Any<KeyValuePair<ICheckType, Dictionary<Guid, CheckConfiguration>>>())
        {
          if (flag1)
          {
            if (flag2)
            {
              collection.AddRange((IEnumerable<CheckRun>) CheckSuiteService.EvaluateChecks(requestContext, projectId, checkSuiteId, evaluationContext, resources, dictionary, out evaluationFailed));
            }
            else
            {
              foreach (KeyValuePair<ICheckType, Dictionary<Guid, CheckConfiguration>> keyValuePair in dictionary)
              {
                ICheckType key = keyValuePair.Key;
                collection.AddRange((IEnumerable<CheckRun>) CheckSuiteService.CreateNotStartedCheckRuns(checkSuiteId, (IReadOnlyDictionary<Guid, CheckConfiguration>) dictionary[key], key, evaluationFailed));
              }
            }
            collection.ForEach((Action<CheckRun>) (c => c.EvaluationOrder = ordering));
            checkRuns.AddRange((IEnumerable<CheckRun>) collection);
            if (flag2 && CheckSuiteResult.GetCheckSuiteStatus(checkRuns, out message) != CheckRunStatus.Approved)
              flag2 = false;
          }
          else
          {
            checkRuns.AddRange((IEnumerable<CheckRun>) CheckSuiteService.EvaluateChecks(requestContext, projectId, checkSuiteId, evaluationContext, resources, dictionary, out evaluationFailed));
            if (CheckSuiteResult.GetCheckSuiteStatus(checkRuns, out message) != CheckRunStatus.Approved)
              break;
          }
        }
      }
      return checkRuns;
    }

    private void PopulateResourceDataInCheckConfigurations(
      Dictionary<CheckEvaluationOrder, Dictionary<ICheckType, Dictionary<Guid, CheckConfiguration>>> orderedCheckTypeToEvaluationRequestMap,
      List<Resource> referencedPermissibleResources)
    {
      if (!orderedCheckTypeToEvaluationRequestMap.Any<KeyValuePair<CheckEvaluationOrder, Dictionary<ICheckType, Dictionary<Guid, CheckConfiguration>>>>())
        return;
      foreach (Dictionary<ICheckType, Dictionary<Guid, CheckConfiguration>> dictionary in orderedCheckTypeToEvaluationRequestMap.Values)
      {
        foreach (KeyValuePair<ICheckType, Dictionary<Guid, CheckConfiguration>> keyValuePair in dictionary)
          keyValuePair.Value.ForEach<KeyValuePair<Guid, CheckConfiguration>>((Action<KeyValuePair<Guid, CheckConfiguration>>) (c => this.PopulateResourceData((CheckConfigurationRef) c.Value, referencedPermissibleResources)));
      }
    }

    private static List<CheckRun> GetCancellationCheckRuns(
      Guid checkSuiteId,
      IReadOnlyDictionary<Guid, CheckConfiguration> requests,
      ICheckType checkType)
    {
      List<CheckRun> cancellationCheckRuns = new List<CheckRun>();
      foreach (KeyValuePair<Guid, CheckConfiguration> request in (IEnumerable<KeyValuePair<Guid, CheckConfiguration>>) requests)
      {
        int id = request.Value.Id;
        int version = request.Value.Version;
        CheckRun checkRun1 = new CheckRun();
        checkRun1.CheckSuiteRef = new CheckSuiteRef()
        {
          Id = checkSuiteId
        };
        checkRun1.Status = CheckRunStatus.Canceled;
        checkRun1.Id = request.Key;
        checkRun1.ResultMessage = PipelineChecksResources.CheckAbortedMessage();
        checkRun1.CheckConfigurationRef = new CheckConfigurationRef()
        {
          Id = id,
          Version = version,
          Type = {
            Name = checkType.CheckTypeName,
            Id = checkType.CheckTypeId
          }
        };
        CheckRun checkRun2 = checkRun1;
        cancellationCheckRuns.Add(checkRun2);
      }
      return cancellationCheckRuns;
    }

    private static List<CheckRun> GetCheckRuns(
      Guid checkSuiteId,
      IReadOnlyDictionary<Guid, CheckRunResult> checkRunResults,
      IReadOnlyDictionary<Guid, CheckConfiguration> requestIdToAssignmentRecordMap,
      ICheckType checkType)
    {
      List<CheckRun> checkRuns = new List<CheckRun>();
      foreach (KeyValuePair<Guid, CheckRunResult> checkRunResult1 in (IEnumerable<KeyValuePair<Guid, CheckRunResult>>) checkRunResults)
      {
        Guid key = checkRunResult1.Key;
        int num1 = 0;
        int num2 = 1;
        Resource resource = (Resource) null;
        CheckConfiguration checkConfiguration;
        if (requestIdToAssignmentRecordMap.TryGetValue(key, out checkConfiguration))
        {
          num1 = checkConfiguration.Id;
          num2 = checkConfiguration.Version;
          resource = checkConfiguration.Resource;
        }
        CheckRunResult checkRunResult2 = checkRunResult1.Value;
        if (num1 > 0 && key != Guid.Empty && checkRunResult2 != null)
        {
          CheckRun checkRun1 = new CheckRun();
          checkRun1.CheckSuiteRef = new CheckSuiteRef()
          {
            Id = checkSuiteId
          };
          checkRun1.Status = checkRunResult2.Status;
          checkRun1.Id = key;
          checkRun1.ResultMessage = checkRunResult2.ResultMessage;
          checkRun1.CheckConfigurationRef = new CheckConfigurationRef()
          {
            Id = num1,
            Version = num2,
            Type = {
              Name = checkType.CheckTypeName,
              Id = checkType.CheckTypeId
            },
            Resource = resource
          };
          checkRun1.ValidTill = checkRunResult1.Value.ValidTill;
          CheckRun checkRun2 = checkRun1;
          checkRuns.Add(checkRun2);
        }
      }
      return checkRuns;
    }

    private static List<CheckRun> CreateNotStartedCheckRuns(
      Guid checkSuiteId,
      IReadOnlyDictionary<Guid, CheckConfiguration> requestIdToAssignmentRecordMap,
      ICheckType checkType,
      bool createAsCanceled)
    {
      List<CheckRun> startedCheckRuns = new List<CheckRun>();
      foreach (KeyValuePair<Guid, CheckConfiguration> assignmentRecord in (IEnumerable<KeyValuePair<Guid, CheckConfiguration>>) requestIdToAssignmentRecordMap)
      {
        Guid key = assignmentRecord.Key;
        int id = assignmentRecord.Value.Id;
        int version = assignmentRecord.Value.Version;
        Resource resource = assignmentRecord.Value.Resource;
        CheckConfiguration checkConfiguration = assignmentRecord.Value;
        if (id > 0 && key != Guid.Empty && checkConfiguration != null)
        {
          CheckRun checkRun1 = new CheckRun();
          checkRun1.CheckSuiteRef = new CheckSuiteRef()
          {
            Id = checkSuiteId
          };
          checkRun1.Status = createAsCanceled ? CheckRunStatus.Canceled : CheckRunStatus.Queued;
          checkRun1.Id = key;
          checkRun1.CheckConfigurationRef = new CheckConfigurationRef()
          {
            Id = id,
            Version = version,
            Type = {
              Name = checkType.CheckTypeName,
              Id = checkType.CheckTypeId
            },
            Resource = resource
          };
          CheckRun checkRun2 = checkRun1;
          startedCheckRuns.Add(checkRun2);
        }
      }
      return startedCheckRuns;
    }

    private void SendCheckSuiteUpdatedNotificationEvent(
      IVssRequestContext requestContext,
      Guid projectId,
      CheckSuite response)
    {
      requestContext.GetService<IChecksEventPublisherService>().NotifyCheckSuiteUpdatedEvent(requestContext, "MS.Azure.Pipelines.CheckSuiteUpdated", projectId, response);
    }

    private static void CancelPluginCheckRuns(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid checkSuiteId,
      List<CheckRun> checkRuns)
    {
      IDictionary<CheckType, List<CheckRun>> checkRunTypeMap = CheckSuiteService.GetCheckRunTypeMap((IList<CheckRun>) checkRuns, true);
      IChecksExtensionService service = requestContext.GetService<IChecksExtensionService>();
      foreach (KeyValuePair<CheckType, List<CheckRun>> keyValuePair in (IEnumerable<KeyValuePair<CheckType, List<CheckRun>>>) checkRunTypeMap)
      {
        CheckType key = keyValuePair.Key;
        ICheckType checkInstance = service.GetCheckInstance(key);
        HashSet<Resource> resourceSet = new HashSet<Resource>();
        if (checkInstance != null)
        {
          try
          {
            checkInstance.Cancel(requestContext, projectId, checkSuiteId, (IList<CheckRun>) keyValuePair.Value);
            requestContext.TraceInfo(34001913, nameof (CheckSuiteService), "CheckSuiteService::CancelPluginCheckRuns Cancelling plugin check runs with requestIds: {0}", (object) string.Join(", ", keyValuePair.Value.Select<CheckRun, string>((Func<CheckRun, string>) (c => c.Id.ToString()))));
          }
          catch (AccessDeniedException ex)
          {
            requestContext.TraceError(34001905, nameof (CheckSuiteService), "CheckSuiteService::CancelPluginCheckRuns Cancellation is unauthorized, reason: {0}", (object) ex.Message);
          }
          catch (Exception ex)
          {
            requestContext.TraceException(34001914, nameof (CheckSuiteService), ex);
          }
        }
      }
    }

    public void ServiceStart(IVssRequestContext requestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext requestContext)
    {
    }

    public void DeleteCheckSuites(
      IVssRequestContext requestContext,
      Guid projectId,
      IList<Guid> checkSuiteIds)
    {
      CheckSuiteService.ValidateBasicData(requestContext, projectId);
      using (new MethodScope(requestContext, nameof (CheckSuiteService), nameof (DeleteCheckSuites)))
      {
        ArgumentUtility.CheckForNull<IList<Guid>>(checkSuiteIds, nameof (checkSuiteIds), "Pipeline.Checks");
        checkSuiteIds = (IList<Guid>) checkSuiteIds.Distinct<Guid>().ToList<Guid>();
        checkSuiteIds.ForEach<Guid>((Action<Guid>) (checkSuiteId => ArgumentUtility.CheckForEmptyGuid(checkSuiteId, nameof (checkSuiteId), "Pipeline.Checks")));
        requestContext.TraceInfo(34001913, nameof (CheckSuiteService), "CheckSuiteService::DeleteCheckSuites Deleting {0} check suites", (object) checkSuiteIds.Count<Guid>());
        IList<CheckSuite> collection;
        using (CheckSuiteComponent component = requestContext.CreateComponent<CheckSuiteComponent>())
          collection = component.DeleteCheckSuites(projectId, checkSuiteIds);
        collection.ForEach<CheckSuite>((Action<CheckSuite>) (checkSuiteResponse => CheckSuiteService.DeletePluginCheckRuns(requestContext, projectId, checkSuiteResponse.Id, (IList<CheckRun>) checkSuiteResponse.CheckRuns)));
      }
    }

    public void ReEvaluate(
      IVssRequestContext requestContext,
      Guid projectId,
      JObject evaluationContext,
      CheckConfiguration checkConfiguration,
      List<Resource> resources,
      Guid checkSuiteId,
      Guid checkRunId)
    {
      ICheckType checkInstance = requestContext.GetService<IChecksExtensionService>().GetCheckInstance(checkConfiguration.Type);
      Dictionary<Guid, CheckConfiguration> dictionary = new Dictionary<Guid, CheckConfiguration>()
      {
        {
          checkRunId,
          checkConfiguration
        }
      };
      IVssRequestContext requestContext1 = requestContext;
      Guid projectId1 = projectId;
      JObject evaluationContext1 = evaluationContext;
      Dictionary<Guid, CheckConfiguration> requests = dictionary;
      List<Resource> resources1 = resources;
      IReadOnlyDictionary<Guid, CheckRunResult> readOnlyDictionary = checkInstance.Evaluate(requestContext1, projectId1, evaluationContext1, (IReadOnlyDictionary<Guid, CheckConfiguration>) requests, resources1);
      if (readOnlyDictionary == null)
        return;
      CheckRunResult result = readOnlyDictionary[checkRunId];
      this.UpdateCheckRun(requestContext, projectId, checkRunId, result, CheckSuiteExpandParameter.None);
      DateTimeOffset? validTill = result.ValidTill;
      if (!validTill.HasValue)
        return;
      validTill = result.ValidTill;
      DateTimeOffset dateTimeOffset = validTill.Value;
      if (!(dateTimeOffset.UtcDateTime > DateTime.UtcNow))
        return;
      validTill = result.ValidTill;
      dateTimeOffset = validTill.Value;
      DateTime utcDateTime = dateTimeOffset.UtcDateTime;
      string serializedEvaluationContext = JsonConvert.SerializeObject((object) evaluationContext);
      string serializedCheckConfiguration = JsonConvert.SerializeObject((object) checkConfiguration);
      this.ScheduleExpiry(requestContext, projectId, checkSuiteId, resources, serializedEvaluationContext, serializedCheckConfiguration, checkRunId, utcDateTime);
    }

    private static void DeletePluginCheckRuns(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid checkSuiteId,
      IList<CheckRun> checkRuns)
    {
      IDictionary<CheckType, List<CheckRun>> checkRunTypeMap = CheckSuiteService.GetCheckRunTypeMap(checkRuns);
      IChecksExtensionService service = requestContext.GetService<IChecksExtensionService>();
      foreach (KeyValuePair<CheckType, List<CheckRun>> keyValuePair in (IEnumerable<KeyValuePair<CheckType, List<CheckRun>>>) checkRunTypeMap)
      {
        CheckType key = keyValuePair.Key;
        ICheckType checkInstance = service.GetCheckInstance(key);
        if (checkInstance != null)
        {
          try
          {
            checkInstance.Delete(requestContext, projectId, checkSuiteId, (IList<CheckRun>) keyValuePair.Value);
            requestContext.TraceInfo(34001913, nameof (CheckSuiteService), "CheckSuiteService::DeletePluginCheckRuns Deleting plugin check runs with requestIds: {0}", (object) string.Join(", ", keyValuePair.Value.Select<CheckRun, string>((Func<CheckRun, string>) (c => c.Id.ToString()))));
          }
          catch (AccessDeniedException ex)
          {
            requestContext.TraceError(34001905, nameof (CheckSuiteService), "CheckSuiteService::DeletePluginCheckRuns Deleting is unauthorized, reason: {0}", (object) ex.Message);
          }
          catch (Exception ex)
          {
            requestContext.TraceException(34001928, nameof (CheckSuiteService), ex);
          }
        }
      }
    }

    private static IDictionary<CheckType, List<CheckRun>> GetCheckRunTypeMap(
      IList<CheckRun> checkRuns,
      bool isCanceled = false)
    {
      Dictionary<CheckType, List<CheckRun>> checkRunTypeMap = new Dictionary<CheckType, List<CheckRun>>();
      foreach (CheckRun checkRun in (IEnumerable<CheckRun>) checkRuns)
      {
        CheckType type = checkRun.CheckConfigurationRef.Type;
        if (!isCanceled || checkRun.Status == CheckRunStatus.Canceled)
        {
          List<CheckRun> checkRunList;
          if (checkRunTypeMap.TryGetValue(type, out checkRunList))
          {
            checkRunList.Add(checkRun);
          }
          else
          {
            checkRunList = new List<CheckRun>();
            checkRunList.Add(checkRun);
            checkRunTypeMap[type] = checkRunList;
          }
        }
      }
      return (IDictionary<CheckType, List<CheckRun>>) checkRunTypeMap;
    }

    private void ScheduleBatchExpiry(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid checkSuiteId,
      List<Resource> resources,
      JObject evaluationContext,
      List<CheckRun> checkRuns,
      Dictionary<ICheckType, Dictionary<Guid, CheckConfiguration>> checkTypeToEvaluationRequestsMap)
    {
      string serializedEvaluationContext = JsonConvert.SerializeObject((object) evaluationContext);
      IChecksExtensionService service = requestContext.GetService<IChecksExtensionService>();
      foreach (CheckRun checkRun in checkRuns.Where<CheckRun>((Func<CheckRun, bool>) (checkRun => checkRun.ValidTill.HasValue && checkRun.ValidTill.Value > DateTimeOffset.UtcNow)))
      {
        ICheckType checkInstance = service.GetCheckInstance(checkRun.CheckConfigurationRef.Type);
        string serializedCheckConfiguration = JsonConvert.SerializeObject((object) checkTypeToEvaluationRequestsMap[checkInstance][checkRun.Id]);
        DateTime utcDateTime = checkRun.ValidTill.Value.UtcDateTime;
        this.ScheduleExpiry(requestContext, projectId, checkSuiteId, resources, serializedEvaluationContext, serializedCheckConfiguration, checkRun.Id, utcDateTime);
      }
    }

    private void ScheduleExpiry(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid checkSuiteId,
      List<Resource> resources,
      string serializedEvaluationContext,
      string serializedCheckConfiguration,
      Guid checkRunId,
      DateTime validTill)
    {
      XmlNode xml = TeamFoundationSerializationUtility.SerializeToXml((object) CheckExpirationJobData.GetJobData(projectId, resources, serializedEvaluationContext, checkRunId, serializedCheckConfiguration));
      JobHelper.ScheduleJob(requestContext, Guid.NewGuid(), JobConstants.ExpirationJobName, JobConstants.CheckReEvaluationJobExtension, xml, validTill);
    }

    private CheckEvaluationOrder? GetEvaluationOrder(
      IVssRequestContext requestContext,
      CheckConfigurationRef checkConfiguration,
      Dictionary<Guid, CheckEvaluationOrder> checkTypeToOrderCache,
      bool checkOrderingEnabled,
      Dictionary<int, CheckEvaluationOrder> configIdToOrdering)
    {
      if (checkOrderingEnabled)
        return new CheckEvaluationOrder?(configIdToOrdering.GetValueOrDefault<int, CheckEvaluationOrder>(checkConfiguration.Id, CheckEvaluationOrder.System));
      if (checkTypeToOrderCache.ContainsKey(checkConfiguration.Type.Id))
        return new CheckEvaluationOrder?(checkTypeToOrderCache[checkConfiguration.Type.Id]);
      ICheckType checkInstance = requestContext.GetService<IChecksExtensionService>().GetCheckInstance(checkConfiguration.Type);
      if (checkInstance == null)
        return new CheckEvaluationOrder?();
      CheckEvaluationOrder checkEvaluationOrder = checkInstance.EvaluationOrder();
      checkTypeToOrderCache.Add(checkConfiguration.Type.Id, checkEvaluationOrder);
      return new CheckEvaluationOrder?(checkEvaluationOrder);
    }

    private void PopulateMissingDetails(
      IVssRequestContext requestContext,
      Guid projectId,
      CheckSuite checkSuiteResponse,
      List<Resource> resources)
    {
      if (checkSuiteResponse == null)
        return;
      List<CheckRun> checkRuns = checkSuiteResponse.CheckRuns;
      if (checkRuns != null)
      {
        foreach (CheckRun checkRun in checkRuns)
          this.PopulateCheckConfigurationRef(requestContext, projectId, checkRun.CheckConfigurationRef, resources);
      }
      ReferenceLinks referenceLinks = new ReferenceLinks();
      string checkSuiteUrl = CheckSuiteService.GetCheckSuiteUrl(requestContext, projectId, checkSuiteResponse.Id);
      referenceLinks.AddLink("self", checkSuiteUrl);
      checkSuiteResponse.Links = referenceLinks;
    }

    private static string GetCheckSuiteUrl(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid checkSuiteId)
    {
      if (requestContext == null)
        return (string) null;
      object routeValues = (object) new
      {
        batchEvaluationId = checkSuiteId
      };
      Guid checksRunsLocationId = Microsoft.Azure.Pipelines.Checks.WebApi.Constants.ChecksRunsLocationId;
      return requestContext.GetService<ILocationService>().GetResourceUri(requestContext, "PipelinesChecks", checksRunsLocationId, projectId, routeValues).AbsoluteUri;
    }

    private void PopulateCheckConfigurationRef(
      IVssRequestContext requestContext,
      Guid projectId,
      CheckConfigurationRef checkConfigurationRef,
      List<Resource> resources)
    {
      ICheckType checkInstance = requestContext.GetService<IChecksExtensionService>().GetCheckInstance(checkConfigurationRef.Type);
      checkConfigurationRef.Type.Name = checkInstance.CheckTypeName;
      string configurationUrl = checkConfigurationRef.GetCheckConfigurationUrl(requestContext, projectId, checkConfigurationRef.Id);
      checkConfigurationRef.Url = configurationUrl;
      this.PopulateResourceData(checkConfigurationRef, resources);
    }

    private void PopulateResourceData(
      CheckConfigurationRef checkConfigurationRef,
      List<Resource> resources)
    {
      if (resources == null || checkConfigurationRef.Resource == null || !string.IsNullOrWhiteSpace(checkConfigurationRef.Resource.Name) && !string.IsNullOrWhiteSpace(checkConfigurationRef.Resource.Id))
        return;
      Resource resource = resources.FirstOrDefault<Resource>((Func<Resource, bool>) (r => r.Equals(checkConfigurationRef.Resource)));
      if (resource == null)
        return;
      checkConfigurationRef.Resource = resource;
    }

    private CheckRun GetCheckRun(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid checkSuiteId,
      Guid checkRunId)
    {
      try
      {
        return this.GetCheckSuite(requestContext, projectId, checkSuiteId, CheckSuiteExpandParameter.None).CheckRuns.Where<CheckRun>((Func<CheckRun, bool>) (currentCheckRun => currentCheckRun.Id == checkRunId)).FirstOrDefault<CheckRun>();
      }
      catch (Exception ex)
      {
        requestContext.TraceError(34001925, nameof (CheckSuiteService), "CheckSuiteService::GetCheckRun Exception received while fetching check run: {0}", (object) ex);
        throw;
      }
    }

    private void CancelCheckRunPendingTimeoutJobs(
      IVssRequestContext requestContext,
      IList<CheckRun> checkRuns)
    {
      try
      {
        IList<Guid> list = (IList<Guid>) checkRuns.Where<CheckRun>((Func<CheckRun, bool>) (checkRun => checkRun.TimeoutJobId.HasValue)).Select<CheckRun, Guid>((Func<CheckRun, Guid>) (checkRun => checkRun.TimeoutJobId.Value)).ToList<Guid>();
        JobHelper.CancelJobs(requestContext, (IEnumerable<Guid>) list);
      }
      catch (Exception ex)
      {
        requestContext.TraceError(34001926, nameof (CheckSuiteService), "CheckSuiteService::CancelCheckRunPendingTimeoutJobs Exception received while cancelling job: {0}", (object) ex);
        throw;
      }
    }

    private static void ValidateBasicData(IVssRequestContext requestContext, Guid projectId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext), "Pipeline.Checks");
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId), "Pipeline.Checks");
    }

    private static void ValidateCheckSuiteUpdateData(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid checkSuiteId,
      Guid checkRunId)
    {
      CheckSuiteService.ValidateBasicData(requestContext, projectId);
      ArgumentUtility.CheckForEmptyGuid(checkSuiteId, nameof (checkSuiteId), "Pipeline.Checks");
      ArgumentUtility.CheckForEmptyGuid(checkRunId, nameof (checkRunId), "Pipeline.Checks");
    }

    private static void ValidateCheckRunData(CheckRun record)
    {
      ArgumentUtility.CheckForNull<CheckRun>(record, nameof (record), "Pipeline.Checks");
      ArgumentUtility.CheckForNull<CheckConfigurationRef>(record.CheckConfigurationRef, "CheckConfigurationRef", "Pipeline.Checks");
      ArgumentUtility.CheckForNonPositiveInt(record.CheckConfigurationRef.Id, "Id", "Pipeline.Checks");
      ArgumentUtility.CheckForNonPositiveInt(record.CheckConfigurationRef.Version, "Version", "Pipeline.Checks");
      ArgumentUtility.CheckForDefinedEnum<CheckRunStatus>(record.Status, "Status", "Pipeline.Checks");
      ArgumentUtility.CheckForEmptyGuid(record.Id, "Id", "Pipeline.Checks");
    }

    private static void ValidateCheckRunResultUpdateParametersData(
      Guid requestId,
      CheckRunResult result)
    {
      ArgumentUtility.CheckForNull<CheckRunResult>(result, nameof (result), "Pipeline.Checks");
      ArgumentUtility.CheckForDefinedEnum<CheckRunStatus>(result.Status, "Status", "Pipeline.Checks");
      ArgumentUtility.CheckForEmptyGuid(requestId, nameof (requestId), "Pipeline.Checks");
    }
  }
}
