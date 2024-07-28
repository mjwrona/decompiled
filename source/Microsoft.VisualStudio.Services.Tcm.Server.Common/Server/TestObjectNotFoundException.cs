// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestObjectNotFoundException
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Client;
using System;
using System.Data.SqlClient;
using System.Globalization;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  [Serializable]
  public class TestObjectNotFoundException : TestManagementServiceException
  {
    public TestObjectNotFoundException(string message, ObjectTypes objectType)
      : base(message)
    {
      this.Initialize(-100, objectType);
    }

    public TestObjectNotFoundException(
      IVssRequestContext requestContext,
      int id,
      ObjectTypes objectType)
      : base(TestObjectNotFoundException.FormatMessage(requestContext, id, objectType))
    {
      this.Initialize(id, objectType);
    }

    public TestObjectNotFoundException(
      IVssRequestContext requestContext,
      SqlException ex,
      SqlError sqlError)
      : base(TestObjectNotFoundException.FormatMessage(requestContext, sqlError))
    {
      ObjectTypes objectType = (ObjectTypes) TeamFoundationServiceException.ExtractInt(sqlError, "type");
      if (objectType == ObjectTypes.TestSettings)
        this.ErrorCode = 5;
      this.Initialize(TeamFoundationServiceException.ExtractInt(sqlError, "id0"), objectType);
    }

    public int Id { get; private set; }

    public ObjectTypes ObjectType { get; private set; }

    public override void GetExceptionProperties(ExceptionPropertyCollection properties)
    {
      base.GetExceptionProperties(properties);
      properties.Set("ID", this.Id);
      properties.Set("OBJECTTYPE", (int) this.ObjectType);
    }

    private void Initialize(int id, ObjectTypes objectType)
    {
      this.Id = id;
      this.ObjectType = objectType;
    }

    private static string FormatMessage(
      IVssRequestContext requestContext,
      int id,
      ObjectTypes objectType)
    {
      switch (objectType)
      {
        case ObjectTypes.TestRun:
          return string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TestRunNotFound, (object) id);
        case ObjectTypes.TestConfiguration:
          return id != 0 ? string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TestConfigurationNotFound, (object) id) : ServerResources.DefaultTestConfigurationNotFound;
        case ObjectTypes.TestPlan:
          return string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TestPlanNotFound, (object) id);
        case ObjectTypes.TestPoint:
          return string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TestPointNotFound, (object) id);
        case ObjectTypes.TestResult:
        case ObjectTypes.TestSuiteEntry:
        case ObjectTypes.TeamProject:
        case ObjectTypes.TestVariableValue:
        case ObjectTypes.TestConfigurationVariable:
        case ObjectTypes.BugFieldMapping:
        case ObjectTypes.TestController:
        case ObjectTypes.DataCollector:
        case ObjectTypes.Other:
          requestContext.TraceAndDebugFail("Exceptions", "TestObjectNotFoundException.FormatMessage should not be called for these object type as they don't have integer ids.");
          return (string) null;
        case ObjectTypes.TestVariable:
          return string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TestVariableNotFound, (object) id);
        case ObjectTypes.TestResolutionState:
          return string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TestResStateNotFound, (object) id);
        case ObjectTypes.TestSettings:
          return string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TestSettingsNotFound, (object) id);
        case ObjectTypes.Attachment:
          return string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TestAttachmentNotFound, (object) id);
        case ObjectTypes.TestSuite:
          return string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TestSuiteNotFound, (object) id);
        case ObjectTypes.Session:
          return string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.SessionNotFound, (object) id);
        case ObjectTypes.TestCase:
          return string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TestCaseNotFound, (object) id);
        case ObjectTypes.SharedSteps:
          return string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.SharedStepsNotFound, (object) id);
        case ObjectTypes.TestFailureType:
          return string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TestFailureTypeNotFound, (object) id);
        case ObjectTypes.TestPlanClone:
          return string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TestPlanCloneNotFound, (object) id);
        case ObjectTypes.TestSuiteClone:
          return string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TestSuiteCloneNotFound, (object) id);
        default:
          requestContext.TraceAndDebugFail("Exceptions", "TestObjectNotFoundException should have a valid object type.");
          return (string) null;
      }
    }

    private static string FormatMessage(IVssRequestContext context, SqlError sqlError)
    {
      int managementErrorCode = TestManagementServiceException.GetTestManagementErrorCode(sqlError);
      ObjectTypes objectType = (ObjectTypes) TeamFoundationServiceException.ExtractInt(sqlError, "type");
      int[] numArray = new int[3]
      {
        TeamFoundationServiceException.ExtractInt(sqlError, "id0"),
        TeamFoundationServiceException.ExtractInt(sqlError, "id1"),
        TeamFoundationServiceException.ExtractInt(sqlError, "id2")
      };
      string str = TeamFoundationServiceException.ExtractString(sqlError, "sid0");
      if (managementErrorCode == 550031)
        return ServerResources.RunSummaryNotComputedException;
      switch (objectType)
      {
        case ObjectTypes.TestResult:
          return string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TestResultNotFound, (object) numArray[0], (object) numArray[1]);
        case ObjectTypes.Attachment:
          return numArray[1] > 0 && numArray[2] > 0 ? string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TestResultAttachmentNotFound, (object) numArray[0], (object) numArray[1], (object) numArray[2]) : string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TestAttachmentNotFound, (object) numArray[0]);
        case ObjectTypes.TestSuiteEntry:
          return ServerResources.TestSuiteEntryNotFound;
        case ObjectTypes.TestController:
          return string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TestControllerNotFound, (object) str);
        case ObjectTypes.DataCollector:
          return string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.DataCollectorNotFound, (object) str);
        case ObjectTypes.CustomTestField:
          return string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.SystemFieldNotFound, (object) str);
        case ObjectTypes.LogStoreStorageAccount:
          return ServerResources.LogStoreStorageAccountNotFound;
        default:
          return TestObjectNotFoundException.FormatMessage(context, numArray[0], objectType);
      }
    }
  }
}
