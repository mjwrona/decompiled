// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestManagementDatabase75
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class TestManagementDatabase75 : TestManagementDatabase74
  {
    public override long UpdateLogStoreContentSizeByRelease(
      Guid projectId,
      int releaseId,
      int fieldId,
      int stateFieldId,
      int stateFieldValue,
      int newStateFieldValue,
      bool isDeleted = false)
    {
      try
      {
        this.RequestContext.TraceEnter(0, "TestManagement", "Database", nameof (UpdateLogStoreContentSizeByRelease));
        this.PrepareStoredProcedure("prc_UpdateLogStoreContentSizeByRelease");
        this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
        this.BindInt("@releaseId", releaseId);
        this.BindInt("@fieldId", fieldId);
        this.BindInt("@stateFieldId", stateFieldId);
        this.BindInt("@stateFieldValue", stateFieldValue);
        this.BindInt("@newStateFieldValue", newStateFieldValue);
        this.BindBoolean("@isDeleted", isDeleted);
        SqlDataReader reader = this.ExecuteReader();
        SqlColumnBinder sqlColumnBinder = new SqlColumnBinder("TotalBlobSize");
        if (reader.Read())
          return sqlColumnBinder.GetInt64((IDataReader) reader);
      }
      finally
      {
        this.RequestContext.TraceLeave(0, "TestManagement", "Database", nameof (UpdateLogStoreContentSizeByRelease));
      }
      return 0;
    }

    internal TestManagementDatabase75(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestManagementDatabase75()
    {
    }
  }
}
