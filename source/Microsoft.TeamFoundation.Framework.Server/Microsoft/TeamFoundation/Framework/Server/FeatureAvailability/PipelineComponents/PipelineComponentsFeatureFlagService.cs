// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.FeatureAvailability.PipelineComponents.PipelineComponentsFeatureFlagService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server.FeatureAvailability.PipelineComponents
{
  internal class PipelineComponentsFeatureFlagService : 
    IPipelineComponentsFeatureFlagService,
    IVssFrameworkService
  {
    private static readonly RegistryQuery s_distributedTaskFlagsQuery = new RegistryQuery("/FeatureAvailability/Entries/**");

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public IEnumerable<FeatureAvailabilityInformation> GetFeatures(
      IVssRequestContext requestContext,
      PipelineComponentType componentType)
    {
      string pattern;
      if (!PipelineComponentsFeatureFlagService.TryGetFeaturesNamePattern(componentType, out pattern))
        return Enumerable.Empty<FeatureAvailabilityInformation>();
      Dictionary<string, FeatureAvailabilityInformation> filteredFeatures = PipelineComponentsFeatureFlagService.GetFilteredFeatures(requestContext.To(TeamFoundationHostType.Deployment), pattern);
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        return (IEnumerable<FeatureAvailabilityInformation>) filteredFeatures.Values;
      foreach (FeatureAvailabilityInformation availabilityInformation1 in PipelineComponentsFeatureFlagService.GetFilteredFeatures(requestContext, pattern).Values)
      {
        FeatureAvailabilityInformation availabilityInformation2;
        if (filteredFeatures.TryGetValue(availabilityInformation1.Name, out availabilityInformation2))
        {
          if (availabilityInformation1.ExplicitState == FeatureAvailabilityState.Off || availabilityInformation2.ExplicitState == FeatureAvailabilityState.Off)
          {
            availabilityInformation2.EffectiveState = FeatureAvailabilityState.Off;
            availabilityInformation2.ExplicitState = FeatureAvailabilityState.Off;
          }
          else if (availabilityInformation1.ExplicitState != availabilityInformation2.ExplicitState)
            filteredFeatures[availabilityInformation1.Name] = availabilityInformation1;
        }
        else
          filteredFeatures.Add(availabilityInformation1.Name, availabilityInformation1);
      }
      return (IEnumerable<FeatureAvailabilityInformation>) filteredFeatures.Values;
    }

    private static Dictionary<string, FeatureAvailabilityInformation> GetFilteredFeatures(
      IVssRequestContext requestContext,
      string featuresNamePattern)
    {
      return requestContext.GetService<IVssRegistryService>().Read(requestContext, in PipelineComponentsFeatureFlagService.s_distributedTaskFlagsQuery).Select<RegistryItem, FeatureAvailabilityInformation>((Func<RegistryItem, FeatureAvailabilityInformation>) (ri => new FeatureAvailabilityInformation()
      {
        Name = ((IEnumerable<string>) ri.Path.Split('/')).Skip<string>(3).First<string>(),
        EffectiveState = ri.Value == "1" ? FeatureAvailabilityState.On : FeatureAvailabilityState.Off,
        ExplicitState = PipelineComponentsFeatureFlagService.ToFeatureState(ri.Value)
      })).Where<FeatureAvailabilityInformation>((Func<FeatureAvailabilityInformation, bool>) (f => f.Name.StartsWith(featuresNamePattern))).ToDictionary<FeatureAvailabilityInformation, string, FeatureAvailabilityInformation>((Func<FeatureAvailabilityInformation, string>) (f => f.Name), (Func<FeatureAvailabilityInformation, FeatureAvailabilityInformation>) (f => f));
    }

    private static bool TryGetFeaturesNamePattern(
      PipelineComponentType componentType,
      out string pattern)
    {
      if (componentType != PipelineComponentType.Agent)
      {
        if (componentType == PipelineComponentType.Tasks)
        {
          pattern = "DistributedTask.Tasks.";
          return true;
        }
        pattern = (string) null;
        return false;
      }
      pattern = "DistributedTask.Agent.";
      return true;
    }

    private static FeatureAvailabilityState ToFeatureState(string state)
    {
      FeatureAvailabilityState featureState;
      switch (state)
      {
        case "0":
          featureState = FeatureAvailabilityState.Off;
          break;
        case "1":
          featureState = FeatureAvailabilityState.On;
          break;
        default:
          featureState = FeatureAvailabilityState.Undefined;
          break;
      }
      return featureState;
    }
  }
}
