// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Integration.Server.ServerExceptionUtilities
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.DirectoryServices.ActiveDirectory;
using System.Net;
using System.Security;
using System.Security.Authentication;
using System.Threading;
using System.Web.Services.Protocols;
using System.Xml;

namespace Microsoft.TeamFoundation.Integration.Server
{
  internal class ServerExceptionUtilities
  {
    internal static readonly string SqlNumber = nameof (SqlNumber);
    internal static readonly string BaseExceptionName = nameof (BaseExceptionName);
    internal static readonly string ExceptionMessage = nameof (ExceptionMessage);
    internal static readonly string ServerTimeStamp = nameof (ServerTimeStamp);
    internal static readonly Type[] s_nonReportableExceptions = new Type[26]
    {
      typeof (ArgumentException),
      typeof (ArgumentNullException),
      typeof (AuthenticationException),
      typeof (ConfigurationException),
      typeof (DatabaseConnectionException),
      typeof (DatabaseConfigurationException),
      typeof (DatabaseFullException),
      typeof (DatabaseRuntimeException),
      typeof (DatabaseOperationTimeoutException),
      typeof (InvalidOperationException),
      typeof (SoapException),
      typeof (WebException),
      typeof (AuthorizationSubsystemServiceException),
      typeof (GroupSecuritySubsystemServiceException),
      typeof (SecurityException),
      typeof (ThreadAbortException),
      typeof (UnauthorizedAccessException),
      typeof (AnalysisServiceConnectionException),
      typeof (CommonStructureSubsystemServiceException),
      typeof (SyncSubsystemServiceException),
      typeof (TeamFoundationServerUnauthorizedException),
      typeof (TeamFoundationServiceUnavailableException),
      typeof (TeamFoundationServerVersionCheckException),
      typeof (ActiveDirectoryObjectNotFoundException),
      typeof (XmlException),
      typeof (ProjectException)
    };

    public static Exception LogAndFilter(Exception e, string userName)
    {
      e = ServerExceptionUtilities.TranslateSqlException(e);
      if (ServerExceptionUtilities.IsReportable(e))
      {
        Microsoft.TeamFoundation.Common.Diagnostics.ReportException(Microsoft.TeamFoundation.Common.Diagnostics.ELeadCategory, e);
      }
      else
      {
        switch (e)
        {
          case ConfigurationException _:
          case XmlException _:
          case WebException _:
            TeamFoundationEventLog.Default.LogException(e.Message, e, TeamFoundationEventId.ConfigurationException, EventLogEntryType.Error);
            break;
          case SqlException _:
            TeamFoundationEventLog.Default.LogException(e.Message, e, TeamFoundationEventId.DatabaseConnectionException, EventLogEntryType.Error);
            break;
          case AnalysisServiceConnectionException _:
            TeamFoundationEventLog.Default.LogException(e.Message, e, TeamFoundationEventId.AnalysisServiceConnectionException, EventLogEntryType.Error);
            break;
          case TeamFoundationServiceException _:
            TeamFoundationServiceException serviceException = (TeamFoundationServiceException) e;
            if (serviceException.LogException)
            {
              TeamFoundationEventLog.Default.LogException(e.Message, e, serviceException.EventId, serviceException.LogLevel);
              break;
            }
            break;
          case UnauthorizedAccessException _:
            break;
          default:
            TeamFoundationEventLog.Default.LogException(e.Message, e, TeamFoundationEventId.DefaultExceptionEventId, EventLogEntryType.Error);
            break;
        }
      }
      return e;
    }

    internal static bool IsReportable(Exception ex)
    {
      if (ex is TeamFoundationServiceException)
        return ((VssException) ex).ReportException;
      Type type = ex.GetType();
      foreach (Type reportableException in ServerExceptionUtilities.s_nonReportableExceptions)
      {
        if (reportableException.IsAssignableFrom(type))
          return false;
      }
      return true;
    }

    internal static Exception TranslateSqlException(Exception ex) => ex is SqlException sqlException ? TeamFoundationSqlResourceComponent.TranslateSqlException(sqlException) : ex;
  }
}
