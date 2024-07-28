// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Security.Server.AccessControlListsController
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Security.Server
{
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "Security", ResourceName = "AccessControlLists")]
  public class AccessControlListsController : TfsApiController
  {
    [HttpGet]
    [ClientExample("GET__accesscontrollists__securityNamespaceId__.json", "All ACLs in a security namespace", null, null)]
    [ClientExample("GET__accesscontrollists__securityNamespaceId___token-_existingToken_.json", "Filter by token", null, null)]
    [ClientExample("GET__accesscontrollists__securityNamespaceId___descriptors-_descriptor1_.json", "Filter by descriptors", null, null)]
    [ClientExample("GET__accesscontrollists__securityNamespaceId___token-_existingToken__includeExtendedInfo-True.json", "Include extended info properties", null, null)]
    [ClientExample("GET__accesscontrollists__securityNamespaceId___token-_existingToken__includeExtendedInfo-False_recurse-True.json", "Include child ACLs", null, null)]
    public IQueryable<Microsoft.VisualStudio.Services.Security.AccessControlList> QueryAccessControlLists(
      Guid securityNamespaceId,
      string token = null,
      string descriptors = "",
      bool includeExtendedInfo = false,
      bool recurse = false)
    {
      ArgumentUtility.CheckForEmptyGuid(securityNamespaceId, nameof (securityNamespaceId));
      IList<IdentityDescriptor> descriptors1 = (IList<IdentityDescriptor>) null;
      if (!string.IsNullOrEmpty(descriptors))
        descriptors1 = IdentityParser.GetDescriptorsFromString(descriptors);
      return SecurityConverter.Convert((this.TfsRequestContext.GetService<SecuredTeamFoundationSecurityService>().GetSecurityNamespace(this.TfsRequestContext, securityNamespaceId) ?? throw new InvalidSecurityNamespaceException(securityNamespaceId)).QueryAccessControlLists(this.TfsRequestContext, token, (IEnumerable<IdentityDescriptor>) descriptors1, includeExtendedInfo, recurse)).AsQueryable<Microsoft.VisualStudio.Services.Security.AccessControlList>();
    }

    [HttpPost]
    [ClientExample("POST__accesscontrollists__securityNamespaceId__.json", null, null, null)]
    public void SetAccessControlLists(
      Guid securityNamespaceId,
      VssJsonCollectionWrapper<AccessControlListsCollection> accessControlLists)
    {
      ArgumentUtility.CheckForEmptyGuid(securityNamespaceId, nameof (securityNamespaceId));
      ArgumentUtility.CheckForNull<VssJsonCollectionWrapper<AccessControlListsCollection>>(accessControlLists, nameof (accessControlLists));
      ArgumentUtility.CheckForNull<AccessControlListsCollection>(accessControlLists.Value, "accessControlLists.Value");
      IEnumerable<Microsoft.TeamFoundation.Framework.Server.AccessControlList> accessControlLists1 = SecurityConverter.Convert((IEnumerable<Microsoft.VisualStudio.Services.Security.AccessControlList>) accessControlLists.Value);
      (this.TfsRequestContext.GetService<SecuredTeamFoundationSecurityService>().GetSecurityNamespace(this.TfsRequestContext, securityNamespaceId) ?? throw new InvalidSecurityNamespaceException(securityNamespaceId)).SetAccessControlLists(this.TfsRequestContext, (IEnumerable<IAccessControlList>) accessControlLists1);
    }

    [HttpDelete]
    [ClientExample("DELETE__accesscontrollists__securityNamespaceId___tokens-_newToken1_,_newToken2__recurse-False.json", null, null, null)]
    public bool RemoveAccessControlLists(Guid securityNamespaceId, string tokens = "", bool recurse = false)
    {
      ArgumentUtility.CheckForEmptyGuid(securityNamespaceId, nameof (securityNamespaceId));
      string[] tokensFromString = this.GetTokensFromString(tokens);
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) tokensFromString, nameof (tokens));
      return (this.TfsRequestContext.GetService<SecuredTeamFoundationSecurityService>().GetSecurityNamespace(this.TfsRequestContext, securityNamespaceId) ?? throw new InvalidSecurityNamespaceException(securityNamespaceId)).RemoveAccessControlLists(this.TfsRequestContext, (IEnumerable<string>) tokensFromString, recurse);
    }

    private string[] GetTokensFromString(string tokens)
    {
      if (string.IsNullOrEmpty(tokens))
        return Array.Empty<string>();
      return tokens.Split(new char[1]{ ',' }, StringSplitOptions.RemoveEmptyEntries);
    }

    public override string TraceArea => "SecurityService";

    public override string ActivityLogArea => "Framework";
  }
}
