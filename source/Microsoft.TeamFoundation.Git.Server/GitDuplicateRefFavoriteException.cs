// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GitDuplicateRefFavoriteException
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.Git.Server
{
  [Serializable]
  public class GitDuplicateRefFavoriteException : TeamFoundationServiceException
  {
    public GitDuplicateRefFavoriteException(string refName, string userId, string repoId)
      : base(Resources.Format("GitRefFavoriteAlreadyExists", (object) refName, (object) userId, (object) repoId))
    {
    }

    public GitDuplicateRefFavoriteException(
      IVssRequestContext requestContext,
      SqlException ex,
      SqlError sqlError)
      : this(TeamFoundationServiceException.ExtractString(sqlError, "refName"), TeamFoundationServiceException.ExtractString(sqlError, "identityId"), TeamFoundationServiceException.ExtractString(sqlError, "repositoryId"))
    {
    }
  }
}
