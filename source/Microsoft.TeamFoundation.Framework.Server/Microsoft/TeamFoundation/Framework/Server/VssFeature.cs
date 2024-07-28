// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.VssFeature
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class VssFeature : Feature<IVssRequestContext>
  {
    private readonly string _contextItemsKey;

    public VssFeature(
      IVssRequestContext requestContext,
      string featureName,
      IFeature<IVssRequestContext> parent1,
      IFeature<IVssRequestContext> parent2,
      FeatureState initialValue)
      : base(parent1, parent2, initialValue)
    {
      this._contextItemsKey = VssFeature.GetContextItemsKey(requestContext, featureName);
    }

    public VssFeature(
      IVssRequestContext requestContext,
      string featureName,
      IFeature<IVssRequestContext> parent,
      FeatureState initialValue)
      : base(parent, initialValue)
    {
      this._contextItemsKey = VssFeature.GetContextItemsKey(requestContext, featureName);
    }

    public VssFeature(
      IVssRequestContext requestContext,
      string featureName,
      FeatureState initialValue)
      : base(initialValue)
    {
      this._contextItemsKey = VssFeature.GetContextItemsKey(requestContext, featureName);
    }

    internal static string GetContextItemsKey(IVssRequestContext requestContext, string feature) => string.Format("FFState_{0}_{1}", (object) feature, (object) (int) requestContext.ServiceHost.HostType);

    public override FeatureState GetFeatureState(IVssRequestContext context)
    {
      object obj;
      if (context.Items.TryGetValue(this._contextItemsKey, out obj) && obj is FeatureState featureState1)
        return featureState1;
      FeatureState featureState2 = base.GetFeatureState(context);
      context.Items[this._contextItemsKey] = (object) featureState2;
      return featureState2;
    }
  }
}
