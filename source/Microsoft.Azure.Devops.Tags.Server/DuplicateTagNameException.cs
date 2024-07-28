// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Devops.Tags.Server.DuplicateTagNameException
// Assembly: Microsoft.Azure.Devops.Tags.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10FDC8E3-D1DB-4668-B2F2-04DAA10A7143
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Devops.Tags.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data.SqlClient;

namespace Microsoft.Azure.Devops.Tags.Server
{
  [Serializable]
  public class DuplicateTagNameException : TeamFoundationServiceException
  {
    public DuplicateTagNameException(string message)
      : base(message)
    {
    }

    public DuplicateTagNameException(
      IVssRequestContext requestContext,
      SqlException sqlException,
      SqlError sqlError)
      : base(DuplicateTagNameException.GetErrorMessage(sqlError))
    {
    }

    private static string GetErrorMessage(SqlError sqlError)
    {
      string str = TeamFoundationServiceException.ExtractString(sqlError, "message");
      return string.IsNullOrEmpty(str) ? Resources.InvalidTagName_Duplicate((object) TeamFoundationServiceException.ExtractString(sqlError, "name")) : str;
    }
  }
}
