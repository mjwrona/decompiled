// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.GraphControllerBase
// Assembly: Microsoft.VisualStudio.Services.Graph, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00390AA0-D8BB-45EB-AEF5-70DC8BFC765D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Graph.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Directories;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Microsoft.VisualStudio.Services.Graph
{
  public abstract class GraphControllerBase : TfsApiController
  {
    protected const string FeatureEnableGraphApi = "VisualStudio.Services.Graph.EnableGraphApi";
    protected const string FeatureEnablePagingGraphApi = "VisualStudio.Services.Graph.EnablePagingGraphApi";
    private static readonly Dictionary<System.Type, HttpStatusCode> s_httpExceptions = new Dictionary<System.Type, HttpStatusCode>()
    {
      {
        typeof (IdentityDomainMismatchException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (AddMemberCyclicMembershipException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (IdentityPropertyRequiredException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (IdentityExpressionException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (InvalidDisplayNameException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (IdentityMapReadOnlyException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (IdentityNotServiceIdentityException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (InvalidServiceIdentityNameException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (IllegalIdentityException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (IncompatibleScopeException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (IdentityInvalidTypeIdException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (GraphBadRequestException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (GroupNameNotRecognizedException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (TooManyRequestedItemsException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (InvalidSubjectTypeException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (UnsupportedSubjectTypeForMembershipTraversal),
        HttpStatusCode.BadRequest
      },
      {
        typeof (ArgumentException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (GroupNameIsReservedBySystemException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (RemoveAccountOwnerFromAdminGroupException),
        HttpStatusCode.Forbidden
      },
      {
        typeof (RemoveSelfFromAdminGroupException),
        HttpStatusCode.Forbidden
      },
      {
        typeof (AddGroupMemberIllegalMemberException),
        HttpStatusCode.Forbidden
      },
      {
        typeof (AddGroupMemberIllegalWindowsIdentityException),
        HttpStatusCode.Forbidden
      },
      {
        typeof (AddGroupMemberIllegalInternetIdentityException),
        HttpStatusCode.Forbidden
      },
      {
        typeof (RemoveSpecialGroupException),
        HttpStatusCode.Forbidden
      },
      {
        typeof (NotApplicationGroupException),
        HttpStatusCode.Forbidden
      },
      {
        typeof (ModifyEveryoneGroupException),
        HttpStatusCode.Forbidden
      },
      {
        typeof (NotASecurityGroupException),
        HttpStatusCode.Forbidden
      },
      {
        typeof (RemoveMemberServiceAccountException),
        HttpStatusCode.Forbidden
      },
      {
        typeof (AccountPreferencesAlreadyExistException),
        HttpStatusCode.Forbidden
      },
      {
        typeof (InvalidAccessException),
        HttpStatusCode.Forbidden
      },
      {
        typeof (CannotUpdateWellKnownGraphGroupException),
        HttpStatusCode.Forbidden
      },
      {
        typeof (AllowTeamAdminsInvitationsPolicyException),
        HttpStatusCode.Forbidden
      },
      {
        typeof (VssUnauthorizedException),
        HttpStatusCode.Forbidden
      },
      {
        typeof (RemoveGroupMemberNotMemberException),
        HttpStatusCode.NotFound
      },
      {
        typeof (RemoveNonexistentGroupException),
        HttpStatusCode.NotFound
      },
      {
        typeof (FindGroupSidDoesNotExistException),
        HttpStatusCode.NotFound
      },
      {
        typeof (GroupScopeDoesNotExistException),
        HttpStatusCode.NotFound
      },
      {
        typeof (IdentityNotFoundException),
        HttpStatusCode.NotFound
      },
      {
        typeof (GraphSubjectNotFoundException),
        HttpStatusCode.NotFound
      },
      {
        typeof (GraphMembershipNotFoundException),
        HttpStatusCode.NotFound
      },
      {
        typeof (UserDoesNotExistException),
        HttpStatusCode.NotFound
      },
      {
        typeof (TooManyIdentitiesReturnedException),
        HttpStatusCode.NotFound
      },
      {
        typeof (StorageKeyNotFoundException),
        HttpStatusCode.NotFound
      },
      {
        typeof (GroupCreationException),
        HttpStatusCode.Conflict
      },
      {
        typeof (AddMemberIdentityAlreadyMemberException),
        HttpStatusCode.Conflict
      },
      {
        typeof (AddProjectGroupProjectMismatchException),
        HttpStatusCode.Conflict
      },
      {
        typeof (GroupScopeCreationException),
        HttpStatusCode.Conflict
      },
      {
        typeof (IdentityAlreadyExistsException),
        HttpStatusCode.Conflict
      },
      {
        typeof (IdentityAccountNameAlreadyInUseException),
        HttpStatusCode.Conflict
      },
      {
        typeof (IdentityAliasAlreadyInUseException),
        HttpStatusCode.Conflict
      },
      {
        typeof (GraphAccountNameCollisionRepairUnsafeException),
        HttpStatusCode.Conflict
      },
      {
        typeof (GraphAccountNameCollisionRepairFailedException),
        HttpStatusCode.Conflict
      },
      {
        typeof (IdentitySyncException),
        HttpStatusCode.InternalServerError
      },
      {
        typeof (InvalidChangedIdentityException),
        HttpStatusCode.InternalServerError
      },
      {
        typeof (NotImplementedException),
        HttpStatusCode.InternalServerError
      },
      {
        typeof (GraphApiUnavailableException),
        HttpStatusCode.ServiceUnavailable
      },
      {
        typeof (IdentityProviderUnavailableException),
        HttpStatusCode.ServiceUnavailable
      }
    };

    public override string ActivityLogArea => "Graph";

    public override string TraceArea => "Graph";

    public override IDictionary<System.Type, HttpStatusCode> HttpExceptions => (IDictionary<System.Type, HttpStatusCode>) GraphControllerBase.s_httpExceptions;

    protected void CheckPermissions(
      IVssRequestContext requestContext,
      string token,
      int permission)
    {
      requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, GraphSecurityConstants.NamespaceId).CheckPermission(requestContext, token, permission);
    }

    protected bool ShouldForceFilterIdentities()
    {
      IEnumerable<string> values;
      bool result;
      return this.ControllerContext.Request.Headers.TryGetValues("x-ForceFilterIdentities", out values) && bool.TryParse(values.SingleOrDefault<string>(), out result) && result;
    }
  }
}
