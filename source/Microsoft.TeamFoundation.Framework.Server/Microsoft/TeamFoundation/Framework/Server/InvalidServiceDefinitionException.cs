// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.InvalidServiceDefinitionException
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
  public class InvalidServiceDefinitionException : TeamFoundationLocationServiceException
  {
    public InvalidServiceDefinitionException(string message)
      : base(message)
    {
    }

    public InvalidServiceDefinitionException(
      IVssRequestContext requestContext,
      SqlException ex,
      SqlError sqlError)
      : this(InvalidServiceDefinitionException.DetermineErrorString(ex, sqlError))
    {
    }

    protected InvalidServiceDefinitionException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }

    private static string DetermineErrorString(SqlException ex, SqlError sqlError) => TeamFoundationServiceException.ExtractInt(sqlError, "error") == 800006 ? TFCommonResources.ServiceDefinitionWithNoLocations((object) TeamFoundationServiceException.ExtractString(sqlError, "service type")) : FrameworkResources.ServiceDefinitionAlreadyExists((object) TeamFoundationServiceException.ExtractString(sqlError, "service type"), (object) TeamFoundationServiceException.ExtractString(sqlError, "identifier"));
  }
}
