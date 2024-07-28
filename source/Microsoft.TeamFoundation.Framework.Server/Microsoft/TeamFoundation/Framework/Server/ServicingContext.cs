// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ServicingContext
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server.AuditLog;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Security;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class ServicingContext : 
    IInternalServicingContext,
    IServicingContext,
    ICancelable,
    IDisposable
  {
    private object m_tokensLock = new object();
    private object m_itemsLock = new object();
    private int m_targetPartitionId;
    private IVssRequestContext m_aggregationContext;
    private IVssRequestContext m_deploymentRequestContext;
    private IVssRequestContext m_targetRequestContext;
    private ServicingDictionary<string, string> m_tokens;
    private ServicingDictionary<string, object> m_items;
    private IServicingStepDetailLogger m_logger;
    private ITFLogger m_TFLogger;
    private ServicingLogLevel m_loggingLevel;
    private string m_servicingOperation;
    private string m_operationClass;
    private ServicingStepGroup m_stepGroup;
    private string m_stepId;
    private ServicingStepState m_stepResolution;
    private ServicingStepState m_groupResolution;
    private List<string> m_messagesForException;
    private object m_lock = new object();
    private ILockName m_createHostLockName;
    private Guid m_jobSourceHost = Guid.Empty;
    private Stopwatch m_stepStopwatch = new Stopwatch();
    private Stopwatch m_groupStopwatch = new Stopwatch();
    private Stopwatch m_operationStopwatch = new Stopwatch();
    private bool m_isDisposed;
    private bool? m_isOnPrem;
    private Lazy<Microsoft.TeamFoundation.Framework.Server.ServicingLeaseManager> m_servicingLeaseManager;
    private static readonly Dictionary<string, string> s_projectCreateTokens = new Dictionary<string, string>()
    {
      {
        "BUILDSERVICEGROUP",
        ""
      },
      {
        "PROJECTADMINGROUP",
        ""
      },
      {
        "PROJECTCOLLECTIONADMINGROUP",
        ""
      },
      {
        "PROJECTCOLLECTIONBUILDADMINSGROUP",
        ""
      },
      {
        "PROJECTCOLLECTIONBUILDSERVICESGROUP",
        ""
      }
    };
    private static readonly string[] s_tokenDelimiters = new string[2]
    {
      "$$",
      "$i$"
    };

    public ServicingContext(
      IVssRequestContext aggregationContext,
      IVssRequestContext deploymentRequestContext,
      IServicingResourceProvider resourceProvider,
      IServicingStepDetailLogger dbLogger,
      ITFLogger tfLogger,
      IDictionary<string, string> servicingTokens,
      IDictionary<string, object> servicingItems,
      string operationClass)
    {
      ArgumentUtility.CheckForNull<IServicingResourceProvider>(resourceProvider, nameof (resourceProvider));
      ArgumentUtility.CheckForNull<IServicingStepDetailLogger>(dbLogger, nameof (dbLogger));
      ArgumentUtility.CheckForNull<ITFLogger>(tfLogger, nameof (tfLogger));
      ArgumentUtility.CheckForNull<IDictionary<string, string>>(servicingTokens, nameof (servicingTokens));
      ArgumentUtility.CheckForNull<IDictionary<string, object>>(servicingItems, nameof (servicingItems));
      this.m_aggregationContext = aggregationContext;
      this.DeploymentRequestContext = deploymentRequestContext;
      this.ResourceProvider = resourceProvider;
      this.m_logger = (IServicingStepDetailLogger) new StepDetailAggregateLogger(new IServicingStepDetailLogger[2]
      {
        (IServicingStepDetailLogger) new StepDetailLogger(tfLogger),
        dbLogger
      });
      this.m_TFLogger = (ITFLogger) new ServicingLogger((IServicingContext) this);
      this.m_operationClass = operationClass;
      this.m_tokens = new ServicingDictionary<string, string>(this, "token", servicingTokens, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.m_servicingLeaseManager = new Lazy<Microsoft.TeamFoundation.Framework.Server.ServicingLeaseManager>(new Func<Microsoft.TeamFoundation.Framework.Server.ServicingLeaseManager>(this.CreateServicingLeaseManager));
      string str;
      if (!this.m_tokens.TryGetValue(ServicingTokenConstants.LoggingLevel, out str) || !Enum.TryParse<ServicingLogLevel>(str, out this.m_loggingLevel))
        this.m_loggingLevel = ServicingLogLevel.Verbose;
      Dictionary<string, object> initialItems = new Dictionary<string, object>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (KeyValuePair<string, object> servicingItem in (IEnumerable<KeyValuePair<string, object>>) servicingItems)
      {
        object obj = !(servicingItem.Value is SerializationWrapper serializationWrapper) ? servicingItem.Value : serializationWrapper.Value;
        initialItems.Add(servicingItem.Key, obj);
      }
      this.m_items = new ServicingDictionary<string, object>(this, "item", (IDictionary<string, object>) initialItems, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      if (this.IsOnPrem)
        return;
      IServicingContextListener servicingContextListener = (IServicingContextListener) new EtwServicingContextListener(ServicingEtwLogger.Log, this.CreateEtwListenerConfiguration());
      this.ServicingOperationStarted += new EventHandler<ServicingOperationStartedEventArgs>(servicingContextListener.OnOperationStarted);
      this.ServicingOperationEnded += new EventHandler<ServicingOperationEndedEventArgs>(servicingContextListener.OnOperationEnded);
      this.ServicingStepGroupStarted += new EventHandler<ServicingStepGroupStartedEventArgs>(servicingContextListener.OnStepGroupStarted);
      this.ServicingStepGroupEnded += new EventHandler<ServicingStepGroupEndedEventArgs>(servicingContextListener.OnStepGroupEnded);
      this.ServicingStepStarted += new EventHandler<ServicingStepStartedEventArgs>(servicingContextListener.OnStepStarted);
      this.ServicingStepEnded += new EventHandler<ServicingStepEndedEventArgs>(servicingContextListener.OnStepEnded);
    }

    public ServicingContext(
      IVssRequestContext deploymentRequestContext,
      IServicingResourceProvider resourceProvider,
      IServicingStepDetailLogger dbLogger,
      string operationClass = null)
      : this((IVssRequestContext) null, deploymentRequestContext, resourceProvider, dbLogger, (ITFLogger) new NullLogger(), (IDictionary<string, string>) new Dictionary<string, string>(), (IDictionary<string, object>) new Dictionary<string, object>(), operationClass)
    {
    }

    void IDisposable.Dispose()
    {
      lock (this.m_lock)
      {
        if (this.m_servicingLeaseManager.IsValueCreated)
          this.m_servicingLeaseManager.Value.Dispose();
        this.CleanupRequestContext(ref this.m_targetRequestContext);
        this.CleanupRequestContext(ref this.m_deploymentRequestContext);
        if (this.m_items != null)
        {
          foreach (object obj in (IEnumerable<object>) this.m_items.Values)
          {
            if (obj is IDisposable disposable)
              disposable.Dispose();
          }
          this.m_items.Clear();
          this.m_items = (ServicingDictionary<string, object>) null;
        }
        this.m_isDisposed = true;
      }
      GC.SuppressFinalize((object) this);
    }

    public bool IsDisposed => this.m_isDisposed;

    public ServicingOperationTarget ServicingOperationTarget { get; set; }

    public Guid TargetHostId { get; set; }

    public IDictionary<string, string> Tokens => (IDictionary<string, string>) this.m_tokens;

    public IDictionary<string, object> Items => (IDictionary<string, object>) this.m_items;

    public IServicingResourceProvider ResourceProvider { get; private set; }

    public IVssRequestContext DeploymentRequestContext
    {
      get => this.m_deploymentRequestContext;
      internal set
      {
        lock (this.m_lock)
        {
          this.CleanupRequestContext(ref this.m_deploymentRequestContext);
          this.m_deploymentRequestContext = value;
          if (value == null)
            return;
          this.m_createHostLockName = value.ServiceHost.CreateUniqueLockName("ServicingContext.CreateHostLock");
        }
      }
    }

    public ServicingStepState GroupResolution => this.m_groupResolution;

    public string CurrentServicingOperation => this.m_servicingOperation;

    public string CurrentStepGroupName => this.m_stepGroup?.Name;

    public int CurrentStepNumber
    {
      get
      {
        ServicingStepGroup stepGroup = this.m_stepGroup;
        return stepGroup == null ? -1 : stepGroup.Steps.FindIndex((Predicate<ServicingStep>) (o => o.Name == this.m_stepId)) + 1;
      }
    }

    public List<ServicingStep> CurrentStepGroupSteps => this.m_stepGroup?.Steps;

    public string OperationClass => this.m_operationClass;

    public string StepName => this.m_stepId;

    public bool IsOnPrem
    {
      get
      {
        if (!this.m_isOnPrem.HasValue)
        {
          string str;
          if (this.TryGetItem<string>(ServicingTokenConstants.DeploymentType, out str))
          {
            this.m_isOnPrem = new bool?((DeploymentType) Enum.Parse(typeof (DeploymentType), str) == DeploymentType.OnPremises);
          }
          else
          {
            bool flag;
            if (this.TryGetItem<bool>(ServicingItemConstants.HostedDeployment, out flag))
            {
              this.m_isOnPrem = new bool?(!flag);
            }
            else
            {
              ISqlConnectionInfo sqlConnectionInfo;
              if (this.TryGetItem<ISqlConnectionInfo>(ServicingItemConstants.DboConnectionInfo, out sqlConnectionInfo))
                this.m_isOnPrem = new bool?(new SqlConnectionStringBuilder(sqlConnectionInfo.ConnectionString).IntegratedSecurity);
            }
          }
        }
        return this.m_isOnPrem.HasValue && this.m_isOnPrem.Value;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Browsable(false)]
    internal bool IsRerunTesting { get; set; }

    internal Guid JobSourceHost
    {
      get => this.m_jobSourceHost;
      set
      {
        if (this.DeploymentRequestContext == null)
          throw new TeamFoundationServicingException("You can only set the JobSourceHost if there is a non-null DeploymentRequestContext.");
        this.m_jobSourceHost = value;
      }
    }

    public void VerifyDeploymentJobSource()
    {
      if (this.JobSourceHost != this.DeploymentRequestContext.ServiceHost.InstanceId)
        throw new TeamFoundationServicingException(string.Format("Servicing job source host '{0}' is not a deployment host.", (object) this.JobSourceHost));
    }

    public IVssRequestContext GetTargetRequestContext() => this.GetTargetRequestContext(true);

    public void DisposeTargetRequestContext()
    {
      lock (this.m_lock)
      {
        this.CleanupRequestContext(ref this.m_targetRequestContext);
        this.m_targetPartitionId = DatabasePartitionConstants.InvalidPartitionId;
      }
    }

    public void DisposeDeploymentRequestContext()
    {
      lock (this.m_lock)
        this.CleanupRequestContext(ref this.m_deploymentRequestContext);
    }

    public IVssRequestContext GetTargetRequestContext(bool createIfNotExist)
    {
      if (this.m_targetRequestContext == null & createIfNotExist)
      {
        if (this.DeploymentRequestContext == null)
          throw new InvalidOperationException("Cannot create TargetRequestContext when DeploymentRequestContext is null.");
        if (this.TargetHostId == Guid.Empty)
          throw new InvalidOperationException("Cannot create TargetRequestContext when TargetHostId is an empty Guid.");
        using (this.DeploymentRequestContext.AcquireWriterLock(this.m_createHostLockName))
        {
          if (this.m_targetRequestContext == null)
          {
            using (this.DeploymentRequestContext.AcquireExemptionLock())
              this.m_targetRequestContext = this.BeginTargetRequestContext();
            this.m_targetRequestContext.Items[RequestContextItemsKeys.ServicingOperationClass] = (object) this.m_operationClass;
            this.m_targetRequestContext.SetAuditCorrelationId(this.Tokens);
            if (string.Equals(this.m_operationClass, "UpgradeHost", StringComparison.OrdinalIgnoreCase))
            {
              ((IRequestContextInternal) this.m_targetRequestContext).RequestPriority = VssRequestContextPriority.Low;
              this.m_targetRequestContext.Items[RequestContextItemsKeys.ComponentInitInfo] = (object) new VssComponentInitInfo(this, 25);
            }
            if (this.m_targetRequestContext.ExecutionEnvironment.IsHostedDeployment)
              this.m_targetRequestContext.Items[RequestContextItemsKeys.HttpRetryInfo] = (object) new VssHttpRetryInfo(this, 25);
          }
        }
      }
      return this.m_targetRequestContext;
    }

    public int GetTargetPartitionId()
    {
      if (this.m_targetPartitionId == DatabasePartitionConstants.InvalidPartitionId)
      {
        lock (this.m_lock)
        {
          if (this.m_targetRequestContext != null)
          {
            this.m_targetPartitionId = this.m_targetRequestContext.ServiceHost.PartitionId;
            return this.m_targetPartitionId;
          }
        }
        if (this.TargetHostId == Guid.Empty)
          throw new InvalidOperationException("Cannot get TargetPartitionId when TargetHostId is an empty Guid.");
        bool flag;
        if (!this.TryGetItem<bool>(ServicingItemConstants.CollectionDatabaseReachable, out flag) | flag)
        {
          using (DatabasePartitionComponent component = DatabasePartitionComponent.CreateComponent(this.GetConnectionInfo()))
          {
            component.PartitionId = 0;
            this.m_targetPartitionId = component.QueryPartitionId(this.TargetHostId);
          }
        }
      }
      return this.m_targetPartitionId;
    }

    void ICancelable.Cancel()
    {
      this.LogInfo("ServicingContext.Canceled was called. Stack trace: " + EnvironmentWrapper.ToReadableStackTrace());
      lock (this.m_lock)
      {
        if (this.m_targetRequestContext != null && !this.m_targetRequestContext.IsCanceled)
        {
          this.LogInfo("Calling Cancel on target request context.");
          this.m_targetRequestContext.Cancel("Job has been canceled.");
        }
        if (this.m_deploymentRequestContext != null)
        {
          if (!this.m_deploymentRequestContext.IsCanceled)
          {
            this.LogInfo("Calling Cancel on the delployment request context.");
            this.m_deploymentRequestContext.Cancel("Job has been canceled.");
          }
        }
      }
      this.LogInfo("ServicingContext.Canceled complete.");
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Browsable(false)]
    public void SetTargetPartitionId(int partitionId) => this.m_targetPartitionId = partitionId;

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Browsable(false)]
    internal bool UseSystemTargetRequestContext { get; set; }

    public void AddTokenIfNotDefined(string tokenName, string tokenValue)
    {
      lock (this.m_tokensLock)
      {
        if (this.m_tokens.ContainsKey(tokenName))
          return;
        this.m_tokens[tokenName] = tokenValue;
      }
    }

    public void AddItemIfNotDefined(string itemName, object objectValue)
    {
      lock (this.m_itemsLock)
      {
        if (this.m_items.ContainsKey(itemName))
          return;
        this.m_items[itemName] = objectValue;
      }
    }

    public TComponent CreateComponent<TComponent>(
      int partitionId,
      int commandTimeout = 60,
      int deadlockPause = 200,
      int maxDeadlockRetries = 25)
      where TComponent : TeamFoundationSqlResourceComponent, new()
    {
      TComponent componentRaw = TeamFoundationResourceManagementService.CreateComponentRaw<TComponent>(this.GetConnectionInfo(), commandTimeout, deadlockPause, maxDeadlockRetries);
      componentRaw.PartitionId = partitionId;
      return componentRaw;
    }

    public ISqlConnectionInfo GetConnectionInfo()
    {
      ISqlConnectionInfo connectionInfo;
      this.TryGetItem<ISqlConnectionInfo>(ServicingItemConstants.DboConnectionInfo, out connectionInfo);
      if (connectionInfo == null)
      {
        ISqlConnectionInfo defaultConnectionInfo = this.GetDefaultConnectionInfo();
        if (this.IsOnPrem)
          connectionInfo = defaultConnectionInfo;
        else if (this.DeploymentRequestContext != null)
        {
          ITeamFoundationDatabaseProperties databaseProperties;
          this.DeploymentRequestContext.GetService<ITeamFoundationDatabaseManagementService>().TryGetDatabaseProperties(this.DeploymentRequestContext, defaultConnectionInfo.DataSource, defaultConnectionInfo.InitialCatalog, out databaseProperties);
          if (databaseProperties == null)
            throw new DatabaseNotFoundException("Failed to find database properties for database " + defaultConnectionInfo.InitialCatalog + " on " + defaultConnectionInfo.DataSource + "!!");
          connectionInfo = databaseProperties.DboConnectionInfo ?? defaultConnectionInfo;
        }
        else
          connectionInfo = TeamFoundationDatabaseManagementService.GetConfigDbDboConnectionInfoBootStrap(defaultConnectionInfo);
        this.Items[ServicingItemConstants.DboConnectionInfo] = (object) connectionInfo;
      }
      return connectionInfo;
    }

    public ISqlConnectionInfo GetDefaultConnectionInfo() => this.GetItem<ISqlConnectionInfo>(ServicingItemConstants.ConnectionInfo);

    public ISqlConnectionInfo GetDataTierConnectionInfo()
    {
      ISqlConnectionInfo sqlConnectionInfo = this.GetItem<ISqlConnectionInfo>(ServicingItemConstants.ConnectionInfo);
      string connectionString = (string) null;
      this.Tokens.TryGetValue(ServicingTokenConstants.DataTierConnectionString, out connectionString);
      return SqlConnectionInfoFactory.Create(connectionString).CloneReplaceInitialCatalog(sqlConnectionInfo.InitialCatalog);
    }

    public T GetItem<T>(string itemName)
    {
      object obj;
      if (!this.m_items.TryGetValue(itemName, out obj))
        throw new TeamFoundationServicingException(FrameworkResources.ServicingItemNotDefined((object) itemName));
      if (obj == null)
        throw new TeamFoundationServicingException(FrameworkResources.ServicingItemIsNull((object) itemName));
      return typeof (T).IsAssignableFrom(obj.GetType()) ? (T) obj : throw new TeamFoundationServicingException(FrameworkResources.ServicingItemIsWrongType((object) itemName, (object) typeof (T).FullName));
    }

    public bool TryGetItem<T>(string itemName, out T item)
    {
      object obj;
      if (!this.m_items.TryGetValue(itemName, out obj))
      {
        item = default (T);
        return false;
      }
      if (obj == null)
      {
        item = default (T);
        return false;
      }
      if (!typeof (T).IsAssignableFrom(obj.GetType()))
      {
        item = default (T);
        return false;
      }
      item = (T) obj;
      return true;
    }

    public T ParseToken<T>(string tokenName, T defaultValue)
    {
      string str;
      return this.m_tokens.TryGetValue(tokenName, out str) ? RegistryUtility.FromString<T>(str, defaultValue) : defaultValue;
    }

    public string GetRequiredToken(string tokenName)
    {
      string str;
      if (!this.m_tokens.TryGetValue(tokenName, out str))
        throw new TeamFoundationServicingException(FrameworkResources.RequiredServicingTokenNotDefined((object) tokenName));
      return str != null ? str : throw new TeamFoundationServicingException(FrameworkResources.RequiredServicingTokenIsNull((object) tokenName));
    }

    public T GetRequiredToken<T>(string tokenName) => RegistryUtility.FromString<T>(this.GetRequiredToken(tokenName));

    public bool TryGetToken(string tokenName, out string value) => this.m_tokens.TryGetValue(tokenName, out value) && !string.IsNullOrEmpty(value);

    public string ReplaceResources(string templateText)
    {
      ArgumentUtility.CheckForNull<string>(templateText, nameof (templateText));
      bool replacedAll;
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      string message = StringUtil.ReplaceResources(templateText, ServicingContext.\u003C\u003EO.\u003C0\u003E__Escape ?? (ServicingContext.\u003C\u003EO.\u003C0\u003E__Escape = new Func<string, string>(SecurityElement.Escape)), out replacedAll);
      if (!replacedAll)
        this.Error(FrameworkResources.StepDataUndefinedResourcesWarning((object) SecretUtility.ScrubSecrets(message, false)));
      return message;
    }

    public string ReplaceTokens(string templateText)
    {
      ArgumentUtility.CheckForNull<string>(templateText, nameof (templateText));
      bool replacedAll;
      string str = this.ReplaceTokensHelper(templateText, ServicingContext.s_tokenDelimiters, ServicingContext.s_tokenDelimiters, (IDictionary<string, string>) this.m_tokens, out replacedAll);
      if (!replacedAll)
      {
        this.ReplaceTokensHelper(str, ServicingContext.s_tokenDelimiters, ServicingContext.s_tokenDelimiters, (IDictionary<string, string>) ServicingContext.s_projectCreateTokens, out replacedAll);
        if (!replacedAll)
          this.Error(FrameworkResources.StepDataUndefinedTokensWarning((object) SecretUtility.ScrubSecrets(str, false)));
      }
      return str;
    }

    public IServicingLeaseManager ServicingLeaseManager => (IServicingLeaseManager) this.m_servicingLeaseManager.Value;

    protected void AddSecretTokenNames(IEnumerable<string> secretTokenNames) => this.m_tokens.SetSecretTokenNames(secretTokenNames);

    private string ReplaceTokensHelper(
      string inputText,
      string[] tokenPrefixes,
      string[] tokenSuffixes,
      IDictionary<string, string> tokens,
      out bool replacedAll)
    {
      replacedAll = true;
      bool replacedAll1 = false;
      string text = inputText;
      for (int index = 0; index < tokenPrefixes.Length; ++index)
      {
        text = StringUtil.ReplaceTokens(text, tokenPrefixes[index], tokenSuffixes[index], (Func<string, string>) (tokenName =>
        {
          string str;
          if (!tokens.TryGetValue(tokenName, out str))
          {
            if (string.Equals(ServicingTokenConstants.PartitionId, tokenName) && this.TargetHostId != Guid.Empty)
              str = this.GetTargetPartitionId().ToString((IFormatProvider) CultureInfo.InvariantCulture);
          }
          else if (str == null)
            str = "";
          if (!string.IsNullOrEmpty(str))
            str = SecurityElement.Escape(str);
          return str;
        }), out replacedAll1);
        if (!replacedAll1)
          replacedAll = false;
      }
      return text;
    }

    public ITFLogger TFLogger
    {
      get => this.m_TFLogger;
      set
      {
        ArgumentUtility.CheckForNull<ITFLogger>(value, nameof (TFLogger));
        this.m_TFLogger = value;
      }
    }

    public ServicingLogLevel LoggingLevel
    {
      get => this.m_loggingLevel;
      set => this.m_loggingLevel = value;
    }

    public void Log(ServicingStepLogEntryKind entryKind, string message)
    {
      if (this.LoggingLevel == ServicingLogLevel.Verbose || entryKind == ServicingStepLogEntryKind.Error && this.LoggingLevel >= ServicingLogLevel.Error || entryKind == ServicingStepLogEntryKind.Warning && this.LoggingLevel >= ServicingLogLevel.Warning)
      {
        ServicingStepLogEntry stepDetail = new ServicingStepLogEntry();
        stepDetail.ServicingOperation = this.m_servicingOperation;
        stepDetail.ServicingStepGroupId = this.CurrentStepGroupName;
        stepDetail.ServicingStepId = this.m_stepId;
        stepDetail.EntryKind = entryKind;
        stepDetail.Message = message;
        this.m_logger.AddServicingStepDetail((ServicingStepDetail) stepDetail);
      }
      switch (entryKind)
      {
        case ServicingStepLogEntryKind.Warning:
          if (this.m_stepResolution != ServicingStepState.Failed)
            this.m_stepResolution = ServicingStepState.PassedWithWarnings;
          if (this.m_groupResolution == ServicingStepState.Failed)
            break;
          this.m_groupResolution = ServicingStepState.PassedWithWarnings;
          break;
        case ServicingStepLogEntryKind.Error:
          if (this.m_messagesForException == null)
            this.m_messagesForException = new List<string>();
          this.m_messagesForException.Add(message);
          this.m_stepResolution = ServicingStepState.Failed;
          this.m_groupResolution = ServicingStepState.Failed;
          break;
      }
    }

    public void LogInfo(string message) => this.Log(ServicingStepLogEntryKind.Informational, message);

    public void LogInfo(string format, params object[] args) => this.Log(ServicingStepLogEntryKind.Informational, string.Format((IFormatProvider) CultureInfo.InvariantCulture, format, args));

    public void Warn(string message) => this.Log(ServicingStepLogEntryKind.Warning, message);

    public void Warn(string format, params object[] args) => this.Log(ServicingStepLogEntryKind.Warning, string.Format((IFormatProvider) CultureInfo.InvariantCulture, format, args));

    public void Error(string message) => this.Log(ServicingStepLogEntryKind.Error, message);

    public void Error(string format, params object[] args) => this.Log(ServicingStepLogEntryKind.Error, string.Format((IFormatProvider) CultureInfo.InvariantCulture, format, args));

    internal void StartStep(string stepId)
    {
      this.m_stepId = stepId;
      this.m_stepResolution = ServicingStepState.Passed;
      if (this.LoggingLevel >= ServicingLogLevel.Progress)
      {
        ServicingStepStateChange stepDetail = new ServicingStepStateChange();
        stepDetail.ServicingOperation = this.m_servicingOperation;
        stepDetail.ServicingStepGroupId = this.CurrentStepGroupName;
        stepDetail.ServicingStepId = this.m_stepId;
        stepDetail.StepState = ServicingStepState.Executing;
        this.m_logger.AddServicingStepDetail((ServicingStepDetail) stepDetail);
      }
      EventHandler<ServicingStepStartedEventArgs> servicingStepStarted = this.ServicingStepStarted;
      if (servicingStepStarted != null)
      {
        ServicingStepStartedEventArgs e = new ServicingStepStartedEventArgs(this.CurrentServicingOperation, this.CurrentStepGroupName, stepId, ServicingStepState.Executing, DateTime.UtcNow);
        servicingStepStarted((object) this, e);
      }
      this.m_stepStopwatch.Restart();
    }

    internal void StartStepGroup(string servicingOperation, ServicingStepGroup stepGroup)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(servicingOperation, nameof (servicingOperation));
      this.m_groupResolution = ServicingStepState.Passed;
      this.m_stepGroup = stepGroup;
      EventHandler<ServicingStepGroupStartedEventArgs> stepGroupStarted = this.ServicingStepGroupStarted;
      if (stepGroupStarted != null)
      {
        ServicingStepGroupStartedEventArgs e = new ServicingStepGroupStartedEventArgs(servicingOperation, stepGroup.Name, DateTime.UtcNow);
        stepGroupStarted((object) this, e);
      }
      this.m_groupStopwatch.Restart();
    }

    internal void StartOperation(ServicingOperation servicingOperation)
    {
      this.m_servicingOperation = servicingOperation.Name;
      EventHandler<ServicingOperationStartedEventArgs> operationStarted = this.ServicingOperationStarted;
      if (operationStarted != null)
      {
        ServicingOperationStartedEventArgs e = new ServicingOperationStartedEventArgs(servicingOperation.Name, DateTime.UtcNow);
        operationStarted((object) this, e);
      }
      this.m_operationStopwatch.Restart();
    }

    internal ServicingStepState FinishStep(Exception exception)
    {
      TimeSpan elapsed = this.m_stepStopwatch.Elapsed;
      if (exception != null)
      {
        this.Error(exception.Message);
        this.LogInfo(exception.ToReadableStackTrace());
      }
      EventHandler<ServicingStepEndedEventArgs> servicingStepEnded = this.ServicingStepEnded;
      if (servicingStepEnded != null)
      {
        ServicingStepEndedEventArgs e = new ServicingStepEndedEventArgs(this.CurrentServicingOperation, this.CurrentStepGroupName, this.m_stepId, this.m_stepResolution, DateTime.UtcNow, elapsed);
        servicingStepEnded((object) this, e);
      }
      if (this.LoggingLevel >= ServicingLogLevel.Progress && (!this.IsRerunTesting || elapsed.TotalMilliseconds > 1.0))
      {
        ServicingStepStateChange stepDetail1 = new ServicingStepStateChange();
        stepDetail1.ServicingOperation = this.m_servicingOperation;
        stepDetail1.ServicingStepGroupId = this.CurrentStepGroupName;
        stepDetail1.ServicingStepId = this.m_stepId;
        stepDetail1.StepState = this.m_stepResolution;
        this.m_logger.AddServicingStepDetail((ServicingStepDetail) stepDetail1);
        ServicingStepLogEntry stepDetail2 = new ServicingStepLogEntry();
        stepDetail2.ServicingOperation = this.m_servicingOperation;
        stepDetail2.ServicingStepGroupId = this.CurrentStepGroupName;
        stepDetail2.ServicingStepId = this.m_stepId;
        stepDetail2.EntryKind = ServicingStepLogEntryKind.StepDuration;
        stepDetail2.Message = elapsed.TotalSeconds.ToString("0.####");
        this.m_logger.AddServicingStepDetail((ServicingStepDetail) stepDetail2);
        if (this.m_targetRequestContext != null)
        {
          VssHttpRetryInfo vssHttpRetryInfo = (VssHttpRetryInfo) null;
          this.m_targetRequestContext.TryGetItem<VssHttpRetryInfo>(RequestContextItemsKeys.HttpRetryInfo, out vssHttpRetryInfo);
          if (vssHttpRetryInfo != null)
          {
            ServicingStepLogEntry stepDetail3 = new ServicingStepLogEntry();
            stepDetail3.ServicingOperation = this.m_servicingOperation;
            stepDetail3.ServicingStepGroupId = this.CurrentStepGroupName;
            stepDetail3.ServicingStepId = this.m_stepId;
            stepDetail3.EntryKind = ServicingStepLogEntryKind.Informational;
            stepDetail3.Message = vssHttpRetryInfo.ToString();
            this.m_logger.AddServicingStepDetail((ServicingStepDetail) stepDetail3);
            vssHttpRetryInfo.Reset();
          }
          VssComponentInitInfo componentInitInfo = (VssComponentInitInfo) null;
          this.m_targetRequestContext.TryGetItem<VssComponentInitInfo>(RequestContextItemsKeys.ComponentInitInfo, out componentInitInfo);
          if (componentInitInfo != null)
          {
            ServicingStepLogEntry stepDetail4 = new ServicingStepLogEntry();
            stepDetail4.ServicingOperation = this.m_servicingOperation;
            stepDetail4.ServicingStepGroupId = this.CurrentStepGroupName;
            stepDetail4.ServicingStepId = this.m_stepId;
            stepDetail4.EntryKind = ServicingStepLogEntryKind.Informational;
            stepDetail4.Message = componentInitInfo.ToString();
            this.m_logger.AddServicingStepDetail((ServicingStepDetail) stepDetail4);
            componentInitInfo.Reset();
          }
        }
      }
      if (exception != null)
        throw new TeamFoundationServicingException(FrameworkResources.ServicingStepExecutionFailure((object) this.m_stepId, (object) this.CurrentStepGroupName, (object) this.m_servicingOperation, (object) exception.Message), exception);
      int num = this.m_stepResolution != ServicingStepState.Failed ? (int) this.m_stepResolution : throw new TeamFoundationServicingException(FrameworkResources.ServicingStepExecutionFailure((object) this.m_stepId, (object) this.CurrentStepGroupName, (object) this.m_servicingOperation, this.m_messagesForException.Count <= 1 ? (object) this.m_messagesForException[0] : (object) ("Errors:\n" + string.Join("\n ", (IEnumerable<string>) this.m_messagesForException))));
      this.m_stepId = (string) null;
      this.m_stepResolution = ServicingStepState.NotExecuted;
      return (ServicingStepState) num;
    }

    internal void FinishStepGroup()
    {
      if (this.m_stepGroup == null)
        throw new InvalidOperationException(FrameworkResources.FinishStepGroupMustBeCalledAfterStartStepGroupError());
      TimeSpan elapsed = this.m_groupStopwatch.Elapsed;
      EventHandler<ServicingStepGroupEndedEventArgs> servicingStepGroupEnded = this.ServicingStepGroupEnded;
      if (servicingStepGroupEnded != null)
      {
        ServicingStepGroupEndedEventArgs e = new ServicingStepGroupEndedEventArgs(this.CurrentServicingOperation, this.CurrentStepGroupName, DateTime.UtcNow, elapsed);
        servicingStepGroupEnded((object) this, e);
      }
      if (this.LoggingLevel >= ServicingLogLevel.Progress && (!this.IsRerunTesting || elapsed.TotalMilliseconds > 1.0))
      {
        ServicingStepLogEntry stepDetail = new ServicingStepLogEntry();
        stepDetail.ServicingOperation = this.m_servicingOperation;
        stepDetail.ServicingStepGroupId = this.CurrentStepGroupName;
        stepDetail.EntryKind = ServicingStepLogEntryKind.GroupDuration;
        stepDetail.Message = elapsed.TotalSeconds.ToString("0.####");
        this.m_logger.AddServicingStepDetail((ServicingStepDetail) stepDetail);
      }
      this.m_groupResolution = ServicingStepState.NotExecuted;
      this.m_stepGroup = (ServicingStepGroup) null;
    }

    internal void FinishOperation()
    {
      TimeSpan elapsed = this.m_operationStopwatch.Elapsed;
      EventHandler<ServicingOperationEndedEventArgs> servicingOperationEnded = this.ServicingOperationEnded;
      if (servicingOperationEnded != null)
      {
        ServicingOperationEndedEventArgs e = new ServicingOperationEndedEventArgs(this.CurrentServicingOperation, DateTime.UtcNow, elapsed);
        servicingOperationEnded((object) this, e);
      }
      if (this.LoggingLevel >= ServicingLogLevel.Progress && (!this.IsRerunTesting || elapsed.TotalMilliseconds > 1.0))
      {
        ServicingStepLogEntry stepDetail = new ServicingStepLogEntry();
        stepDetail.ServicingOperation = this.m_servicingOperation;
        stepDetail.EntryKind = ServicingStepLogEntryKind.OperationDuration;
        stepDetail.Message = elapsed.TotalSeconds.ToString("0.####");
        this.m_logger.AddServicingStepDetail((ServicingStepDetail) stepDetail);
      }
      this.m_servicingOperation = (string) null;
    }

    internal void SkipStep(string servicingOperation, string stepGroupId, string stepId)
    {
      if (this.LoggingLevel < ServicingLogLevel.Progress || this.IsRerunTesting)
        return;
      ServicingStepStateChange stepDetail = new ServicingStepStateChange();
      stepDetail.ServicingOperation = servicingOperation;
      stepDetail.ServicingStepGroupId = stepGroupId;
      stepDetail.ServicingStepId = stepId;
      stepDetail.StepState = ServicingStepState.Skipped;
      this.m_logger.AddServicingStepDetail((ServicingStepDetail) stepDetail);
    }

    public void SkipChildren()
    {
      this.m_stepResolution = ServicingStepState.PassedWithSkipChildren;
      if (this.LoggingLevel < ServicingLogLevel.Progress)
        return;
      ServicingStepStateChange stepDetail = new ServicingStepStateChange();
      stepDetail.ServicingOperation = this.m_servicingOperation;
      stepDetail.ServicingStepGroupId = this.m_stepGroup.Name;
      stepDetail.ServicingStepId = this.m_stepId;
      stepDetail.StepState = this.m_stepResolution;
      this.m_logger.AddServicingStepDetail((ServicingStepDetail) stepDetail);
    }

    private EtwServicingContextListenerConfiguration CreateEtwListenerConfiguration()
    {
      string str1;
      this.m_tokens.TryGetValue(ServicingTokenConstants.JobId, out str1);
      string str2;
      this.m_tokens.TryGetValue(ServicingTokenConstants.HostedServiceType, out str2);
      string str3;
      this.m_tokens.TryGetValue(ServicingTokenConstants.DeploymentName, out str3);
      return new EtwServicingContextListenerConfiguration()
      {
        ServiceName = str2 ?? Environment.GetEnvironmentVariable("SERVICENAME"),
        ReleaseDefinitionId = Environment.GetEnvironmentVariable("RELEASE_DEFINITIONID"),
        ReleaseId = Environment.GetEnvironmentVariable("RELEASE_RELEASEID"),
        AttemptNumber = Environment.GetEnvironmentVariable("RELEASE_ATTEMPTNUMBER"),
        BranchName = Environment.GetEnvironmentVariable("BUILD_SOURCEBRANCH"),
        BuildNumber = Environment.GetEnvironmentVariable("BUILD_BUILDNUMBER"),
        DeploymentName = str3 ?? Environment.GetEnvironmentVariable("DEPLOYMENTNAME"),
        JobId = str1
      };
    }

    private void CleanupRequestContext(ref IVssRequestContext requestContext)
    {
      if (requestContext == null)
        return;
      try
      {
        if (this.m_aggregationContext == null)
          return;
        PerformanceTimer.AggregatePerformanceTimings(requestContext, this.m_aggregationContext);
      }
      finally
      {
        requestContext.Dispose();
        requestContext = (IVssRequestContext) null;
      }
    }

    private Microsoft.TeamFoundation.Framework.Server.ServicingLeaseManager CreateServicingLeaseManager()
    {
      if (this.DeploymentRequestContext == null)
        throw new InvalidOperationException("Cannot create a servicing lease manager when the DeploymentHost has not been started.");
      if (!string.IsNullOrEmpty(this.OperationClass))
        throw new InvalidOperationException("Cannot create a servicing lease manager for operation class " + this.OperationClass + ".");
      return new Microsoft.TeamFoundation.Framework.Server.ServicingLeaseManager((IInternalServicingContext) this);
    }

    internal virtual IVssRequestContext BeginTargetRequestContext() => this.DeploymentRequestContext.GetService<TeamFoundationHostManagementService>().BeginRequest(this.DeploymentRequestContext, this.TargetHostId, this.UseSystemTargetRequestContext ? RequestContextType.SystemContext : RequestContextType.ServicingContext, true, false);

    public event EventHandler<ServicingStepStartedEventArgs> ServicingStepStarted;

    public event EventHandler<ServicingStepGroupStartedEventArgs> ServicingStepGroupStarted;

    public event EventHandler<ServicingOperationStartedEventArgs> ServicingOperationStarted;

    public event EventHandler<ServicingStepEndedEventArgs> ServicingStepEnded;

    public event EventHandler<ServicingStepGroupEndedEventArgs> ServicingStepGroupEnded;

    public event EventHandler<ServicingOperationEndedEventArgs> ServicingOperationEnded;

    TComponent IServicingContext.CreateComponent<TComponent>(int partitionId) => this.CreateComponent<TComponent>(partitionId, 60, 200, 25);

    TComponent IServicingContext.CreateComponent<TComponent>(int partitionId, int commandTimeout) => this.CreateComponent<TComponent>(partitionId, commandTimeout, 200, 25);

    void IInternalServicingContext.StartStepGroup(string name, ServicingStepGroup stepGroup) => this.StartStepGroup(name, stepGroup);

    void IInternalServicingContext.FinishStepGroup() => this.FinishStepGroup();

    void IInternalServicingContext.SkipStep(
      string operationName,
      string groupName,
      string stepName)
    {
      this.SkipStep(operationName, groupName, stepName);
    }

    void IInternalServicingContext.StartStep(string name) => this.StartStep(name);

    ServicingStepState IInternalServicingContext.FinishStep(Exception executionException) => this.FinishStep(executionException);

    void IInternalServicingContext.FinishOperation() => this.FinishOperation();

    void IInternalServicingContext.StartOperation(ServicingOperation servicingOperation) => this.StartOperation(servicingOperation);

    IVssRequestContext IInternalServicingContext.DeploymentRequestContext
    {
      get => this.DeploymentRequestContext;
      set => this.DeploymentRequestContext = value;
    }
  }
}
