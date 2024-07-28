// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GitRepositoryNotFoundException
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.Git.Server
{
  [Serializable]
  public class GitRepositoryNotFoundException : TeamFoundationServiceException
  {
    public GitRepositoryNotFoundException(string name)
      : base(Resources.Format("GitRepositoryNotFound", (object) name))
    {
    }

    public GitRepositoryNotFoundException(WrappedException wrappedException)
      : base(wrappedException.Message)
    {
    }

    public GitRepositoryNotFoundException(Guid repositoryId)
      : base(Resources.Format("GitRepositoryNotFound", (object) repositoryId.ToString()))
    {
    }

    public GitRepositoryNotFoundException(string name, string repositoryIdAsString)
      : base(Resources.Format("GitRepositoryNotFound", string.IsNullOrEmpty(name) ? (object) repositoryIdAsString : (object) name))
    {
    }

    public GitRepositoryNotFoundException(
      IVssRequestContext requestContext,
      SqlException ex,
      SqlError sqlError)
      : this(TeamFoundationServiceException.ExtractString(sqlError, "name"), TeamFoundationServiceException.ExtractString(sqlError, "repositoryId"))
    {
    }
  }
}
