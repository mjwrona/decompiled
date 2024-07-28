// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Redis.V2.RedisConnection
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CircuitBreaker;
using Microsoft.VisualStudio.Services.Common;
using StackExchange.Redis;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Authentication;
using System.Threading;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.Redis.V2
{
  internal class RedisConnection : IRedisConnection, IDisposable, IRedisConnectionInternal
  {
    private static readonly 
    #nullable disable
    BlockingCollection<Tuple<Action, string>> s_pendingConnectionManagers = new BlockingCollection<Tuple<Action, string>>();
    private static Task s_pendingConnectionManagerTask = (Task) null;
    private const string s_area = "Redis";
    private const string s_layer = "RedisConnection";
    private static readonly CommandPropertiesSetter s_defaultCircuitbreakerProperties = new CommandPropertiesSetter().WithCircuitBreakerDisabled(false).WithCircuitBreakerForceClosed(false).WithCircuitBreakerForceOpen(false).WithFallbackDisabled(false).WithCircuitBreakerRequestVolumeThreshold(20).WithCircuitBreakerErrorThresholdPercentage(5).WithCircuitBreakerMinBackoff(TimeSpan.FromSeconds(0.0)).WithCircuitBreakerMaxBackoff(TimeSpan.FromSeconds(30.0)).WithCircuitBreakerDeltaBackoff(TimeSpan.FromMilliseconds(300.0)).WithExecutionTimeout(TimeSpan.FromMilliseconds(100.0)).WithMetricsHealthSnapshotInterval(TimeSpan.FromSeconds(0.5)).WithMetricsRollingStatisticalWindowInMilliseconds(60000).WithMetricsRollingStatisticalWindowBuckets(60);
    private static readonly int? s_defaultConnectRetry = new int?(3);
    private static readonly int? s_defaultConnectTimeout = new int?((int) TimeSpan.FromSeconds(10.0).TotalMilliseconds);
    private static readonly int? s_defaultRequestTimeout = new int?((int) TimeSpan.FromMilliseconds(500.0).TotalMilliseconds);
    private static readonly int s_defaultReconnectDeltaBackoff = (int) TimeSpan.FromSeconds(5.0).TotalMilliseconds;
    private static readonly int s_defaultReconnectMaxBackoff = (int) TimeSpan.FromMinutes(1.0).TotalMilliseconds;
    private static readonly double s_defaultBatchingRadixBackoff = 1.0;
    private static readonly bool s_defaultUseDefaultBacklogPolicy = false;
    private static readonly int defaultMessageSizeHardCeiling = 102400;
    private static readonly TimeSpan s_minBackoff = TimeSpan.FromMilliseconds(10.0);
    private static readonly TimeSpan s_deltaBackoff = TimeSpan.FromMilliseconds(100.0);
    private static readonly TimeSpan s_maxBackoff = TimeSpan.FromSeconds(1.0);
    private readonly RedisConfiguration m_configuration;
    private readonly Microsoft.VisualStudio.Services.Redis.Tracer m_tracer;
    private readonly RedisReconnectSync m_reconnectSync;
    private readonly string m_poolName;
    private readonly string m_requestShortCircuitKey;
    private readonly ConnectionPoolSettings m_poolSettings;
    private volatile RedisConnection.ConnectionManager m_connectionManager;
    private readonly ConcurrentDictionary<RedisChannel, Action<RedisChannel, RedisValue>> m_subscriptions;
    private int m_circuitBreakerAttemptingReconnect;
    private readonly object m_lock = new object();
    private int m_refCount = 1;
    private int m_reconnections;
    private const string c_enableReconnectDelayFeature = "VisualStudio.FrameworkService.RedisCache.EnableReconnectDelayOnCircuitBreakerTripping";
    private const string c_disableReconnectFeature = "VisualStudio.FrameworkService.RedisCache.DisableReconnectOnCircuitBreakerTripping";
    private const string c_enforceHardMessageSizeCeiling = "VisualStudio.FrameworkService.RedisCache.EnforceHardMessageSizeCeiling";
    private const string c_hardMessageSizeCeilingInBytes = "/Configuration/Caching/Redis/hardMessageSizeCeilingInBytes";
    private const string c_traceRedisExceptionStack = "VisualStudio.FrameworkService.RedisCache.TraceRedisExceptionStack";
    private const string c_connectionPoolScopedCircuitBreaker = "VisualStudio.FrameworkService.RedisCache.ConnectionPoolScopedCircuitBreaker";

    protected RedisConnection(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Redis.Tracer tracer,
      RedisReconnectSync sync,
      RedisConfiguration configuration,
      string poolName,
      string connectionId,
      ConnectionPoolSettings poolSettings)
      : this(requestContext, tracer, sync, configuration, poolName, connectionId, poolSettings, RedisConnection.CreateInitialConnectionManager(requestContext, tracer, configuration, poolName, connectionId, poolSettings))
    {
      if (RedisConnection.s_pendingConnectionManagerTask != null)
        return;
      lock (RedisConnection.s_pendingConnectionManagers)
      {
        if (RedisConnection.s_pendingConnectionManagerTask != null)
          return;
        RedisConnection.s_pendingConnectionManagerTask = Task.Run((Action) (() => RedisConnection.EventProcessor()));
      }
    }

    protected RedisConnection(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Redis.Tracer tracer,
      RedisReconnectSync sync,
      RedisConfiguration configuration,
      string poolName,
      string connectionId,
      ConnectionPoolSettings poolSettings,
      RedisConnection.ConnectionManager initialConnectionManager)
    {
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        throw new UnexpectedHostTypeException(requestContext.ServiceHost.HostType);
      this.Id = connectionId;
      this.m_configuration = configuration;
      this.m_poolName = poolName;
      this.m_poolSettings = poolSettings;
      this.m_tracer = tracer;
      this.m_reconnectSync = sync;
      this.m_subscriptions = new ConcurrentDictionary<RedisChannel, Action<RedisChannel, RedisValue>>();
      this.m_connectionManager = initialConnectionManager;
      this.m_requestShortCircuitKey = RequestContextItemsKeys.RedisException + "/" + poolName;
    }

    public static RedisConnection Create(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Redis.Tracer tracer,
      RedisReconnectSync sync,
      RedisConfiguration configuration,
      string poolName,
      string connectionId,
      ConnectionPoolSettings poolSettings)
    {
      return new RedisConnection(requestContext, tracer, sync, configuration, poolName, connectionId, poolSettings);
    }

    public string Id { get; private set; }

    public override string ToString() => string.Format("[Id={0}, Mux={1}]", (object) this.Id, (object) this.m_connectionManager);

    public bool IsValid(IVssRequestContext requestContext)
    {
      using (RedisConnection.ConnectionManager connectionManager = this.m_connectionManager.AddRef())
        return connectionManager.IsConnected && !connectionManager.IsDisposed;
    }

    public void Call(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Redis.Tracer tracer,
      Action<IRedisDatabase> action)
    {
      using (RedisConnection.ConnectionManager connectionManager = this.m_connectionManager.AddRef())
        this.Call(requestContext, tracer, connectionManager.Settings, (Action) (() => action((IRedisDatabase) connectionManager.Database)));
    }

    public void Call(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Redis.Tracer tracer, Action<IServer> action)
    {
      using (RedisConnection.ConnectionManager connectionManager = this.m_connectionManager.AddRef())
        this.Call(requestContext, tracer, connectionManager.Settings, (Action) (() =>
        {
          ConnectionMultiplexer mux = connectionManager.Mux;
          action(mux.GetServer(((IEnumerable<EndPoint>) mux.GetEndPoints(true)).First<EndPoint>(), (object) null));
        }));
    }

    public void Call(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Redis.Tracer tracer,
      Action<IServer, IDatabase> action)
    {
      using (RedisConnection.ConnectionManager connectionManager = this.m_connectionManager.AddRef())
        this.Call(requestContext, tracer, connectionManager.Settings, (Action) (() =>
        {
          ConnectionMultiplexer mux = connectionManager.Mux;
          action(mux.GetServer(((IEnumerable<EndPoint>) mux.GetEndPoints(true)).First<EndPoint>(), (object) null), mux.GetDatabase(-1, (object) null));
        }));
    }

    public T Call<T>(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Redis.Tracer tracer,
      Func<IRedisDatabase, T> Run,
      Func<bool, Command<T>, T> fallback = null)
    {
      CommandService<T> command = (CommandService<T>) null;
      try
      {
        using (RedisConnection.ConnectionManager connectionManager = this.m_connectionManager.AddRef())
        {
          if (this.ShouldShortCircuitRequest(requestContext, connectionManager))
            return fallback != null ? fallback(false, (Command<T>) null) : default (T);
          Func<T> run = (Func<T>) (() =>
          {
            T result = default (T);
            this.Call(requestContext, tracer, connectionManager.Settings, (Action) (() => result = Run((IRedisDatabase) connectionManager.Database)));
            return result;
          });
          Func<T> fallback1 = (Func<T>) null;
          if (fallback != null)
            fallback1 = (Func<T>) (() => fallback(true, (Command<T>) command));
          CommandSetter commandSetter = this.GetCommandSetter(requestContext);
          command = new CommandService<T>(requestContext, commandSetter, run, fallback1);
          return ((Command<T>) command).Execute();
        }
      }
      finally
      {
        if (command != null)
          this.CheckCircuitBreakerStatusAndPossiblyReconnect(requestContext, command.IsCircuitBreakerOpen);
      }
    }

    public async Task CallAsync(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Redis.Tracer tracer,
      Func<IRedisDatabase, Task> action)
    {
      using (RedisConnection.ConnectionManager connectionManager = this.m_connectionManager.AddRef())
        await this.CallAsync(requestContext, tracer, connectionManager.Settings, (Func<Task>) (() => action((IRedisDatabase) connectionManager.Database)));
    }

    public async Task<T> CallAsync<T>(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Redis.Tracer tracer,
      Func<IRedisDatabase, Task<T>> run,
      Func<bool, CommandAsync<T>, Task<T>> fallback = null)
    {
      CommandServiceAsync<T> command = (CommandServiceAsync<T>) null;
      try
      {
        using (RedisConnection.ConnectionManager connectionManager = this.m_connectionManager.AddRef())
        {
          if (this.ShouldShortCircuitRequest(requestContext, connectionManager))
            return fallback != null ? await fallback(false, (CommandAsync<T>) null) : default (T);
          Func<Task<T>> run1 = (Func<Task<T>>) (async () =>
          {
            T result = default (T);
            await this.CallAsync(requestContext, tracer, connectionManager.Settings, (Func<Task>) (async () => result = await run((IRedisDatabase) connectionManager.Database)));
            return result;
          });
          Func<Task<T>> fallback1 = (Func<Task<T>>) null;
          if (fallback != null)
            fallback1 = (Func<Task<T>>) (() => fallback(true, (CommandAsync<T>) command));
          command = new CommandServiceAsync<T>(requestContext, this.GetCommandSetter(requestContext), run1, fallback1, true);
          return await command.Execute();
        }
      }
      finally
      {
        if (command != null)
          this.CheckCircuitBreakerStatusAndPossiblyReconnect(requestContext, command.IsCircuitBreakerOpen);
      }
    }

    private void CheckCircuitBreakerStatusAndPossiblyReconnect(
      IVssRequestContext requestContext,
      bool isCircuitBreakerOpen)
    {
      if (requestContext.IsFeatureEnabled("VisualStudio.FrameworkService.RedisCache.DisableReconnectOnCircuitBreakerTripping"))
        return;
      if (isCircuitBreakerOpen)
      {
        if ((!requestContext.IsFeatureEnabled("VisualStudio.FrameworkService.RedisCache.EnableReconnectDelayOnCircuitBreakerTripping") || this.m_reconnectSync.ShouldReconnect()) && Interlocked.CompareExchange(ref this.m_circuitBreakerAttemptingReconnect, 1, 0) == 0)
        {
          this.m_tracer.RedisError(requestContext, string.Format("{0}: CircuitBreaker tripped, reconnecting to Redis", (object) this));
          this.InitiateReconnect(requestContext);
        }
        else if (this.m_circuitBreakerAttemptingReconnect == 1)
        {
          this.m_tracer.RedisError(requestContext, string.Format("{0}: CircuitBreaker tripped, reconnection already initiated by a different thread", (object) this));
        }
        else
        {
          if (!requestContext.IsFeatureEnabled("VisualStudio.FrameworkService.RedisCache.EnableReconnectDelayOnCircuitBreakerTripping"))
            return;
          this.m_tracer.RedisError(requestContext, string.Format("{0}: CircuitBreaker tripped, but reconnection cannot be initated until {1} has passed since last reconnection, which happened: {2}", (object) this, (object) this.m_reconnectSync.ReconnectionPeriod, (object) this.m_reconnectSync.LastReconnectionAttempt));
        }
      }
      else
      {
        if (Interlocked.CompareExchange(ref this.m_circuitBreakerAttemptingReconnect, 0, 1) != 1)
          return;
        this.m_tracer.RedisError(requestContext, string.Format("{0}: CircuitBreaker was previously tripped, but the connection to Redis has been reestablished", (object) this));
      }
    }

    private bool ShouldShortCircuitRequest(
      IVssRequestContext requestContext,
      RedisConnection.ConnectionManager connectionManager)
    {
      if (connectionManager.Settings == null || !this.m_configuration.IsRedisEnabled(requestContext))
        return true;
      RedisConnection.ExceptionInfo exceptionInfo;
      if (!requestContext.Items.TryGetValue<RedisConnection.ExceptionInfo>(this.m_requestShortCircuitKey, out exceptionInfo) || connectionManager.Settings.MaxFailuresPerRequest <= 0 || exceptionInfo.TotalExceptions < connectionManager.Settings.MaxFailuresPerRequest)
        return false;
      requestContext.Trace(8110001, TraceLevel.Warning, "Redis", nameof (RedisConnection), "Short-circuiting Redis call due to recurrent exceptions, threshold={0}", (object) connectionManager.Settings.MaxFailuresPerRequest);
      return true;
    }

    private void RecordRedisException(IVssRequestContext requestContext, Exception ex)
    {
      if (ex is RedisConfigurationException || ex is RedisOversizeMessageException)
        return;
      RedisConnection.ExceptionInfo exceptionInfo;
      if (!requestContext.Items.TryGetValue<RedisConnection.ExceptionInfo>(this.m_requestShortCircuitKey, out exceptionInfo))
      {
        exceptionInfo = new RedisConnection.ExceptionInfo();
        requestContext.Items[this.m_requestShortCircuitKey] = (object) exceptionInfo;
      }
      ++exceptionInfo.TotalExceptions;
      exceptionInfo.LastException = ex;
    }

    private CommandSetter GetCommandSetter(IVssRequestContext requestContext)
    {
      string name = this.m_configuration.CircuitBreakerKey.Name;
      ICommandProperties values = (ICommandProperties) new CommandPropertiesRegistry(requestContext, (CommandKey) name, RedisConnection.s_defaultCircuitbreakerProperties);
      if (requestContext.IsFeatureEnabled("VisualStudio.FrameworkService.RedisCache.ConnectionPoolScopedCircuitBreaker"))
      {
        name += this.m_poolName;
        values = (ICommandProperties) new CommandPropertiesRegistry(requestContext, (CommandKey) name, new CommandPropertiesSetter(values));
      }
      return CommandSetter.WithGroupKey((CommandGroupKey) "Framework.").AndCommandKey((CommandKey) name).AndCommandPropertiesDefaults(new CommandPropertiesSetter(values));
    }

    internal void AcquireConnection()
    {
      lock (this.m_lock)
      {
        if (this.m_refCount <= 0)
          return;
        ++this.m_refCount;
      }
    }

    public void Dispose()
    {
      try
      {
        lock (this.m_lock)
        {
          if (this.m_refCount == 0)
            return;
          if (--this.m_refCount > 0)
            return;
        }
        this.m_connectionManager.Dispose();
      }
      catch (Exception ex)
      {
        Microsoft.VisualStudio.Services.Redis.Tracer.RedisError(ex);
      }
    }

    public void Subscribe(
      IVssRequestContext requestContext,
      RedisChannel channel,
      Action<RedisChannel, RedisValue> handler)
    {
      if (!this.m_subscriptions.TryAdd(channel, handler))
        throw new RedisException(string.Format("Subscription already exists for {0} on connection {1}", (object) channel, (object) this.Id));
      bool flag = false;
      try
      {
        using (RedisConnection.ConnectionManager connectionManager = this.m_connectionManager.AddRef())
        {
          connectionManager.Subscriber.Subscribe(channel, handler, (CommandFlags) 0);
          flag = true;
        }
      }
      finally
      {
        if (!flag)
          this.m_subscriptions.TryRemove(channel, out Action<RedisChannel, RedisValue> _);
      }
    }

    public void Unsubscribe(IVssRequestContext requestContext, RedisChannel channel)
    {
      Action<RedisChannel, RedisValue> action = (Action<RedisChannel, RedisValue>) null;
      if (!this.m_subscriptions.TryRemove(channel, out action))
        return;
      using (RedisConnection.ConnectionManager connectionManager = this.m_connectionManager.AddRef())
        connectionManager.Subscriber.Unsubscribe(channel, action, (CommandFlags) 0);
    }

    private void Call(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Redis.Tracer tracer,
      ConnectionSettings settings,
      Action action)
    {
      int num1 = Math.Max((settings != null ? settings.RetryCount : 0) + 1, 1);
      int num2 = 1;
      while (true)
      {
        try
        {
          using (tracer.TimedCall(requestContext))
          {
            action();
            break;
          }
        }
        catch (Exception ex1)
        {
          Exception ex2;
          switch (ex1)
          {
            case RedisException redisException:
              redisException.SetMessage(this.FormatError(requestContext, ex1, settings));
              ex2 = (Exception) redisException;
              break;
            case TimeoutException _:
              ex2 = (Exception) new RedisTimeoutException(this.FormatError(requestContext, ex1, settings));
              break;
            default:
              ex2 = (Exception) new RedisException(this.FormatError(requestContext, ex1, settings));
              break;
          }
          tracer.RedisError(requestContext, ex2);
          if (num2 < num1)
          {
            TimeSpan exponentialBackoff = BackoffTimerHelper.GetExponentialBackoff(num2 - 1, RedisConnection.s_minBackoff, RedisConnection.s_maxBackoff, RedisConnection.s_deltaBackoff);
            tracer.RedisError(requestContext, "Attempt {0}/{1} failed, sleeping for {2} and retrying the call", (object) num2, (object) num1, (object) exponentialBackoff);
            Thread.Sleep(exponentialBackoff);
          }
          else
          {
            this.RecordRedisException(requestContext, ex1);
            throw new RedisException("Redis cache is not available, try again later", ex1);
          }
        }
        ++num2;
      }
    }

    private async Task CallAsync(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Redis.Tracer tracer,
      ConnectionSettings settings,
      Func<Task> action)
    {
      ConnectionSettings connectionSettings = settings;
      int maxAttempts = Math.Max((connectionSettings != null ? connectionSettings.RetryCount : 0) + 1, 1);
      int attempt = 1;
      Exception exception;
      while (true)
      {
        int num;
        try
        {
          using (tracer.TimedCall(requestContext))
            await action();
          goto label_20;
        }
        catch (Exception ex)
        {
          num = 1;
        }
        if (num == 1)
        {
          exception = ex;
          Exception ex;
          switch (exception)
          {
            case RedisException redisException:
              redisException.SetMessage(this.FormatError(requestContext, exception, settings));
              ex = (Exception) redisException;
              break;
            case TimeoutException _:
              ex = (Exception) new RedisTimeoutException(this.FormatError(requestContext, exception, settings));
              break;
            default:
              ex = (Exception) new RedisException(this.FormatError(requestContext, exception, settings));
              break;
          }
          tracer.RedisError(requestContext, ex);
          if (attempt < maxAttempts && this.ShouldRetry(exception))
          {
            TimeSpan exponentialBackoff = BackoffTimerHelper.GetExponentialBackoff(attempt - 1, RedisConnection.s_minBackoff, RedisConnection.s_maxBackoff, RedisConnection.s_deltaBackoff);
            tracer.RedisError(requestContext, "Attempt {0}/{1} failed, sleeping for {2} and retrying the call", (object) attempt, (object) maxAttempts, (object) exponentialBackoff);
            await Task.Delay(exponentialBackoff, requestContext.CancellationToken);
          }
          else
            break;
        }
        ++attempt;
      }
      this.RecordRedisException(requestContext, exception);
      throw new RedisException("Redis cache is not available, try again later", exception);
label_20:;
    }

    private bool ShouldRetry(Exception ex)
    {
      switch (ex)
      {
        case RedisConfigurationException _:
        case RedisOversizeMessageException _:
          return false;
        default:
          return true;
      }
    }

    private string FormatError(
      IVssRequestContext requestContext,
      Exception exception,
      ConnectionSettings connectionSettings)
    {
      string str1 = "(null)";
      bool flag = false;
      if (connectionSettings?.ConfigurationOptions != null)
      {
        str1 = string.Join(",", ((IEnumerable<EndPoint>) connectionSettings.ConfigurationOptions.EndPoints).Select<EndPoint, string>((Func<EndPoint, string>) (endpoint => EndPointCollection.ToString(endpoint))));
        flag = !string.IsNullOrWhiteSpace(connectionSettings.ConfigurationOptions.Password);
      }
      string str2 = string.Format("Failed to contact Redis server(s), connection={0}, endpoint={1}, exception={2}, reason={3}, passwordProvided={4}", (object) this, (object) str1, (object) exception.GetType().FullName, (object) exception.Message, (object) flag);
      if (requestContext.IsFeatureEnabled("VisualStudio.FrameworkService.RedisCache.TraceRedisExceptionStack"))
        str2 = str2 + ", stack=" + EnvironmentWrapper.ToReadableStackTrace();
      return str2;
    }

    private void RestoreSubscriptions(
      RedisConnection.ConnectionManager connectionManager,
      List<KeyValuePair<RedisChannel, Action<RedisChannel, RedisValue>>> subscriptions)
    {
      if (connectionManager.IsDisposed || !connectionManager.IsConnected || subscriptions == null || subscriptions.Count == 0)
        return;
      List<KeyValuePair<RedisChannel, Action<RedisChannel, RedisValue>>> failedSubscriptions = new List<KeyValuePair<RedisChannel, Action<RedisChannel, RedisValue>>>();
      foreach (KeyValuePair<RedisChannel, Action<RedisChannel, RedisValue>> subscription in subscriptions)
      {
        Action<RedisChannel, RedisValue> action;
        if (this.m_subscriptions.TryGetValue(subscription.Key, out action) && action.Target == subscription.Value.Target)
        {
          if (!(action.Method != subscription.Value.Method))
          {
            try
            {
              connectionManager.Subscriber.Subscribe(subscription.Key, subscription.Value, (CommandFlags) 0);
            }
            catch (Exception ex)
            {
              failedSubscriptions.Add(subscription);
              TeamFoundationTracingService.TraceRaw(8110010, TraceLevel.Error, "Redis", nameof (RedisConnection), string.Format("Encountered error while automatically re-establishing subscription for channel {0}: {1}", (object) subscription.Key, (object) ex.ToReadableStackTrace()));
            }
          }
        }
      }
      if (failedSubscriptions.Count <= 0)
        return;
      Task.Delay(TimeSpan.FromSeconds(5.0)).ContinueWith((Action<Task>) (_ => this.RestoreSubscriptions(connectionManager, failedSubscriptions)));
    }

    private static ConnectionSettings GetConnectionSettings(
      IVssRequestContext requestContext,
      RedisConfiguration configuration,
      string poolName,
      ConnectionPoolSettings poolSettings)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      string str1 = vssRequestContext.GetService<IVssRegistryService>().GetValue<string>(vssRequestContext, (RegistryQuery) configuration.Keys.ConnectionString, (string) null);
      if (str1 == null)
        throw new RedisConfigurationException("Redis connection string is not provided in " + configuration.Keys.ConnectionString + " Registry key");
      if (string.IsNullOrWhiteSpace(str1))
        return (ConnectionSettings) null;
      ConfigurationOptions configurationOptions1 = ConfigurationOptions.Parse(str1);
      configurationOptions1.AllowAdmin = true;
      configurationOptions1.AbortOnConnectFail = false;
      ConfigurationOptions configurationOptions2 = configurationOptions1;
      IVssRequestContext requestContext1 = requestContext;
      string connectionRetries = configuration.Keys.ConnectionRetries;
      int? nullable = RedisConnection.s_defaultConnectRetry;
      int defaultValue1 = nullable ?? configurationOptions1.ConnectRetry;
      int registryValue1 = RedisConfiguration.GetRegistryValue<int>(requestContext1, connectionRetries, defaultValue1);
      configurationOptions2.ConnectRetry = registryValue1;
      ConfigurationOptions configurationOptions3 = configurationOptions1;
      IVssRequestContext requestContext2 = requestContext;
      string connectionTimeout = configuration.Keys.ConnectionTimeout;
      nullable = RedisConnection.s_defaultConnectTimeout;
      int defaultValue2 = nullable ?? configurationOptions1.ConnectTimeout;
      int registryValue2 = RedisConfiguration.GetRegistryValue<int>(requestContext2, connectionTimeout, defaultValue2);
      configurationOptions3.ConnectTimeout = registryValue2;
      ConfigurationOptions configurationOptions4 = configurationOptions1;
      IVssRequestContext requestContext3 = requestContext;
      string requestTimeout = configuration.Keys.RequestTimeout;
      nullable = RedisConnection.s_defaultRequestTimeout;
      int defaultValue3 = nullable ?? configurationOptions1.SyncTimeout;
      int registryValue3 = RedisConfiguration.GetRegistryValue<int>(requestContext3, requestTimeout, defaultValue3);
      configurationOptions4.SyncTimeout = registryValue3;
      if (((Collection<EndPoint>) configurationOptions1.EndPoints).Count != 1)
        requestContext.Trace(8110003, TraceLevel.Error, "Redis", nameof (RedisConnection), "Expected single Redis endpoint, found {0} endpoints. All endpoints except the first one will be ignored", (object) ((Collection<EndPoint>) configurationOptions1.EndPoints).Count);
      ITeamFoundationStrongBoxService service = vssRequestContext.GetService<ITeamFoundationStrongBoxService>();
      IVssRequestContext requestContext4 = vssRequestContext.Elevate();
      StrongBoxItemInfo itemInfo = service.GetItemInfo(requestContext4, configuration.StrongBoxDrawerName, configuration.StrongBoxCachingServiceKey, false);
      if (itemInfo != null)
      {
        string str2 = service.GetString(requestContext4, itemInfo);
        if (!string.IsNullOrWhiteSpace(str2))
          configurationOptions1.Password = str2;
      }
      if (configurationOptions1.Ssl)
      {
        if (string.IsNullOrWhiteSpace(configurationOptions1.Password))
          throw new RedisConfigurationException("Password has to be provided for SSL-enabled Redis connection");
        if (!configurationOptions1.SslProtocols.HasValue)
          configurationOptions1.SslProtocols = new SslProtocols?(SslProtocols.Tls12);
      }
      configurationOptions1.ClientName = Environment.MachineName;
      configurationOptions1.TieBreaker = "";
      int registryValue4 = RedisConfiguration.GetRegistryValue<int>(requestContext, configuration.Keys.ReconnectDeltaBackoff, RedisConnection.s_defaultReconnectDeltaBackoff);
      int registryValue5 = RedisConfiguration.GetRegistryValue<int>(requestContext, configuration.Keys.ReconnectMaxBackoff, RedisConnection.s_defaultReconnectMaxBackoff);
      configurationOptions1.ReconnectRetryPolicy = (IReconnectRetryPolicy) new ExponentialRetry(registryValue4, registryValue5);
      double registryValue6 = RedisConfiguration.GetRegistryValue<double>(requestContext, configuration.Keys.BatchingRadixBackoff, RedisConnection.s_defaultBatchingRadixBackoff);
      if (!RedisConfiguration.GetRegistryValue<bool>(requestContext, configuration.Keys.NeedsPubSub(poolName), poolSettings.NeedsPubSub))
        configurationOptions1.CommandMap = CommandMap.Create(new HashSet<string>()
        {
          "SUBSCRIBE"
        }, false);
      if (!RedisConfiguration.GetRegistryValue<bool>(requestContext, configuration.Keys.UseDefaultBacklogPolicy, RedisConnection.s_defaultUseDefaultBacklogPolicy))
        configurationOptions1.BacklogPolicy = BacklogPolicy.FailFast;
      int maxMessageSize = RedisConnection.GetMaxMessageSize(vssRequestContext, configuration, poolName, poolSettings);
      int registryValue7 = RedisConfiguration.GetRegistryValue<int>(requestContext, configuration.Keys.RetryCount(poolName), poolSettings.RetryCount);
      int registryValue8 = RedisConfiguration.GetRegistryValue<int>(requestContext, configuration.Keys.MaxFailuresPerRequest(poolName), poolSettings.MaxFailuresPerRequest);
      return new ConnectionSettings(configurationOptions1, maxMessageSize, registryValue7, registryValue8, registryValue6);
    }

    private static int GetMaxMessageSize(
      IVssRequestContext requestContext,
      RedisConfiguration configuration,
      string poolName,
      ConnectionPoolSettings poolSettings)
    {
      int maxMessageSize = RedisConfiguration.GetRegistryValue<int>(requestContext, configuration.Keys.MaxMessageSize(poolName), poolSettings.MaxMessageSize);
      if (requestContext.IsFeatureEnabled("VisualStudio.FrameworkService.RedisCache.EnforceHardMessageSizeCeiling"))
      {
        int num = requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Configuration/Caching/Redis/hardMessageSizeCeilingInBytes", RedisConnection.defaultMessageSizeHardCeiling);
        if (maxMessageSize <= 0 || maxMessageSize > num)
          maxMessageSize = num;
      }
      return maxMessageSize;
    }

    protected static RedisConnection.ConnectionManager CreateInitialConnectionManager(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Redis.Tracer tracer,
      RedisConfiguration configuration,
      string poolName,
      string connectionId,
      ConnectionPoolSettings poolSettings,
      Action<RedisConnection.ConnectionManager> onCompleted = null)
    {
      ConnectionSettings configurationSettings = (ConnectionSettings) null;
      try
      {
        configurationSettings = RedisConnection.GetConnectionSettings(requestContext, configuration, poolName, poolSettings);
      }
      catch (Exception ex)
      {
        requestContext.Trace(8110013, TraceLevel.Error, "Redis", nameof (RedisConnection), "Failed to retrieve connection settings, exception={0}", (object) ex);
        tracer.RedisError(requestContext, ex);
      }
      return new RedisConnection.ConnectionManager(configurationSettings, poolSettings.DatabaseId, string.Format("{0}:0:{1}", (object) connectionId, (object) DateTime.UtcNow), onCompleted);
    }

    public void UpdateSettings(IVssRequestContext requestContext)
    {
      try
      {
        if (this.m_connectionManager.NeedsReconnect(RedisConnection.GetConnectionSettings(requestContext, this.m_configuration, this.m_poolName, this.m_poolSettings)))
        {
          this.m_tracer.RedisError(requestContext, string.Format("{0}: Redis configuration updated, switching to the new one", (object) this));
          this.InitiateReconnect(requestContext);
        }
        else
          this.m_tracer.RedisError(requestContext, string.Format("{0}: Redis configuration updated, keeping current one as no changes detected", (object) this));
      }
      catch (Exception ex)
      {
        this.m_tracer.RedisError(requestContext, ex);
      }
    }

    protected virtual RedisConnection.ConnectionManager InitiateReconnect(
      IVssRequestContext requestContext)
    {
      ConnectionSettings connectionSettings;
      try
      {
        connectionSettings = RedisConnection.GetConnectionSettings(requestContext, this.m_configuration, this.m_poolName, this.m_poolSettings);
      }
      catch (Exception ex)
      {
        requestContext.Trace(8110013, TraceLevel.Error, "Redis", nameof (RedisConnection), "Failed to retrieve connection settings, exception={0}", (object) ex);
        this.m_tracer.RedisError(requestContext, ex);
        return (RedisConnection.ConnectionManager) null;
      }
      RedisConnection.ConnectionManager connectionManager = (RedisConnection.ConnectionManager) null;
      try
      {
        ++this.m_reconnections;
        connectionManager = new RedisConnection.ConnectionManager(connectionSettings, this.m_poolSettings.DatabaseId, string.Format("{0}:{1}:{2}", (object) this.Id, (object) this.m_reconnections, (object) DateTime.UtcNow), new Action<RedisConnection.ConnectionManager>(this.Publish));
      }
      catch (Exception ex)
      {
        requestContext.Trace(8110013, TraceLevel.Error, "Redis", nameof (RedisConnection), "Failed to create instance of ConnectionManager object, exception={0}", (object) ex);
        this.m_tracer.RedisError(requestContext, ex);
      }
      requestContext.TraceAlways(8110014, TraceLevel.Info, "Redis", nameof (RedisConnection), string.Format("Created new connection manager: {0}", (object) connectionManager));
      return connectionManager;
    }

    private void Publish(
      RedisConnection.ConnectionManager connectionManager)
    {
      RedisConnection.QueueWorkItem((Action) (() => this.Swap(connectionManager)), string.Format("Activating connection manager for {0}", (object) this));
    }

    private void Swap(
      RedisConnection.ConnectionManager newConnectionManager)
    {
      RedisConnection.ConnectionManager connectionManager = (RedisConnection.ConnectionManager) null;
      lock (this.m_lock)
      {
        if (this.m_refCount > 0)
        {
          TeamFoundationTracingService.TraceRawAlwaysOn(8110012, TraceLevel.Info, "Redis", nameof (RedisConnection), "Activating new multiplexer for connection {0}", (object) this);
          connectionManager = this.m_connectionManager;
          this.m_connectionManager = newConnectionManager;
        }
      }
      if (connectionManager != null)
      {
        this.RestoreSubscriptions(newConnectionManager, this.m_subscriptions.ToList<KeyValuePair<RedisChannel, Action<RedisChannel, RedisValue>>>());
        connectionManager.Dispose();
      }
      else
        newConnectionManager.Dispose();
    }

    private static void EventProcessor()
    {
      TeamFoundationTracingService.TraceRawAlwaysOn(8110012, TraceLevel.Info, "Redis", nameof (RedisConnection), "PendingConnectionManagerProcessor starting");
      Stopwatch stopwatch = new Stopwatch();
      foreach (Tuple<Action, string> consuming in RedisConnection.s_pendingConnectionManagers.GetConsumingEnumerable())
      {
        try
        {
          TeamFoundationTracingService.TraceRawAlwaysOn(8110012, TraceLevel.Info, "Redis", nameof (RedisConnection), "Starting processing action '{0}'", (object) consuming.Item2);
          stopwatch.Restart();
          consuming.Item1();
          stopwatch.Stop();
          TeamFoundationTracingService.TraceRawAlwaysOn(8110012, TraceLevel.Info, "Redis", nameof (RedisConnection), "Finished processing action '{0}', elapsed={1} ", (object) consuming.Item2, (object) stopwatch.Elapsed);
        }
        catch (Exception ex)
        {
          TeamFoundationTracingService.TraceExceptionRaw(8110011, TraceLevel.Error, "Redis", nameof (RedisConnection), ex);
        }
      }
      TeamFoundationTracingService.TraceRawAlwaysOn(8110012, TraceLevel.Info, "Redis", nameof (RedisConnection), "PendingConnectionManagerProcessor exiting");
    }

    private static void QueueWorkItem(Action action, string message) => RedisConnection.s_pendingConnectionManagers.Add(Tuple.Create<Action, string>(action, message));

    internal class ConnectionManager : IDisposable
    {
      private readonly string m_id;
      private readonly Task m_connectTask;
      private ConnectionMultiplexer m_mux;
      private VssRedisDatabase m_database;
      private ISubscriber m_subscriber;
      private readonly object m_lock = new object();
      private string m_disposeStack;
      private int m_refCount = 1;

      public ConnectionManager(
        ConnectionSettings configurationSettings,
        int databaseId,
        string connectionManagerId,
        Action<RedisConnection.ConnectionManager> onConnected)
      {
        RedisConnection.ConnectionManager connectionManager = this;
        this.Settings = configurationSettings;
        this.m_id = connectionManagerId;
        if (this.Settings != null)
        {
          StringWriter log = new StringWriter();
          this.m_connectTask = ConnectionMultiplexer.ConnectAsync(this.Settings.ConfigurationOptions, (TextWriter) log).ContinueWith<Task>((Func<Task<ConnectionMultiplexer>, Task>) (async antecedant =>
          {
            string str;
            try
            {
              str = SecretUtility.ScrubSecrets(log.ToString(), false);
            }
            catch
            {
              str = "<Unavailable>";
            }
            TeamFoundationTracingService.TraceRawAlwaysOn(8110006, TraceLevel.Info, "Redis", nameof (RedisConnection), "ConnectionMultiplexerCreated: id=" + closure_0.m_id + ", log=" + str);
            try
            {
              ConnectionMultiplexer mux = antecedant.Result;
              mux.IncludeDetailInExceptions = true;
              mux.IncludePerformanceCountersInExceptions = true;
              await closure_0.WarmupAsync(mux);
              mux.ConfigurationChanged += new EventHandler<EndPointEventArgs>(closure_0.OnConfigurationChanged);
              mux.ConfigurationChangedBroadcast += new EventHandler<EndPointEventArgs>(closure_0.OnConfigurationChangedBroadcast);
              mux.ConnectionFailed += new EventHandler<ConnectionFailedEventArgs>(closure_0.OnConnectionFailed);
              mux.ConnectionRestored += new EventHandler<ConnectionFailedEventArgs>(closure_0.OnConnectionRestored);
              mux.ErrorMessage += new EventHandler<RedisErrorEventArgs>(closure_0.OnErrorMessage);
              mux.InternalError += new EventHandler<InternalErrorEventArgs>(closure_0.OnInternalError);
              mux.HashSlotMoved += new EventHandler<HashSlotMovedEventArgs>(closure_0.OnHashSlotMoved);
              closure_0.m_mux = mux;
              IDatabase database = mux.GetDatabase(databaseId, (object) null);
              closure_0.m_database = new VssRedisDatabase(database, closure_0.Settings.MaxMessageSize, closure_0.Settings.BatchingRadixBackoff);
              closure_0.m_subscriber = mux.GetSubscriber((object) null);
              TeamFoundationTracingService.TraceRawAlwaysOn(8110006, TraceLevel.Info, "Redis", nameof (RedisConnection), "ConnectionMultiplexerConnectSucceeded: id=" + closure_0.m_id);
              mux = (ConnectionMultiplexer) null;
            }
            catch (Exception ex)
            {
              TeamFoundationTracingService.TraceRawAlwaysOn(8110013, TraceLevel.Info, "Redis", nameof (RedisConnection), string.Format("ConnectionMultiplexerConnectFailed: id={0}, exception={1}", (object) closure_0.m_id, (object) ex));
              throw;
            }
          })).Unwrap();
        }
        else
          this.m_connectTask = Task.FromException((Exception) new RedisConfigurationException("Redis configuration not provided"));
        this.m_connectTask.ContinueWith((Action<Task>) (_ =>
        {
          Action<RedisConnection.ConnectionManager> action = onConnected;
          if (action == null)
            return;
          action(connectionManager);
        }));
      }

      private async Task WarmupAsync(ConnectionMultiplexer mux)
      {
        EndPoint[] endPointArray = mux.GetEndPoints(true);
        for (int index = 0; index < endPointArray.Length; ++index)
        {
          EndPoint endpoint = endPointArray[index];
          int attempts = 3;
          while (attempts-- > 0)
          {
            try
            {
              TimeSpan timeSpan = await ((IRedisAsync) mux.GetServer(endpoint, (object) null)).PingAsync((CommandFlags) 0);
              break;
            }
            catch (Exception ex)
            {
              TeamFoundationTracingService.TraceRawAlwaysOn(8110006, TraceLevel.Info, "Redis", nameof (RedisConnection), string.Format("ConnectionMultiplexerWarmuperror: id={0}, exception={1}", (object) this.m_id, (object) ex));
            }
          }
          endpoint = (EndPoint) null;
        }
        endPointArray = (EndPoint[]) null;
      }

      public void Dispose()
      {
        lock (this.m_lock)
        {
          if (this.m_refCount == 0 || --this.m_refCount > 0)
            return;
          this.m_disposeStack = EnvironmentWrapper.ToReadableStackTrace();
        }
        this.m_connectTask.ContinueWith((Action<Task>) (_ =>
        {
          TeamFoundationTracingService.TraceRawAlwaysOn(8110006, TraceLevel.Info, "Redis", nameof (RedisConnection), "ConnectionMultiplexerDisposeBegin: id=" + this.m_id);
          if (this.m_subscriber != null)
          {
            try
            {
              this.m_subscriber.UnsubscribeAll((CommandFlags) 0);
            }
            catch (Exception ex)
            {
              TeamFoundationTracingService.TraceRawAlwaysOn(8110006, TraceLevel.Info, "Redis", nameof (RedisConnection), string.Format("ConnectionMultiplexerDisposeError: failed to unsubscribe, id={0}, ex={1}", (object) this.m_id, (object) ex));
            }
          }
          if (this.m_mux != null)
          {
            this.m_mux.ConfigurationChanged -= new EventHandler<EndPointEventArgs>(this.OnConfigurationChanged);
            this.m_mux.ConfigurationChangedBroadcast -= new EventHandler<EndPointEventArgs>(this.OnConfigurationChangedBroadcast);
            this.m_mux.ConnectionFailed -= new EventHandler<ConnectionFailedEventArgs>(this.OnConnectionFailed);
            this.m_mux.ConnectionRestored -= new EventHandler<ConnectionFailedEventArgs>(this.OnConnectionRestored);
            this.m_mux.ErrorMessage -= new EventHandler<RedisErrorEventArgs>(this.OnErrorMessage);
            this.m_mux.InternalError -= new EventHandler<InternalErrorEventArgs>(this.OnInternalError);
            this.m_mux.HashSlotMoved -= new EventHandler<HashSlotMovedEventArgs>(this.OnHashSlotMoved);
            try
            {
              this.m_mux.Close(true);
            }
            catch (Exception ex)
            {
              TeamFoundationTracingService.TraceRawAlwaysOn(8110006, TraceLevel.Info, "Redis", nameof (RedisConnection), string.Format("ConnectionMultiplexerDisposeError: failed to close, id={0}, ex={1}", (object) this.m_id, (object) ex));
            }
          }
          TeamFoundationTracingService.TraceRawAlwaysOn(8110006, TraceLevel.Info, "Redis", nameof (RedisConnection), "ConnectionMultiplexerDisposeEnd: id=" + this.m_id);
        }));
      }

      public bool IsDisposed => this.m_refCount == 0;

      public bool IsConnected
      {
        get
        {
          Task connectTask = this.m_connectTask;
          return connectTask != null && connectTask.Status == TaskStatus.RanToCompletion;
        }
      }

      public ConnectionSettings Settings { get; }

      public virtual VssRedisDatabase Database
      {
        get
        {
          this.CheckConnected();
          this.CheckDisposed();
          return this.m_database;
        }
      }

      public ConnectionMultiplexer Mux
      {
        get
        {
          this.CheckConnected();
          this.CheckDisposed();
          return this.m_mux;
        }
      }

      public ISubscriber Subscriber
      {
        get
        {
          this.CheckConnected();
          this.CheckDisposed();
          return this.m_subscriber;
        }
      }

      public bool NeedsReconnect(ConnectionSettings settings) => this.Settings != null && settings != null ? !this.Settings.Equals((object) settings) : this.Settings != settings;

      public override string ToString() => string.Format("[{0}(connectStatus={1}, refCount={2})]", (object) this.m_id, (object) this.m_connectTask?.Status, (object) this.m_refCount);

      public RedisConnection.ConnectionManager AddRef()
      {
        lock (this.m_lock)
        {
          if (this.m_refCount > 0)
            ++this.m_refCount;
        }
        return this;
      }

      private void CheckConnected()
      {
        if (this.Settings == null)
          throw new RedisConfigurationException("Redis is not configured or configuration is invalid");
        if (!this.IsConnected)
          throw new RedisConnectionException(string.Format("Redis is not connected (status={0})", (object) this.m_connectTask.Status));
      }

      private void CheckDisposed()
      {
        if (this.IsDisposed)
          throw new ObjectDisposedException("Redis connection", "Disposed at: " + this.m_disposeStack);
      }

      private void OnConnectionFailed(object sender, ConnectionFailedEventArgs args) => TeamFoundationTracingService.TraceRawAlwaysOn(8110007, TraceLevel.Info, "Redis", nameof (RedisConnection), string.Format("ConnectionFailed: id={0}, endpoint={1}, connectionType={2}, failureType={3}, exception={4}", (object) this.m_id, (object) args.EndPoint, (object) args.ConnectionType, (object) args.FailureType, (object) args.Exception));

      private void OnConnectionRestored(object sender, ConnectionFailedEventArgs args) => TeamFoundationTracingService.TraceRawAlwaysOn(8110007, TraceLevel.Info, "Redis", nameof (RedisConnection), string.Format("ConnectionRestored: id={0}, endpoint={1}, connectionType={2}, failureType={3}, exception={4}", (object) this.m_id, (object) args.EndPoint, (object) args.ConnectionType, (object) args.FailureType, (object) args.Exception));

      private void OnConfigurationChanged(object sender, EndPointEventArgs args) => TeamFoundationTracingService.TraceRawAlwaysOn(8110008, TraceLevel.Info, "Redis", nameof (RedisConnection), string.Format("ConfigurationChanged: id={0}, endpoint={1}", (object) this.m_id, (object) args.EndPoint));

      private void OnConfigurationChangedBroadcast(object sender, EndPointEventArgs args) => TeamFoundationTracingService.TraceRawAlwaysOn(8110008, TraceLevel.Info, "Redis", nameof (RedisConnection), string.Format("ConfigurationChangedBroadcast: id={0}, endpoint={1}", (object) this.m_id, (object) args.EndPoint));

      private void OnErrorMessage(object sender, RedisErrorEventArgs args) => TeamFoundationTracingService.TraceRawAlwaysOn(8110008, TraceLevel.Info, "Redis", nameof (RedisConnection), string.Format("ErrorMessage: id={0}, endpoint={1}, message={2}", (object) this.m_id, (object) args.EndPoint, (object) args.Message));

      private void OnInternalError(object sender, InternalErrorEventArgs args) => TeamFoundationTracingService.TraceRawAlwaysOn(8110008, TraceLevel.Info, "Redis", nameof (RedisConnection), string.Format("InternalError: id={0}, endpoint={1}, connectionType={2}, origin={3}, exception={4}", (object) this.m_id, (object) args.EndPoint, (object) args.ConnectionType, (object) args.Origin, (object) args.Exception));

      private void OnHashSlotMoved(object sender, HashSlotMovedEventArgs args) => TeamFoundationTracingService.TraceRawAlwaysOn(8110008, TraceLevel.Info, "Redis", nameof (RedisConnection), string.Format("HashSlotMoved: id={0}, hashSlot={1}, oldEndpoint={2}, newEndpoint={3}", (object) this.m_id, (object) args.HashSlot, (object) args.OldEndPoint, (object) args.NewEndPoint));
    }

    private class ExceptionInfo
    {
      public int TotalExceptions;
      public Exception LastException;
    }
  }
}
