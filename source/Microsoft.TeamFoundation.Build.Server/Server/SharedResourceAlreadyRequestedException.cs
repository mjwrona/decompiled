// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.SharedResourceAlreadyRequestedException
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.Build.Server
{
  [Serializable]
  public sealed class SharedResourceAlreadyRequestedException : AdministrationException
  {
    public SharedResourceAlreadyRequestedException(
      IVssRequestContext requestContext,
      SqlException ex,
      SqlError err)
      : base(SharedResourceAlreadyRequestedException.FormatMessage(requestContext, ex, err))
    {
    }

    private static string FormatMessage(
      IVssRequestContext requestContext,
      SqlException ex,
      SqlError err)
    {
      return AdministrationResources.SharedResourceAlreadyRequested((object) TeamFoundationServiceException.ExtractString(err, "resourceName"), (object) TeamFoundationServiceException.ExtractString(err, "requestedBy"));
    }
  }
}
