// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DatabaseNotFoundException
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [Serializable]
  public class DatabaseNotFoundException : TeamFoundationServiceException
  {
    public DatabaseNotFoundException()
    {
    }

    public DatabaseNotFoundException(int databaseId)
      : base(FrameworkResources.DatabaseNotFoundExceptionMessage((object) databaseId))
    {
    }

    public DatabaseNotFoundException(string databaseName)
      : base(FrameworkResources.DatabaseNotFoundByNameMessage((object) databaseName))
    {
    }

    public DatabaseNotFoundException(WrappedException wrappedException)
      : base(wrappedException.Message)
    {
    }

    public DatabaseNotFoundException(
      IVssRequestContext requestContext,
      SqlException ex,
      SqlError sqlError)
      : this(TeamFoundationServiceException.ExtractInt(sqlError, "databaseId"))
    {
    }

    protected DatabaseNotFoundException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
