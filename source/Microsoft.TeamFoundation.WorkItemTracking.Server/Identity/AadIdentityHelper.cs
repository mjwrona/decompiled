// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Identity.AadIdentityHelper
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Directories;
using Microsoft.VisualStudio.Services.Directories.DirectoryService;
using Microsoft.VisualStudio.Services.Directories.DirectoryService.Components;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Organization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Identity
{
  internal class AadIdentityHelper
  {
    private static readonly IReadOnlyDictionary<string, object> CommonEntityDescriptorPropertiesForGroups = (IReadOnlyDictionary<string, object>) new Dictionary<string, object>()
    {
      {
        "InvitationMethod",
        (object) "WorkItemAadIdentityCreator"
      },
      {
        "AllowIntroductionOfMembersNotPreviouslyInScope",
        (object) true
      }
    };
    private static readonly IReadOnlyDictionary<string, object> CommonEntityDescriptorPropertiesForUsers = (IReadOnlyDictionary<string, object>) new Dictionary<string, object>()
    {
      {
        "InvitationMethod",
        (object) "WorkItemAadIdentityCreator"
      },
      {
        "RootWithActiveMembership",
        (object) true
      }
    };

    public virtual IdentityScopeInfo GetIdentityInScope(
      IVssRequestContext requestContext,
      IList<Guid> aadObjectIds,
      bool isGroups)
    {
      ArgumentUtility.CheckForNull<IList<Guid>>(aadObjectIds, nameof (aadObjectIds));
      IdentityScopeInfo identityInScope = new IdentityScopeInfo();
      if (!aadObjectIds.Any<Guid>())
        return identityInScope;
      IVssRequestContext vssRequestContext = WorkItemTrackingFeatureFlags.IsAadIdentityHelperFixEnabled(requestContext) ? requestContext : requestContext.Elevate();
      IdentityDirectoryWrapperService<Microsoft.VisualStudio.Services.Identity.Identity> directoryWrapperService = vssRequestContext.GetService<IDirectoryService>().IncludeIdentities(vssRequestContext);
      HashSet<Guid> guidSet = new HashSet<Guid>();
      IReadOnlyList<DirectoryEntityDescriptor> entityDescriptorList = (IReadOnlyList<DirectoryEntityDescriptor>) aadObjectIds.Select<Guid, DirectoryEntityDescriptor>((Func<Guid, DirectoryEntityDescriptor>) (id => new DirectoryEntityDescriptor(isGroups ? "Group" : "User", "aad", id.ToString(), properties: isGroups ? AadIdentityHelper.CommonEntityDescriptorPropertiesForGroups : AadIdentityHelper.CommonEntityDescriptorPropertiesForUsers))).ToList<DirectoryEntityDescriptor>().AsReadOnly();
      IVssRequestContext requestContext1 = vssRequestContext;
      IReadOnlyList<DirectoryEntityDescriptor> members = entityDescriptorList;
      foreach (IdentityDirectoryEntityResult<Microsoft.VisualStudio.Services.Identity.Identity> addMember in (IEnumerable<IdentityDirectoryEntityResult<Microsoft.VisualStudio.Services.Identity.Identity>>) directoryWrapperService.AddMembers(requestContext1, (IReadOnlyList<IDirectoryEntityDescriptor>) members))
      {
        if (addMember.Status == "NotInScope")
          guidSet.Add(new Guid(addMember.Entity.OriginId));
        else if (addMember.Status == "Success")
          identityInScope.InScopeIdentitiesMap.Add(new Guid(addMember.Entity.OriginId), addMember.Identity);
      }
      identityInScope.NotInScopeIds = guidSet;
      return identityInScope;
    }

    public virtual bool IsAccountAadEnabled(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      return requestContext.ExecutionEnvironment.IsHostedDeployment && requestContext.IsOrganizationAadBacked();
    }

    private bool IsObjectDirectlyInScope(IVssRequestContext requestContext, Guid objectId)
    {
      IdentityService service = requestContext.GetService<IdentityService>();
      IdentityDescriptor descriptorFromSid = IdentityHelper.CreateDescriptorFromSid(SidIdentityHelper.ConstructAadGroupSid(objectId));
      IVssRequestContext requestContext1 = requestContext;
      IdentityDescriptor everyoneGroup = GroupWellKnownIdentityDescriptors.EveryoneGroup;
      IdentityDescriptor memberDescriptor = descriptorFromSid;
      return service.IsMember(requestContext1, everyoneGroup, memberDescriptor);
    }
  }
}
