// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.RegistryUninitializedException
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Data.SqlClient;
using System.Globalization;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [Serializable]
  public class RegistryUninitializedException : TeamFoundationServiceException
  {
    public RegistryUninitializedException(int partitionId)
      : base(FrameworkResources.RegistryUninitializedForPartition((object) partitionId.ToString((IFormatProvider) CultureInfo.InvariantCulture)))
    {
    }

    public RegistryUninitializedException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }

    public RegistryUninitializedException(
      IVssRequestContext requestContext,
      SqlException ex,
      SqlError sqlError)
      : this(TeamFoundationServiceException.ExtractInt(sqlError, "partitionId"))
    {
    }
  }
}
