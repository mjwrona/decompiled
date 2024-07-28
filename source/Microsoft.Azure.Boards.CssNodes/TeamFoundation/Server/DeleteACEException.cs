// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.DeleteACEException
// Assembly: Microsoft.Azure.Boards.CssNodes, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D887A041-2C68-42E5-BA83-E261159AB40A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Boards.CssNodes.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data.SqlClient;
using System.Globalization;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server
{
  [Serializable]
  public class DeleteACEException : GroupSecuritySubsystemServiceException
  {
    public DeleteACEException(string objectId, string actionId, string user, int deny)
      : base(Microsoft.Azure.Boards.CssNodes.ServerResources.GSS_DELETEACEERROR((object) objectId, (object) actionId, (object) user, (object) deny.ToString((IFormatProvider) CultureInfo.CurrentCulture)))
    {
    }

    public DeleteACEException(
      IVssRequestContext requestContext,
      SqlException ex,
      SqlError sqlError)
      : this(TeamFoundationServiceException.ExtractString(sqlError, "object_id"), TeamFoundationServiceException.ExtractString(sqlError, "action_id"), TeamFoundationServiceException.ExtractString(sqlError, "user"), TeamFoundationServiceException.ExtractInt(sqlError, "deny"))
    {
    }

    protected DeleteACEException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
