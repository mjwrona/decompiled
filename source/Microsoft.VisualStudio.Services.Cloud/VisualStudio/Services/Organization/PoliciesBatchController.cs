// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Organization.PoliciesBatchController
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cloud;
using Microsoft.VisualStudio.Services.Organization.Client;
using System.Collections.Generic;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Organization
{
  [ControllerApiVersion(3.0)]
  [VersionedApiControllerCustomName(Area = "OrganizationPolicy", ResourceName = "PoliciesBatch")]
  public class PoliciesBatchController : PolicyBaseController
  {
    [HttpGet]
    [TraceFilter(2194020, 2194029)]
    public IDictionary<string, Policy> GetPolicies([ClientParameterAsIEnumerable(typeof (string), ',')] string policyNames, [ClientParameterAsIEnumerable(typeof (string), ',')] string defaultValues)
    {
      PolicyBaseController.ValidateContext(this.TfsRequestContext);
      IList<string> commaSeparatedString1 = PolicyBaseController.ParseCommaSeparatedString(policyNames);
      IList<string> commaSeparatedString2 = PolicyBaseController.ParseCommaSeparatedString(defaultValues);
      PoliciesBatchController.ValidatePolicyNamesAndDefaultValues(commaSeparatedString1, commaSeparatedString2);
      IOrganizationPolicyService service = this.TfsRequestContext.GetService<IOrganizationPolicyService>();
      IDictionary<string, Policy> policies = (IDictionary<string, Policy>) new Dictionary<string, Policy>();
      for (int index = 0; index < commaSeparatedString1.Count; ++index)
      {
        string str = commaSeparatedString1[index];
        string defaultValue = commaSeparatedString2[index];
        Policy<string> policy = service.GetPolicy<string>(this.TfsRequestContext, str, defaultValue);
        policies[str] = policy.ToClient<string>();
      }
      return policies;
    }

    private static void ValidatePolicyNamesAndDefaultValues(
      IList<string> policyNames,
      IList<string> defaultValues)
    {
      if (policyNames.IsNullOrEmpty<string>())
        throw new OrganizationBadRequestException(HostingResources.EmptyPolicyNames());
      if (defaultValues.IsNullOrEmpty<string>())
        throw new OrganizationBadRequestException(HostingResources.EmptyDefaultValues());
      if (policyNames.Count != defaultValues.Count)
        throw new OrganizationBadRequestException(HostingResources.PolicyNamesAndDeafultValuesCountMismatch((object) policyNames.Count, (object) defaultValues.Count));
    }
  }
}
