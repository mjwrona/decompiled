// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestExecution.Server.TestCommandCatalogService
// Assembly: Microsoft.VisualStudio.Services.TE.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BC2680F-A5FB-41BE-A4CF-F78BF7AC3E02
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.TE.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Test.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.TestExecution.Server
{
  [CLSCompliant(false)]
  public class TestCommandCatalogService : ITestCommandCatalogService, IVssFrameworkService
  {
    private ITfsMessageQueueServiceHelper _messageQueueServiceHelper;

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void CreateQueue(
      TestExecutionRequestContext testExecutionRequestContext,
      string queueName,
      string description)
    {
      DtaLogger logger = this.GetLogger(testExecutionRequestContext);
      logger.Verbose(6200161, "TestCommandCatalogService:CreateQueue() called with queue Name " + queueName);
      try
      {
        this.MessageQueueServiceHelper.CreateQueue(testExecutionRequestContext, queueName, description);
      }
      catch (MessageQueueAlreadyExistsException ex)
      {
        logger.Error(6200162, string.Format("Exception: {0} occurred. Emptying the queue. Queue name :{1}", (object) ex, (object) queueName));
        this.MessageQueueServiceHelper.EmptyQueue(testExecutionRequestContext, queueName);
      }
      logger.Verbose(6200163, "TestCommandCatalogService:CreateQueue() queue with name " + queueName + " created.");
    }

    public void DeleteQueue(
      TestExecutionRequestContext testExecutionRequestContext,
      string queueName)
    {
      DtaLogger logger = this.GetLogger(testExecutionRequestContext);
      logger.Verbose(6200164, "TestCommandCatalogService:DeleteQueue() called with queue Name " + queueName + ".");
      try
      {
        if (this.QueueExists(testExecutionRequestContext, queueName))
          this.MessageQueueServiceHelper.DeleteQueue(testExecutionRequestContext, queueName);
        else
          logger.Warning(6200165, "TestCommandCatalogService:DeleteQueue() Queue with Name " + queueName + " not found.");
      }
      finally
      {
        logger.Verbose(6200166, "TestCommandCatalogService:DeleteQueue() queue with name " + queueName + " deleted.");
      }
    }

    public async Task<TestExecutionServiceCommand> GetCommandAsync(
      TestExecutionRequestContext testExecutionRequestContext,
      int testAgentId,
      long? lastCommandId = null)
    {
      DtaLogger logger = this.GetLogger(testExecutionRequestContext);
      logger.Verbose(6200167, "TestCommandCatalogService:GetCommandAsync() called for agent {0} with lastCommandId {1}", (object) testAgentId, (object) (lastCommandId.HasValue ? lastCommandId.Value : 0L));
      TestAgent testAgent = this.GetTestAgent(testExecutionRequestContext, testAgentId);
      if (testAgent == null)
      {
        logger.Error(6200171, "TestCommandCatalogService:GetCommandAsync() The requested test agent is not found. testAgentId : {0}", (object) testAgentId);
        throw new TestExecutionObjectNotFoundException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, TestExecutionServiceResources.AgentNotFound, (object) testAgentId));
      }
      string url = testAgent.DtlEnvironment.Url;
      MessageQueueDetails nameAndSessionId = this.TryGetQueueNameAndSessionId(testExecutionRequestContext, testAgent.TestRunId);
      if (nameAndSessionId != null)
      {
        Guid sessionId = nameAndSessionId.SessionId;
        string queueName = nameAndSessionId.QueueName;
        logger.Info(6200168, "TestCommandCatalogService:GetCommandAsync() SessionId {0} QueueName {1} retrieved.", (object) sessionId, (object) queueName);
        if (!this.QueueExists(testExecutionRequestContext, queueName))
        {
          logger.Error(6200169, string.Format("TestCommandCatalogService:GetCommandAsync() Queue {0} not found for Agent: {1} TestRunId: {2}.", (object) queueName, (object) testAgentId, (object) testAgent.TestRunId));
          return new TestExecutionServiceCommand()
          {
            Body = JsonConvert.SerializeObject((object) this.GetNoneCommand())
          };
        }
        TestExecutionServiceCommand testAgentMessage = (await this.MessageQueueServiceHelper.GetMessageAsync(testExecutionRequestContext, queueName, sessionId, TimeSpan.FromSeconds((double) TestHttpClient.LongPollTimeOutForMessageQueueInSeconds), lastCommandId)).ToTestAgentMessage();
        TestCommandCatalogService.UpdateHeatBeatForAgent(testAgentId, testExecutionRequestContext, testAgent.LastHeartBeat, testAgentMessage);
        return testAgentMessage;
      }
      logger.Error(6200170, string.Format("TestCommandCatalogService:GetCommandAsync() No queue found for Env {0} Agent: {1} TestRunId: {2}", (object) url, (object) testAgentId, (object) testAgent.TestRunId));
      return new TestExecutionServiceCommand()
      {
        Body = JsonConvert.SerializeObject((object) this.GetNoneCommand())
      };
    }

    public bool TryEnqueueCommand(
      TestExecutionRequestContext testExecutionRequestContext,
      string queueName,
      TestExecutionServiceCommand command)
    {
      ArgumentUtility.CheckForNull<TestExecutionServiceCommand>(command, nameof (command), testExecutionRequestContext.RequestContext.ServiceName);
      ArgumentUtility.CheckStringForNullOrWhiteSpace(command.Body, "command.Body", testExecutionRequestContext.RequestContext.ServiceName);
      DtaLogger logger = this.GetLogger(testExecutionRequestContext);
      logger.Verbose(6200171, "TestCommandCatalogService:EnqueueCommand() started for queue {0} with body {1}", (object) queueName, (object) command.Body);
      try
      {
        this.MessageQueueServiceHelper.EnqueueMessage(testExecutionRequestContext, queueName, DtaConstants.TestServiceMessageType, command.Body);
        return true;
      }
      catch (MessageQueueNotFoundException ex)
      {
        logger.Error(6200172, "TestCommandCatalogService:EnqueueCommand() cannot enqueue as the queue name {0} is invalid.", (object) queueName);
        return false;
      }
      finally
      {
        logger.Verbose(6200173, "TestCommandCatalogService:EnqueueCommand() done for queue {0} with body {1}", (object) queueName, (object) command.Body);
      }
    }

    private static void UpdateHeatBeatForAgent(
      int testAgentId,
      TestExecutionRequestContext tfsRequestContext,
      DateTime lastHeartBeatTime,
      TestExecutionServiceCommand messageToTestAgent)
    {
      if (lastHeartBeatTime == DateTime.MinValue && messageToTestAgent == null)
        return;
      using (DtaAgentDatabase component = tfsRequestContext.RequestContext.CreateComponent<DtaAgentDatabase>())
        component.UpdateAgentHeartBeat(testAgentId);
    }

    private bool QueueExists(TestExecutionRequestContext requestContext, string queueName)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(queueName, nameof (queueName), requestContext.RequestContext.ServiceName);
      return this.MessageQueueServiceHelper.QueueExists(requestContext, queueName);
    }

    private TestAgent GetTestAgent(TestExecutionRequestContext requestContext, int testAgentId)
    {
      using (DtaAgentDatabase component = requestContext.RequestContext.CreateComponent<DtaAgentDatabase>())
        return component.QueryTestAgent(testAgentId);
    }

    private MessageQueueDetails TryGetQueueNameAndSessionId(
      TestExecutionRequestContext requestContext,
      int testRunId)
    {
      if (testRunId <= 0)
        return (MessageQueueDetails) null;
      using (DtaMessageQueueDatabase component = requestContext.RequestContext.CreateComponent<DtaMessageQueueDatabase>())
        return component.QueryMessageQueueDetailsByTestRunId(testRunId);
    }

    private TestExecutionCommandMessage GetNoneCommand() => new TestExecutionCommandMessage()
    {
      Command = TestExecutionCommand.None
    };

    private DtaLogger GetLogger(TestExecutionRequestContext tfsRequestContext) => new DtaLogger(tfsRequestContext, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.ServiceLayer);

    public ITfsMessageQueueServiceHelper MessageQueueServiceHelper
    {
      get
      {
        if (this._messageQueueServiceHelper == null)
          this._messageQueueServiceHelper = (ITfsMessageQueueServiceHelper) new TfsMessageQueueServiceHelper();
        return this._messageQueueServiceHelper;
      }
      set => this._messageQueueServiceHelper = value;
    }
  }
}
