// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Orchestration.Server.OrchestrationComponent
// Assembly: Microsoft.VisualStudio.Services.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 953225F5-5DFE-4840-B8F7-3B94A5257E43
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Orchestration.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Orchestration.Server.DataAccess;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Orchestration.Server
{
  internal class OrchestrationComponent : TeamFoundationSqlResourceComponent
  {
    public const string OrchestrationDataspaceCategory = "Orchestration";
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[17]
    {
      (IComponentCreator) new ComponentCreator<OrchestrationComponent>(1, true),
      (IComponentCreator) new ComponentCreator<OrchestrationComponent2>(2),
      (IComponentCreator) new ComponentCreator<OrchestrationComponent3>(3),
      (IComponentCreator) new ComponentCreator<OrchestrationComponent4>(4),
      (IComponentCreator) new ComponentCreator<OrchestrationComponent5>(5),
      (IComponentCreator) new ComponentCreator<OrchestrationComponent6>(6),
      (IComponentCreator) new ComponentCreator<OrchestrationComponent7>(7),
      (IComponentCreator) new ComponentCreator<OrchestrationComponent8>(8),
      (IComponentCreator) new ComponentCreator<OrchestrationComponent9>(9),
      (IComponentCreator) new ComponentCreator<OrchestrationComponent10>(10),
      (IComponentCreator) new ComponentCreator<OrchestrationComponent11>(11),
      (IComponentCreator) new ComponentCreator<OrchestrationComponent12>(12),
      (IComponentCreator) new ComponentCreator<OrchestrationComponent13>(13),
      (IComponentCreator) new ComponentCreator<OrchestrationComponent14>(14),
      (IComponentCreator) new ComponentCreator<OrchestrationComponent15>(15),
      (IComponentCreator) new ComponentCreator<OrchestrationComponent16>(16),
      (IComponentCreator) new ComponentCreator<OrchestrationComponent17>(17)
    }, "Orchestration", "Orchestration", 15);
    private static readonly Lazy<IDictionary<int, SqlExceptionFactory>> s_translatedExceptions = new Lazy<IDictionary<int, SqlExceptionFactory>>(new Func<IDictionary<int, SqlExceptionFactory>>(OrchestrationComponent.CreateExceptionMap));

    public OrchestrationComponent()
    {
      this.ContainerErrorCode = 50000;
      this.SelectedFeatures |= SqlResourceComponentFeatures.BindPartitionIdByDefault;
    }

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => OrchestrationComponent.s_translatedExceptions.Value;

    public virtual Task<IEnumerable<ActivityDispatcherDescriptor>> AddActivityDispatchersAsync(
      string hubName,
      IEnumerable<ActivityDispatcherDescriptor> dispatchers)
    {
      return Task.FromResult<IEnumerable<ActivityDispatcherDescriptor>>((IEnumerable<ActivityDispatcherDescriptor>) null);
    }

    public virtual Task<int> CleanupMessageContentAsync(int sqlCommandTimeout) => Task.FromResult<int>(0);

    public virtual Task<int> CleanupOrchestrationStateHistoryAsync(
      DateTime cutOffTime,
      int sqlCommandTimeout)
    {
      return Task.FromResult<int>(0);
    }

    public virtual Task CompleteActivityMessageAsync(
      string hubName,
      string dispatcherType,
      long messageIdToDelete,
      OrchestrationMessage newMessage)
    {
      return this.UpdateOrchestrationSessionAsync(hubName, OrchestrationHubDispatcherType.Activity, messagesToDelete: (IEnumerable<long>) new long[1]
      {
        messageIdToDelete
      }, orchestrationMessages: (IEnumerable<OrchestrationMessage>) new OrchestrationMessage[1]
      {
        newMessage
      });
    }

    public virtual OrchestrationHubDescription CreateHub(OrchestrationHubDescription description) => throw new ServiceVersionNotSupportedException("Orchestration", 1, 1);

    public virtual void CreateOrchestrationSession(
      OrchestrationHubDescription hub,
      string sessionId,
      IEnumerable<OrchestrationMessage> messages)
    {
      throw new ServiceVersionNotSupportedException("Orchestration", 1, 1);
    }

    public virtual IEnumerable<OrchestrationHubDescription> GetHubs(string hubName) => (IEnumerable<OrchestrationHubDescription>) Array.Empty<OrchestrationHubDescription>();

    public virtual Task<IList<OrchestrationMessage>> GetActivityMessagesAsync(
      string hubName,
      string dispatcherType,
      int maxMessageCount,
      long? lastMessageId)
    {
      return Task.FromResult<IList<OrchestrationMessage>>((IList<OrchestrationMessage>) Array.Empty<OrchestrationMessage>());
    }

    public virtual Task<IList<RunnableOrchestrationSession>> GetRunnableSessionsAsync(
      string hubName,
      OrchestrationHubDispatcherType dispatcherType,
      int maxSessionCount)
    {
      return Task.FromResult<IList<RunnableOrchestrationSession>>((IList<RunnableOrchestrationSession>) Array.Empty<RunnableOrchestrationSession>());
    }

    public virtual async Task<RunnableOrchestrationSessionsBatch> GetRunnableSessionsV2Async(
      string hubName,
      OrchestrationHubDispatcherType dispatcherType,
      int maxSessionCount)
    {
      IList<RunnableOrchestrationSession> runnableSessionsAsync = await this.GetRunnableSessionsAsync(hubName, dispatcherType, maxSessionCount);
      RunnableOrchestrationSession orchestrationSession = runnableSessionsAsync.FirstOrDefault<RunnableOrchestrationSession>();
      RunnableOrchestrationSessionsBatch runnableSessionsV2Async = new RunnableOrchestrationSessionsBatch();
      runnableSessionsV2Async.Sessions = runnableSessionsAsync.ToList<RunnableOrchestrationSession>();
      OrchestrationSessionNextRun orchestrationSessionNextRun;
      if (orchestrationSession == null)
      {
        orchestrationSessionNextRun = (OrchestrationSessionNextRun) null;
      }
      else
      {
        orchestrationSessionNextRun = new OrchestrationSessionNextRun();
        orchestrationSessionNextRun.SessionId = orchestrationSession.Session.SessionId;
        orchestrationSessionNextRun.NextRun = orchestrationSession.Session.NextRun;
      }
      runnableSessionsV2Async.NextSessionRun = orchestrationSessionNextRun;
      return runnableSessionsV2Async;
    }

    public virtual OrchestrationSession GetOrchestrationSession(string hubName, string sessionId) => (OrchestrationSession) null;

    public virtual IList<OrchestrationState> GetOrchestrationState(
      string hubName,
      string instanceId,
      string executionId)
    {
      return (IList<OrchestrationState>) Array.Empty<OrchestrationState>();
    }

    public virtual IList<OrchestrationState> GetRunningOrchestrationState(
      string hubName,
      string instanceId)
    {
      return (IList<OrchestrationState>) Array.Empty<OrchestrationState>();
    }

    public virtual void InstallData()
    {
    }

    public virtual OrchestrationHubDescription UpdateHub(string hubName, string newHubName) => throw new ServiceVersionNotSupportedException("Orchestration", 1, 1);

    public virtual Task UpdateOrchestrationSessionAsync(
      string hubName,
      OrchestrationHubDispatcherType dispatcherType,
      IEnumerable<OrchestrationSession> sessions = null,
      IEnumerable<long> messagesToDelete = null,
      IEnumerable<OrchestrationMessage> activityMessages = null,
      IEnumerable<OrchestrationMessage> orchestrationMessages = null,
      IEnumerable<OrchestrationState> instances = null,
      bool ensureSessionExists = false,
      bool alwaysSaveToHistory = false)
    {
      throw new ServiceVersionNotSupportedException("Orchestration", 1, 1);
    }

    public virtual Task<int> GetActivityMessagesCountAsync(string hubName, string dispatcherType) => Task.FromResult<int>(0);

    public virtual void RemovePoisonedOrchestrations(
      IList<string> orchestrationIds,
      TimeSpan? timeout = null)
    {
    }

    public virtual void RemovePoisonedOrchestrations(
      string hubName,
      IList<string> orchestrationIds,
      TimeSpan? timeout = null)
    {
    }

    internal virtual IList<OrchestrationHubMessageCount> GetOrchestrationHubMessageCounts(
      IEnumerable<string> hubNames,
      int? sqlCommandTimeout = null)
    {
      return (IList<OrchestrationHubMessageCount>) new List<OrchestrationHubMessageCount>();
    }

    protected virtual ObjectBinder<OrchestrationSession> GetOrchestrationSessionBinder() => (ObjectBinder<OrchestrationSession>) new OrchestrationSessionBinder();

    private static Exception CreateException(
      IVssRequestContext requestContext,
      int errorNumber,
      SqlException sqlException,
      SqlError sqlError)
    {
      Exception exception = (Exception) null;
      switch (errorNumber)
      {
        case 907105:
          exception = (Exception) new OrchestrationHubExistsException(OrchestrationResources.HubExists((object) sqlError.ExtractString("hubName")));
          break;
        case 907106:
          exception = (Exception) new OrchestrationHubNotFoundException(OrchestrationResources.HubNotFound((object) sqlError.ExtractString("hubName")));
          break;
        case 907107:
          exception = (Exception) new OrchestrationSessionExistsException(OrchestrationResources.SessionExists((object) sqlError.ExtractString("sessionId"), (object) sqlError.ExtractString("hubName")));
          break;
        case 907108:
          exception = (Exception) new OrchestrationSessionNotFoundException(OrchestrationResources.SessionNotFound((object) sqlError.ExtractString("sessionId"), (object) sqlError.ExtractString("hubName")));
          break;
      }
      return exception;
    }

    private static IDictionary<int, SqlExceptionFactory> CreateExceptionMap() => (IDictionary<int, SqlExceptionFactory>) new Dictionary<int, SqlExceptionFactory>()
    {
      {
        907105,
        new SqlExceptionFactory(typeof (OrchestrationHubExistsException), OrchestrationComponent.\u003C\u003EO.\u003C0\u003E__CreateException ?? (OrchestrationComponent.\u003C\u003EO.\u003C0\u003E__CreateException = new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(OrchestrationComponent.CreateException)))
      },
      {
        907106,
        new SqlExceptionFactory(typeof (OrchestrationHubNotFoundException), OrchestrationComponent.\u003C\u003EO.\u003C0\u003E__CreateException ?? (OrchestrationComponent.\u003C\u003EO.\u003C0\u003E__CreateException = new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(OrchestrationComponent.CreateException)))
      },
      {
        907107,
        new SqlExceptionFactory(typeof (OrchestrationSessionExistsException), OrchestrationComponent.\u003C\u003EO.\u003C0\u003E__CreateException ?? (OrchestrationComponent.\u003C\u003EO.\u003C0\u003E__CreateException = new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(OrchestrationComponent.CreateException)))
      },
      {
        907108,
        new SqlExceptionFactory(typeof (OrchestrationSessionNotFoundException), OrchestrationComponent.\u003C\u003EO.\u003C0\u003E__CreateException ?? (OrchestrationComponent.\u003C\u003EO.\u003C0\u003E__CreateException = new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(OrchestrationComponent.CreateException)))
      }
    };
  }
}
