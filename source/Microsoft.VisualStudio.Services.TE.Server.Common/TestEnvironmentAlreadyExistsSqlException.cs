// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestExecution.Server.TestEnvironmentAlreadyExistsSqlException
// Assembly: Microsoft.VisualStudio.Services.TE.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BC2680F-A5FB-41BE-A4CF-F78BF7AC3E02
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.TE.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Server;
using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;

namespace Microsoft.TeamFoundation.TestExecution.Server
{
  [CLSCompliant(false)]
  [Serializable]
  public class TestEnvironmentAlreadyExistsSqlException : TestExecutionServiceSqlException
  {
    public TestEnvironmentAlreadyExistsSqlException() => this.Initialize();

    public TestEnvironmentAlreadyExistsSqlException(string errorMessage)
      : base(errorMessage)
    {
      this.Initialize();
    }

    public TestEnvironmentAlreadyExistsSqlException(
      TestExecutionRequestContext requestContext,
      SqlException ex,
      SqlError sqlError)
      : this(TestEnvironmentAlreadyExistsSqlException.FormatMessage(requestContext, sqlError))
    {
    }

    private void Initialize() => this.EventId = TeamFoundationEventId.TestEnvironmentAlreadyExistsException;

    private static string FormatMessage(
      TestExecutionRequestContext requestContext,
      SqlError sqlError)
    {
      if (TestExecutionServiceSqlException.GetTestExecutionErrorCode(sqlError) == SqlMessageCode.TestEnvironmentAlreadyExists)
        return string.Format((IFormatProvider) CultureInfo.CurrentCulture, TestExecutionServiceResources.TestEnvironmentAlreadyExistsException, (object) TeamFoundationServiceException.ExtractString(sqlError, "environmentUrl"));
      requestContext.RequestContext.Trace(0, TraceLevel.Error, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.DatabaseLayer, "TestEnvironmentAlreadyExistsException should have a known error code.");
      return (string) null;
    }
  }
}
