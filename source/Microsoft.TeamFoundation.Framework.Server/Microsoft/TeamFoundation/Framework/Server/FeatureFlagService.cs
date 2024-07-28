// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.FeatureFlagService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class FeatureFlagService : IVssFrameworkService
  {
    private readonly ConcurrentDictionary<string, VssFeature> _features = new ConcurrentDictionary<string, VssFeature>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private string _stage;
    private static readonly Regex s_featureRegex = new Regex("^/FeatureAvailability/Entries/([\\w\\.]+)/AvailabilityState$", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);
    internal static bool UseNewFeatureService = false;
    internal const string NewFeatureServiceFeatureName = "Microsoft.VisualStudio.Services.FeaturesV2";

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.GetService<IVssRegistryService>().RegisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged), "/FeatureAvailability/Entries/...");
      if (!systemRequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection) || !systemRequestContext.To(TeamFoundationHostType.Deployment).IsFeatureEnabled("VisualStudio.Services.FeatureAvailability.FirstClassCustomerStages"))
        return;
      this._stage = systemRequestContext.GetService<ICustomerStageService>().GetCustomerStage(systemRequestContext);
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext) => systemRequestContext.GetService<IVssRegistryService>().UnregisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged));

    private void OnRegistryChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      foreach (RegistryEntry changedEntry in changedEntries)
      {
        if (!(changedEntry.Name != "AvailabilityState"))
        {
          Match match = FeatureFlagService.s_featureRegex.Match(changedEntry.Path);
          if (match.Success)
          {
            string featureName = match.Groups[1].Value;
            this.GetFeature(requestContext, featureName).SetValue(FeatureFlagService.ToFeatureState(changedEntry.GetValue((string) null)));
          }
        }
      }
    }

    public VssFeature GetFeature(IVssRequestContext requestContext, string featureName)
    {
      if (requestContext.IsVirtualServiceHost())
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Parent);
        return vssRequestContext.GetService<FeatureFlagService>().GetFeature(vssRequestContext, featureName);
      }
      VssFeature feature;
      if (this._features.TryGetValue(featureName, out feature))
        return feature;
      feature = this.CreateFeature(requestContext, featureName);
      return this._features.GetOrAdd(featureName, feature);
    }

    private VssFeature CreateFeature(IVssRequestContext requestContext, string featureName)
    {
      VssFeature parent1 = (VssFeature) null;
      VssFeature parent2 = (VssFeature) null;
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
      {
        IVssRequestContext vssRequestContext1 = requestContext.To(TeamFoundationHostType.Parent);
        parent1 = vssRequestContext1.GetService<FeatureFlagService>().GetFeature(vssRequestContext1, featureName);
        if (!string.IsNullOrEmpty(this._stage))
        {
          IVssRequestContext vssRequestContext2 = requestContext.To(TeamFoundationHostType.Deployment);
          parent2 = vssRequestContext2.GetService<FeatureFlagStageService>().GetFeature(vssRequestContext2, this._stage, featureName);
        }
      }
      string state = requestContext.GetService<IVssRegistryService>().GetValue(requestContext, (RegistryQuery) TeamFoundationFeatureAvailabilityService.GenerateServiceHostAvailabilityStateRegistryPath(featureName), (string) null);
      return new VssFeature(requestContext, featureName, (IFeature<IVssRequestContext>) parent1, (IFeature<IVssRequestContext>) parent2, FeatureFlagService.ToFeatureState(state));
    }

    internal static FeatureState ToFeatureState(string state)
    {
      switch (state)
      {
        case "0":
          return FeatureState.Off;
        case "1":
          return FeatureState.On;
        default:
          return FeatureState.Undefined;
      }
    }
  }
}
