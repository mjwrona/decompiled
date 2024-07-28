// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.AgentCloudTypesService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  public class AgentCloudTypesService : IAgentCloudTypesService, IVssFrameworkService
  {
    public const string AgentCloudTypes = "ms.vss-build.agentcloud-types";
    private const string c_layer = "AgentCloudTypesService";

    public IEnumerable<TaskAgentCloudType> GetAgentCloudTypes(IVssRequestContext requestContext)
    {
      using (new MethodScope(requestContext, nameof (AgentCloudTypesService), nameof (GetAgentCloudTypes)))
      {
        IEnumerable<Contribution> contributions = requestContext.GetService<IContributionService>().QueryContributionsForTarget(requestContext, "ms.vss-build.agentcloud-types");
        HashSet<string> stringSet1 = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        HashSet<string> stringSet2 = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        foreach (Contribution contribution in contributions)
        {
          if (string.Equals(contribution.Id.Split('.')[0], "ms", StringComparison.OrdinalIgnoreCase))
          {
            stringSet1.Add(contribution.Id);
            stringSet2.Add(this.GetContributionName(contribution));
          }
        }
        IList<TaskAgentCloudType> agentCloudTypes = (IList<TaskAgentCloudType>) new List<TaskAgentCloudType>();
        foreach (Contribution contribution in contributions)
        {
          if ((stringSet1.Contains(contribution.Id) || !stringSet2.Contains(this.GetContributionName(contribution))) && this.IsFeatureAvailable(requestContext, contribution))
          {
            TaskAgentCloudType taskAgentCloudType = contribution.ToTaskAgentCloudType(requestContext);
            agentCloudTypes.Add(taskAgentCloudType);
          }
        }
        return (IEnumerable<TaskAgentCloudType>) agentCloudTypes;
      }
    }

    public string GetContributionName(Contribution contribution) => AgentCloudContributionExtensions.GetRequiredValue<string>(contribution.Properties, "name");

    public bool IsFeatureAvailable(IVssRequestContext requestContext, Contribution contribution)
    {
      string optionalValue = AgentCloudContributionExtensions.GetOptionalValue<string>(contribution.Properties, "featureFlag");
      return optionalValue == null || requestContext.GetService<ITeamFoundationFeatureAvailabilityService>().IsFeatureEnabled(requestContext, optionalValue);
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext requestContext)
    {
      if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        return;
      requestContext.CheckProjectCollectionRequestContext();
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext requestContext)
    {
    }
  }
}
