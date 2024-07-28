// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer.Sql.SqlAzureDataAccess
// Assembly: Microsoft.VisualStudio.Services.Search.Server.DataAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3B684226-797D-4C9F-9AC1-E10D39E316D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.DataAccess.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer.Sql.Common;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer.Sql
{
  public class SqlAzureDataAccess
  {
    protected const int SqlAzureInsertBatchSize = 100;
    [StaticSafe]
    protected static TraceMetaData s_TraceMetadata = new TraceMetaData(1080403, "Indexing Pipeline", "Framework");
    protected ITableAccessPlatform m_tableAccessPlatform;

    public SqlAzureDataAccess()
      : this((ITableAccessPlatform) new TableAccessPlatform())
    {
    }

    protected SqlAzureDataAccess(ITableAccessPlatform tableAccessPlatform) => this.m_tableAccessPlatform = tableAccessPlatform;

    internal T InvokeTableOperation<T>(Func<T> operation)
    {
      try
      {
        return operation();
      }
      catch (Exception ex)
      {
        SqlAzureDataAccessUtility.WrapAndThrowException(ex);
        throw;
      }
    }

    internal void ValidateNotNull<T>(string propertyName, T value)
    {
      if ((object) value == null)
        throw new DataAccessException(DataAccessErrorCodeEnum.INVALID_ARGUMENTS, (Exception) new ArgumentNullException(propertyName));
    }

    internal void ValidateNotNullOrWhiteSpace(string propertyName, string value)
    {
      if (string.IsNullOrWhiteSpace(value))
        throw new DataAccessException(DataAccessErrorCodeEnum.INVALID_ARGUMENTS, (Exception) new ArgumentNullException(propertyName));
    }

    internal void ValidateNotEmptyGuid(string propertyName, Guid value)
    {
      if (Guid.Empty == value)
        throw new DataAccessException(DataAccessErrorCodeEnum.INVALID_ARGUMENTS, (Exception) new ArgumentNullException(propertyName));
    }

    internal void ValidateNotNullOrEmptyList<T>(string propertyName, IList<T> objectList)
    {
      if (objectList == null || objectList.Count == 0)
        throw new DataAccessException(DataAccessErrorCodeEnum.INVALID_ARGUMENTS, (Exception) new ArgumentException(propertyName + " is null or empty"));
    }
  }
}
