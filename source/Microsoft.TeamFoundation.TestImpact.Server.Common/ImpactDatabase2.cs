// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestImpact.Server.Common.ImpactDatabase2
// Assembly: Microsoft.TeamFoundation.TestImpact.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 079E4AEE-0642-4BDD-8B76-CECF38EBB798
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestImpact.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestImpact.WebApi.Contracts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.TestImpact.Server.Common
{
  [CLSCompliant(false)]
  public class ImpactDatabase2 : ImpactDatabase
  {
    public ImpactDatabase2()
    {
    }

    internal ImpactDatabase2(string connectionString, IVssRequestContext requestContext)
      : base(connectionString, requestContext)
    {
    }

    public override ImpactedTests QueryAllTests(
      Guid projectId,
      int definitionType,
      int definitionId,
      int runId)
    {
      this.PrepareStoredProcedure(TestImpactServiceDatabaseBase.GetDatabaseObjectName("prc_QueryAllTests"));
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId, "TestManagement"));
      this.BindByte("@definitionType", (byte) definitionType);
      this.BindInt("@definitionId", definitionId);
      ImpactedTests impactedTests = new ImpactedTests();
      impactedTests.Tests = new List<Microsoft.TeamFoundation.TestImpact.WebApi.Contracts.Test>();
      ImpactDatabase.QueryAllTestsColumn queryAllTestsColumn = new ImpactDatabase.QueryAllTestsColumn();
      using (SqlDataReader reader = this.ExecuteReader())
      {
        if (reader.HasRows)
        {
          while (reader.Read())
          {
            string str = queryAllTestsColumn.TestMethodName.GetString((IDataReader) reader, false);
            impactedTests.Tests.Add(new Microsoft.TeamFoundation.TestImpact.WebApi.Contracts.Test()
            {
              TestName = str
            });
          }
        }
      }
      return impactedTests;
    }
  }
}
