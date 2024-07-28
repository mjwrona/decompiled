// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestManagementInvalidOperationException
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Client;
using System;
using System.Data.SqlClient;
using System.Globalization;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  [Serializable]
  public class TestManagementInvalidOperationException : TestManagementServiceException
  {
    public TestManagementInvalidOperationException(string message)
      : base(message)
    {
    }

    public TestManagementInvalidOperationException(
      IVssRequestContext requestContext,
      SqlException ex,
      SqlError sqlError)
      : base(TestManagementInvalidOperationException.FormatMessage(requestContext, sqlError))
    {
    }

    private static string FormatMessage(IVssRequestContext requestContext, SqlError sqlError)
    {
      switch (TestManagementServiceException.GetTestManagementErrorCode(sqlError))
      {
        case 550005:
          switch ((ObjectTypes) TeamFoundationServiceException.ExtractInt(sqlError, "type"))
          {
            case ObjectTypes.TestRun:
              TestRunState testRunState1 = (TestRunState) TeamFoundationServiceException.ExtractInt(sqlError, "oldState");
              TestRunState testRunState2 = (TestRunState) TeamFoundationServiceException.ExtractInt(sqlError, "newState");
              return string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.IllegalRunStateTransition, (object) testRunState1.ToString(), (object) testRunState2.ToString());
            case ObjectTypes.TestResult:
              TestResultState testResultState1 = (TestResultState) TeamFoundationServiceException.ExtractInt(sqlError, "oldState");
              TestResultState testResultState2 = (TestResultState) TeamFoundationServiceException.ExtractInt(sqlError, "newState");
              return string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.IllegalResultStateTransition, (object) testResultState1.ToString(), (object) testResultState2.ToString());
            default:
              requestContext.TraceAndDebugFail("Exceptions", "Don't know which object has an illegal state transition.");
              return (string) null;
          }
        case 550008:
          ObjectTypes objectTypes = (ObjectTypes) TeamFoundationServiceException.ExtractInt(sqlError, "type");
          string str = TeamFoundationServiceException.ExtractString(sqlError, "name");
          string format;
          switch (objectTypes)
          {
            case ObjectTypes.TestConfiguration:
              format = ServerResources.TestConfigurationAlreadyExists;
              break;
            case ObjectTypes.TestPoint:
              format = ServerResources.TestPointAlreadyExists;
              break;
            case ObjectTypes.TestVariable:
              format = ServerResources.TestVariableAlreadyExists;
              break;
            case ObjectTypes.TestResolutionState:
              format = ServerResources.TestResStateAlreadyExists;
              break;
            case ObjectTypes.TestSettings:
              format = ServerResources.TestSettingsAlreadyExists;
              break;
            case ObjectTypes.TestVariableValue:
              format = ServerResources.ValueAlreadyExistsInVariable;
              break;
            case ObjectTypes.TestConfigurationVariable:
              format = ServerResources.TestVariableAlreadyExistsInConfiguration;
              break;
            case ObjectTypes.Session:
              format = ServerResources.SessionAlreadyExists;
              break;
            case ObjectTypes.TestController:
              format = ServerResources.TestControllerAlreadyExists;
              break;
            case ObjectTypes.DataCollector:
              format = ServerResources.DataCollectorAlreadyExists;
              break;
            case ObjectTypes.TestFailureType:
              format = ServerResources.TestFailureTypeAlreadyExists;
              break;
            default:
              requestContext.TraceAndDebugFail("Exceptions", "TestObjectAlreadyExists should have a valid object type.");
              return (string) null;
          }
          return string.Format((IFormatProvider) CultureInfo.CurrentCulture, format, (object) str);
        case 550012:
          return ServerResources.CannotMoveSuiteEntriesAcrossPlans;
        case 550026:
          return string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TestRunAlreadyCanceled, (object) TeamFoundationServiceException.ExtractInt(sqlError, "id"));
        case 550027:
          return string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.CannotCancelAlreadyCompletedTestRun, (object) TeamFoundationServiceException.ExtractInt(sqlError, "id"), (object) ((TestRunState) TeamFoundationServiceException.ExtractInt(sqlError, "state")).ToString());
        case 550030:
          return string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.MaxTestFlakinessBranchLimitExceeded, (object) TeamFoundationServiceException.ExtractInt(sqlError, "id"));
        default:
          requestContext.TraceAndDebugFail("Exceptions", "TestManagementInvalidOperationException should have a known error code.");
          return (string) null;
      }
    }
  }
}
