// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Favorites.Server.ServiceAvailabilityHelper
// Assembly: Microsoft.VisualStudio.Services.Favorites.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EC45EF48-58C1-4A3C-8054-12C79AD0CC5B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Favorites.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.FeatureManagement;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.FeatureManagement;
using Microsoft.VisualStudio.Services.Favorites.WebApi;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Favorites.Server
{
  public class ServiceAvailabilityHelper
  {
    public static bool AreServicesEnabled(
      IVssRequestContext requestContext,
      ArtifactScope artifactScope,
      IEnumerable<string> serviceIds)
    {
      if (serviceIds.Count<string>() > 0)
      {
        IContributedFeatureService service = requestContext.GetService<IContributedFeatureService>();
        Dictionary<string, string> dictionary = new Dictionary<string, string>();
        dictionary.Add(artifactScope.Type.ToLower(), artifactScope.Id);
        IVssRequestContext requestContext1 = requestContext;
        IEnumerable<string> contributionIds = serviceIds;
        Dictionary<string, string> scopeValues = dictionary;
        foreach (ContributedFeatureState contributedFeatureState in (IEnumerable<ContributedFeatureState>) service.GetFeatureStates(requestContext1, contributionIds, (IDictionary<string, string>) scopeValues).Values)
        {
          if (contributedFeatureState.State == ContributedFeatureEnabledValue.Disabled)
            return false;
        }
      }
      return true;
    }

    public static bool IsServiceEnabled(
      IVssRequestContext requestContext,
      ArtifactScope artifactScope,
      string serviceId)
    {
      string[] serviceIds = new string[1]{ serviceId };
      return ServiceAvailabilityHelper.AreServicesEnabled(requestContext, artifactScope, (IEnumerable<string>) serviceIds);
    }
  }
}
