// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.ReclassifiedToSubTreeException
// Assembly: Microsoft.Azure.Boards.CssNodes, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D887A041-2C68-42E5-BA83-E261159AB40A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Boards.CssNodes.dll

using Microsoft.Azure.Boards.CssNodes;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.Server
{
  [Serializable]
  public class ReclassifiedToSubTreeException : InvalidReclassificationException
  {
    public ReclassifiedToSubTreeException(string objectId, string reclassifyId)
      : base(Microsoft.Azure.Boards.CssNodes.ServerResources.CSS_RECLASSIFICATION_TO_SUBTREE((object) CommonStructureUtils.GetNodeUri(objectId), (object) CommonStructureUtils.GetNodeUri(reclassifyId)))
    {
    }

    public ReclassifiedToSubTreeException(
      IVssRequestContext requestContext,
      SqlException ex,
      SqlError sqlError)
      : this(TeamFoundationServiceException.ExtractString(sqlError, "id"), TeamFoundationServiceException.ExtractString(sqlError, "reclassify_id"))
    {
    }
  }
}
