// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.IAnalyticsViewsAccessService
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Analytics.WebApi;
using System;

namespace Microsoft.VisualStudio.Services.Analytics
{
  [DefaultServiceImplementation(typeof (AnalyticsViewsAccessService))]
  public interface IAnalyticsViewsAccessService : IVssFrameworkService
  {
    bool HasReadViewPermission(
      IVssRequestContext requestContext,
      AnalyticsViewVisibility visibility,
      Guid projectId,
      Guid viewId);

    void CheckReadViewPermission(
      IVssRequestContext requestContext,
      AnalyticsViewVisibility visibility,
      Guid projectId,
      Guid viewId);

    void CheckEditViewPermission(
      IVssRequestContext requestContext,
      AnalyticsViewVisibility visibility,
      Guid projectId,
      Guid viewId);

    void CheckDeleteViewPermission(
      IVssRequestContext requestContext,
      AnalyticsViewVisibility visibility,
      Guid projectId,
      Guid viewId);

    void CheckCreateViewPermission(
      IVssRequestContext requestContext,
      AnalyticsViewVisibility visibility,
      Guid projectId);

    void CheckExecViewsPermission(IVssRequestContext requestContext, Guid projectId);

    void SetViewOwnerPermission(
      IVssRequestContext requestContext,
      AnalyticsViewVisibility visibility,
      Guid projectId,
      Guid viewId);

    int GetEffectivePermissions(
      IVssRequestContext requestContext,
      AnalyticsViewVisibility visibility,
      Guid projectId,
      Guid? viewId);

    void RemovePermissionsForVisibility(
      IVssRequestContext requestContext,
      AnalyticsViewVisibility visibility,
      Guid projectId,
      Guid? viewId);
  }
}
