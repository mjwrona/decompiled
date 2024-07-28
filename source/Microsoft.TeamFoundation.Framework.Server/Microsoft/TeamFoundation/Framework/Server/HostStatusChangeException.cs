// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.HostStatusChangeException
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using System;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [Serializable]
  public class HostStatusChangeException : HostManagementException
  {
    public HostStatusChangeException(string message)
      : this(message, Guid.Empty)
    {
    }

    public HostStatusChangeException(
      IVssRequestContext requestContext,
      int errorNumber,
      SqlException ex,
      SqlError sqlError)
      : this(HostStatusChangeException.FormatMessage(requestContext, errorNumber, ex, sqlError), HostStatusChangeException.GetHostId(sqlError, "identifier"))
    {
    }

    public HostStatusChangeException(string message, Guid hostId)
      : base(message)
    {
    }

    public Guid HostId { get; set; }

    private static string FormatMessage(
      IVssRequestContext requestContext,
      int errorNumber,
      SqlException ex,
      SqlError sqlError)
    {
      string str = TeamFoundationServiceException.ExtractString(sqlError, "name");
      Guid hostId = HostStatusChangeException.GetHostId(sqlError, "identifier");
      string hostActionLocalized = HostStatusChangeException.GetHostActionLocalized(sqlError);
      if (errorNumber == 800029)
      {
        string hostStatusLocalized = HostStatusChangeException.GetHostStatusLocalized(sqlError, "status");
        return requestContext != null && hostId == requestContext.ServiceHost.InstanceId ? FrameworkResources.ConfigurationHostCannotChangeStatus((object) hostActionLocalized, (object) hostActionLocalized) : FrameworkResources.CollectionHostCannotChangeStatus((object) str, (object) hostActionLocalized, (object) hostStatusLocalized);
      }
      string hostStatusLocalized1 = HostStatusChangeException.GetHostStatusLocalized(sqlError, "parentStatus");
      return FrameworkResources.CollectionCannotChangeStatusParent((object) str, (object) hostActionLocalized, (object) hostStatusLocalized1);
    }

    private static string GetHostActionLocalized(SqlError sqlError)
    {
      TeamFoundationServiceHostStatus serviceHostStatus = (TeamFoundationServiceHostStatus) TeamFoundationServiceException.ExtractInt(sqlError, "action");
      string hostActionLocalized;
      switch (serviceHostStatus)
      {
        case TeamFoundationServiceHostStatus.Started:
          hostActionLocalized = FrameworkResources.HostActionStarted();
          break;
        case TeamFoundationServiceHostStatus.Stopped:
          hostActionLocalized = FrameworkResources.HostActionStopped();
          break;
        case TeamFoundationServiceHostStatus.Paused:
          hostActionLocalized = FrameworkResources.HostActionPaused();
          break;
        default:
          hostActionLocalized = serviceHostStatus.ToString();
          break;
      }
      return hostActionLocalized;
    }

    private static string GetHostStatusLocalized(SqlError sqlError, string key)
    {
      TeamFoundationServiceHostStatus serviceHostStatus = (TeamFoundationServiceHostStatus) TeamFoundationServiceException.ExtractInt(sqlError, key);
      string hostStatusLocalized;
      switch (serviceHostStatus)
      {
        case TeamFoundationServiceHostStatus.Starting:
          hostStatusLocalized = FrameworkResources.HostStateStarting();
          break;
        case TeamFoundationServiceHostStatus.Started:
          hostStatusLocalized = FrameworkResources.HostStateStarted();
          break;
        case TeamFoundationServiceHostStatus.Stopping:
          hostStatusLocalized = FrameworkResources.HostStateStopping();
          break;
        case TeamFoundationServiceHostStatus.Stopped:
          hostStatusLocalized = FrameworkResources.HostStateStopped();
          break;
        case TeamFoundationServiceHostStatus.Pausing:
          hostStatusLocalized = FrameworkResources.HostStatePausing();
          break;
        case TeamFoundationServiceHostStatus.Paused:
          hostStatusLocalized = FrameworkResources.HostStatePaused();
          break;
        default:
          hostStatusLocalized = serviceHostStatus.ToString();
          break;
      }
      return hostStatusLocalized;
    }

    private static Guid GetHostId(SqlError error, string key)
    {
      try
      {
        if (string.IsNullOrEmpty(TeamFoundationServiceException.ExtractString(error, key)))
          return new Guid(TeamFoundationServiceException.ExtractString(error, key));
      }
      catch (Exception ex)
      {
      }
      return Guid.Empty;
    }
  }
}
