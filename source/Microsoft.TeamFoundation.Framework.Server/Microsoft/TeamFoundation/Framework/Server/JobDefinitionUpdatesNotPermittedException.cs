// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.JobDefinitionUpdatesNotPermittedException
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [Serializable]
  public class JobDefinitionUpdatesNotPermittedException : TeamFoundationJobServiceException
  {
    public JobDefinitionUpdatesNotPermittedException(Guid jobId)
      : base(FrameworkResources.JobDefinitionUpdatesNotPermitted((object) jobId.ToString()))
    {
    }

    public JobDefinitionUpdatesNotPermittedException(string message)
      : base(message)
    {
    }

    public JobDefinitionUpdatesNotPermittedException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    protected JobDefinitionUpdatesNotPermittedException(
      SerializationInfo info,
      StreamingContext context)
      : base(info, context)
    {
    }

    public JobDefinitionUpdatesNotPermittedException(
      IVssRequestContext requestContext,
      SqlException ex,
      SqlError sqlError)
      : this(Guid.Parse(TeamFoundationServiceException.ExtractString(sqlError, "jobid")))
    {
    }
  }
}
