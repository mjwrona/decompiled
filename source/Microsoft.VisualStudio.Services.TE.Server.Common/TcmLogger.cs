// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestExecution.Server.TcmLogger
// Assembly: Microsoft.VisualStudio.Services.TE.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BC2680F-A5FB-41BE-A4CF-F78BF7AC3E02
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.TE.Server.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestExecution.Server
{
  [CLSCompliant(false)]
  public class TcmLogger : ITcmLogger
  {
    public void AddLogToTcmRun(
      TestExecutionRequestContext requestContext,
      int testRunId,
      TeamProjectReference project,
      string message)
    {
      DtaLogger dtaLogger = new DtaLogger(requestContext);
      try
      {
        List<TestMessageLogDetails> logEntries = new List<TestMessageLogDetails>()
        {
          new TestMessageLogDetails()
          {
            Message = message,
            EntryId = testRunId,
            DateCreated = DateTime.Now
          }
        };
        new TestManagementRunHelper().UpdateTestRun(requestContext, testRunId, new RunUpdateModel(logEntries: logEntries), project);
      }
      catch (Exception ex)
      {
        dtaLogger.Error(6200206, string.Format("Failed to update the tcm runlogs for run {0} : Message : {1} : Exception : {2}", (object) testRunId, (object) message, (object) ex));
      }
    }
  }
}
