// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestExecution.Server.TestExecutionServiceSqlException
// Assembly: Microsoft.VisualStudio.Services.TE.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BC2680F-A5FB-41BE-A4CF-F78BF7AC3E02
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.TE.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Server;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.TestExecution.Server
{
  [CLSCompliant(false)]
  [Serializable]
  public class TestExecutionServiceSqlException : TeamFoundationServiceException
  {
    internal static readonly int TestExecutionObjectAlreadyExistsEventId = TeamFoundationEventId.TestExecutionBaseEventId + 3;

    public TestExecutionServiceSqlException() => this.Initialize();

    public TestExecutionServiceSqlException(string message)
      : base(message)
    {
      this.Initialize();
    }

    public TestExecutionServiceSqlException(string message, Exception innerException)
      : base(message, innerException)
    {
      this.Initialize();
    }

    private void Initialize()
    {
      this.EventId = TeamFoundationEventId.TestExecutionBaseEventId;
      this.LogException = true;
    }

    public static SqlMessageCode GetTestExecutionErrorCode(SqlError sqlError)
    {
      List<int> ints = TeamFoundationServiceException.ExtractInts(sqlError, "error");
      return ints == null || ints.Count == 0 ? SqlMessageCode.Success : (SqlMessageCode) ints[0];
    }
  }
}
