// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.RegistryComponent7
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class RegistryComponent7 : RegistryComponent5
  {
    public override long UpdateRegistry(
      string identityName,
      long currentSequenceId,
      IEnumerable<RegistryItem> entriesToUpdate,
      bool overwriteExisting,
      out IList<RegistryUpdateRecord> updateRecords)
    {
      this.PrepareStoredProcedure("prc_UpdateRegistry");
      this.BindKeyValuePairStringTableNullable("@registryUpdates", entriesToUpdate.Select<RegistryItem, KeyValuePair<string, string>>((System.Func<RegistryItem, KeyValuePair<string, string>>) (s => new KeyValuePair<string, string>(RegistryComponent.RegistryToDatabasePath(s.Path), s.Value))));
      this.BindBoolean("@overwriteExisting", overwriteExisting);
      this.BindLong("@currentSequenceId", currentSequenceId);
      if (!string.IsNullOrEmpty(identityName))
        this.BindString("@identityName", identityName, 1024, false, SqlDbType.NVarChar);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<long>((ObjectBinder<long>) new RegistryComponent5.SequenceIdColumns());
        resultCollection.AddBinder<RegistryUpdateRecord>((ObjectBinder<RegistryUpdateRecord>) new RegistryComponent5.RegistryUpdateRecordColumns());
        long num = resultCollection.GetCurrent<long>().First<long>();
        resultCollection.NextResult();
        updateRecords = (IList<RegistryUpdateRecord>) resultCollection.GetCurrent<RegistryUpdateRecord>().Items;
        return num;
      }
    }
  }
}
