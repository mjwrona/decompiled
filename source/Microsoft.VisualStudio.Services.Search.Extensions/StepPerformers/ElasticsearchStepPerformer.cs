// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Extensions.StepPerformers.ElasticsearchStepPerformer
// Assembly: Microsoft.VisualStudio.Services.Search.Extensions, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1D8FF195-304B-4BBA-9D1C-F4A6093CE2E1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Extensions.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Server.Jobs;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Implementations;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Xml.Serialization;

namespace Microsoft.VisualStudio.Services.Search.Extensions.StepPerformers
{
  [StepPerformer("Elasticsearch")]
  public class ElasticsearchStepPerformer : TeamFoundationStepPerformerBase
  {
    [StaticSafe]
    private static Mutex s_mutex;

    [ServicingStep]
    public void SetIndexSettings(
      IVssRequestContext requestContext,
      ServicingContext servicingContext,
      ElasticsearchStepPerformer.SetIndexSettingsStepData stepData)
    {
      requestContext.CheckDeploymentRequestContext();
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.SetLogicalContext((IDiagnosticContext) new TfsDiagnosticContext(requestContext));
      try
      {
        ElasticsearchIndexSettingsUpdaterJob.JobData jobData = new ElasticsearchIndexSettingsUpdaterJob.JobData();
        foreach (ElasticsearchStepPerformer.SetIndexSettingData indexSetting in stepData.IndexSettings)
          jobData.IndexSettings.Add(new ElasticsearchIndexSettingsUpdaterJob.IndexSettingData()
          {
            Index = indexSetting.Index,
            Setting = indexSetting.Setting,
            Value = indexSetting.Value
          });
        ITeamFoundationJobService service = requestContext.GetService<ITeamFoundationJobService>();
        Guid jobId = service.QueueOneTimeJob(requestContext, "Update Elasticsearch Index Settings", FormattableString.Invariant(FormattableStringFactory.Create("{0}", (object) typeof (ElasticsearchIndexSettingsUpdaterJob).FullName)), jobData.Serialize(), JobPriorityLevel.Highest, TimeSpan.Zero);
        servicingContext.LogInfo(FormattableString.Invariant(FormattableStringFactory.Create("Queued job [{0}] for updating Elasticsearch index settings.", (object) jobId)));
        try
        {
          DateTime utcNow = DateTime.UtcNow;
          while (DateTime.UtcNow - utcNow < TimeSpan.FromMinutes(10.0))
          {
            TeamFoundationJobHistoryEntry foundationJobHistoryEntry = service.QueryLatestJobHistory(requestContext, jobId);
            if (foundationJobHistoryEntry != null)
            {
              if (foundationJobHistoryEntry.Result == TeamFoundationJobResult.Succeeded)
              {
                servicingContext.LogInfo(FormattableString.Invariant(FormattableStringFactory.Create("Job [{0}] completed successfully.", (object) jobId)));
                return;
              }
              servicingContext.Error(FormattableString.Invariant(FormattableStringFactory.Create("Job [{0}] completed with result [{1}] instead of [{2}].", (object) jobId, (object) foundationJobHistoryEntry.Result.ToString(), (object) "Succeeded")));
              break;
            }
            servicingContext.LogInfo(FormattableString.Invariant(FormattableStringFactory.Create("Did not find entry of job [{0}] in dbo.tbl_JobHistory. Trying again...", (object) jobId)));
            Thread.Sleep(TimeSpan.FromMinutes(1.0));
          }
        }
        finally
        {
          try
          {
            service.DeleteJobDefinitions(requestContext, (IEnumerable<Guid>) new Guid[1]
            {
              jobId
            });
            servicingContext.LogInfo(FormattableString.Invariant(FormattableStringFactory.Create("Deleted definition of job [{0}] successfully.", (object) jobId)));
          }
          catch (Exception ex)
          {
            servicingContext.Log(ServicingStepLogEntryKind.Warning, FormattableString.Invariant(FormattableStringFactory.Create("Failed to delete definition of job [{0}] with exception [{1}]. Not failing the servicing step though.", (object) jobId, (object) ex)));
          }
        }
        servicingContext.Log(ServicingStepLogEntryKind.Error, FormattableString.Invariant(FormattableStringFactory.Create("Failed to update Elasticsearch settings. Check logs of job [{0}] to debug further.", (object) jobId)));
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.ClearLogicalContext();
      }
    }

