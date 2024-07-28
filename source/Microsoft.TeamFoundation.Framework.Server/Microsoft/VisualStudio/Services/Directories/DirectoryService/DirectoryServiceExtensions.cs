// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Directories.DirectoryService.DirectoryServiceExtensions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Directories.DirectoryService.Components;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Directories.DirectoryService
{
  public static class DirectoryServiceExtensions
  {
    public static IdentityDirectoryWrapperService<Microsoft.VisualStudio.Services.Identity.Identity> IncludeIdentities(
      this IDirectoryService directoryService,
      IVssRequestContext requestContext)
    {
      return (IdentityDirectoryWrapperService<Microsoft.VisualStudio.Services.Identity.Identity>) requestContext.GetService<DirectoryServiceExtensions.VssIdentityDirectoryWrapperService>();
    }

    public class VssIdentityDirectoryWrapperService : IdentityDirectoryWrapperService<Microsoft.VisualStudio.Services.Identity.Identity>
    {
      protected override IList<Microsoft.VisualStudio.Services.Identity.Identity> GetIdentities(
        IVssRequestContext requestContext,
        IList<Guid> identityIds)
      {
        return requestContext.GetService<IdentityService>().ReadIdentities(requestContext, identityIds, QueryMembership.None, (IEnumerable<string>) null);
      }
    }
  }
}
