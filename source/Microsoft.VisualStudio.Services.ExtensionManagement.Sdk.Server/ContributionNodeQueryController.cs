// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.ContributionNodeQueryController
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3FE9DD3E-5758-4D1B-8056-ECAF8A4B7A77
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System.Collections;
using System.Collections.Generic;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server
{
  [VersionedApiControllerCustomName(Area = "Contribution", ResourceName = "ContributionNodeQuery")]
  public class ContributionNodeQueryController : TfsApiController
  {
    public override string TraceArea => "Contributions";

    public override string ActivityLogArea => "Contribution";

    [HttpPost]
    [ClientLocationId("DB7F2146-2309-4CEE-B39C-C767777A1C55")]
    [PublicCollectionRequestRestrictions(false, true, null)]
    public ContributionNodeQueryResult QueryContributionNodes(ContributionNodeQuery query)
    {
      ArgumentUtility.CheckForNull<ContributionNodeQuery>(query, nameof (query));
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) query.ContributionIds, "query.contributionIds");
      this.TfsRequestContext.GetService<IExtensionDataProviderService>().SetRequestDataProviderContext(this.TfsRequestContext, (IDictionary<string, object>) query.DataProviderContext?.Properties);
      string dataspaceId = this.TfsRequestContext.DataspaceIdentifier.ToString();
      ContributionNodeQueryResult contributionNodeQueryResult = new ContributionNodeQueryResult(dataspaceId);
      this.CheckPermission(this.TfsRequestContext, (ISecuredObject) contributionNodeQueryResult);
      ContributionQueryCallback queryCallback = (ContributionQueryCallback) ((requestContext, contribution, parentContribution, relationshipValue, conrtibutionOptions, evaluatedConditions) => query.QueryOptions != ContributionQueryOptions.None ? query.QueryOptions : ContributionQueryOptions.IncludeSelf);
      IContributionService service = this.TfsRequestContext.GetService<IContributionService>();
      IDictionary<string, ContributionNode> dictionary = service.QueryContributions(this.TfsRequestContext, (IEnumerable<string>) query.ContributionIds, ContributionQueryOptions.IncludeSelf, queryCallback);
      foreach (string key in (IEnumerable<string>) dictionary.Keys)
      {
        ContributionNode contributionNode1 = dictionary[key];
        ClientContributionNode contributionNode2 = new ClientContributionNode(dataspaceId)
        {
          Contribution = new ClientContribution(dataspaceId)
          {
            Id = contributionNode1.Contribution.Id,
            Type = contributionNode1.Contribution.Type,
            Description = contributionNode1.Contribution.Description,
            Properties = contributionNode1.Contribution.Properties,
            Includes = contributionNode1.Contribution.Includes,
            Targets = contributionNode1.Contribution.Targets
          }
        };
        contributionNodeQueryResult.Nodes.TryAdd<string, ClientContributionNode>(contributionNode2.Contribution.Id, contributionNode2);
        if (query.IncludeProviderDetails)
        {
          ContributionProviderDetails contributionProviderDetails = service.QueryContributionProviderDetails(this.TfsRequestContext, contributionNode2.Contribution.Id);
          contributionNodeQueryResult.ProviderDetails[contributionProviderDetails.Name] = new ClientContributionProviderDetails(dataspaceId)
          {
            Name = contributionProviderDetails.Name,
            DisplayName = contributionProviderDetails.DisplayName,
            Version = contributionProviderDetails.Version,
            Properties = contributionProviderDetails.Properties
          };
        }
        if (contributionNode1.Children != null)
        {
          foreach (ContributionNode child in contributionNode1.Children)
            contributionNode2.Children.Add(child.Contribution.Id);
        }
        if (contributionNode1.Parents != null)
        {
          foreach (ContributionNode parent in contributionNode1.Parents)
            contributionNode2.Parents.Add(parent.Contribution.Id);
        }
      }
      return contributionNodeQueryResult;
    }

    private void CheckPermission(IVssRequestContext requestContext, ISecuredObject securedObject)
    {
      IVssSecurityNamespace securityNamespace = requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, securedObject.NamespaceId);
      if (securityNamespace == null)
        return;
      securityNamespace.CheckPermission(requestContext, securedObject.GetToken(), securedObject.RequiredPermissions);
    }
  }
}
