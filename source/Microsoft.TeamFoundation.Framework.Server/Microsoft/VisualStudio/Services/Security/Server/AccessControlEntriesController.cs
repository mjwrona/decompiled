// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Security.Server.AccessControlEntriesController
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Security.Server
{
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "Security", ResourceName = "AccessControlEntries")]
  public class AccessControlEntriesController : TfsApiController
  {
    [HttpPost]
    [ClientExample("POST__accesscontrolentries__securityNamespaceId__.json", "Replace", null, null)]
    [ClientExample("POST__accesscontrolentries__securityNamespaceId__merge.json", "Merge", null, null)]
    public IQueryable<Microsoft.VisualStudio.Services.Security.AccessControlEntry> SetAccessControlEntries(
      Guid securityNamespaceId,
      JObject container)
    {
      ArgumentUtility.CheckForEmptyGuid(securityNamespaceId, nameof (securityNamespaceId));
      ArgumentUtility.CheckForNull<JObject>(container, nameof (container));
      JToken jtoken1 = container["token"] ?? (JToken) new JProperty("token", (object[]) null);
      JToken jtoken2 = container["accessControlEntries"] ?? (JToken) new JProperty("accessControlEntries", (object) new List<Microsoft.VisualStudio.Services.Security.AccessControlEntry>());
      JToken jtoken3 = container["merge"] ?? (JToken) new JProperty("merge", (object) bool.FalseString);
      string str = jtoken1.ToObject<string>();
      List<Microsoft.VisualStudio.Services.Security.AccessControlEntry> vssAces = jtoken2.ToObject<List<Microsoft.VisualStudio.Services.Security.AccessControlEntry>>();
      bool merge = jtoken3.ToObject<bool>();
      ArgumentUtility.CheckForNull<string>(str, "token");
      IVssSecurityNamespace securityNamespace = this.TfsRequestContext.GetService<SecuredTeamFoundationSecurityService>().GetSecurityNamespace(this.TfsRequestContext, securityNamespaceId);
      if (securityNamespace == null)
        throw new InvalidSecurityNamespaceException(securityNamespaceId);
      IEnumerable<IAccessControlEntry> accessControlEntries = (IEnumerable<IAccessControlEntry>) SecurityConverter.Convert((IEnumerable<Microsoft.VisualStudio.Services.Security.AccessControlEntry>) vssAces);
      return SecurityConverter.Convert(securityNamespace.SetAccessControlEntries(this.TfsRequestContext, str, accessControlEntries, merge)).AsQueryable<Microsoft.VisualStudio.Services.Security.AccessControlEntry>();
    }

    [HttpDelete]
    [ClientExample("DELETE__accesscontrolentries__securityNamespaceId___token-_token__descriptors-_descriptor1_,_descriptor2_.json", null, null, null)]
    public bool RemoveAccessControlEntries(
      Guid securityNamespaceId,
      string token = "",
      string descriptors = "")
    {
      ArgumentUtility.CheckForEmptyGuid(securityNamespaceId, nameof (securityNamespaceId));
      ArgumentUtility.CheckForNull<string>(token, nameof (token));
      IList<IdentityDescriptor> descriptors1 = (IList<IdentityDescriptor>) null;
      if (!string.IsNullOrEmpty(descriptors))
        descriptors1 = IdentityParser.GetDescriptorsFromString(descriptors);
      return (this.TfsRequestContext.GetService<SecuredTeamFoundationSecurityService>().GetSecurityNamespace(this.TfsRequestContext, securityNamespaceId) ?? throw new InvalidSecurityNamespaceException(securityNamespaceId)).RemoveAccessControlEntries(this.TfsRequestContext, token, (IEnumerable<IdentityDescriptor>) descriptors1);
    }

    public override string TraceArea => "SecurityService";

    public override string ActivityLogArea => "Framework";
  }
}
