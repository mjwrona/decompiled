// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.RegistryComponent4
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class RegistryComponent4 : RegistryComponent3
  {
    public override IEnumerable<RegistryItem> QueryRegistry(
      string registryPath,
      int depth,
      out long sequenceId)
    {
      sequenceId = 0L;
      this.PrepareStoredProcedure("prc_QueryRegistry");
      this.BindString("@registryPath", RegistryComponent.RegistryToDatabasePath(registryPath), 260, false, SqlDbType.NVarChar);
      this.BindByte("@depth", RegistryComponent4.GetSqlDepth(depth));
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "prc_QueryRegistry", this.RequestContext);
      resultCollection.AddBinder<RegistryItem>((ObjectBinder<RegistryItem>) new RegistryComponent.RegistryItemColumns());
      return (IEnumerable<RegistryItem>) resultCollection.GetCurrent<RegistryItem>();
    }

    protected static byte GetSqlDepth(int depth)
    {
      switch (depth)
      {
        case 0:
        case 1:
          return (byte) depth;
        case int.MaxValue:
          return 120;
        default:
          throw new ArgumentOutOfRangeException(nameof (depth));
      }
    }
  }
}
