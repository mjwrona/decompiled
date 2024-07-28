// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.ServerException
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Web.Services.Protocols;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  [Serializable]
  public abstract class ServerException : SoapSerializableException
  {
    protected ServerException()
    {
      this.EventId = TeamFoundationEventId.ServerException;
      this.Source = Resources.Get("ExceptionSource");
    }

    protected ServerException(string message)
      : base(message)
    {
      this.EventId = TeamFoundationEventId.ServerException;
    }

    protected ServerException(string message, Exception innerException)
      : base(message, innerException)
    {
      this.EventId = TeamFoundationEventId.ServerException;
    }

    public override SoapException ToSoapException() => new Failure((Exception) this).toSoapException();

    public virtual void SetFailureInfo(Failure failure)
    {
    }

    protected static string ExtractServerItem(SqlError error, string key) => DBPath.DatabaseToServerPath(TeamFoundationServiceException.ExtractString(error, key));

    protected static string ExtractDataspaceServerItem(
      IVssRequestContext requestContext,
      SqlError error,
      string key)
    {
      string serverItem = ServerException.ExtractServerItem(error, key);
      try
      {
        return ProjectUtility.ConvertToPathWithProjectName(requestContext, serverItem, false);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(700290, TraceArea.Exceptions, TraceLayer.Component, ex);
      }
      return serverItem;
    }

    protected static string ExtractLocalItem(SqlError error, string key) => DBPath.DatabaseToLocalPath(TeamFoundationServiceException.ExtractString(error, key));

    protected static string ExtractLocalOrServerItem(SqlError error, string key) => DBPath.DatabaseToLocalOrServerPath(TeamFoundationServiceException.ExtractString(error, key));

    protected static string ExtractIdentityName(
      IVssRequestContext requestContext,
      SqlError error,
      string key)
    {
      string identityDisplayName = TeamFoundationServiceException.ExtractString(error, key);
      Guid result;
      if (Guid.TryParse(identityDisplayName, out result))
      {
        TeamFoundationVersionControlService service = requestContext.GetService<TeamFoundationVersionControlService>();
        using (requestContext.AcquireExemptionLock())
          identityDisplayName = service.SecurityWrapper.FindIdentityDisplayName(requestContext, result);
      }
      else
      {
        string message = string.Format("SQL Error returned invalid guid. Key:{0} Value:{1}", (object) key, (object) identityDisplayName);
        requestContext.Trace(700321, TraceLevel.Error, TraceArea.Identities, TraceLayer.Component, message);
      }
      return identityDisplayName;
    }
  }
}
