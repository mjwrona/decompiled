// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestManagementValidationException
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data.SqlClient;
using System.Globalization;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  [Serializable]
  public class TestManagementValidationException : TestManagementServiceException
  {
    private SqlError m_sqlError;

    public TestManagementValidationException(string message)
      : base(message)
    {
    }

    public TestManagementValidationException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    public TestManagementValidationException(
      IVssRequestContext requestContext,
      SqlException ex,
      SqlError sqlError)
      : base(TestManagementValidationException.FormatMessage(requestContext, sqlError))
    {
      this.m_sqlError = sqlError;
    }

    private static string FormatMessage(IVssRequestContext context, SqlError sqlError)
    {
      switch (TestManagementServiceException.GetTestManagementErrorCode(sqlError))
      {
        case 550009:
          return string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.StartDateAfterEndDateError, (object) TeamFoundationServiceException.ExtractString(sqlError, "startdate"), (object) TeamFoundationServiceException.ExtractString(sqlError, "enddate"));
        case 550013:
          return string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.InvalidSQLArgument, (object) TeamFoundationServiceException.ExtractString(sqlError, "argumentName"), (object) TeamFoundationServiceException.ExtractString(sqlError, "value"));
        case 550018:
          return string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.DeepCopyInvalidOperationId, (object) TeamFoundationServiceException.ExtractInt(sqlError, "ID"));
        case 550019:
          return ServerResources.DeepCopySourceAndDestinationCannotBeInSamePlan;
        default:
          context.TraceAndDebugFail("Exceptions", "TestManagementValidationException should have a known error code.");
          return (string) null;
      }
    }

    internal string GetArgumentName() => this.m_sqlError != null ? TeamFoundationServiceException.ExtractString(this.m_sqlError, "argumentName") : (string) null;
  }
}
