// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.RegisterObjectBadParentException
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
  public class RegisterObjectBadParentException : AuthorizationSubsystemServiceException
  {
    public RegisterObjectBadParentException(string objectId, Exception innerException)
      : base(Microsoft.Azure.Boards.CssNodes.ServerResources.GSS_REGISTEROBJECTBADPARENTERROR((object) objectId), innerException)
    {
    }

    public RegisterObjectBadParentException(string objectId)
      : this(objectId, (Exception) null)
    {
    }

    public RegisterObjectBadParentException(
      IVssRequestContext requestContext,
      SqlException ex,
      SqlError sqlError)
      : this(TeamFoundationServiceException.ExtractString(sqlError, "object_id"))
    {
    }

    protected RegisterObjectBadParentException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
