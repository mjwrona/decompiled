// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.RegistryComponent2
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class RegistryComponent2 : RegistryComponent
  {
    public RegistryComponent2() => this.SelectedFeatures |= SqlResourceComponentFeatures.BindPartitionIdByDefault;

    public override long UpdateRegistry(
      string identityName,
      long currentSequenceId,
      IEnumerable<RegistryItem> entriesToUpdate,
      bool overwriteExisting,
      out IList<RegistryUpdateRecord> updateRecords)
    {
      if (!overwriteExisting)
        throw new ArgumentOutOfRangeException(nameof (overwriteExisting));
      updateRecords = (IList<RegistryUpdateRecord>) null;
      this.PrepareStoredProcedure("prc_UpdateRegistry");
      this.BindString("@identityName", identityName, 1024, false, SqlDbType.NVarChar);
      this.BindKeyValuePairStringTableNullable("@registryUpdates", entriesToUpdate.Select<RegistryItem, KeyValuePair<string, string>>((System.Func<RegistryItem, KeyValuePair<string, string>>) (s => new KeyValuePair<string, string>(RegistryComponent.RegistryToDatabasePath(s.Path), s.Value))));
      this.BindBoolean("@logRegistryChanges", false);
      this.ExecuteNonQuery();
      return 0;
    }
  }
}
