// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.InvalidResolutionException
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  [Serializable]
  public class InvalidResolutionException : ServerException
  {
    private string m_message;

    public InvalidResolutionException(
      IVssRequestContext requestContext,
      SqlException ex,
      SqlError sqlError)
      : base(string.Empty)
    {
      foreach (int num in TeamFoundationServiceException.ExtractInts(sqlError, "error"))
      {
        if (num == 500205)
        {
          string dataspaceServerItem = ServerException.ExtractDataspaceServerItem(requestContext, sqlError, "item");
          this.m_message = Resources.Format(TeamFoundationServiceException.ExtractInt(sqlError, "itemtype") == 2 ? "FileFolderOnlyValidResolutionIsAcceptTheirs" : "FolderFileOnlyValidResolutionIsAcceptTheirs", (object) dataspaceServerItem);
        }
        if (!string.IsNullOrEmpty(this.m_message))
          break;
      }
    }

    public override string Message => this.m_message;
  }
}
