// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.InvalidBuildGroupItemUpdateException
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data.SqlClient;
using System.Web.Services.Protocols;

namespace Microsoft.TeamFoundation.Build.Server
{
  [Serializable]
  public class InvalidBuildGroupItemUpdateException : BuildServerException
  {
    internal InvalidBuildGroupItemUpdateException(string groupItemUri)
      : base(ResourceStrings.BuildGroupItemCannotMoveAcrossTeamProjects((object) groupItemUri))
    {
      this.SetExceptionProperties();
    }

    public InvalidBuildGroupItemUpdateException(
      IVssRequestContext context,
      SqlException ex,
      SqlError err)
      : this(DBHelper.CreateArtifactUri("Definition", TeamFoundationServiceException.ExtractInt(err, "definitionId")))
    {
    }

    protected void SetExceptionProperties() => this.FaultCode = Soap12FaultCodes.SenderFaultCode;
  }
}
