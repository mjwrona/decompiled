// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.MaxRowsEvaluatedException
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  [Serializable]
  public class MaxRowsEvaluatedException : ServerException
  {
    public MaxRowsEvaluatedException(
      IVssRequestContext requestContext,
      SqlException ex,
      SqlError sqlError)
      : base(Resources.Format(nameof (MaxRowsEvaluatedException), (object) MaxRowsEvaluatedException.MaxRowsEvaluated(requestContext)))
    {
    }

    private static int MaxRowsEvaluated(IVssRequestContext requestContext)
    {
      if (requestContext != null && requestContext.ServiceHost != null)
      {
        TeamFoundationVersionControlService service = requestContext.GetService<TeamFoundationVersionControlService>();
        if (service != null)
        {
          VersionControlRequestContext vcRequestContext = new VersionControlRequestContext(requestContext, service);
          return service.GetMaxRowsEvaluated(vcRequestContext);
        }
      }
      return -1;
    }
  }
}
