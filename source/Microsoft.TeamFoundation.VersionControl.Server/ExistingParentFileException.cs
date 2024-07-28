// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.ExistingParentFileException
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  [Serializable]
  public class ExistingParentFileException : ServerItemException
  {
    public ExistingParentFileException(string serverItem)
      : base(nameof (ExistingParentFileException), serverItem)
    {
    }

    public ExistingParentFileException(
      IVssRequestContext requestContext,
      SqlException ex,
      SqlError sqlError)
      : this(ServerException.ExtractDataspaceServerItem(requestContext, sqlError, "serverItem"))
    {
    }
  }
}
