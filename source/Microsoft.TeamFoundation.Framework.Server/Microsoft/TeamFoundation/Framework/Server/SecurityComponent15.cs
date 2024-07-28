// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SecurityComponent15
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class SecurityComponent15 : SecurityComponent14
  {
    public override ResultCollection QuerySecurityData(
      Guid namespaceId,
      bool usesInheritInformation,
      int lastSyncId,
      char separator = '\0')
    {
      this.PrepareStoredProcedure("prc_QuerySecurityInfo");
      this.BindServiceHostId();
      this.BindGuid("@namespaceGuid", namespaceId);
      this.BindBoolean("@getInheritInformation", usesInheritInformation);
      this.BindInt("@lastSyncId", lastSyncId);
      this.BindSeparator("@separator", separator);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "prc_QuerySecurityInfo", this.RequestContext);
      resultCollection.AddBinder<DatabaseAccessControlEntry>((ObjectBinder<DatabaseAccessControlEntry>) this.GetDatabaseAccessControlEntryColumns());
      if (usesInheritInformation)
        resultCollection.AddBinder<string>((ObjectBinder<string>) this.GetNoInheritColumns());
      resultCollection.AddBinder<int>((ObjectBinder<int>) new SecurityComponent.ChangeIdColumns());
      return resultCollection;
    }
  }
}
