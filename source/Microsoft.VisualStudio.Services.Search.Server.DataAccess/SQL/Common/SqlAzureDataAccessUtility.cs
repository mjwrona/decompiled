// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer.Sql.Common.SqlAzureDataAccessUtility
// Assembly: Microsoft.VisualStudio.Services.Search.Server.DataAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3B684226-797D-4C9F-9AC1-E10D39E316D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.DataAccess.dll

using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer.Sql.Common
{
  internal static class SqlAzureDataAccessUtility
  {
    public static void WrapAndThrowException(Exception e)
    {
      string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Azure Platform Exception : {0}", (object) e.ToString());
      AggregateException aggregateException = typeof (AggregateException).IsAssignableFrom(e.GetType()) ? e as AggregateException : throw new DataAccessException(DataAccessErrorCodeEnum.UNEXPECTED_ERROR, e.InnerException, message);
      ReadOnlyCollection<Exception> innerExceptions = aggregateException.InnerExceptions;
      if (innerExceptions == null || innerExceptions.Count == 0)
        throw new AggregateException((IEnumerable<Exception>) innerExceptions);
      if (innerExceptions.All<Exception>((Func<Exception, bool>) (ex => typeof (TableAccessException).IsAssignableFrom(ex.GetType()) && TableAcessErrorCodeEnum.UNEXPECTED_ERROR == (ex as TableAccessException).ErrorCode)))
        throw new DataAccessException(DataAccessErrorCodeEnum.UNEXPECTED_ERROR, (Exception) aggregateException.Flatten(), message);
      throw aggregateException.Flatten();
    }
  }
}
