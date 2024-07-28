// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.AbstractHealthStatusJob
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties;
using Microsoft.VisualStudio.Services.Search.Common.Entities.HealthJobData;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Api;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Actions;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Analyzers;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Data;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManagerAPI;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Implementations;
using Microsoft.VisualStudio.Services.WebPlatform;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs
{
  public abstract class AbstractHealthStatusJob : 
    ISearchServiceJobExtension,
    ITeamFoundationJobExtension
  {
    private const string TraceArea = "Health Manager";
    private const string TraceLayer = "Job";
    private StringBuilder m_resultMessage;
    private AbstractDataProviderFactory m_dataProviderFactory;
    private AbstractAnalyzerFactory m_analyzerFactory;
    private AbstractContextBuilderFactory m_contextBuilderFactory;
    private List<HealthData> m_inputDataToAnalyze;
    private HashSet<DataType> m_dataTypes;
    private HashSet<IAnalyzer> m_analyzers;
    private Dictionary<DataType, ProviderContext> m_contextDataSet;
    private CollectionIndexingProperties m_collectionIndexingProperties;

    [Info("InternalForTestPurpose")]
    internal AbstractHealthStatusJob(IDataAccessFactory dataAccessFactory, Guid healthStatusJobId)
    {
      this.DataAccessFactory = dataAccessFactory;
      this.HealthStatusJobId = healthStatusJobId;
    }

    public TeamFoundationJobExecutionResult Run(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition jobDefinition,
      DateTime queueTime,
      out string resultMessage)
    {
      this.ValidateRequestContext(requestContext);
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.SetLogicalContext((IDiagnosticContext) new TfsDiagnosticContext(requestContext));
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1083050, "Health Manager", "Job", nameof (Run));
      Stopwatch stopwatch = Stopwatch.StartNew();
      ExecutionContext executionContext = requestContext.GetExecutionContext(TracerCICorrelationDetailsFactory.GetCICorrelationDetails(requestContext, jobDefinition.Name, 50));
      bool flag = true;
      int currentAttemptCount = 0;
      string countRegistryPath = this.GetAttemptCountRegistryPath(requestContext);
      JobStatus jobStatus = JobStatus.InProgress;
      HealthStatusRecord healthStatusRecord = (HealthStatusRecord) null;
      TeamFoundationJobExecutionResult jobResult = TeamFoundationJobExecutionResult.Failed;
      try
      {
        currentAttemptCount = requestContext.GetCurrentHostConfigValue<int>(countRegistryPath) + 1;
        this.Initialize();
        List<ActionData> actionData = new List<ActionData>();
        healthStatusRecord = this.GetHealthStatusRecord(requestContext, jobDefinition.Name);
        this.GetCollectionIndexProperties(requestContext, healthStatusRecord);
        this.UpdateHealthStatusRecordWithJobRunStatus(requestContext, healthStatusRecord, jobStatus);
        foreach (Scenario scenario in healthStatusRecord.GetScenariosFromInputRecord())
        {
          this.m_resultMessage.Append(FormattableString.Invariant(FormattableStringFactory.Create("Starting execution for {0} scenario. ", (object) scenario)));
          actionData.AddRange((IEnumerable<ActionData>) this.ExecutePipeline(requestContext, scenario, healthStatusRecord));
          this.m_resultMessage.Append(FormattableString.Invariant(FormattableStringFactory.Create("Completed execution for {0} scenario. ", (object) scenario)));
        }
        this.HandleActions(requestContext, actionData, healthStatusRecord.Mode);
        flag = false;
        jobResult = TeamFoundationJobExecutionResult.Succeeded;
        jobStatus = JobStatus.Succeeded;
      }
      catch (Exception ex)
      {
        jobResult = TeamFoundationJobExecutionResult.Failed;
        requestContext.SetCurrentHostConfigValue<int>(countRegistryPath, currentAttemptCount);
        int statusJobMaxRetries = executionContext.ServiceSettings.JobSettings.AccountHealthStatusJobMaxRetries;
        string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "'{0}' failed with error : {1}. ", (object) this.GetType().Name, (object) ex);
        string message;
        if (currentAttemptCount >= statusJobMaxRetries)
        {
          flag = false;
          message = str + FormattableString.Invariant(FormattableStringFactory.Create("All retries exhausted, Max Attempts possible {0}, Current Attempt Count {1}. Aborting retries now.", (object) statusJobMaxRetries, (object) currentAttemptCount));
          jobStatus = JobStatus.Failed;
          requestContext.DeleteCurrentHostConfigValue(countRegistryPath);
          TeamFoundationEventLog.Default.Log(message, SearchEventId.AccountHealthStatusJobFailed, EventLogEntryType.Error);
        }
        else
        {
          jobStatus = JobStatus.FailedRetry;
          message = str + FormattableString.Invariant(FormattableStringFactory.Create("Max Attempts possible {0}, Current Attempt Count {1}. Requeuing this job.", (object) statusJobMaxRetries, (object) currentAttemptCount));
        }
        this.m_resultMessage.Append(message);
      }
      finally
      {
        resultMessage = this.m_resultMessage.ToString();
        stopwatch.Stop();
        long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
        this.UpdateHealthStatusRecordWithJobRunStatus(requestContext, healthStatusRecord, jobStatus);
        if (flag)
          this.QueueAccountHealthStatusJob(executionContext, currentAttemptCount);
        this.PublishJobRunData(jobDefinition, jobResult, elapsedMilliseconds, currentAttemptCount);
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1083050, "Health Manager", "Job", nameof (Run));
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.ClearLogicalContext();
        this.m_resultMessage.Clear();
      }
      return jobResult;
    }

    [Info("InternalForTestPurpose")]
    internal virtual void ValidateRequestContext(IVssRequestContext requestContext)
    {
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        throw new UnsupportedHostTypeException(requestContext.ServiceHost.HostType);
    }

    [Info("InternalForTestPurpose")]
    internal void Initialize()
    {
      this.m_analyzerFactory = (AbstractAnalyzerFactory) new AnalyzerFactory();
      this.m_dataProviderFactory = (AbstractDataProviderFactory) new DataProviderFactory();
      this.m_contextBuilderFactory = (AbstractContextBuilderFactory) new ContextBuilderFactory();
      this.m_contextDataSet = new Dictionary<DataType, ProviderContext>();
      this.m_inputDataToAnalyze = new List<HealthData>();
      this.m_resultMessage = new StringBuilder();
    }

    [Info("InternalForTestPurpose")]
    internal List<HealthData> PrepareDataToAnalyzeForScenario(
      IVssRequestContext requestContext,
      Scenario scenario,
      HashSet<DataType> dataTypes,
      HealthStatusRecord inputRecord)
    {
      List<HealthData> analyzeForScenario = new List<HealthData>();
      foreach (DataType dataType in dataTypes)
      {
        ProviderContext providerContext = this.m_contextBuilderFactory.GetProviderContextBuilder(dataType).BuildContext(requestContext, scenario, inputRecord.Data, this.m_collectionIndexingProperties);
        this.m_contextDataSet.Add(dataType, providerContext);
        IDataProvider dataProvider = this.m_dataProviderFactory.GetDataProvider(dataType);
        analyzeForScenario.AddRange((IEnumerable<HealthData>) dataProvider.GetData(providerContext));
      }
      return analyzeForScenario;
    }

    [Info("InternalForTestPurpose")]
    internal virtual List<ActionData> ExecutePipeline(
      IVssRequestContext requestContext,
      Scenario scenario,
      HealthStatusRecord inputRecord)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerbose(1083050, "Health Manager", "Job", FormattableString.Invariant(FormattableStringFactory.Create("Starting data fetch for scenario {0}.", (object) scenario)));
      this.GetDataTypesAndCreateDataForScenario(requestContext, scenario, inputRecord);
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerbose(1083050, "Health Manager", "Job", FormattableString.Invariant(FormattableStringFactory.Create("Data fetch completed for scenario {0}.", (object) scenario)));
      this.m_resultMessage.Append(FormattableString.Invariant(FormattableStringFactory.Create("Data fetch completed.")));
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerbose(1083050, "Health Manager", "Job", FormattableString.Invariant(FormattableStringFactory.Create("Starting analysis for scenario {0}.", (object) scenario)));
      this.m_resultMessage.Append("Starting Analysis phase.");
      Stopwatch stopwatch = Stopwatch.StartNew();
      List<ActionData> actionDataList = new List<ActionData>();
      foreach (IAnalyzer analyzer in this.m_analyzers)
      {
        string result;
        actionDataList.AddRange((IEnumerable<ActionData>) analyzer.Analyze(this.m_inputDataToAnalyze, this.m_contextDataSet, out result));
        this.m_resultMessage.Append(result);
      }
      stopwatch.Stop();
      long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
      FriendlyDictionary<string, object> properties = new FriendlyDictionary<string, object>();
      properties.Add("TimeInMilliSecondsForAnalysisPhaseCompletion", (object) elapsedMilliseconds);
      string message = FormattableString.Invariant(FormattableStringFactory.Create("Completed analysis for scenario {0} in time period {1}. ", (object) scenario, (object) elapsedMilliseconds));
      this.m_resultMessage.Append(message);
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishClientTrace("Health Manager", "Job", (IDictionary<string, object>) properties, true);
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerbose(1083050, "Health Manager", "Job", message);
      return actionDataList;
    }

    [Info("InternalForTestPurpose")]
    internal virtual void InvokeActionOrchestrator(
      IVssRequestContext requestContext,
      List<ActionData> actionData,
      AbstractActionFactory actionFactory,
      ActionSanitizer actionSanitizer,
      out string resultMessage)
    {
      StringBuilder stringBuilder = new StringBuilder();
      actionData = actionSanitizer.Sanitize(actionData);
      foreach (IAction action in actionFactory.GetActions(actionData))
      {
        string resultMessage1;
        action.Invoke(requestContext, out resultMessage1);
        stringBuilder.Append(resultMessage1);
      }
      resultMessage = stringBuilder.ToString();
      stringBuilder.Clear();
    }

    [Info("InternalForTestPurpose")]
    internal virtual void HandleActions(
      IVssRequestContext requestContext,
      List<ActionData> actionData,
      JobMode mode)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("Starting actions phase.");
      if (mode == JobMode.Mitigate)
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerbose(1083050, "Health Manager", "Job", FormattableString.Invariant(FormattableStringFactory.Create("Starting execution of actions.")));
        ActionFactory actionFactory = new ActionFactory();
        ActionSanitizer actionSanitizer = new ActionSanitizer();
        string resultMessage;
        this.InvokeActionOrchestrator(requestContext, actionData, (AbstractActionFactory) actionFactory, actionSanitizer, out resultMessage);
        stringBuilder.Append(resultMessage);
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerbose(1083050, "Health Manager", "Job", FormattableString.Invariant(FormattableStringFactory.Create("Completed execution of actions.")));
      }
      else
      {
        stringBuilder.Append(FormattableString.Invariant(FormattableStringFactory.Create("As JobMode is {0}, not running actions. Recommended actions: ", (object) mode)));
        foreach (ActionData actionData1 in actionData)
          stringBuilder.Append((object) actionData1.ActionType).Append(", ");
      }
      string message = stringBuilder.ToString();
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishClientTraceMessage("Health Manager", "Job", Level.Info, message);
      this.m_resultMessage.Append(message);
      stringBuilder.Clear();
    }

    [Info("InternalForTestPurpose")]
    internal void GetDataTypesAndCreateDataForScenario(
      IVssRequestContext requestContext,
      Scenario scenario,
      HealthStatusRecord inputRecord)
    {
      this.m_contextDataSet = new Dictionary<DataType, ProviderContext>();
      this.m_analyzers = this.m_analyzerFactory.GetAnalyzers(scenario);
      this.m_dataTypes = this.m_analyzers.SelectMany<IAnalyzer, DataType>((Func<IAnalyzer, IEnumerable<DataType>>) (x => (IEnumerable<DataType>) x.GetDataTypes())).ToHashSet<DataType>();
      this.m_inputDataToAnalyze = this.PrepareDataToAnalyzeForScenario(requestContext, scenario, this.m_dataTypes, inputRecord);
    }

    [Info("InternalForTestPurpose")]
    internal HealthStatusRecord GetHealthStatusRecord(
      IVssRequestContext requestContext,
      string jobName)
    {
      IHealthStatusDataAccess statusDataAccess = this.DataAccessFactory.GetHealthStatusDataAccess();
      IEnumerable<HealthStatusRecord> statusJobDataRecords = statusDataAccess.GetHealthStatusJobDataRecords(requestContext, 1, requestContext.GetCollectionID(), jobName, JobStatus.FailedRetry);
      if (!statusJobDataRecords.Any<HealthStatusRecord>())
        statusJobDataRecords = statusDataAccess.GetHealthStatusJobDataRecords(requestContext, 1, requestContext.GetCollectionID(), jobName, JobStatus.Queued);
      if (!statusJobDataRecords.Any<HealthStatusRecord>())
        return this.GetDefaultHealthStatusRecord(requestContext);
      HealthStatusRecord healthStatusRecord = statusJobDataRecords.Count<HealthStatusRecord>() == 1 ? statusJobDataRecords.First<HealthStatusRecord>() : throw new SearchServiceException(FormattableString.Invariant(FormattableStringFactory.Create("Incorrect number of status record(s) found {0} for collection {1}", (object) statusJobDataRecords.Count<HealthStatusRecord>(), (object) requestContext.GetCollectionID())));
      this.ValidateHealthStatusRecord(healthStatusRecord);
      return healthStatusRecord;
    }

    internal void UpdateHealthStatusRecordWithJobRunStatus(
      IVssRequestContext requestContext,
      HealthStatusRecord healthStatusRecord,
      JobStatus jobStatus)
    {
      if (healthStatusRecord.Id == 0)
        return;
      IHealthStatusDataAccess statusDataAccess = this.DataAccessFactory.GetHealthStatusDataAccess();
      int id1 = healthStatusRecord.Id;
      JobStatus jobStatus1 = jobStatus;
      IVssRequestContext requestContext1 = requestContext;
      int id2 = id1;
      int num = (int) jobStatus1;
      statusDataAccess.UpdateHealthStatusEntry(requestContext1, id2, (JobStatus) num);
    }

    [Info("InternalForTestPurpose")]
    internal string GetAttemptCountRegistryPath(IVssRequestContext requestContext) => "/Service/ALMSearch/Settings/" + requestContext.GetCurrentHostIdAsNormalizedString();

    [Info("InternalForTestPurpose")]
    internal virtual void QueueAccountHealthStatusJob(
      ExecutionContext executionContext,
      int currentAttemptCount)
    {
      int faultInJobDelayInSec = executionContext.ServiceSettings.JobSettings.AccountFaultInJobDelayInSec;
      executionContext.RequestContext.QueueDelayedNamedJob(this.HealthStatusJobId, faultInJobDelayInSec * currentAttemptCount, JobPriorityLevel.Normal);
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1083050, "Health Manager", "Job", FormattableString.Invariant(FormattableStringFactory.Create("Queued next iteration of '{0}' to run after {1} seconds", (object) this.GetType().Name, (object) faultInJobDelayInSec)));
    }

    [Info("InternalForTestPurpose")]
    internal void PublishJobRunData(
      TeamFoundationJobDefinition jobDefinition,
      TeamFoundationJobExecutionResult jobResult,
      long elapsedTime,
      int currentAttemptCount)
    {
      FriendlyDictionary<string, object> properties = new FriendlyDictionary<string, object>();
      properties.Add("JobExecutionTime", (object) elapsedTime);
      properties.Add("OperationStatus", (object) jobResult.ToString());
      properties.Add("JobName", (object) jobDefinition.Name);
      properties.Add("AttemptCount", (object) currentAttemptCount);
      if (jobResult != TeamFoundationJobExecutionResult.Succeeded)
        properties.Add("JobExecutionResultMessage", (object) this.m_resultMessage);
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishClientTrace("Health Manager", "Job", (IDictionary<string, object>) properties, true);
    }

    private void GetCollectionIndexProperties(
      IVssRequestContext requestContext,
      HealthStatusRecord inputData)
    {
      this.m_collectionIndexingProperties = new CollectionIndexingProperties();
      this.m_collectionIndexingProperties = this.DataAccessFactory.GetIndexingUnitDataAccess().GetIndexingUnit(requestContext, requestContext.GetCollectionID(), "Collection", inputData.Data.EntityType)?.Properties as CollectionIndexingProperties;
    }

    internal abstract HealthStatusRecord GetDefaultHealthStatusRecord(
      IVssRequestContext requestContext);

    internal abstract void ValidateHealthStatusRecord(HealthStatusRecord healthStatusRecord);

    protected Guid HealthStatusJobId { get; }

    [Info("InternalForTestPurpose")]
    internal IDataAccessFactory DataAccessFactory { get; set; }
  }
}
