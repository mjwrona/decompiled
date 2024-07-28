// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IIdentityServiceInternal
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.ComponentModel;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal interface IIdentityServiceInternal : 
    IIdentityServiceInternalRequired,
    IdentityService,
    IUserIdentityService,
    ISystemIdentityService,
    IVssFrameworkService
  {
    Microsoft.VisualStudio.Services.Identity.Identity CreateUser(
      IVssRequestContext requestContext,
      Guid scopeId,
      string userSid,
      string domainName,
      string accountName,
      string description);

    ChangedIdentities GetIdentityChanges(
      IVssRequestContext requestContext,
      ChangedIdentitiesContext sequenceContext);

    IdentityChanges GetIdentityChanges(
      IVssRequestContext requestContext,
      int sequenceId,
      long identitySequenceId,
      long groupSequenceId,
      long organizationIdentitySequenceId);

    [EditorBrowsable(EditorBrowsableState.Never)]
    bool RefreshIdentity(IVssRequestContext requestContext, IdentityDescriptor descriptor);

    [EditorBrowsable(EditorBrowsableState.Never)]
    bool LastUserAccessUpdateScheduled(IVssRequestContext requestContext, Guid identityId);

    [EditorBrowsable(EditorBrowsableState.Never)]
    void ScheduleLastUserAccessUpdate(IVssRequestContext requestContext, Guid identityId);

    [EditorBrowsable(EditorBrowsableState.Never)]
    void RefreshSearchIdentitiesCache(IVssRequestContext requestContext, Guid scopeId);

    [EditorBrowsable(EditorBrowsableState.Never)]
    int UpgradeIdentitiesToTargetResourceVersion(
      IVssRequestContext requestContext,
      int targetResourceVersion,
      int updateBatchSize,
      int maxNumberOfIdentitiesToUpdate);

    [EditorBrowsable(EditorBrowsableState.Never)]
    int DowngradeIdentitiesToTargetResourceVersion(
      IVssRequestContext requestContext,
      int targetResourceVersion,
      int updateBatchSize,
      int maxNumberOfIdentitiesToUpdate);

    IdentityDomain Domain { get; }

    event EventHandler<DescriptorChangeEventArgs> DescriptorsChanged;

    event EventHandler<DescriptorChangeNotificationEventArgs> DescriptorsChangedNotification;
  }
}
