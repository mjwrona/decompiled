// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.ReclassificationNodeUriDoesNotExistException
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
  public class ReclassificationNodeUriDoesNotExistException : 
    CommonStructureSubsystemServiceException
  {
    public ReclassificationNodeUriDoesNotExistException()
    {
    }

    public ReclassificationNodeUriDoesNotExistException(
      string reclassifyId,
      Exception innerException)
      : base(Microsoft.Azure.Boards.CssNodes.ServerResources.CSS_RECLASSIFICATION_NODE_DOES_NOT_EXIST((object) CommonStructureUtils.GetNodeUri(reclassifyId)), innerException)
    {
    }

    public ReclassificationNodeUriDoesNotExistException(string reclassifyId)
      : this(reclassifyId, (Exception) null)
    {
    }

    public ReclassificationNodeUriDoesNotExistException(
      IVssRequestContext requestContext,
      SqlException ex,
      SqlError sqlError)
      : this(TeamFoundationServiceException.ExtractString(sqlError, "reclassify_id"))
    {
    }

    protected ReclassificationNodeUriDoesNotExistException(
      SerializationInfo info,
      StreamingContext context)
      : base(info, context)
    {
    }
  }
}
