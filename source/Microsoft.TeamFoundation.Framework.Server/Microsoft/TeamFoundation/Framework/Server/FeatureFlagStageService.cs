// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.FeatureFlagStageService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class FeatureFlagStageService : IVssFrameworkService
  {
    private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, VssFeature>> _stages = new ConcurrentDictionary<string, ConcurrentDictionary<string, VssFeature>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private static readonly Regex s_customerStageRegex = new Regex("^/FeatureAvailability/Entries/([\\w\\.]+)/CustomerStages/([^/]+)/AvailabilityState$", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public void ServiceStart(IVssRequestContext deploymentRequestContext)
    {
      deploymentRequestContext.CheckDeploymentRequestContext();
      deploymentRequestContext.GetService<IVssRegistryService>().RegisterNotification(deploymentRequestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged), "/FeatureAvailability/Entries/...");
    }

    public void ServiceEnd(IVssRequestContext deploymentRequestContext)
    {
      deploymentRequestContext.CheckDeploymentRequestContext();
      deploymentRequestContext.GetService<IVssRegistryService>().UnregisterNotification(deploymentRequestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged));
    }

    private void OnRegistryChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      foreach (RegistryEntry changedEntry in changedEntries)
      {
        if (!(changedEntry.Name != "AvailabilityState"))
        {
          Match match = FeatureFlagStageService.s_customerStageRegex.Match(changedEntry.Path);
          if (match.Success)
          {
            string featureName = match.Groups[1].Value;
            string stage = match.Groups[2].Value;
            this.GetFeature(requestContext, stage, featureName).SetValue(FeatureFlagService.ToFeatureState(changedEntry.GetValue((string) null)));
          }
        }
      }
    }

    public VssFeature GetFeature(
      IVssRequestContext requestContext,
      string stage,
      string featureName)
    {
      ConcurrentDictionary<string, VssFeature> orAdd;
      if (!this._stages.TryGetValue(stage, out orAdd))
      {
        ConcurrentDictionary<string, VssFeature> concurrentDictionary = new ConcurrentDictionary<string, VssFeature>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        orAdd = this._stages.GetOrAdd(stage, concurrentDictionary);
      }
      VssFeature feature1;
      if (orAdd.TryGetValue(featureName, out feature1))
        return feature1;
      VssFeature feature2 = this.CreateFeature(requestContext, stage, featureName);
      return orAdd.GetOrAdd(featureName, feature2);
    }

    private VssFeature CreateFeature(
      IVssRequestContext requestContext,
      string stage,
      string featureName)
    {
      string state = requestContext.GetService<IVssRegistryService>().GetValue(requestContext, (RegistryQuery) TeamFoundationFeatureAvailabilityService.GenerateCustomerStageAvailabilityStatesRegistryPath(stage, featureName), (string) null);
      return new VssFeature(requestContext, featureName + "_" + stage, FeatureFlagService.ToFeatureState(state));
    }
  }
}
