// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.XEventSessionHelper
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data.SqlClient;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public static class XEventSessionHelper
  {
    private const string c_area = "XEvent";
    private const string c_layer = "XEventSessionHelper";

    public static bool CreateXEventSession(
      IVssRequestContext requestContext,
      ITeamFoundationDatabaseProperties properties,
      string storagePath = null,
      string diagnosticsConnectionString = null)
    {
      XEventSessionSettings settings = (XEventSessionSettings) null;
      return properties.ManageXEventSession(requestContext, (Action<TeamFoundationDataTierComponent, TeamFoundationDataTierComponent>) ((dtComponent, dtComponentReadOnly) =>
      {
        dtComponent.CreateXEventSession(settings.BlobStoragePath, settings.PathSeparator, settings.SharedAccessSignature);
        if (!properties.IsReadScaleOutXEventSupported(requestContext))
          return;
        dtComponent.CreateReadScaleOutXEventSession(settings.BlobStoragePath, settings.PathSeparator, settings.SharedAccessSignature);
      }), (Action) (() => settings = properties.GetXEventSessionSettings(requestContext, storagePath, diagnosticsConnectionString)));
    }

    public static void EnableXEventLogging(
      IVssRequestContext requestContext,
      ITeamFoundationDatabaseProperties properties,
      int retryCount,
      int retryIntervalSeconds,
      Action<Exception> onRetryableException,
      Action<Exception> onFinalException)
    {
      try
      {
        properties.ManageXEventSession(requestContext, (Action<TeamFoundationDataTierComponent, TeamFoundationDataTierComponent>) ((dtComponent, dtComponentReadOnly) =>
        {
          dtComponent.StartXEventSession();
          XEventSessionHelper.StartReadScaleOutXEventSession(requestContext, properties, retryCount, retryIntervalSeconds, onRetryableException, onFinalException);
        }), (Action) (() => requestContext.GetService<ITeamFoundationDatabaseManagementService>().UpdateDatabaseProperties(requestContext, properties.DatabaseId, (Action<TeamFoundationDatabaseProperties>) (editableProperties => editableProperties.LoggingOptions |= TeamFoundationDatabaseLoggingOptions.XEvents))));
      }
      catch (Exception ex) when (ex is SqlException || ex is InvalidOperationException)
      {
        onFinalException(ex);
      }
    }

    public static void StartReadScaleOutXEventSession(
      IVssRequestContext requestContext,
      ITeamFoundationDatabaseProperties properties,
      int retryCount,
      int retryIntervalSeconds,
      Action<Exception> onRetryableException,
      Action<Exception> onFinalException)
    {
      try
      {
        if (!properties.IsReadScaleOutXEventSupported(requestContext))
          return;
        // ISSUE: method pointer
        properties.ManageXEventSession(requestContext, (Action<TeamFoundationDataTierComponent, TeamFoundationDataTierComponent>) ((_, dtComponentReadOnly) => new RetryManager(retryCount, TimeSpan.FromSeconds((double) retryIntervalSeconds), new Action<Exception>((object) this, __methodptr(\u003CStartReadScaleOutXEventSession\u003Eg__onException\u007C2)), new Func<Exception, bool>(canRetry)).Invoke((Action) (() => dtComponentReadOnly.StartReadScaleOutXEventSession()))));
      }
      catch (Exception ex) when (ex is SqlException || ex is InvalidOperationException)
      {
        onFinalException(ex);
      }

      static bool canRetry(Exception e) => e is SqlException sqlException && sqlException.Number == 15151 || e is InvalidOperationException;
    }

    public static void StopReadScaleOutXEventSessionInPrimary(
      IVssRequestContext requestContext,
      ITeamFoundationDatabaseProperties properties,
      int retryCount,
      int retryIntervalSeconds,
      Action<Exception> onRetryableException,
      Action<Exception> onFinalException)
    {
      try
      {
        if (!properties.IsReadScaleOutXEventSupported(requestContext))
          return;
        // ISSUE: method pointer
        properties.ManageXEventSession(requestContext, (Action<TeamFoundationDataTierComponent, TeamFoundationDataTierComponent>) ((dtComponent, _) => new RetryManager(retryCount, TimeSpan.FromSeconds((double) retryIntervalSeconds), new Action<Exception>((object) this, __methodptr(\u003CStopReadScaleOutXEventSessionInPrimary\u003Eg__onException\u007C2)), new Func<Exception, bool>(canRetry)).Invoke((Action) (() => dtComponent.StopReadScaleOutXEventSession(true)))));
      }
      catch (Exception ex) when (ex is SqlException)
      {
        onFinalException(ex);
      }

      static bool canRetry(Exception e) => e is SqlException sqlException && sqlException.Number != 15151 && sqlException.Number != 25704;
    }

    public static void StopReadScaleOutSessionIfDroppingEvents(
      IVssRequestContext requestContext,
      ITeamFoundationDatabaseProperties properties)
    {
      if (!properties.IsReadScaleOutXEventSupported(requestContext))
        return;
      properties.ManageXEventSession(requestContext, (Action<TeamFoundationDataTierComponent, TeamFoundationDataTierComponent>) ((_, dtComponentReadOnly) =>
      {
        if (!dtComponentReadOnly.IsXEventSessionDroppingBuffers(XEventsConstants.ReadScaleOutXEventSessionName, true))
          return;
        dtComponentReadOnly.StopReadScaleOutXEventSession();
      }));
    }
  }
}
