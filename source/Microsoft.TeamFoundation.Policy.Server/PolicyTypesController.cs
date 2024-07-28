// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Policy.Server.PolicyTypesController
// Assembly: Microsoft.TeamFoundation.Policy.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C7B03386-B27B-4823-BB4F-89F7D7E42DDD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Policy.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Policy.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Policy.Server
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class PolicyTypesController : PolicyApiController
  {
    [HttpGet]
    [ClientLocationId("44096322-2D3D-466A-BB30-D1B7DE69F61F")]
    public IEnumerable<PolicyType> GetPolicyTypes() => this.PolicyService.GetPolicyTypes(this.TfsRequestContext).Select<ITeamFoundationPolicy, PolicyType>((Func<ITeamFoundationPolicy, PolicyType>) (t => t.ToWebApi(this.TfsRequestContext, this.ProjectId)));

    [HttpGet]
    [ClientLocationId("44096322-2D3D-466A-BB30-D1B7DE69F61F")]
    public PolicyType GetPolicyType(Guid typeId) => this.PolicyService.GetPolicyType(this.TfsRequestContext, typeId).ToWebApi(this.TfsRequestContext, this.ProjectId);
  }
}
