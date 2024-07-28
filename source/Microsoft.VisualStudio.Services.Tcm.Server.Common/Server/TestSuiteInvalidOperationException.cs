// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestSuiteInvalidOperationException
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
  public class TestSuiteInvalidOperationException : TestManagementInvalidOperationException
  {
    public TestSuiteInvalidOperationException(string message)
      : base(message)
    {
    }

    public TestSuiteInvalidOperationException(
      IVssRequestContext requestContext,
      SqlException ex,
      SqlError sqlError)
      : base(TestSuiteInvalidOperationException.FormatMessage(requestContext, sqlError))
    {
      if (TestManagementServiceException.GetTestManagementErrorCode(sqlError) != 550007)
        return;
      if (TeamFoundationServiceException.ExtractInt(sqlError, "type") == 1)
        this.ErrorCode = 4;
      else
        this.ErrorCode = 2;
    }

    private static string FormatMessage(IVssRequestContext requestContext, SqlError sqlError)
    {
      switch (TestManagementServiceException.GetTestManagementErrorCode(sqlError))
      {
        case 550006:
          return ServerResources.CannotMoveChildSuiteBelow;
        case 550007:
          int num = TeamFoundationServiceException.ExtractInt(sqlError, "suiteid");
          return TeamFoundationServiceException.ExtractInt(sqlError, "type") == 1 ? string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.DuplicateTestCaseInSuite, (object) num) : string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.DuplicateSuiteName, (object) num);
        case 550010:
          return ServerResources.ParentSuiteNotFound;
        case 550011:
          return ServerResources.SuiteDepthOverLimit;
        case 550029:
          return string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TestConfigurationInactive, (object) TeamFoundationServiceException.ExtractInt(sqlError, "id"));
        default:
          requestContext.TraceAndDebugFail("Exceptions", "TestSuiteInvalidOperationException should have a known error code.");
          return (string) null;
      }
    }
  }
}
