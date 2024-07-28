// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.QueuedBuildUpdateException
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.Build.Server
{
  [Serializable]
  public sealed class QueuedBuildUpdateException : AdministrationException
  {
    public QueuedBuildUpdateException(
      IVssRequestContext requestContext,
      SqlException ex,
      SqlError err)
      : base(QueuedBuildUpdateException.FormatMessage(requestContext, ex, err))
    {
    }

    private static string LocalizeStatus(int queueStatus)
    {
      string str = BuildTypeResource.Status_InProgress();
      switch (queueStatus)
      {
        case 4:
          str = BuildTypeResource.Status_Queued();
          break;
        case 8:
          str = BuildTypeResource.Status_Postponed();
          break;
        case 16:
          str = BuildTypeResource.Status_Completed();
          break;
        case 32:
          str = BuildTypeResource.Status_Canceled();
          break;
      }
      return str;
    }

    private static string FormatMessage(
      IVssRequestContext requestContext,
      SqlException ex,
      SqlError err)
    {
      switch (TeamFoundationServiceException.ExtractInt(err, "error"))
      {
        case 900532:
          return AdministrationResources.CannotCancelQueuedBuild((object) TeamFoundationServiceException.ExtractInt(err, "queueId"), (object) QueuedBuildUpdateException.LocalizeStatus(TeamFoundationServiceException.ExtractInt(err, "status")), (object) DBHelper.DBPathToServerPath(TeamFoundationServiceException.ExtractString(err, "controllerName")));
        case 900538:
          return AdministrationResources.CannotRetryQueuedBuild((object) TeamFoundationServiceException.ExtractInt(err, "queueId"), (object) QueuedBuildUpdateException.LocalizeStatus(TeamFoundationServiceException.ExtractInt(err, "status")), (object) DBHelper.DBPathToServerPath(TeamFoundationServiceException.ExtractString(err, "controllerName")));
        default:
          return AdministrationResources.CannotUpdateQueuedBuild((object) TeamFoundationServiceException.ExtractInt(err, "queueId"), (object) QueuedBuildUpdateException.LocalizeStatus(TeamFoundationServiceException.ExtractInt(err, "status")), (object) DBHelper.DBPathToServerPath(TeamFoundationServiceException.ExtractString(err, "controllerName")));
      }
    }
  }
}
