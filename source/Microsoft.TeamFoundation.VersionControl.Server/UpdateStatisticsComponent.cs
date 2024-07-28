// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.UpdateStatisticsComponent
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class UpdateStatisticsComponent : VersionControlSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[2]
    {
      (IComponentCreator) new ComponentCreator<UpdateStatisticsComponent>(1, true),
      (IComponentCreator) new ComponentCreator<UpdateStatisticsComponent2>(2)
    }, "VCUpdateStatistics");

    public UpdateStatisticsComponent() => this.SelectedFeatures = SqlResourceComponentFeatures.None;

    public virtual int UpdateStatistics(int rowCountThreshold) => 0;
  }
}
