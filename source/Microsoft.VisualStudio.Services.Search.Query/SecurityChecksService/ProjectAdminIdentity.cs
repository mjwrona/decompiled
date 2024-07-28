// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Query.SecurityChecksService.ProjectAdminIdentity
// Assembly: Microsoft.VisualStudio.Services.Search.Query, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 71E00698-03D3-4C67-B313-A550333DA80C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Query.dll

using Microsoft.VisualStudio.Services.Identity;
using System;

namespace Microsoft.VisualStudio.Services.Search.Query.SecurityChecksService
{
  internal class ProjectAdminIdentity
  {
    public Guid ProjectId { get; }

    public IdentityDescriptor Administrators { get; }

    public ProjectAdminIdentity(Guid projectId, IdentityDescriptor administrators)
    {
      this.ProjectId = projectId;
      this.Administrators = administrators;
    }

    public override bool Equals(object other) => other is ProjectAdminIdentity projectAdminIdentity && this.ProjectId.Equals(projectAdminIdentity.ProjectId);

    public override int GetHashCode() => this.ProjectId.GetHashCode();
  }
}
