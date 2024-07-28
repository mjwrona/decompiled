// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Security.Server.PermissionEvaluationsController
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Authorization;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Security.Server
{
  [VersionedApiControllerCustomName(Area = "Security", ResourceName = "PermissionEvaluationBatch")]
  [ClientGroupByResource("Permissions")]
  public class PermissionEvaluationsController : TfsApiController
  {
    [HttpPost]
    [PublicCollectionRequestRestrictions(false, true, null)]
    [ClientExample("POST__security_permissionevaluationbatch__api-version-3.0-preview.json", null, null, null)]
    public PermissionEvaluationBatch HasPermissionsBatch(PermissionEvaluationBatch evalBatch)
    {
      ArgumentUtility.CheckForNull<PermissionEvaluationBatch>(evalBatch, nameof (evalBatch));
      ITeamFoundationSecurityService service = (ITeamFoundationSecurityService) this.TfsRequestContext.GetService<SecuredTeamFoundationSecurityService>();
      IRequestActor userActor = this.TfsRequestContext.GetUserActor();
      IEnumerable<IRequestActor> actors;
      if (userActor == null)
        actors = Enumerable.Empty<IRequestActor>();
      else
        actors = (IEnumerable<IRequestActor>) new IRequestActor[1]
        {
          userActor
        };
      foreach (IGrouping<Guid, PermissionEvaluation> grouping in ((IEnumerable<PermissionEvaluation>) evalBatch.Evaluations).GroupBy<PermissionEvaluation, Guid>((Func<PermissionEvaluation, Guid>) (c => c.SecurityNamespaceId)))
      {
        IVssSecurityNamespace securityNamespace = service.GetSecurityNamespace(this.TfsRequestContext, grouping.Key);
        if (securityNamespace == null)
          throw new InvalidSecurityNamespaceException(grouping.Key);
        foreach (PermissionEvaluation permissionEvaluation in (IEnumerable<PermissionEvaluation>) grouping)
        {
          ArgumentUtility.CheckForNull<string>(permissionEvaluation.Token, "token");
          permissionEvaluation.Value = securityNamespace.HasPermissionOnActors(this.TfsRequestContext, actors, permissionEvaluation.Token, permissionEvaluation.Permissions, out EvaluationPrincipal _, evalBatch.AlwaysAllowAdministrators);
        }
      }
      return evalBatch;
    }
  }
}
