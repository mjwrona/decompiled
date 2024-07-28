// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.IdenticalPathsDueToCollationException
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  [Serializable]
  public class IdenticalPathsDueToCollationException : ServerException
  {
    public IdenticalPathsDueToCollationException(string failedPath1, string failedPath2)
      : base(Resources.Format("IdenticalPathsError", (object) failedPath1, (object) failedPath2))
    {
    }

    public IdenticalPathsDueToCollationException(
      IVssRequestContext requestContext,
      SqlException ex,
      SqlError sqlError)
      : this(ServerException.ExtractDataspaceServerItem(requestContext, sqlError, "item1"), ServerException.ExtractDataspaceServerItem(requestContext, sqlError, "item2"))
    {
    }
  }
}
