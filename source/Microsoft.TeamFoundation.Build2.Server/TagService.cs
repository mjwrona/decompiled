// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.TagService
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public sealed class TagService : ITagService, IVssFrameworkService
  {
    private const string TraceLayer = "BuildService";
    private readonly IBuildSecurityProvider SecurityProvider;

    public TagService()
      : this((IBuildSecurityProvider) new BuildSecurityProvider())
    {
    }

    internal TagService(IBuildSecurityProvider securityProvider) => this.SecurityProvider = securityProvider;

    public IEnumerable<string> DeleteTags(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<string> tags)
    {
      ArgumentUtility.CheckForNull<IEnumerable<string>>(tags, nameof (tags), "Build2");
      using (requestContext.TraceScope("BuildService", nameof (DeleteTags)))
      {
        this.SecurityProvider.CheckProjectPermission(requestContext, projectId, TeamProjectPermissions.AdministerBuild);
        IEnumerable<string> strings = (IEnumerable<string>) null;
        using (Build2Component component = requestContext.CreateComponent<Build2Component>())
          strings = component.DeleteTags(projectId, tags);
        return strings;
      }
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext requestContext)
    {
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext requestContext)
    {
    }
  }
}
