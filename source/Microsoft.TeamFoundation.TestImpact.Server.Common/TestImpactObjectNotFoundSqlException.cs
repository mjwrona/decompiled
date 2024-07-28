// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestImpact.Server.Common.TestImpactObjectNotFoundSqlException
// Assembly: Microsoft.TeamFoundation.TestImpact.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 079E4AEE-0642-4BDD-8B76-CECF38EBB798
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestImpact.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data.SqlClient;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.TestImpact.Server.Common
{
  [CLSCompliant(false)]
  [Serializable]
  public class TestImpactObjectNotFoundSqlException : TestImpactServiceSqlException
  {
    public TestImpactObjectNotFoundSqlException(string errorMessage)
      : base(errorMessage)
    {
      this.Initialize();
    }

    public TestImpactObjectNotFoundSqlException(
      IVssRequestContext requestContext,
      SqlException ex,
      SqlError sqlError)
      : this(TestImpactObjectNotFoundSqlException.FormatMessage(requestContext, sqlError))
    {
    }

    private void Initialize() => this.EventId = TeamFoundationEventId.TestExecutionObjectNotFoundException;

    private static string FormatMessage(IVssRequestContext context, SqlError sqlError)
    {
      TeamFoundationServiceException.ExtractString(sqlError, "type");
      TeamFoundationServiceException.ExtractInt(sqlError, "id");
      context.Trace(0, TraceLevel.Error, TestImpactServiceConstants.TestImpactArea, TestImpactServiceConstants.DataBaseLayer, "TestImpactObjectNotFoundSqlException should have a known object type.");
      return (string) null;
    }
  }
}
