// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.RequestContextExtensions
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Organization;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public static class RequestContextExtensions
  {
    public static void TraceExceptionMsg(
      this IVssRequestContext requestContext,
      int tracepoint,
      string area,
      string layer,
      Exception exception,
      string format,
      params object[] args)
    {
      requestContext.RequestTracer.TraceException(tracepoint, TraceLevel.Error, area, layer, exception, "{0}\r\n{1}", (object) string.Format(format, args), (object) exception);
    }

    public static bool IsCollectionSoftDeleted(this IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      if (requestContext.IsFeatureEnabled("Notifications.DisableNotificationsForLogicallyDeletedAccounts"))
      {
        try
        {
          return requestContext.GetService<ICollectionService>().GetCollection(requestContext, (IEnumerable<string>) null).Status == CollectionStatus.LogicallyDeleted;
        }
        catch (Exception ex)
        {
          requestContext.TraceException(1003010, "Notifications", nameof (IsCollectionSoftDeleted), ex);
        }
      }
      return false;
    }
  }
}
