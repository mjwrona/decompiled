// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.InstallationComponent
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class InstallationComponent : TeamFoundationSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[3]
    {
      (IComponentCreator) new ComponentCreator<InstallationComponent>(1, true),
      (IComponentCreator) new ComponentCreator<InstallationComponent2>(2),
      (IComponentCreator) new ComponentCreator<InstallationComponent3>(3)
    }, "VCInstallation");

    public InstallationComponent() => this.SelectedFeatures = SqlResourceComponentFeatures.BindPartitionIdByDefault;

    public virtual void InstallData(Guid ownerId)
    {
      this.PrepareStoredProcedure("prc_InstallVC");
      this.BindGuid("@adminId", ownerId);
      this.ExecuteNonQuery();
    }
  }
}
