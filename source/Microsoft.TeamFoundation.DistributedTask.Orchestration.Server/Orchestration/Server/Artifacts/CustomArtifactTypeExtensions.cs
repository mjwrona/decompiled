// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.CustomArtifactTypeExtensions
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Newtonsoft.Json.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts
{
  public static class CustomArtifactTypeExtensions
  {
    public static bool IsFeatureAvailable(
      this Contribution contribution,
      IVssRequestContext requestContext)
    {
      string optionalValue = contribution.Properties.GetOptionalValue<string>("featureFlag");
      return optionalValue == null || requestContext.GetService<ITeamFoundationFeatureAvailabilityService>().IsFeatureEnabled(requestContext, optionalValue);
    }

    private static T GetOptionalValue<T>(this JObject jo, string propertyName, T defaultValue = null)
    {
      JToken jtoken;
      return jo.TryGetValue(propertyName, out jtoken) ? jtoken.ToObject<T>() : defaultValue;
    }
  }
}
