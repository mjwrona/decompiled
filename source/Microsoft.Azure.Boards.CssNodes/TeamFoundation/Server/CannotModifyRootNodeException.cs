// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.CannotModifyRootNodeException
// Assembly: Microsoft.Azure.Boards.CssNodes, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D887A041-2C68-42E5-BA83-E261159AB40A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Boards.CssNodes.dll

using Microsoft.Azure.Boards.CssNodes;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server
{
  [Serializable]
  public class CannotModifyRootNodeException : CommonStructureSubsystemServiceException
  {
    public CannotModifyRootNodeException()
    {
    }

    public CannotModifyRootNodeException(string objectId, Exception innerException)
      : base(Microsoft.Azure.Boards.CssNodes.ServerResources.CSS_CANNOT_MODIFY_ROOT_NODE((object) CommonStructureUtils.GetNodeUri(objectId)))
    {
    }

    public CannotModifyRootNodeException(string objectId)
      : this(objectId, (Exception) null)
    {
    }

    public CannotModifyRootNodeException(
      IVssRequestContext requestContext,
      SqlException ex,
      SqlError sqlError)
      : this(TeamFoundationServiceException.ExtractString(sqlError, "id"))
    {
    }

    protected CannotModifyRootNodeException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
