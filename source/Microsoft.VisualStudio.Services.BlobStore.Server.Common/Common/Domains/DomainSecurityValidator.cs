// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Common.Domains.DomainSecurityValidator
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB48D0BF-32A2-483C-A1D4-2F10DEBB3D56
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using System;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Common.Domains
{
  public class DomainSecurityValidator : IDomainSecurityValidator
  {
    public const int DomainForbiddenTracepoint = 5703001;

    public IVssRequestContext RequestContext { get; }

    public DomainSecurityValidator(IVssRequestContext request) => this.RequestContext = request;

    public bool HasPermissionToProject(Guid projectId, int projectPermissions)
    {
      if (this.RequestContext.GetService<IProjectService>().HasProjectPermission(this.RequestContext, ProjectInfo.GetProjectUri(projectId), projectPermissions))
        return true;
      this.RequestContext.TraceAlways(5703001, TraceLevel.Warning, "BlobStore", nameof (DomainSecurityValidator), string.Format("User does not have permission to project {0}.  Domain validation failed.", (object) projectId));
      return false;
    }
  }
}
