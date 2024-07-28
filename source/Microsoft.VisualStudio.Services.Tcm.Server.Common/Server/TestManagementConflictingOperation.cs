// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestManagementConflictingOperation
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
  public class TestManagementConflictingOperation : TestManagementServiceException
  {
    public TestManagementConflictingOperation(
      IVssRequestContext requestContext,
      SqlException ex,
      SqlError sqlError)
      : base(TestManagementConflictingOperation.FormatMessage(requestContext, sqlError))
    {
    }

    private static string FormatMessage(IVssRequestContext requestContext, SqlError sqlError)
    {
      int num1 = TeamFoundationServiceException.ExtractInt(sqlError, "type");
      int num2 = TeamFoundationServiceException.ExtractInt(sqlError, "id");
      if (num1 == 11)
        return string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TestCaseAddtoSuiteFailed, (object) num2);
      requestContext.TraceAndDebugFail("Exceptions", "TestManagementConflictingOperation should have a known type.");
      return (string) null;
    }
  }
}
