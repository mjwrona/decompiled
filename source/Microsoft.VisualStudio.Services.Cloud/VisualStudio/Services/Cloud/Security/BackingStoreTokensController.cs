// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.Security.BackingStoreTokensController
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
  [VersionedApiControllerCustomName(Area = "SBS", ResourceName = "SBSTokens")]
  public class BackingStoreTokensController : SecurityBackingStoreController
  {
    [HttpPatch]
    public int RenameTokens(Guid securityNamespaceId, JObject container)
    {
      ArgumentUtility.CheckForEmptyGuid(securityNamespaceId, nameof (securityNamespaceId));
      ArgumentUtility.CheckForNull<JObject>(container, nameof (container));
      CachingAclStore aclStore = this.GetAclStore(this.GetSecurityNamespace(securityNamespaceId, true), WellKnownAclStores.User);
      TokenStoreSequenceId newSequenceId = aclStore.BackingStore.RenameTokens(this.TfsRequestContext, SecurityConverter.Convert((IEnumerable<Microsoft.VisualStudio.Services.Security.TokenRename>) container["renames"].ToObject<List<Microsoft.VisualStudio.Services.Security.TokenRename>>()));
      aclStore.NotifyChanged(this.TfsRequestContext, newSequenceId, false);
      return checked ((int) newSequenceId.ToScalarForRestReply());
    }
  }
}
