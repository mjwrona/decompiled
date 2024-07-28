// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.Security.BackingStoreAccessControlListsController
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Security;
using Microsoft.VisualStudio.Services.Security.Server;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Cloud.Security
{
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "SBS", ResourceName = "SBSAcls")]
  public class BackingStoreAccessControlListsController : SecurityBackingStoreController
  {
    [HttpPost]
    public int SetAccessControlLists(Guid securityNamespaceId, JObject container)
    {
      ArgumentUtility.CheckForEmptyGuid(securityNamespaceId, nameof (securityNamespaceId));
      ArgumentUtility.CheckForNull<JObject>(container, nameof (container));
      CachingAclStore aclStore = this.GetAclStore(this.GetSecurityNamespace(securityNamespaceId, true), WellKnownAclStores.User);
      List<Microsoft.VisualStudio.Services.Security.AccessControlList> vssAcls = container["accessControlLists"].ToObject<List<Microsoft.VisualStudio.Services.Security.AccessControlList>>();
      bool throwOnInvalidIdentity = container["throwOnInvalidIdentity"].ToObject<bool>();
      TokenStoreSequenceId newSequenceId = aclStore.BackingStore.SetAccessControlLists(this.TfsRequestContext, (IEnumerable<IAccessControlList>) SecurityConverter.Convert((IEnumerable<Microsoft.VisualStudio.Services.Security.AccessControlList>) vssAcls), throwOnInvalidIdentity);
      aclStore.NotifyChanged(this.TfsRequestContext, newSequenceId, false);
      return checked ((int) newSequenceId.ToScalarForRestReply());
    }

    [HttpPatch]
    public int RemoveAccessControlLists(Guid securityNamespaceId, JObject container)
    {
      ArgumentUtility.CheckForEmptyGuid(securityNamespaceId, nameof (securityNamespaceId));
      ArgumentUtility.CheckForNull<JObject>(container, nameof (container));
      CachingAclStore aclStore = this.GetAclStore(this.GetSecurityNamespace(securityNamespaceId, true), WellKnownAclStores.User);
      TokenStoreSequenceId newSequenceId = aclStore.BackingStore.RemoveAccessControlLists(this.TfsRequestContext, (IEnumerable<string>) container["tokens"].ToObject<List<string>>(), container["recurse"].ToObject<bool>());
      aclStore.NotifyChanged(this.TfsRequestContext, newSequenceId, false);
      return checked ((int) newSequenceId.ToScalarForRestReply());
    }
  }
}
