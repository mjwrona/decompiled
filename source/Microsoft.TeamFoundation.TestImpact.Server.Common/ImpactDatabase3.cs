// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestImpact.Server.Common.ImpactDatabase3
// Assembly: Microsoft.TeamFoundation.TestImpact.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 079E4AEE-0642-4BDD-8B76-CECF38EBB798
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestImpact.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.TestImpact.Server.Common
{
  [CLSCompliant(false)]
  public class ImpactDatabase3 : ImpactDatabase2
  {
    public ImpactDatabase3()
    {
    }

    internal ImpactDatabase3(string connectionString, IVssRequestContext requestContext)
      : base(connectionString, requestContext)
    {
    }

    public override void PublishTestSignatures(
      Guid projectId,
      int testRunId,
      int testResultId,
      int configurationId,
      int definitionType,
      int definitionId,
      IEnumerable<Microsoft.TeamFoundation.TestImpact.WebApi.Contracts.Signature> signatures,
      string automatedTestName)
    {
      this.PrepareStoredProcedure(TestImpactServiceDatabaseBase.GetDatabaseObjectName("prc_PublishCodeSignatures"));
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId, "TestManagement"));
      this.BindString("@testMethodName", automatedTestName, 512, false, SqlDbType.NVarChar);
      this.BindByte("@definitionType", (byte) definitionType);
      this.BindInt("@definitionId", definitionId);
      this.BindSignatureTable("@signatures", signatures);
      this.ExecuteNonQuery();
    }
  }
}
