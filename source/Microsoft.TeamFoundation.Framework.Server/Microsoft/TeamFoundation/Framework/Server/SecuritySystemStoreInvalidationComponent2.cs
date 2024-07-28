// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SecuritySystemStoreInvalidationComponent2
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class SecuritySystemStoreInvalidationComponent2 : SecuritySystemStoreInvalidationComponent
  {
    public SecuritySystemStoreInvalidationComponent2() => this.SelectedFeatures = SqlResourceComponentFeatures.BindPartitionIdByDefault;

    public override long InvalidateSystemStore(IVssRequestContext requestContext, string reason)
    {
      this.PrepareStoredProcedure("prc_InvalidateSystemStore");
      this.BindString("@reason", reason, 256, false, SqlDbType.NVarChar);
      return (long) this.ExecuteScalar();
    }
  }
}
