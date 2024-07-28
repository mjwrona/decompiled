// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.LocationMappingDoesNotExistException
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common.Internal;
using System;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [Serializable]
  public class LocationMappingDoesNotExistException : TeamFoundationLocationServiceException
  {
    public LocationMappingDoesNotExistException()
    {
    }

    public LocationMappingDoesNotExistException(
      string serviceType,
      string identifier,
      string moniker)
      : base(TFCommonResources.LocationMappingDoesNotExist((object) serviceType, (object) identifier, (object) moniker))
    {
    }

    public LocationMappingDoesNotExistException(
      IVssRequestContext requestContext,
      SqlException ex,
      SqlError sqlError)
      : this(TeamFoundationServiceException.ExtractString(sqlError, "service type"), TeamFoundationServiceException.ExtractString(sqlError, "identifier"), TeamFoundationServiceException.ExtractString(sqlError, "moniker"))
    {
    }

    protected LocationMappingDoesNotExistException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
