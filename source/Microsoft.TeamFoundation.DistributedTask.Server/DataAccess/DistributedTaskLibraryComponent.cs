// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.DistributedTaskLibraryComponent
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal class DistributedTaskLibraryComponent : TaskSqlComponentBase
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[2]
    {
      (IComponentCreator) new ComponentCreator<DistributedTaskLibraryComponent>(1, true),
      (IComponentCreator) new ComponentCreator<DistributedTaskLibraryComponent2>(2)
    }, nameof (DistributedTaskLibraryComponent), "DistributedTask");

    public DistributedTaskLibraryComponent() => this.SelectedFeatures |= SqlResourceComponentFeatures.BindPartitionIdByDefault;

    public virtual void DeleteTeamProject(Guid projectId)
    {
    }
  }
}
