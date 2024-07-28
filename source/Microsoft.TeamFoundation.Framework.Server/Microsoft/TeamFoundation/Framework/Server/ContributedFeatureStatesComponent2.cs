// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ContributedFeatureStatesComponent2
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class ContributedFeatureStatesComponent2 : ContributedFeatureStatesComponent
  {
    public ContributedFeatureStatesComponent2()
    {
      this.ContainerErrorCode = 50000;
      this.SelectedFeatures = SqlResourceComponentFeatures.None;
    }

    public override List<ContributedFeatureContainer> GetAllContributedFeaturesInfo()
    {
      this.PrepareStoredProcedure("prc_QueryContributedFeatures");
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ContributedFeatureContainer>((ObjectBinder<ContributedFeatureContainer>) new ContributedFeatureColumns());
        return resultCollection.GetCurrent<ContributedFeatureContainer>().Items;
      }
    }
  }
}
