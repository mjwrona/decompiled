// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ContributedFeatureStatesComponent
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class ContributedFeatureStatesComponent : TeamFoundationSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[2]
    {
      (IComponentCreator) new ComponentCreator<ContributedFeatureStatesComponent>(1, true),
      (IComponentCreator) new ComponentCreator<ContributedFeatureStatesComponent2>(2)
    }, nameof (ContributedFeatureStatesComponent));

    public ContributedFeatureStatesComponent()
    {
      this.ContainerErrorCode = 50000;
      this.SelectedFeatures = SqlResourceComponentFeatures.None;
    }

    public virtual List<ContributedFeatureContainer> GetAllContributedFeaturesInfo() => new List<ContributedFeatureContainer>();
  }
}
