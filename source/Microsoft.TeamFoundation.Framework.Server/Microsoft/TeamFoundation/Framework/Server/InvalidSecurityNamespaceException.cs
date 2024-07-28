// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.InvalidSecurityNamespaceException
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [Serializable]
  public class InvalidSecurityNamespaceException : TeamFoundationSecurityServiceException
  {
    public InvalidSecurityNamespaceException(Guid namespaceId)
      : base(TFCommonResources.InvalidSecurityNamespaceGuid((object) namespaceId))
    {
    }

    private InvalidSecurityNamespaceException(string namespaceId)
      : base(TFCommonResources.InvalidSecurityNamespaceGuid((object) namespaceId))
    {
    }

    public InvalidSecurityNamespaceException(
      IVssRequestContext requestContext,
      SqlException ex,
      SqlError sqlError)
      : this(TeamFoundationServiceException.ExtractString(sqlError, "id"))
    {
    }

    public InvalidSecurityNamespaceException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public InvalidSecurityNamespaceException(WrappedException wrappedException)
      : base(wrappedException.Message)
    {
    }
  }
}
