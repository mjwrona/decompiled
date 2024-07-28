// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Directories.DirectoryService.TeamFoundationDirectoryServiceExtensions
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.VisualStudio.Services.Directories.DirectoryService.Components;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Directories.DirectoryService
{
  public static class TeamFoundationDirectoryServiceExtensions
  {
    public static IdentityDirectoryWrapperService<TeamFoundationIdentity> IncludeTeamFoundationIdentities(
      this IDirectoryService directoryService,
      IVssRequestContext requestContext)
    {
      return (IdentityDirectoryWrapperService<TeamFoundationIdentity>) requestContext.GetService<TeamFoundationDirectoryServiceExtensions.TeamFoundationIdentityDirectoryWrapperService>();
    }

    public class TeamFoundationIdentityDirectoryWrapperService : 
      IdentityDirectoryWrapperService<TeamFoundationIdentity>
    {
      protected override IList<TeamFoundationIdentity> GetIdentities(
        IVssRequestContext requestContext,
        IList<Guid> identityIds)
      {
        return (IList<TeamFoundationIdentity>) requestContext.GetService<TeamFoundationIdentityService>().ReadIdentities(requestContext, identityIds.ToArray<Guid>());
      }
    }
  }
}
