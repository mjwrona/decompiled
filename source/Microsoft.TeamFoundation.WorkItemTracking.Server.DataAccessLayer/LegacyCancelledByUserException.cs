// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.LegacyCancelledByUserException
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  [Serializable]
  public class LegacyCancelledByUserException : LegacyServiceException
  {
    public LegacyCancelledByUserException(string message, int id)
      : base(message, id)
    {
      this.LogException = false;
    }

    public LegacyCancelledByUserException(
      IVssRequestContext requestContext,
      int errorNumber,
      SqlException ex,
      SqlError sqlError)
      : base(LegacyCancelledByUserException.GetCancelledError(requestContext, ex, sqlError), LegacyCancelledByUserException.GetCancelledErrorCode(requestContext, ex, sqlError))
    {
      this.LogException = false;
    }

    private static string GetCancelledError(
      IVssRequestContext requestContext,
      SqlException ex,
      SqlError sqlError)
    {
      string cancelledError = ex.Message;
      if (requestContext != null && requestContext.IsCanceled && sqlError.Class == (byte) 11)
        cancelledError = "CancelledByUser";
      return cancelledError;
    }

    private static int GetCancelledErrorCode(
      IVssRequestContext requestContext,
      SqlException ex,
      SqlError sqlError)
    {
      int cancelledErrorCode = 10054;
      if (requestContext != null && requestContext.IsCanceled && sqlError.Class == (byte) 11)
        cancelledErrorCode = 602006;
      return cancelledErrorCode;
    }
  }
}
