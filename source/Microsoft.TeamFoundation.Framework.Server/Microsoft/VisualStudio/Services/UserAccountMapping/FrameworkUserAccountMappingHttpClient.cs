// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.UserAccountMapping.FrameworkUserAccountMappingHttpClient
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.UserAccountMapping.Client;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.UserAccountMapping
{
  [ResourceArea("B2F5FAA8-CAAF-436F-B40C-FC45778E174D")]
  public class FrameworkUserAccountMappingHttpClient : UserAccountMappingHttpClientBase
  {
    public FrameworkUserAccountMappingHttpClient(
      Uri baseUrl,
      HttpMessageHandler pipeline,
      bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public Task ActivateUserAccountMappingAsync(
      SubjectDescriptor descriptor,
      Guid accountId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.ActivateUserAccountMappingAsync((string) descriptor, accountId, userState, cancellationToken);
    }

    public Task ActivateUserAccountMappingAsync(
      SubjectDescriptor descriptor,
      Guid accountId,
      UserRole userRole,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.ActivateUserAccountMappingAsync((string) descriptor, accountId, userRole, userState, cancellationToken);
    }

    public new Task SetUserAccountLicenseInfoAsync(
      string descriptor,
      Guid accountId,
      VisualStudioLevel maxVsLevelFromAccountLicense,
      VisualStudioLevel maxVsLevelFromAccountExtensions,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return base.SetUserAccountLicenseInfoAsync(descriptor, accountId, maxVsLevelFromAccountLicense, maxVsLevelFromAccountExtensions, userState, cancellationToken);
    }

    public Task DeactivateUserAccountMappingAsync(
      SubjectDescriptor descriptor,
      Guid accountId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.DeactivateUserAccountMappingAsync((string) descriptor, accountId, userState, cancellationToken);
    }

    public Task<List<Guid>> QueryAccountIdsAsync(
      SubjectDescriptor descriptor,
      UserRole userRole,
      bool? useEqualsCheckForUserRoleMatch = null,
      bool? includeDeletedAccounts = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.QueryAccountIdsAsync((string) descriptor, userRole, useEqualsCheckForUserRoleMatch, includeDeletedAccounts, userState, cancellationToken);
    }

    public Task<bool> HasMappingsAsync(
      SubjectDescriptor descriptor,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.HasMappingsAsync((string) descriptor, userState, cancellationToken);
    }
  }
}
