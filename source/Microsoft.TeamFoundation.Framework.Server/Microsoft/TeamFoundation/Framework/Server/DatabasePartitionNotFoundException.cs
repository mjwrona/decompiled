// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DatabasePartitionNotFoundException
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Data.SqlClient;
using System.Globalization;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [Serializable]
  public class DatabasePartitionNotFoundException : TeamFoundationServiceException
  {
    public DatabasePartitionNotFoundException(Guid hostId)
      : base(FrameworkResources.DatabasePartitionNotFound((object) hostId.ToString("D", (IFormatProvider) CultureInfo.InvariantCulture)))
    {
    }

    public DatabasePartitionNotFoundException(int partitionId)
      : base(FrameworkResources.DatabasePartitionIdNotFound((object) partitionId))
    {
    }

    public DatabasePartitionNotFoundException(string databaseName)
      : base(FrameworkResources.NoDatabasePartitionFound((object) databaseName))
    {
    }

    public DatabasePartitionNotFoundException(
      IVssRequestContext requestContext,
      SqlException ex,
      SqlError sqlError)
      : base(!string.IsNullOrEmpty(TeamFoundationServiceException.ExtractString(sqlError, "HostId")) ? FrameworkResources.DatabasePartitionNotFound((object) TeamFoundationServiceException.ExtractString(sqlError, "HostId")) : FrameworkResources.DatabasePartitionIdNotFound((object) TeamFoundationServiceException.ExtractInt(sqlError, "PartitionId")))
    {
    }

    public DatabasePartitionNotFoundException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }

    public DatabasePartitionNotFoundException(WrappedException wrappedException)
      : base(wrappedException.Message)
    {
    }
  }
}
