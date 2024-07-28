// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.BuildServerException
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using System;
using System.Data.SqlClient;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Build.Server
{
  [Serializable]
  public class BuildServerException : TeamFoundationServiceException
  {
    internal BuildServerException()
    {
      this.LogLevel = EventLogEntryType.Error;
      this.EventId = TeamFoundationEventId.BuildServerException;
    }

    internal BuildServerException(string message)
      : base(message)
    {
      this.LogLevel = EventLogEntryType.Error;
      this.EventId = TeamFoundationEventId.BuildServerException;
    }

    internal BuildServerException(string message, Exception innerException)
      : base(message, innerException)
    {
      this.LogLevel = EventLogEntryType.Error;
      this.EventId = TeamFoundationEventId.BuildServerException;
    }

    protected static string GetTeamProjectName(IVssRequestContext context, string projectUri) => context.GetService<IProjectService>().GetTeamProjectFromUri(context, projectUri).Name;

    protected static string GetTeamProjectName(
      IVssRequestContext context,
      SqlError err,
      string key)
    {
      string s = TeamFoundationServiceException.ExtractString(err, key);
      int result;
      if (!int.TryParse(s, out result))
        return s;
      Guid dataspaceIdentifier = context.GetService<IDataspaceService>().QueryDataspace(context.Elevate(), result).DataspaceIdentifier;
      return context.GetService<IProjectService>().GetProjectName(context.Elevate(), dataspaceIdentifier);
    }
  }
}
