// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestObjectInUseException
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
  public class TestObjectInUseException : TestManagementServiceException
  {
    public TestObjectInUseException(string message)
      : base(message)
    {
    }

    public TestObjectInUseException(
      IVssRequestContext requestContext,
      SqlException ex,
      SqlError sqlError)
      : base(TestObjectInUseException.FormatMessage(requestContext, sqlError))
    {
    }

    private static string FormatMessage(IVssRequestContext requestContext, SqlError sqlError)
    {
      ObjectTypes objectTypes = (ObjectTypes) TeamFoundationServiceException.ExtractInt(sqlError, "type");
      int num = TeamFoundationServiceException.ExtractInt(sqlError, "id");
      switch (objectTypes)
      {
        case ObjectTypes.TestConfiguration:
          return string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TestConfigurationInUse, (object) num);
        case ObjectTypes.TestPlan:
          return string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TestPlanInUse, (object) num);
        case ObjectTypes.TestVariable:
          return string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TestVariableInUse, (object) num);
        case ObjectTypes.TestResolutionState:
          return string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TestResStateInUse, (object) num);
        case ObjectTypes.TestSettings:
          return string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TestSettingsInUse, (object) num);
        case ObjectTypes.TestVariableValue:
          return string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TestVariableValueInUse, (object) num);
        case ObjectTypes.TestFailureType:
          return string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TestFailureTypeInUse, (object) num);
        default:
          requestContext.TraceAndDebugFail("Exceptions", "ObjectInUseException should have an object type.");
          return (string) null;
      }
    }
  }
}
