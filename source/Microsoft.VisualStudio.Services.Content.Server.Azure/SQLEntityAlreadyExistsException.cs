// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Azure.SQLEntityAlreadyExistsException
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7823E4AE-BEB6-4A7C-9914-276DEAE1FB1F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Azure.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data.SqlClient;
using System.Net;

namespace Microsoft.VisualStudio.Services.Content.Server.Azure
{
  [Serializable]
  internal class SQLEntityAlreadyExistsException : ASTableException
  {
    public override HttpStatusCode HttpStatusCode => HttpStatusCode.Conflict;

    public SQLEntityAlreadyExistsException(string msg, int index)
      : base(msg, index)
    {
    }

    public static SqlExceptionFactory CreateSqlExceptionFactory() => new SqlExceptionFactory(typeof (SQLEntityAlreadyExistsException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, errNo, sqlException, sqlError) => (Exception) new SQLEntityAlreadyExistsException(sqlException.Message, sqlError.ExtractInt("index"))));
  }
}
