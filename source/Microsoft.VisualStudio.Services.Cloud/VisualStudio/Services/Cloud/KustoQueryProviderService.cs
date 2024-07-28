// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.KustoQueryProviderService
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Kusto.Data;
using Kusto.Data.Common;
using Kusto.Data.Exceptions;
using Kusto.Data.Linq;
using Kusto.Ingest.Exceptions;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CircuitBreaker;
using Microsoft.VisualStudio.Services.CloudConfiguration;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public class KustoQueryProviderService : IKustoQueryProviderService, IVssFrameworkService
  {
    private KustoDataContext m_kustoDataContext;
    private KustoQueryProviderSettings m_settings;
    private string m_circuitBreakerCommandKey;
    private bool m_isManagedIdentitiesEnabled;
    internal static readonly int DefaultMaxQueryRetryCount = 1;
    internal static readonly int DefaultSlowQueryThreshold = 15;
    private static readonly TimeSpan s_circuitBreakerTimeout = TimeSpan.FromSeconds(15.0);
    private static readonly CommandPropertiesSetter s_circuitBreakerSettings = new CommandPropertiesSetter().WithExecutionTimeout(KustoQueryProviderService.s_circuitBreakerTimeout);
    private const string s_area = "Kusto";
    private const string s_layer = "KustoQueryProviderService";
    private const string s_kustoTimeoutExceptionMessage = "RequestExecutionTimeout";

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.CheckDeploymentRequestContext();
      try
      {
        IVssRegistryService service = systemRequestContext.GetService<IVssRegistryService>();
        service.RegisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnConnectionUriRegistryChanged), this.RegistryConnectionUri);
        KustoDataContext kustoDataContext = this.CreateKustoDataContext(systemRequestContext);
        if (Interlocked.CompareExchange<KustoDataContext>(ref this.m_kustoDataContext, kustoDataContext, (KustoDataContext) null) != null)
          kustoDataContext.Dispose();
        Interlocked.CompareExchange<string>(ref this.m_circuitBreakerCommandKey, this.BuildCommandKey(systemRequestContext), (string) null);
        service.RegisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnQuerySettingsRegistryChanged), KustoRegistryConstants.AllQuerySettings);
        Interlocked.CompareExchange<KustoQueryProviderSettings>(ref this.m_settings, this.ReadSettings(systemRequestContext), (KustoQueryProviderSettings) null);
      }
      catch (Exception ex)
      {
        systemRequestContext.TraceException(15103000, "Kusto", nameof (KustoQueryProviderService), ex);
        throw new KustoConfigurationException(HostingResources.KustoConfigurationExceptionMessage(), ex);
      }
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      IVssRegistryService service = systemRequestContext.GetService<IVssRegistryService>();
      service.UnregisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnQuerySettingsRegistryChanged));
      service.UnregisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnConnectionUriRegistryChanged));
      if (this.m_kustoDataContext == null)
        return;
      this.m_kustoDataContext.Dispose();
      this.m_kustoDataContext = (KustoDataContext) null;
    }

    public IDataReader ExecuteQuery(IVssRequestContext requestContext, string query)
    {
      if (this.HasFeatureFlagChanged(requestContext))
        this.RefreshKustoDataContext(requestContext);
      return this.ExecuteWithRetries<IDataReader>(requestContext, query, (Func<string, ClientRequestProperties, IDataReader>) ((q, rp) => this.EffectiveKustoDataContext.ExecuteQuery(q, (string) null, rp)));
    }

    public IEnumerable<T> ExecuteQuery<T>(IVssRequestContext requestContext, string query) where T : class
    {
      if (this.HasFeatureFlagChanged(requestContext))
        this.RefreshKustoDataContext(requestContext);
      return this.ExecuteWithRetries<IEnumerable<T>>(requestContext, query, (Func<string, ClientRequestProperties, IEnumerable<T>>) ((q, rp) => this.EffectiveKustoDataContext.ExecuteQuery<T>(q, (string) null, rp)));
    }

    public bool ReadDataReader(IDataReader dataReader)
    {
      try
      {
        return dataReader.Read();
      }
      catch (KustoServicePartialQueryFailureException ex) when (((Exception) ex).Message.Contains("Query is expired"))
      {
        throw new KustoQueryFailedException(HostingResources.KustoQueryExecutionTimeoutExceptionMessage(), (Exception) ex);
      }
    }

    private void OnConnectionUriRegistryChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection registryEntries)
    {
      this.RefreshKustoDataContext(requestContext);
      this.m_circuitBreakerCommandKey = this.BuildCommandKey(requestContext);
    }

    private void OnQuerySettingsRegistryChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection registryEntries)
    {
      Volatile.Write<KustoQueryProviderSettings>(ref this.m_settings, this.ReadSettings(requestContext));
    }

    private void RefreshKustoDataContext(IVssRequestContext requestContext) => Interlocked.Exchange<KustoDataContext>(ref this.m_kustoDataContext, this.CreateKustoDataContext(requestContext))?.Dispose();

    private bool HasFeatureFlagChanged(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("AzureDevOps.Services.ManagedIdentity.UseForKustoAccess") != this.m_isManagedIdentitiesEnabled;

    private KustoDataContext CreateKustoDataContext(IVssRequestContext requestContext)
    {
      string connectionUri = requestContext.GetService<IVssRegistryService>().GetValue<string>(requestContext, (RegistryQuery) this.RegistryConnectionUri, false, (string) null);
      return this.CreateKustoDataContext(requestContext, connectionUri);
    }

    protected virtual string RegistryConnectionUri => KustoRegistryConstants.ConnectionUri;

    protected KustoDataContext CreateKustoDataContext(
      IVssRequestContext requestContext,
      string connectionUri,
      bool overrideStandardCredentials = false)
    {
      Uri uri = new Uri(connectionUri);
      string str1 = uri.Scheme + "://" + uri.Host;
      if (!AzureRoleUtil.IsAvailable)
      {
        string str2 = str1;
        string accessToken = ((AzureTokenProviderBase) new AzureTokenProvider("https://login.microsoftonline.com", AzureRoleUtil.GetOverridableConfigurationSetting("ResourceManagerAadTenantId"), str2, "872cd9fa-d31f-45e0-9eab-6e460a02d1f1", "urn:ietf:wg:oauth:2.0:oob", false, (ITFLogger) null)).GetAuthResult().AccessToken;
        KustoDataContext kustoDataContext = new KustoDataContext(new KustoConnectionStringBuilder(connectionUri).WithAadApplicationTokenAuthentication(accessToken));
        requestContext.TraceAlways(15103010, TraceLevel.Info, "Kusto", nameof (KustoQueryProviderService), "Successfully created the deployment agent Kusto data context.");
        return kustoDataContext;
      }
      this.m_isManagedIdentitiesEnabled = requestContext.IsFeatureEnabled("AzureDevOps.Services.ManagedIdentity.UseForKustoAccess");
      if (this.m_isManagedIdentitiesEnabled)
        return new KustoDataContext(new KustoConnectionStringBuilder(connectionUri).WithAadSystemManagedIdentity());
      KustoConnectionStringBuilder connectionStringBuilder;
      if (!overrideStandardCredentials)
        connectionStringBuilder = new KustoConnectionStringBuilder(connectionUri)
        {
          FederatedSecurity = true,
          ApplicationClientId = AzureRoleUtil.GetOverridableConfigurationSetting(ServicingTokenConstants.RuntimeServicePrincipalClientId),
          ApplicationCertificateThumbprint = AzureRoleUtil.GetOverridableConfigurationSetting("RuntimeServicePrincipalCertThumbprint"),
          Authority = AzureRoleUtil.GetOverridableConfigurationSetting("RuntimeServicePrincipalApplicationTenantId")
        };
      else
        connectionStringBuilder = new KustoConnectionStringBuilder(connectionUri);
      KustoDataContext kustoDataContext1 = new KustoDataContext(connectionStringBuilder);
      requestContext.TraceAlways(15103010, TraceLevel.Info, "Kusto", nameof (KustoQueryProviderService), "Successfully created the RTSP Kusto data context.");
      return kustoDataContext1;
    }

    private string BuildCommandKey(IVssRequestContext requestContext) => this.BuildCommandKey(requestContext, this.m_kustoDataContext);

    protected string BuildCommandKey(
      IVssRequestContext requestContext,
      KustoDataContext kustoDataContext)
    {
      string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}-{1}-{2}", (object) typeof (KustoQueryProviderService).FullName, (object) kustoDataContext.KustoUri.Host, (object) kustoDataContext.DefaultDatabaseName);
      requestContext.TraceAlways(15103020, TraceLevel.Info, "Kusto", nameof (KustoQueryProviderService), "Circuit breaker command key: '{0}'.", (object) str);
      return str;
    }

    protected KustoQueryProviderSettings ReadSettings(IVssRequestContext requestContext)
    {
      RegistryEntryCollection registryEntryCollection = requestContext.GetService<IVssRegistryService>().ReadEntries(requestContext, (RegistryQuery) KustoRegistryConstants.AllQuerySettings);
      KustoQueryProviderSettings providerSettings = new KustoQueryProviderSettings();
      providerSettings.MaxQueryRetryCount = registryEntryCollection.GetValueFromPath<int>(KustoRegistryConstants.MaxQueryRetryCount, KustoQueryProviderService.DefaultMaxQueryRetryCount);
      requestContext.TraceAlways(15103030, TraceLevel.Info, "Kusto", nameof (KustoQueryProviderService), "MaxQueryRetryCount is set to '{0}'.", (object) providerSettings.MaxQueryRetryCount);
      providerSettings.SlowQueryThreshold = registryEntryCollection.GetValueFromPath<int>(KustoRegistryConstants.SlowQueryThreshold, KustoQueryProviderService.DefaultSlowQueryThreshold);
      requestContext.TraceAlways(15103032, TraceLevel.Info, "Kusto", nameof (KustoQueryProviderService), "SlowQueryThreshold is set to '{0}'.", (object) providerSettings.SlowQueryThreshold);
      TimeSpan valueFromPath = registryEntryCollection.GetValueFromPath<TimeSpan>(KustoRegistryConstants.QueryTimeout, this.DefaultQueryTimeout);
      providerSettings.QueryProperties.SetOption("servertimeout", (object) valueFromPath);
      requestContext.TraceAlways(15103034, TraceLevel.Info, "Kusto", nameof (KustoQueryProviderService), "QueryTimeout is set to '{0}'.", (object) valueFromPath);
      return providerSettings;
    }

    private TResult ExecuteWithRetries<TResult>(
      IVssRequestContext requestContext,
      string query,
      Func<string, ClientRequestProperties, TResult> executeQuery,
      int retryCount = 0)
    {
      KustoQueryProviderSettings settings = this.m_settings;
      CommandSetter setter = CommandSetter.WithGroupKey((CommandGroupKey) "Framework.").AndCommandKey((CommandKey) this.EffectiveCircuitBreakerCommandKey).AndCommandPropertiesDefaults(KustoQueryProviderService.s_circuitBreakerSettings);
      CommandService<TResult> commandService = new CommandService<TResult>(requestContext, setter, (Func<TResult>) (() => this.ExecuteAndTrace<TResult>(requestContext, query, executeQuery, settings)));
      try
      {
        return commandService.Execute();
      }
      catch (ObjectDisposedException ex) when (retryCount < settings.MaxQueryRetryCount)
      {
        return this.ExecuteWithRetries<TResult>(requestContext, query, executeQuery, retryCount + 1);
      }
      catch (KustoException ex) when (!ex.IsPermanent && retryCount < settings.MaxQueryRetryCount)
      {
        return this.ExecuteWithRetries<TResult>(requestContext, query, executeQuery, retryCount + 1);
      }
      catch (KustoException ex) when (ex.FailureCode == 504 && ex.FailureSubCode.Equals("RequestExecutionTimeout"))
      {
        throw new KustoQueryFailedException(HostingResources.KustoQueryExecutionTimeoutExceptionMessage(), (Exception) ex);
      }
      catch (KustoServiceTimeoutException ex)
      {
        throw new KustoQueryFailedException(HostingResources.KustoQueryExecutionTimeoutExceptionMessage(), (Exception) ex);
      }
      catch (Exception ex)
      {
        throw new KustoQueryFailedException(HostingResources.KustoQueryFailedExceptionMessage(), ex);
      }
    }

    private TResult ExecuteAndTrace<TResult>(
      IVssRequestContext requestContext,
      string query,
      Func<string, ClientRequestProperties, TResult> executeQuery,
      KustoQueryProviderSettings settings)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(query, nameof (query));
      try
      {
        using (new KustoQueryProviderService.KustoQueryExecutionTracer(requestContext, query, settings.SlowQueryThreshold))
          return executeQuery(query, settings.QueryProperties);
      }
      catch (Exception ex)
      {
        requestContext.Trace(15103040, TraceLevel.Error, "Kusto", nameof (KustoQueryProviderService), "Failed query: '{0}'. Exception: {1}", (object) query, (object) ex);
        throw;
      }
    }

    protected virtual KustoDataContext EffectiveKustoDataContext => this.m_kustoDataContext;

    protected virtual string EffectiveCircuitBreakerCommandKey => this.m_circuitBreakerCommandKey;

    protected virtual TimeSpan DefaultQueryTimeout => TimeSpan.FromSeconds(20.0);

    private struct KustoQueryExecutionTracer : IDisposable
    {
      private IVssRequestContext m_requestContext;
      private string m_query;
      private int m_slowQueryThreshold;
      private PerformanceTimer m_performanceTimer;
      private bool m_shouldDispose;

      public KustoQueryExecutionTracer(
        IVssRequestContext requestContext,
        string query,
        int slowQueryThreshold)
      {
        this.m_requestContext = requestContext;
        this.m_query = query;
        this.m_slowQueryThreshold = slowQueryThreshold;
        this.m_performanceTimer = PerformanceTimer.StartMeasure(requestContext, "KustoQueryExecution");
        this.m_shouldDispose = true;
      }

      public void Dispose()
      {
        if (!this.m_shouldDispose)
          return;
        this.m_performanceTimer.End();
        this.TraceExecutionTime(Convert.ToDouble(this.m_performanceTimer.Duration) / 10000000.0);
        this.m_performanceTimer.Dispose();
        this.m_shouldDispose = false;
      }

      private void TraceExecutionTime(double executionTime) => this.m_requestContext.Trace(15103050, executionTime > (double) this.m_slowQueryThreshold ? TraceLevel.Error : TraceLevel.Info, "Kusto", nameof (KustoQueryProviderService), "The query took {0} seconds to finish: {1}", (object) executionTime, (object) this.m_query);
    }
  }
}
