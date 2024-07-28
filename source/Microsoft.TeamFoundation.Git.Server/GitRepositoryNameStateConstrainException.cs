// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GitRepositoryNameStateConstrainException
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
  public class GitRepositoryNameStateConstrainException : TeamFoundationServiceException
  {
    public GitRepositoryNameStateConstrainException(string name)
      : base(Resources.Format("GitRepositoryNameStateConstrain", (object) name))
    {
    }

    public GitRepositoryNameStateConstrainException(WrappedException wrappedException)
      : base(wrappedException.Message)
    {
    }

    public GitRepositoryNameStateConstrainException(
      IVssRequestContext requestContext,
      SqlException ex,
      SqlError sqlError)
      : this(TeamFoundationServiceException.ExtractString(sqlError, "name"))
    {
    }
  }
}
