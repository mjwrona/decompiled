// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestExecution.Server.TestExecutionObjectNotFoundSqlException
// Assembly: Microsoft.VisualStudio.Services.TE.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BC2680F-A5FB-41BE-A4CF-F78BF7AC3E02
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.TE.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;

namespace Microsoft.TeamFoundation.TestExecution.Server
{
  [CLSCompliant(false)]
  [Serializable]
  public class TestExecutionObjectNotFoundSqlException : TestExecutionServiceSqlException
  {
    public TestExecutionObjectNotFoundSqlException(string errorMessage)
      : base(errorMessage)
    {
      this.Initialize();
    }

    public TestExecutionObjectNotFoundSqlException(
      TestExecutionRequestContext requestContext,
      SqlException ex,
      SqlError sqlError)
      : this(TestExecutionObjectNotFoundSqlException.FormatMessage(requestContext, sqlError))
    {
    }

    private void Initialize() => this.EventId = TeamFoundationEventId.TestExecutionObjectNotFoundException;

    private static string FormatMessage(TestExecutionRequestContext context, SqlError sqlError)
    {
      string str = TeamFoundationServiceException.ExtractString(sqlError, "type");
      int num = TeamFoundationServiceException.ExtractInt(sqlError, "id");
      switch (str)
      {
        case "TestAgent":
          return string.Format((IFormatProvider) CultureInfo.CurrentCulture, TestExecutionServiceResources.AgentNotFound, (object) num);
        case "AutomatedTestRunSlice":
          return string.Format((IFormatProvider) CultureInfo.CurrentCulture, TestExecutionServiceResources.InvalidSliceId, (object) num);
        case "AutomatedTestRun":
          return string.Format((IFormatProvider) CultureInfo.CurrentCulture, TestExecutionServiceResources.InvalidTestManagementRunId, (object) num);
        case "TestWorkFlow":
          return string.Format((IFormatProvider) CultureInfo.CurrentCulture, TestExecutionServiceResources.WorkFlowNotFound, (object) num);
        default:
          context.RequestContext.Trace(0, TraceLevel.Error, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.DatabaseLayer, "TestExecutionObjectNotFoundException should have a known object type.");
          return (string) null;
      }
    }
  }
}
