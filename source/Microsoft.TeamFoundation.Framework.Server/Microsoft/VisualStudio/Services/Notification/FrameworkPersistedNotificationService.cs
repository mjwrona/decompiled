// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notification.FrameworkPersistedNotificationService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Notification.Client;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Notification
{
  internal class FrameworkPersistedNotificationService : 
    IPersistedNotificationService,
    IVssFrameworkService
  {
    private static readonly string s_area = FrameworkPersistedNotificationTracePoints.Area;
    private static readonly string s_layer = nameof (FrameworkPersistedNotificationService);

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void SaveNotifications(
      IVssRequestContext requestContext,
      IEnumerable<Microsoft.VisualStudio.Services.Notification.Notification> notifications)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IEnumerable<Microsoft.VisualStudio.Services.Notification.Notification>>(notifications, nameof (notifications));
      requestContext.TraceBlock(FrameworkPersistedNotificationTracePoints.ServiceSaveNotificationsEnter, FrameworkPersistedNotificationTracePoints.ServiceSaveNotificationsLeave, FrameworkPersistedNotificationTracePoints.ServiceSaveNotificationsException, FrameworkPersistedNotificationService.s_area, FrameworkPersistedNotificationService.s_layer, (Action) (() => this.GetClient(requestContext).SaveNotificationsAsync(notifications).SyncResult()), nameof (SaveNotifications));
    }

    public IList<Microsoft.VisualStudio.Services.Notification.Notification> GetRecipientNotifications(
      IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      return requestContext.TraceBlock<IList<Microsoft.VisualStudio.Services.Notification.Notification>>(FrameworkPersistedNotificationTracePoints.ServiceGetRecipientNotificationsEnter, FrameworkPersistedNotificationTracePoints.ServiceGetRecipientNotificationsLeave, FrameworkPersistedNotificationTracePoints.ServiceGetRecipientNotificationsException, FrameworkPersistedNotificationService.s_area, FrameworkPersistedNotificationService.s_layer, (Func<IList<Microsoft.VisualStudio.Services.Notification.Notification>>) (() => this.GetClient(requestContext).GetRecipientNotificationsAsync().SyncResult<IList<Microsoft.VisualStudio.Services.Notification.Notification>>()), nameof (GetRecipientNotifications));
    }

    public RecipientMetadata GetRecipientMetadata(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      return requestContext.TraceBlock<RecipientMetadata>(FrameworkPersistedNotificationTracePoints.ServiceGetRecipientMetadataEnter, FrameworkPersistedNotificationTracePoints.ServiceGetRecipientMetadataLeave, FrameworkPersistedNotificationTracePoints.ServiceGetRecipientMetadataException, FrameworkPersistedNotificationService.s_area, FrameworkPersistedNotificationService.s_layer, (Func<RecipientMetadata>) (() => this.GetClient(requestContext).GetRecipientMetadataAsync().SyncResult<RecipientMetadata>()), nameof (GetRecipientMetadata));
    }

    public RecipientMetadata UpdateRecipientMetadata(
      IVssRequestContext requestContext,
      RecipientMetadata metadata)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<RecipientMetadata>(metadata, nameof (metadata));
      return requestContext.TraceBlock<RecipientMetadata>(FrameworkPersistedNotificationTracePoints.ServiceUpdateRecipientMetadataEnter, FrameworkPersistedNotificationTracePoints.ServiceUpdateRecipientMetadataLeave, FrameworkPersistedNotificationTracePoints.ServiceUpdateRecipientMetadataException, FrameworkPersistedNotificationService.s_area, FrameworkPersistedNotificationService.s_layer, (Func<RecipientMetadata>) (() => this.GetClient(requestContext).UpdateRecipientMetadataAsync(metadata).SyncResult<RecipientMetadata>()), nameof (UpdateRecipientMetadata));
    }

    private PersistedNotificationHttpClient GetClient(IVssRequestContext requestContext) => requestContext.To(TeamFoundationHostType.Application).GetClient<PersistedNotificationHttpClient>();
  }
}
