// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.NodeAlreadyExistsException
// Assembly: Microsoft.Azure.Boards.CssNodes, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D887A041-2C68-42E5-BA83-E261159AB40A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Boards.CssNodes.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server
{
  [Serializable]
  public class NodeAlreadyExistsException : CommonStructureSubsystemServiceException
  {
    public NodeAlreadyExistsException()
    {
    }

    public NodeAlreadyExistsException(string name, Exception innerException)
      : base(Microsoft.Azure.Boards.CssNodes.ServerResources.CSS_NODE_ALREADY_EXISTS((object) DBPath.DatabaseToUserPath(name)), innerException)
    {
    }

    public NodeAlreadyExistsException(string name)
      : this(name, (Exception) null)
    {
    }

    public NodeAlreadyExistsException(
      IVssRequestContext requestContext,
      SqlException ex,
      SqlError sqlError)
      : this(TeamFoundationServiceException.ExtractString(sqlError, "name"))
    {
    }

    protected NodeAlreadyExistsException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
