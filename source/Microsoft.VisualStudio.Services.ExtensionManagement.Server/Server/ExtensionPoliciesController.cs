// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Server.ExtensionPoliciesController
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 57F50803-C5C4-41A9-A26F-AD293D563111
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ExtensionManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Server
{
  [VersionedApiControllerCustomName(Area = "ExtensionManagement", ResourceName = "Policies")]
  [ClientInternalUseOnly(false)]
  public class ExtensionPoliciesController : TfsApiController
  {
    [HttpGet]
    public UserExtensionPolicy GetPolicies(string userId)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(userId, nameof (userId));
      if (!"me".Equals(userId, StringComparison.InvariantCultureIgnoreCase))
        throw new ArgumentException(ExtensionResources.CannotGetPoliciesForOtherUserThanMe(), nameof (userId));
      return this.TfsRequestContext.GetService<IExtensionPoliciesService>().GetPolicies(this.TfsRequestContext);
    }
  }
}
