// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Server.IExtensionPoliciesService
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 57F50803-C5C4-41A9-A26F-AD293D563111
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ExtensionManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Server
{
  [DefaultServiceImplementation(typeof (ExtensionPoliciesService))]
  public interface IExtensionPoliciesService : IVssFrameworkService
  {
    UserExtensionPolicy GetPolicies(IVssRequestContext requestContext);

    void CheckManagePermission(IVssRequestContext requestContext);

    bool HasManagePermission(IVssRequestContext requestContext);

    bool HasAdminPermission(IVssRequestContext requestContext);

    IList<Microsoft.VisualStudio.Services.Identity.Identity> GetExtensionManagers(
      IVssRequestContext requestContext,
      int maxResults);
  }
}
