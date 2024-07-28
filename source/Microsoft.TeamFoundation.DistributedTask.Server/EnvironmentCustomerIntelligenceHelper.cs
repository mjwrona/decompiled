// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.EnvironmentCustomerIntelligenceHelper
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.Azure.Pipelines.Deployment.Sdk.Server.Traceability;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.Constants;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.Models;
using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Runtime;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  public static class EnvironmentCustomerIntelligenceHelper
  {
    private const string c_area = "EnvironmentDeployment";

    public static void PublishDeploymentPhaseStarted(
      IVssRequestContext requestContext,
      ProviderPhaseRequest phaseRequest)
    {
      using (new MethodScope(requestContext, "EnvironmentCustomerIntelligence", nameof (PublishDeploymentPhaseStarted)))
      {
        try
        {
          CustomerIntelligenceService service = requestContext.GetService<CustomerIntelligenceService>();
          if (!service.IsTracingEnabled(requestContext) || phaseRequest == null)
            return;
          CustomerIntelligenceData properties = new CustomerIntelligenceData();
          using (SHA512CryptoServiceProvider cryptoServiceProvider = new SHA512CryptoServiceProvider())
          {
            properties.Add("PlanId", (object) phaseRequest.PlanId);
            properties.Add("PlanType", phaseRequest.PlanType);
            properties.Add("PhaseId", phaseRequest.Phase?.Id);
            if (!string.IsNullOrEmpty(phaseRequest.Phase?.Name))
              properties.Add("PhaseName", HexConverter.ToString(cryptoServiceProvider.ComputeHash(Encoding.ASCII.GetBytes(phaseRequest.Phase.Name))));
            properties.Add("ProjectId", (object) phaseRequest.Project?.Id);
            if (!string.IsNullOrEmpty(phaseRequest.Project?.Name))
              properties.Add("ProjectName", HexConverter.ToString(cryptoServiceProvider.ComputeHash(Encoding.ASCII.GetBytes(phaseRequest.Project.Name))));
            properties.Add("PipelineId", (object) phaseRequest.ServiceOwner);
            properties.Add("StageId", phaseRequest.Stage?.Id);
            CustomerIntelligenceData intelligenceData = properties;
            ProviderPhase providerPhase = phaseRequest.ProviderPhase;
            string str;
            if (providerPhase == null)
            {
              str = (string) null;
            }
            else
            {
              Dictionary<string, JToken> strategy = providerPhase.Strategy;
              str = strategy != null ? strategy.FirstOrDefault<KeyValuePair<string, JToken>>().Key : (string) null;
            }
            intelligenceData.Add("Strategy", str);
            if (!string.IsNullOrEmpty(phaseRequest.Stage?.Name))
              properties.Add("StageName", HexConverter.ToString(cryptoServiceProvider.ComputeHash(Encoding.ASCII.GetBytes(phaseRequest.Stage.Name))));
            string environmentName = phaseRequest.ProviderPhase?.EnvironmentTarget?.EnvironmentName;
            if (!string.IsNullOrEmpty(environmentName))
              properties.Add("EnvironmentName", HexConverter.ToString(cryptoServiceProvider.ComputeHash(Encoding.ASCII.GetBytes(environmentName))));
            properties.Add("EnvironmentId", (object) phaseRequest.ProviderPhase?.EnvironmentTarget?.EnvironmentId);
            properties.Add("ResourceId", (object) phaseRequest.ProviderPhase?.EnvironmentTarget?.Resource?.Id);
            properties.Add("ResourceType", (object) phaseRequest.ProviderPhase?.EnvironmentTarget?.Resource?.Type);
          }
          service.Publish(requestContext, "EnvironmentDeployment", "DeploymentJobStarted", properties);
        }
        catch (Exception ex)
        {
          requestContext.TraceException("EnvironmentCustomerIntelligence", ex);
        }
      }
    }

    public static void PublishJobCompleted(
      IVssRequestContext requestContext,
      string phaseOrchestrationId,
      JobInstance job)
    {
      using (new MethodScope(requestContext, "EnvironmentCustomerIntelligence", nameof (PublishJobCompleted)))
      {
        try
        {
          CustomerIntelligenceService service = requestContext.GetService<CustomerIntelligenceService>();
          if (!service.IsTracingEnabled(requestContext) || phaseOrchestrationId == null)
            return;
          string[] strArray = phaseOrchestrationId.Split('.');
          CustomerIntelligenceData properties = new CustomerIntelligenceData();
          using (SHA512CryptoServiceProvider cryptoServiceProvider = new SHA512CryptoServiceProvider())
          {
            properties.Add("PlanId", strArray[0]);
            properties.Add("PhaseId", strArray[1]);
            properties.Add("JobId", (object) job.Definition?.Id);
            if (!string.IsNullOrEmpty(job.Definition?.Name))
              properties.Add("JobName", HexConverter.ToString(cryptoServiceProvider.ComputeHash(Encoding.ASCII.GetBytes(job.Definition.Name))));
            properties.Add("JobResult", (object) job.Result);
          }
          service.Publish(requestContext, "EnvironmentDeployment", "JobCompleted", properties);
        }
        catch (Exception ex)
        {
          requestContext.TraceException("EnvironmentCustomerIntelligence", ex);
        }
      }
    }

    public static void PublishAddEnvironment(
      IVssRequestContext requestContext,
      EnvironmentInstance environment,
      string source)
    {
      using (new MethodScope(requestContext, "EnvironmentCustomerIntelligence", nameof (PublishAddEnvironment)))
      {
        try
        {
          CustomerIntelligenceService service = requestContext.GetService<CustomerIntelligenceService>();
          if (!service.IsTracingEnabled(requestContext))
            return;
          CustomerIntelligenceData properties = new CustomerIntelligenceData();
          using (SHA512CryptoServiceProvider cryptoServiceProvider = new SHA512CryptoServiceProvider())
          {
            properties.Add("EnvironmentName", HexConverter.ToString(cryptoServiceProvider.ComputeHash(Encoding.ASCII.GetBytes(environment.Name))));
            properties.Add("EnvironmentId", environment.Id.ToString());
            properties.Add("Source", source);
          }
          service.Publish(requestContext, "EnvironmentDeployment", "AddEnvironment", properties);
        }
        catch (Exception ex)
        {
          requestContext.TraceException("EnvironmentCustomerIntelligence", ex);
        }
      }
    }

    public static void PublishJobCommitterDetails(
      IVssRequestContext requestContext,
      Guid scopeId,
      int runId,
      int definitionId,
      string jobRequestId,
      string planType,
      string stageName,
      string jobName,
      string planTemplateType,
      int targetEnvironmentId)
    {
      using (new MethodScope(requestContext, "EnvironmentCustomerIntelligence", nameof (PublishJobCommitterDetails)))
      {
        try
        {
          CustomerIntelligenceService service1 = requestContext.GetService<CustomerIntelligenceService>();
          if (!service1.IsTracingEnabled(requestContext) || string.IsNullOrEmpty(jobRequestId))
            return;
          IArtifactTraceabilityService traceabilityService = requestContext.GetService<IArtifactTraceabilityService>()?.GetArtifactTraceabilityService(ArtifactTraceabilityConstants.ArtifactTraceabilityDeploymentService);
          IList<ArtifactVersion> traceabilityDataForJob = traceabilityService.GetArtifactTraceabilityDataForJob(requestContext, scopeId, runId, jobRequestId, includeSourceDetails: true);
          IDictionary<string, EnvironmentDeploymentExecutionRecord> deploymentByRunIdOrJobs = requestContext.GetService<IEnvironmentDeploymentExecutionHistoryService>().GetLastSuccessfulDeploymentByRunIdOrJobs(requestContext, scopeId, planType, targetEnvironmentId, definitionId, runId, stageName, (IList<string>) new List<string>()
          {
            jobName
          });
          IList<ArtifactVersion> source = (IList<ArtifactVersion>) new List<ArtifactVersion>();
          string key = jobName;
          EnvironmentDeploymentExecutionRecord deploymentExecutionRecord;
          ref EnvironmentDeploymentExecutionRecord local = ref deploymentExecutionRecord;
          if (deploymentByRunIdOrJobs.TryGetValue(key, out local) && deploymentExecutionRecord != null)
            source = traceabilityService.GetArtifactTraceabilityDataForJob(requestContext, scopeId, deploymentExecutionRecord.Owner.Id, deploymentExecutionRecord.RequestIdentifier, includeSourceDetails: true);
          IDictionary<string, ArtifactVersion> dictionary = (IDictionary<string, ArtifactVersion>) source.ToDictionary<ArtifactVersion, string, ArtifactVersion>((Func<ArtifactVersion, string>) (a => a.Alias), (Func<ArtifactVersion, ArtifactVersion>) (a => a));
          ITraceabilityService service2 = requestContext.GetService<ITraceabilityService>();
          Dictionary<string, object> data = new Dictionary<string, object>()
          {
            {
              "JobRequestIdentifier",
              (object) EnvironmentCustomerIntelligenceHelper.ComputeHash(jobRequestId)
            },
            {
              "JobName",
              (object) EnvironmentCustomerIntelligenceHelper.ComputeHash(jobName)
            },
            {
              "StageName",
              (object) EnvironmentCustomerIntelligenceHelper.ComputeHash(stageName)
            },
            {
              "PipelineRunId",
              (object) runId
            },
            {
              "DefinitionId",
              (object) definitionId
            },
            {
              "ProjectId",
              (object) scopeId.ToString()
            },
            {
              "EnvironmentId",
              (object) targetEnvironmentId
            },
            {
              "PlanType",
              (object) planType
            },
            {
              "PlanTemplateType",
              (object) planTemplateType
            }
          };
          foreach (ArtifactVersion currentArtifactVersion in (IEnumerable<ArtifactVersion>) traceabilityDataForJob)
          {
            ArtifactVersion baseArtifactVersion;
            dictionary.TryGetValue(currentArtifactVersion.Alias, out baseArtifactVersion);
            try
            {
              foreach (Microsoft.Azure.Pipelines.Deployment.Sdk.Server.Traceability.Models.Change change in (IEnumerable<Microsoft.Azure.Pipelines.Deployment.Sdk.Server.Traceability.Models.Change>) service2.GetChanges(requestContext, currentArtifactVersion, baseArtifactVersion).Changes)
              {
                CustomerIntelligenceData properties = new CustomerIntelligenceData((IDictionary<string, object>) data);
                string uniqueName = change?.Author?.UniqueName;
                string input = string.IsNullOrWhiteSpace(uniqueName) ? change?.Author?.Id : uniqueName;
                properties.Add("CommitterHash", EnvironmentCustomerIntelligenceHelper.ComputeHash(input));
                properties.Add("CommitType", change?.Type);
                properties.Add("CommitId", change?.Id);
                service1.Publish(requestContext, "EnvironmentDeployment", "JobCommitterDetails", properties);
              }
            }
            catch (Exception ex)
            {
              requestContext.TraceException("EnvironmentCustomerIntelligence", ex);
            }
          }
        }
        catch (Exception ex)
        {
          requestContext.TraceException("EnvironmentCustomerIntelligence", ex);
        }
      }
    }

    private static string ComputeHash(string input)
    {
      if (string.IsNullOrWhiteSpace(input))
        return string.Empty;
      using (SHA512CryptoServiceProvider cryptoServiceProvider = new SHA512CryptoServiceProvider())
        return HexConverter.ToString(cryptoServiceProvider.ComputeHash(Encoding.ASCII.GetBytes(input)));
    }
  }
}
