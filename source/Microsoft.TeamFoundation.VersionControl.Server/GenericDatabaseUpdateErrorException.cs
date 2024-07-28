// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.GenericDatabaseUpdateErrorException
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data.SqlClient;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  [Serializable]
  public class GenericDatabaseUpdateErrorException : ServerException
  {
    public GenericDatabaseUpdateErrorException()
      : base(Resources.Get("GenericDatabaseUpdateError"))
    {
    }

    public GenericDatabaseUpdateErrorException(
      IVssRequestContext requestContext,
      SqlException ex,
      SqlError sqlError)
      : this()
    {
      if (ex == null)
        return;
      string message = string.Format("{0} LineNumber {1}", (object) ex.Message, (object) ex.LineNumber);
      requestContext.Trace(700330, TraceLevel.Error, TraceArea.General, TraceLayer.Component, message);
    }
  }
}
