// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.LegacyQueryItemGenericException
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  [Serializable]
  public class LegacyQueryItemGenericException : LegacyValidationException
  {
    public LegacyQueryItemGenericException(
      IVssRequestContext requestContext,
      int errorNumber,
      SqlException ex,
      SqlError sqlError)
      : base(requestContext, DalResourceStrings.Manager, errorNumber, (string) null, TeamFoundationServiceException.ExtractString(sqlError, "MessageResource"), LegacyQueryItemGenericException.GetParams(sqlError))
    {
    }

    private static object[] GetParams(SqlError sqlError)
    {
      string empty = TeamFoundationServiceException.ExtractString(sqlError, "Params");
      if (empty.Equals("%s"))
        empty = string.Empty;
      string[] strArray = empty.Split(',');
      object[] objArray = new object[strArray.Length];
      for (int index = 0; index < strArray.Length; ++index)
        objArray[index] = (object) strArray[index];
      return objArray;
    }
  }
}
