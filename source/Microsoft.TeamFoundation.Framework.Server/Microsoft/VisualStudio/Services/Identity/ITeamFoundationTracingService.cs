// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.ITeamFoundationTracingService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Directories.DirectoryService.Components;
using System;

namespace Microsoft.VisualStudio.Services.Identity
{
  public interface ITeamFoundationTracingService
  {
    void TraceIdentityChanges(
      Guid vsid,
      Guid cuid,
      string identityCategory,
      string identityProvider,
      string identityProviderId,
      string identityProviderTenantId,
      DateTime modifiedDate);

    void TraceDirectoryMemberStorageKeyProvisioning(
      string eventType,
      Guid hostId,
      DirectoryMember directoryMember,
      SubjectDescriptor subjectDescriptor,
      Guid storageKey);
  }
}
