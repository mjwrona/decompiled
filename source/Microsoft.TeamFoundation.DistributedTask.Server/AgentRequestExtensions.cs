// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.AgentRequestExtensions
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.Azure.Pipelines.PoolProvider.Contracts;
using Microsoft.TeamFoundation.Framework.Server;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  internal static class AgentRequestExtensions
  {
    public const string RepositoryTier = "repositoryTier";
    private const string c_requestTagsSettingsPath = "/Service/DistributedTask/InternalCloudPoolProviderRequestTags";

    private static void AddOrOverwriteAgentSpecificationSetting<T>(
      this AgentRequest request,
      string property,
      T newValue)
    {
      if (request == null)
        return;
      if (request.AgentSpecification == null)
        request.AgentSpecification = new JObject();
      else if (request.AgentSpecification.ContainsKey(property))
        request.AgentSpecification.Remove(property);
      request.AgentSpecification.Add((object) new JProperty(property, (object) newValue));
    }

    public static void AddTierToAgentSpecification(this AgentRequest request, int tier)
    {
      if (tier <= 0)
        return;
      request.AddOrOverwriteAgentSpecificationSetting<string>("ActionsBillingAndAbuseInformation", new Dictionary<string, string>()
      {
        {
          "repositoryTier",
          tier.ToString()
        }
      }.Serialize<Dictionary<string, string>>());
    }

    public static void AddRegionToAgentSpecification(
      this AgentRequest request,
      IVssRequestContext requestContext)
    {
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        return;
      string hostRegion = requestContext.GetService<IHostRegionService>().GetHostRegion(requestContext);
      request.AddOrOverwriteAgentSpecificationSetting<string>("Region", hostRegion);
      if (request != null && request.AgentSpecification != null && request.AgentSpecification.ContainsKey("Region"))
        requestContext.TraceInfo(10015239, nameof (AgentRequestExtensions), "User region setting: " + hostRegion + ", Agent specification region setting: " + request.AgentSpecification.GetValue("Region")?.ToString());
      else
        requestContext.TraceInfo(10015239, nameof (AgentRequestExtensions), "Could not set host region for request");
    }

    public static void AddRequestTags(this AgentRequest request, IVssRequestContext requestContext)
    {
      RegistryEntryCollection registryEntryCollection = requestContext.GetService<IVssRegistryService>().ReadEntries(requestContext, new RegistryQuery("/Service/DistributedTask/InternalCloudPoolProviderRequestTags" + "/*"));
      JArray newValue = new JArray();
      foreach (RegistryEntry registryEntry in registryEntryCollection)
      {
        if (registryEntry.GetValue<bool>(false))
          newValue.Add((JToken) registryEntry.Name);
      }
      if (newValue.Count <= 0)
        return;
      request.AddOrOverwriteAgentSpecificationSetting<JArray>("RequestTags", newValue);
    }
  }
}
