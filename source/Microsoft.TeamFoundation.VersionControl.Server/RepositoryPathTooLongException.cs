// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.RepositoryPathTooLongException
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  [Serializable]
  public class RepositoryPathTooLongException : ServerException
  {
    public RepositoryPathTooLongException(string serverItem, int maxServerPath)
      : base(Resources.Format("RepositoryPathTooLong", (object) serverItem, (object) (maxServerPath - 11), (object) maxServerPath))
    {
    }

    public RepositoryPathTooLongException(
      IVssRequestContext requestContext,
      SqlException ex,
      SqlError sqlError)
      : this(ServerException.ExtractDataspaceServerItem(requestContext, sqlError, "serverItem"), 434)
    {
    }
  }
}
