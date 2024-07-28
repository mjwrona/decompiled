// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Common.Domains.DomainSecurityValidatorExtensions
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB48D0BF-32A2-483C-A1D4-2F10DEBB3D56
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.Content.Common.MultiDomainExceptions;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Common.Domains
{
  public static class DomainSecurityValidatorExtensions
  {
    public static bool HasWritePermissionToDomain(
      this IDomainSecurityValidator validator,
      IDomainId domainId)
    {
      return !(domainId is ProjectDomainId projectDomainId) || validator.HasPermissionToProject(projectDomainId.ProjectId, TeamProjectPermissions.GenericRead);
    }

    public static void AssertWritePermissionToDomain(
      this IDomainSecurityValidator validator,
      IDomainId domainId)
    {
      if (!validator.HasWritePermissionToDomain(domainId))
        throw new DomainNotFoundException();
    }
  }
}
