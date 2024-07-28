// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.CannotAddDateToNonIterationException
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
  public class CannotAddDateToNonIterationException : CommonStructureSubsystemServiceException
  {
    public CannotAddDateToNonIterationException(string objectId)
      : base(Microsoft.Azure.Boards.CssNodes.ServerResources.CSS_CANNOT_ADD_DATE_TO_NONITERATION((object) CommonStructureUtils.GetNodeUri(objectId)))
    {
    }

    public CannotAddDateToNonIterationException(
      IVssRequestContext requestContext,
      SqlException ex,
      SqlError sqlError)
      : this(TeamFoundationServiceException.ExtractString(sqlError, "id"))
    {
    }
  }
}
