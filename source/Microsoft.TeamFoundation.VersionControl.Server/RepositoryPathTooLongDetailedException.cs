// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.RepositoryPathTooLongDetailedException
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  [Serializable]
  public class RepositoryPathTooLongDetailedException : ServerException
  {
    public RepositoryPathTooLongDetailedException(string serverItem, int maxPathLength)
    {
      string message;
      if (maxPathLength != 259)
        message = Resources.Format("RepositoryPathTooLong", (object) serverItem, (object) (maxPathLength - 11), (object) maxPathLength);
      else
        message = Microsoft.TeamFoundation.VersionControl.Common.Internal.Resources.Format("RepositoryPathTooLongDetailed", (object) serverItem);
      // ISSUE: explicit constructor call
      base.\u002Ector(message);
      this.MaxServerPathLength = maxPathLength;
    }

    public RepositoryPathTooLongDetailedException(
      IVssRequestContext requestContext,
      SqlException ex,
      SqlError sqlError)
      : this(ServerException.ExtractDataspaceServerItem(requestContext, sqlError, "serverItem"), TeamFoundationServiceException.ExtractInt(sqlError, "maxPathLength") - 1)
    {
    }

    public int MaxServerPathLength { get; private set; }
  }
}
