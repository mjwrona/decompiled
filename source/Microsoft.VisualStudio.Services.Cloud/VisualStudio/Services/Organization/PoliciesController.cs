// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Organization.PoliciesController
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Organization.Client;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using System;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Organization
{
  [ControllerApiVersion(3.0)]
  [VersionedApiControllerCustomName(Area = "OrganizationPolicy", ResourceName = "Policies")]
  [ClientInternalUseOnly(false)]
  public class PoliciesController : PolicyBaseController
  {
    [HttpGet]
    [TraceFilter(2194000, 2194009)]
    public Policy GetPolicy(string policyName, string defaultValue)
    {
      PolicyBaseController.ValidateContext(this.TfsRequestContext);
      return this.TfsRequestContext.GetService<IOrganizationPolicyService>().GetPolicy<string>(this.TfsRequestContext, policyName, defaultValue).ToClient<string>();
    }

    [HttpPatch]
    [TraceFilter(2194010, 2194019)]
    [ClientRequestContentMediaType("application/json-patch+json")]
    [RequestContentTypeRestriction(AllowJson = false)]
    public void UpdatePolicy(string policyName, [ClientParameterType(typeof (JsonPatchDocument), false)] PatchDocument<Policy> patchDocument)
    {
      PolicyBaseController.ValidateContext(this.TfsRequestContext);
      JsonPatchDocumentHelper.ValidateUpdatePatchDocument<Policy>(patchDocument);
      IOrganizationPolicyService service = this.TfsRequestContext.GetService<IOrganizationPolicyService>();
      foreach (IPatchOperation<Policy> operation in patchDocument.Operations)
      {
        string operationPath = JsonPatchDocumentHelper.ParseOperationPath(operation.Path);
        if ("Value".Equals(operationPath, StringComparison.InvariantCultureIgnoreCase))
        {
          string str = (string) operation.Value;
          service.SetPolicyValue<string>(this.TfsRequestContext, policyName, str);
        }
        else
        {
          if (!"Enforce".Equals(operationPath, StringComparison.InvariantCultureIgnoreCase))
            throw new OrganizationBadRequestException(FrameworkResources.NotSupportedJsonPatchOperation((object) operation, (object) operationPath, operation.Value));
          bool enforce = (bool) operation.Value;
          service.SetPolicyEnforcementValue(this.TfsRequestContext, policyName, enforce);
        }
      }
    }
  }
}
