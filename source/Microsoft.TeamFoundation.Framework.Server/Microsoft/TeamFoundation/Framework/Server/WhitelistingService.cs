// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.WhitelistingService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class WhitelistingService : IVssFrameworkService
  {
    private long m_defaultExecutionTimeThreshold;
    private Dictionary<WhitelistedCommandKey, long> m_whitelistedCommands;
    private Dictionary<string, List<ExpectedExceptionType>> m_whitelistedExceptions;
    private HashSet<string> m_interactiveUserAgentPrefixes;
    private const string c_loggingExecutionTimeThresholdKey = "/Configuration/Logging/DefaultExecutionTimeThreshold";
    private const string c_AnyApplication = "*";
    private static readonly string s_Area = "ActivityLog";
    private static readonly string s_Layer = nameof (WhitelistingService);
    private static readonly string[] s_ValidDynamicExceptions = new string[3]
    {
      "ArgumentException",
      "ArgumentNullException",
      "ArgumentOutOfRangeException"
    };
    private INotificationRegistration m_whiteListedRegistration;
    private INotificationRegistration m_whiteListExceptionRegistration;
    private INotificationRegistration m_interactiveUserAgentsRegistration;

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.CheckDeploymentRequestContext();
      this.m_whitelistedCommands = new Dictionary<WhitelistedCommandKey, long>();
      this.m_whitelistedExceptions = new Dictionary<string, List<ExpectedExceptionType>>((IEqualityComparer<string>) StringComparer.InvariantCultureIgnoreCase);
      this.m_interactiveUserAgentPrefixes = new HashSet<string>((IEqualityComparer<string>) StringComparer.InvariantCultureIgnoreCase);
      this.m_defaultExecutionTimeThreshold = systemRequestContext.GetService<CachedRegistryService>().GetValue<long>(systemRequestContext, (RegistryQuery) "/Configuration/Logging/DefaultExecutionTimeThreshold", 10000000L);
      this.LoadCommandWhitelistingConfiguration(systemRequestContext);
      this.LoadExceptionWhitelistingConfiguration(systemRequestContext);
      this.LoadInteractiveUserAgentsWhitelistingConfiguration(systemRequestContext);
      ITeamFoundationSqlNotificationService service = systemRequestContext.GetService<ITeamFoundationSqlNotificationService>();
      this.m_whiteListedRegistration = service.CreateRegistration(systemRequestContext, "Default", SqlNotificationEventClasses.WhiteListedCommandChanged, new SqlNotificationCallback(this.OnWhitelistedCommandChanged), false, false);
      this.m_whiteListExceptionRegistration = service.CreateRegistration(systemRequestContext, "Default", SqlNotificationEventClasses.WhiteListedExceptionChanged, new SqlNotificationCallback(this.OnWhitelistedExceptionChanged), false, false);
      this.m_interactiveUserAgentsRegistration = service.CreateRegistration(systemRequestContext, "Default", SqlNotificationEventClasses.InteractiveUserAgentChanged, new SqlNotificationCallback(this.OnInteractiveUserAgentChanged), false, false);
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      this.m_whiteListedRegistration.Unregister(systemRequestContext);
      this.m_whiteListExceptionRegistration.Unregister(systemRequestContext);
      this.m_interactiveUserAgentsRegistration.Unregister(systemRequestContext);
    }

    private void LoadCommandWhitelistingConfiguration(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(7041, WhitelistingService.s_Area, WhitelistingService.s_Layer, nameof (LoadCommandWhitelistingConfiguration));
      try
      {
        Dictionary<WhitelistedCommandKey, long> dictionary = new Dictionary<WhitelistedCommandKey, long>();
        foreach (SlowCommandDefinition commandDefinition in this.LoadWhitelistedCommand(requestContext))
          dictionary.Add(new WhitelistedCommandKey(commandDefinition.Application, commandDefinition.Command), commandDefinition.ExecutionTimeThreshold);
        this.m_whitelistedCommands = dictionary;
      }
      catch (ServiceNotRegisteredException ex)
      {
        requestContext.Trace(7044, TraceLevel.Error, WhitelistingService.s_Area, WhitelistingService.s_Layer, "The Whitelisting service is not available: {0}", (object) ex);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(7042, WhitelistingService.s_Area, WhitelistingService.s_Layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(7043, WhitelistingService.s_Area, WhitelistingService.s_Layer, nameof (LoadCommandWhitelistingConfiguration));
      }
    }

    private void LoadExceptionWhitelistingConfiguration(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(7051, WhitelistingService.s_Area, WhitelistingService.s_Layer, nameof (LoadExceptionWhitelistingConfiguration));
      try
      {
        Dictionary<string, List<ExpectedExceptionType>> dictionary = new Dictionary<string, List<ExpectedExceptionType>>((IEqualityComparer<string>) StringComparer.InvariantCultureIgnoreCase);
        foreach (ExpectedExceptionType expectedExceptionType in this.LoadWhitelistedException(requestContext))
        {
          List<ExpectedExceptionType> expectedExceptionTypeList;
          if (!dictionary.TryGetValue(expectedExceptionType.Type, out expectedExceptionTypeList))
          {
            expectedExceptionTypeList = new List<ExpectedExceptionType>();
            dictionary[expectedExceptionType.Type] = expectedExceptionTypeList;
          }
          expectedExceptionTypeList.Add(expectedExceptionType);
        }
        this.m_whitelistedExceptions = dictionary;
      }
      catch (ServiceNotRegisteredException ex)
      {
        requestContext.Trace(7054, TraceLevel.Error, WhitelistingService.s_Area, WhitelistingService.s_Layer, "The Whitelisting service is not available: {0}", (object) ex);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(7052, WhitelistingService.s_Area, WhitelistingService.s_Layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(7053, WhitelistingService.s_Area, WhitelistingService.s_Layer, nameof (LoadExceptionWhitelistingConfiguration));
      }
    }

    private void LoadInteractiveUserAgentsWhitelistingConfiguration(
      IVssRequestContext requestContext)
    {
      using (requestContext.TraceBlock(7061, 7063, WhitelistingService.s_Area, WhitelistingService.s_Layer, nameof (LoadInteractiveUserAgentsWhitelistingConfiguration)))
      {
        try
        {
          List<string> source = (List<string>) null;
          using (WhitelistingComponent component = requestContext.CreateComponent<WhitelistingComponent>())
            source = component.GetInteractiveUserAgents();
          this.m_interactiveUserAgentPrefixes = source.ToHashSet<string>();
        }
        catch (ServiceNotRegisteredException ex)
        {
          requestContext.Trace(7064, TraceLevel.Error, WhitelistingService.s_Area, WhitelistingService.s_Layer, "The Whitelisting service is not available: {0}", (object) ex);
        }
        catch (Exception ex)
        {
          requestContext.TraceException(7062, WhitelistingService.s_Area, WhitelistingService.s_Layer, ex);
          throw;
        }
      }
    }

    private void OnWhitelistedCommandChanged(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      if (string.IsNullOrEmpty(eventData))
        return;
      this.LoadCommandWhitelistingConfiguration(requestContext);
    }

    private void OnWhitelistedExceptionChanged(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      if (string.IsNullOrEmpty(eventData))
        return;
      this.LoadExceptionWhitelistingConfiguration(requestContext);
    }

    private void OnInteractiveUserAgentChanged(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      if (string.IsNullOrWhiteSpace(eventData))
        return;
      this.LoadInteractiveUserAgentsWhitelistingConfiguration(requestContext);
    }

    private List<SlowCommandDefinition> LoadWhitelistedCommand(IVssRequestContext requestContext)
    {
      using (WhitelistingComponent component = requestContext.CreateComponent<WhitelistingComponent>())
        return component.GetSlowCommands().GetCurrent<SlowCommandDefinition>().Items;
    }

    private List<ExpectedExceptionType> LoadWhitelistedException(IVssRequestContext requestContext)
    {
      using (WhitelistingComponent component = requestContext.CreateComponent<WhitelistingComponent>())
        return component.GetExpectedExceptions().GetCurrent<ExpectedExceptionType>().Items;
    }

    public long GetCommandExecutionTimeThreshold(
      IVssRequestContext requestContext,
      string application,
      string command,
      string userAgent)
    {
      bool flag = true;
      if (requestContext.IsFeatureEnabled(FrameworkServerConstants.OnlyCountInteractiveUserAgentsAsSlow))
        flag = this.IsInteractiveUserAgentPrefix(userAgent);
      if (!flag)
        return 1000000000000000000;
      long num;
      return !string.IsNullOrWhiteSpace(command) && (this.m_whitelistedCommands.TryGetValue(new WhitelistedCommandKey(application ?? string.Empty, command), out num) || this.m_whitelistedCommands.TryGetValue(new WhitelistedCommandKey("*", command), out num)) ? num : this.m_defaultExecutionTimeThreshold;
    }

    public bool IsCommandWhitelisted(string application, string command)
    {
      if (string.IsNullOrWhiteSpace(command))
        return false;
      return this.m_whitelistedCommands.ContainsKey(new WhitelistedCommandKey(application ?? string.Empty, command)) || this.m_whitelistedCommands.ContainsKey(new WhitelistedCommandKey("*", command));
    }

    public bool IsExceptionExpected(Exception exception, string area)
    {
      string loggingExceptionName = TeamFoundationTracingService.GetActivityLoggingExceptionName(exception);
      List<ExpectedExceptionType> expectedExceptionTypeList;
      if (string.IsNullOrEmpty(loggingExceptionName) || !this.m_whitelistedExceptions.TryGetValue(loggingExceptionName, out expectedExceptionTypeList))
        return false;
      bool flag = false;
      foreach (ExpectedExceptionType expectedExceptionType in expectedExceptionTypeList)
      {
        if (expectedExceptionType.Area == null)
          return true;
        if (expectedExceptionType.Area.Equals(area ?? "", StringComparison.InvariantCultureIgnoreCase))
        {
          if (!expectedExceptionType.DynamicOnly)
            return true;
          flag = exception.IsExpected(area);
        }
      }
      return flag;
    }

    public bool IsInteractiveUserAgentPrefix(string userAgent) => this.m_interactiveUserAgentPrefixes.Any<string>((Func<string, bool>) (ua =>
    {
      if (ua == null)
        return false;
      string str = userAgent;
      return str != null && str.StartsWith(ua, StringComparison.OrdinalIgnoreCase);
    }));

    public void UpdateWhitelistingConfiguration(
      IVssRequestContext requestContext,
      WhitelistingConfig[] configCollection)
    {
      configCollection = ((IEnumerable<WhitelistingConfig>) configCollection).OrderBy<WhitelistingConfig, int>((Func<WhitelistingConfig, int>) (x => x.ServicingPriority)).ToArray<WhitelistingConfig>();
      Dictionary<string, List<SlowCommandUpdate>> dictionary1 = new Dictionary<string, List<SlowCommandUpdate>>((IEqualityComparer<string>) StringComparer.InvariantCultureIgnoreCase);
      Dictionary<string, List<ExpectedExceptionUpdate>> dictionary2 = new Dictionary<string, List<ExpectedExceptionUpdate>>((IEqualityComparer<string>) StringComparer.InvariantCultureIgnoreCase);
      Dictionary<string, List<InteractiveUserAgentUpdate>> dictionary3 = new Dictionary<string, List<InteractiveUserAgentUpdate>>((IEqualityComparer<string>) StringComparer.InvariantCultureIgnoreCase);
      foreach (WhitelistingConfig config in configCollection)
      {
        if (config.Commands != null)
        {
          List<SlowCommandUpdate> slowCommandUpdateList;
          if (!dictionary1.TryGetValue(config.DataSource, out slowCommandUpdateList))
          {
            slowCommandUpdateList = new List<SlowCommandUpdate>();
            dictionary1.Add(config.DataSource, slowCommandUpdateList);
          }
          foreach (SlowCommand command in config.Commands)
          {
            SlowCommandUpdate slowCommandUpdate = new SlowCommandUpdate()
            {
              Application = command.Application ?? config.Area,
              Command = command.Command,
              ExecutionTimeThreshold = command.ExecutionTimeThreshold,
              Note = command.Note
            };
            slowCommandUpdateList.Add(slowCommandUpdate);
          }
        }
        if (config.Exceptions != null)
        {
          List<ExpectedExceptionUpdate> expectedExceptionUpdateList;
          if (!dictionary2.TryGetValue(config.DataSource, out expectedExceptionUpdateList))
          {
            expectedExceptionUpdateList = new List<ExpectedExceptionUpdate>();
            dictionary2.Add(config.DataSource, expectedExceptionUpdateList);
          }
          foreach (ExpectedExceptionType exception in config.Exceptions)
          {
            if (!exception.DynamicOnly || ((IEnumerable<string>) WhitelistingService.s_ValidDynamicExceptions).Contains<string>(exception.Type) && (exception.Area ?? config.Area) != null)
            {
              ExpectedExceptionUpdate expectedExceptionUpdate = new ExpectedExceptionUpdate()
              {
                ExceptionType = exception.Type,
                Note = exception.Note,
                Area = exception.Area ?? config.Area,
                DynamicOnly = exception.DynamicOnly
              };
              expectedExceptionUpdateList.Add(expectedExceptionUpdate);
            }
          }
        }
        if (config.InteractiveUserAgentPrefixes != null)
        {
          List<InteractiveUserAgentUpdate> interactiveUserAgentUpdateList;
          if (!dictionary3.TryGetValue(config.DataSource, out interactiveUserAgentUpdateList))
          {
            interactiveUserAgentUpdateList = new List<InteractiveUserAgentUpdate>();
            dictionary3.Add(config.DataSource, interactiveUserAgentUpdateList);
          }
          interactiveUserAgentUpdateList.AddRange(((IEnumerable<InteractiveUserAgentPrefix>) config.InteractiveUserAgentPrefixes).Select<InteractiveUserAgentPrefix, InteractiveUserAgentUpdate>((Func<InteractiveUserAgentPrefix, InteractiveUserAgentUpdate>) (x => new InteractiveUserAgentUpdate()
          {
            UserAgent = x.UserAgentPrefix,
            Note = x.Note
          })));
        }
      }
      foreach (KeyValuePair<string, List<SlowCommandUpdate>> keyValuePair in dictionary1)
      {
        string key = keyValuePair.Key;
        List<SlowCommandUpdate> updates = keyValuePair.Value;
        using (WhitelistingComponent component = requestContext.CreateComponent<WhitelistingComponent>())
          component.UpdateSlowCommands(key, updates);
      }
      foreach (KeyValuePair<string, List<ExpectedExceptionUpdate>> keyValuePair in dictionary2)
      {
        string key = keyValuePair.Key;
        List<ExpectedExceptionUpdate> updates = keyValuePair.Value;
        using (WhitelistingComponent component = requestContext.CreateComponent<WhitelistingComponent>())
          component.UpdateExpectedException(key, updates);
      }
      foreach (KeyValuePair<string, List<InteractiveUserAgentUpdate>> keyValuePair in dictionary3)
      {
        string key = keyValuePair.Key;
        List<InteractiveUserAgentUpdate> interactiveUserAgents = keyValuePair.Value;
        using (WhitelistingComponent component = requestContext.CreateComponent<WhitelistingComponent>())
          component.UpdateInteractiveUserAgents(key, interactiveUserAgents);
      }
    }

    internal void TEST_SetWhitelistedCommands(Dictionary<WhitelistedCommandKey, long> commands) => this.m_whitelistedCommands = commands;

    internal void TEST_SetWhitelistedExceptions(
      Dictionary<string, List<ExpectedExceptionType>> whitelist)
    {
      this.m_whitelistedExceptions = whitelist;
    }

    internal void TEST_SetInteractiveUserAgents(IEnumerable<string> userAgents) => this.m_interactiveUserAgentPrefixes = userAgents.ToHashSet<string>();
  }
}
