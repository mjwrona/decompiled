// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Policy.Server.PolicyRevisionsController
// Assembly: Microsoft.TeamFoundation.Policy.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C7B03386-B27B-4823-BB4F-89F7D7E42DDD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Policy.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Policy.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Policy.Server
{
  public class PolicyRevisionsController : PolicyApiController
  {
    [HttpGet]
    [ClientLocationId("FE1E68A2-60D3-43CB-855B-85E41AE97C95")]
    [ClientResponseType(typeof (List<PolicyConfiguration>), null, null)]
    public HttpResponseMessage GetPolicyConfigurationRevisions(
      int configurationId,
      [FromUri(Name = "$top")] int? top = null,
      [FromUri(Name = "$skip")] int? skip = null)
    {
      return this.GenerateResponse<PolicyConfiguration>(this.PolicyService.GetPolicyConfigurationRecordRevisions(this.TfsRequestContext, this.ProjectId, configurationId, top, skip).Select<PolicyConfigurationRecord, PolicyConfiguration>((Func<PolicyConfigurationRecord, PolicyConfiguration>) (x => x.ToWebApi(this.TfsRequestContext))));
    }

    [HttpGet]
    [ClientLocationId("FE1E68A2-60D3-43CB-855B-85E41AE97C95")]
    public PolicyConfiguration GetPolicyConfigurationRevision(int configurationId, int revisionId) => this.PolicyService.GetPolicyConfigurationRecord(this.TfsRequestContext, this.ProjectId, configurationId, new int?(revisionId)).ToWebApi(this.TfsRequestContext);
  }
}
