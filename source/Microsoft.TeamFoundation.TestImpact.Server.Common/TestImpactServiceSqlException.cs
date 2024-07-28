// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestImpact.Server.Common.TestImpactServiceSqlException
// Assembly: Microsoft.TeamFoundation.TestImpact.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 079E4AEE-0642-4BDD-8B76-CECF38EBB798
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestImpact.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.TestImpact.Server.Common
{
  [CLSCompliant(false)]
  [Serializable]
  public class TestImpactServiceSqlException : TeamFoundationServiceException
  {
    internal static readonly int TestExecutionObjectAlreadyExistsEventId = TeamFoundationEventId.TestExecutionBaseEventId + 3;

    public TestImpactServiceSqlException() => this.Initialize();

    public TestImpactServiceSqlException(string message)
      : base(message)
    {
      this.Initialize();
    }

    public TestImpactServiceSqlException(string message, Exception innerException)
      : base(message, innerException)
    {
      this.Initialize();
    }

    private void Initialize()
    {
      this.EventId = TeamFoundationEventId.TestExecutionBaseEventId;
      this.LogException = true;
    }

    internal static SqlMessageCode GetTestExecutionErrorCode(SqlError sqlError)
    {
      List<int> ints = TeamFoundationServiceException.ExtractInts(sqlError, "error");
      return ints == null || ints.Count == 0 ? SqlMessageCode.Success : (SqlMessageCode) ints[0];
    }
  }
}
