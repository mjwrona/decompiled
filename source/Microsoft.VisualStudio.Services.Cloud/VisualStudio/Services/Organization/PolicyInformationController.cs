// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Organization.PolicyInformationController
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Organization
{
  [ControllerApiVersion(3.0)]
  [VersionedApiControllerCustomName(Area = "OrganizationPolicy", ResourceName = "PolicyInformation")]
  public class PolicyInformationController : PolicyBaseController
  {
    [HttpGet]
    [TraceFilter(2194100, 2194109)]
    public Microsoft.VisualStudio.Services.Organization.Client.PolicyInfo GetPolicyInformation(
      string policyName)
    {
      PolicyBaseController.ValidateContext(this.TfsRequestContext);
      return this.TfsRequestContext.GetService<IOrganizationPolicyService>().GetPolicyInfo(this.TfsRequestContext, policyName).ToClient();
    }

    [HttpGet]
    [TraceFilter(2194110, 2194119)]
    public IDictionary<string, Microsoft.VisualStudio.Services.Organization.Client.PolicyInfo> GetPolicyInformations(
      [ClientParameterAsIEnumerable(typeof (string), ',')] string policyNames = null)
    {
      PolicyBaseController.ValidateContext(this.TfsRequestContext);
      IList<string> commaSeparatedString = PolicyBaseController.ParseCommaSeparatedString(policyNames);
      IOrganizationPolicyService service = this.TfsRequestContext.GetService<IOrganizationPolicyService>();
      IDictionary<string, Microsoft.VisualStudio.Services.Organization.Client.PolicyInfo> policyInformations = (IDictionary<string, Microsoft.VisualStudio.Services.Organization.Client.PolicyInfo>) new Dictionary<string, Microsoft.VisualStudio.Services.Organization.Client.PolicyInfo>();
      foreach (string str in (IEnumerable<string>) commaSeparatedString)
      {
        PolicyInfo policyInfo = service.GetPolicyInfo(this.TfsRequestContext, str);
        policyInformations[str] = policyInfo.ToClient();
      }
      return policyInformations;
    }
  }
}
