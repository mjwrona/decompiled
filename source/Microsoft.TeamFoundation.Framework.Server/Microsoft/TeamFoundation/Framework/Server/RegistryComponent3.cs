// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.RegistryComponent3
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class RegistryComponent3 : RegistryComponent2
  {
    public override IEnumerable<RegistryItem> QueryRegistry(
      string registryPath,
      int depth,
      out long sequenceId)
    {
      sequenceId = 0L;
      this.PrepareStoredProcedure("prc_QueryRegistry");
      this.BindString("@registryPath", RegistryComponent.RegistryToDatabasePath(registryPath), 260, false, SqlDbType.NVarChar);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "prc_QueryRegistry", this.RequestContext);
      resultCollection.AddBinder<RegistryItem>((ObjectBinder<RegistryItem>) new RegistryComponent.RegistryItemColumns());
      return (IEnumerable<RegistryItem>) resultCollection.GetCurrent<RegistryItem>();
    }

    public override IEnumerable<RegistryAuditEntry> QueryAuditLog(
      string registryPath,
      long changeIndex,
      bool returnOlder,
      int pageSize)
    {
      RegistryComponent3 registryComponent3 = this;
      registryComponent3.PrepareStoredProcedure("prc_QueryRegistryAuditLog");
      registryComponent3.BindString("@registryPath", RegistryComponent.RegistryToDatabasePath(registryPath), 260, false, SqlDbType.NVarChar);
      registryComponent3.BindLong("@changeIndex", changeIndex);
      registryComponent3.BindBoolean("@returnOlder", returnOlder);
      registryComponent3.BindInt("@pageSize", pageSize);
      using (ResultCollection result = new ResultCollection((IDataReader) registryComponent3.ExecuteReader(), "prc_QueryRegistryAuditLog", registryComponent3.RequestContext))
      {
        result.AddBinder<RegistryAuditEntry>((ObjectBinder<RegistryAuditEntry>) new RegistryComponent.RegistryAuditEntryColumns());
        foreach (RegistryAuditEntry registryAuditEntry in result.GetCurrent<RegistryAuditEntry>())
          yield return registryAuditEntry;
      }
    }
  }
}
