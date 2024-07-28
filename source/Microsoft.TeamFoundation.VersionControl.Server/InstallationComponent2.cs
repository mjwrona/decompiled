// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.InstallationComponent2
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using System;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class InstallationComponent2 : InstallationComponent
  {
    public override void InstallData(Guid ownerId)
    {
      this.PrepareStoredProcedure("prc_InstallVC");
      this.BindGuid("@adminId", ownerId);
      this.BindInt("@serviceDataspaceId", this.GetDataspaceId(Guid.Empty));
      this.BindInt("@rootItemDataspaceId", this.GetDataspaceId(Guid.Empty, "VersionControl"));
      this.ExecuteNonQuery();
    }
  }
}
