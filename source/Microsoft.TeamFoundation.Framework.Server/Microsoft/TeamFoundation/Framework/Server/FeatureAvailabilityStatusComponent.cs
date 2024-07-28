// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.FeatureAvailabilityStatusComponent
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class FeatureAvailabilityStatusComponent : TeamFoundationSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[3]
    {
      (IComponentCreator) new ComponentCreator<FeatureAvailabilityStatusComponent>(1, true),
      (IComponentCreator) new ComponentCreator<FeatureAvailabilityStatusComponent>(2),
      (IComponentCreator) new ComponentCreator<FeatureAvailabilityStatusComponent>(3)
    }, nameof (FeatureAvailabilityStatusComponent));

    public FeatureAvailabilityStatusComponent()
    {
      this.ContainerErrorCode = 50000;
      this.SelectedFeatures = SqlResourceComponentFeatures.None;
    }

    public virtual List<FeatureFlagAvailabilityContainer> GetAllFeatureFlagOverrideInfo()
    {
      this.PrepareStoredProcedure("prc_QueryFeatureFlagOverrides");
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<FeatureFlagAvailabilityContainer>((ObjectBinder<FeatureFlagAvailabilityContainer>) new FeatureFlagAvailabilityColumns());
        return resultCollection.GetCurrent<FeatureFlagAvailabilityContainer>().Items;
      }
    }
  }
}
