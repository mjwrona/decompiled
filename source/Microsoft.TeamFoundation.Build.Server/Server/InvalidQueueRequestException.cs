// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.InvalidQueueRequestException
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.Build.Server
{
  [Serializable]
  public sealed class InvalidQueueRequestException : AdministrationException
  {
    public InvalidQueueRequestException(
      IVssRequestContext requestContext,
      SqlException ex,
      SqlError err)
      : base(AdministrationResources.CannotSatisfyQueuePosition((object) DBHelper.DBPathToServerPath(TeamFoundationServiceException.ExtractString(err, "controllerName")), (object) TeamFoundationServiceException.ExtractInt(err, "maxQueuePosition"), (object) TeamFoundationServiceException.ExtractInt(err, "currentQueuePosition")))
    {
    }
  }
}
