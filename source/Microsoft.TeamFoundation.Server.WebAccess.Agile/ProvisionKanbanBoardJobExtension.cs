// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.ProvisionKanbanBoardJobExtension
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.Agile.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile
{
  public class ProvisionKanbanBoardJobExtension : ITeamFoundationJobExtension
  {
    public TeamFoundationJobExecutionResult Run(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition jobDefinition,
      DateTime queueTime,
      out string resultMessage)
    {
      resultMessage = "Completed Successfully";
      TeamFoundationJobExecutionResult jobExecutionResult = TeamFoundationJobExecutionResult.Succeeded;
      ProvisionKanbanBoardJobExtension.ResultMessageLogger resultMessageLogger = new ProvisionKanbanBoardJobExtension.ResultMessageLogger();
      bool flag;
      if (jobDefinition.Data != null)
      {
        Guid[] teamIds = TeamFoundationSerializationUtility.Deserialize<Guid[]>(jobDefinition.Data);
        flag = KanbanBoardProvisioningUtils.ProvisionKanbanBoard(requestContext, (IEnumerable<Guid>) teamIds, 0, (KanbanBoardProvisioningUtils.Logger) resultMessageLogger, 1);
      }
      else
        flag = KanbanBoardProvisioningUtils.ProvisionKanbanBoards(requestContext, 0, (KanbanBoardProvisioningUtils.Logger) resultMessageLogger, 2);
      if (!flag)
      {
        jobExecutionResult = TeamFoundationJobExecutionResult.Failed;
        resultMessage = resultMessageLogger.GetMessage();
      }
      return jobExecutionResult;
    }

    private class ResultMessageLogger : KanbanBoardProvisioningUtils.Logger
    {
      private StringBuilder m_message = new StringBuilder();

      public override void LogMessage(TraceLevel traceLevel, string message) => this.m_message.AppendLine(message);

      public string GetMessage() => this.m_message.ToString();
    }
  }
}