    [ServicingStep]
    public void InstallElasticsearchService(
      IVssRequestContext requestContext,
      ServicingContext servicingContext,
      ElasticsearchStepPerformer.StartElasticsearchStepData stepData)
    {
      if (stepData.DeploymentEnvironment != DeploymentEnvironment.DevFabric)
        servicingContext.LogInfo("Skipping the Elasticsearch installation as this is not a devfabric environment.");
      else if (string.IsNullOrWhiteSpace(stepData.ServiceName))
      {
        servicingContext.Warn("Could not determine the service name. Skipping this servicing step.");
      }
      else
      {
        ElasticsearchStepPerformer.s_mutex = new Mutex(false, "Global\\" + stepData.ServiceName);
        if (stepData.InstallElasticsearch)
        {
          ElasticsearchConfigurationData configData = new ElasticsearchConfigurationData()
          {
            ConfigurationScriptPath = Path.Combine("C:\\", "LR", stepData.ServiceName, "Services", stepData.ServiceName, "Modules\\ElasticSearch\\DeploymentV2\\zip\\Configure-TFSSearch.ps1"),
            Operation = OperationType.Install,
            ElasticsearchInstallPath = stepData.ElasticsearchInstallPath,
            ElasticsearchIndexPath = stepData.ElasticsearchIndexPath,
            RemoveElasticsearchData = stepData.RemoveElasticsearchData,
            ElasticsearchServiceName = stepData.ElasticsearchServiceName,
            ElasticsearchPort = stepData.ElasticsearchPort,
            ClusterName = stepData.ClusterName,
            ElasticsearchUser = stepData.ElasticsearchUser,
            ElasticsearchPassword = stepData.ElasticsearchPassword
          };
          string modulePath = Path.Combine("C:\\", "LR", stepData.ServiceName, "Services", stepData.ServiceName, "Modules\\ElasticSearch\\DeploymentV2\\zip");
          string logFilePath = Path.Combine("C:\\", "LR", stepData.ServiceName, "Logs\\InstallElasticsearch.log");
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.SetLogicalContext((IDiagnosticContext) new TfsDiagnosticContext(requestContext));
          servicingContext.LogInfo("Waiting for mutex");
          bool flag;
          try
          {
            flag = ElasticsearchStepPerformer.s_mutex.WaitOne(TimeSpan.FromMinutes(10.0));
          }
          catch (AbandonedMutexException ex)
          {
            flag = true;
          }
          if (!flag)
          {
            servicingContext.LogInfo("Failed to acquire mutex. Skipping the Elasticsearch installation.");
          }
          else
          {
            servicingContext.LogInfo("Acquired mutex. Now configuring Elasticsearch...");
            try
            {
              using (InstallElasticsearchServicingStepHelper servicingStepHelper = new InstallElasticsearchServicingStepHelper(servicingContext))
              {
                if (servicingStepHelper.ConfigureAndInstallElasticsearch(configData, modulePath, logFilePath))
                  return;
                servicingContext.Error("Elasticsearch installation failed.");
              }
            }
            finally
            {
              servicingContext.LogInfo("Releasing the mutex...");
              ElasticsearchStepPerformer.s_mutex.ReleaseMutex();
              servicingContext.LogInfo("Released the mutex.");
              Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.ClearLogicalContext();
            }
          }
        }
        else
          servicingContext.LogInfo("Skipping the Elasticsearch installation as InstallElasticsearch is set to false in deployment file.");
      }
    }

    public class StartElasticsearchStepData
    {
      [XmlElement("ClusterName")]
      public string ClusterName { get; set; }

      [XmlElement("DeploymentEnvironment")]
      public DeploymentEnvironment DeploymentEnvironment { get; set; }

      [XmlElement("ElasticsearchServiceName")]
      public string ElasticsearchServiceName { get; set; }

      [XmlElement("ElasticsearchPort")]
      public int ElasticsearchPort { get; set; }

      [XmlElement("ServiceName")]
      public string ServiceName { get; set; }

      [XmlElement("InstallElasticsearch")]
      public bool InstallElasticsearch { get; set; }

      [XmlElement("ElasticsearchInstallPath")]
      public string ElasticsearchInstallPath { get; set; }

      [XmlElement("ElasticsearchIndexPath")]
      public string ElasticsearchIndexPath { get; set; }

      [XmlElement("RemoveElasticsearchData")]
      public bool RemoveElasticsearchData { get; set; }

      [XmlElement("ElasticSearchUser")]
      public string ElasticsearchUser { get; set; }

      [XmlElement("ElasticSearchPassword")]
      public string ElasticsearchPassword { get; set; }
    }

    public class SetIndexSettingsStepData
    {
      [XmlElement("SetIndexSetting")]
      public ElasticsearchStepPerformer.SetIndexSettingData[] IndexSettings { get; set; }
    }

    [DebuggerDisplay("Index: {Index}, Setting: {Setting}, Value: {Value}")]
    public class SetIndexSettingData
    {
      [XmlAttribute("index")]
      public string Index { get; set; }

      [XmlAttribute("setting")]
      public string Setting { get; set; }

      [XmlAttribute("value")]
      public string Value { get; set; }
    }
  }
}
