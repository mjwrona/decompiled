// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ResourceStreamNotFoundException
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Data.SqlClient;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [Serializable]
  public class ResourceStreamNotFoundException : TeamFoundationServiceException
  {
    public ResourceStreamNotFoundException() => this.EventId = TeamFoundationEventId.ResourceStreamNotFoundException;

    public ResourceStreamNotFoundException(string resourceName)
      : base(FrameworkResources.ResourceStreamNotFoundException((object) resourceName))
    {
      this.LogException = true;
      this.LogLevel = EventLogEntryType.Warning;
      this.EventId = TeamFoundationEventId.ResourceStreamNotFoundException;
    }

    public ResourceStreamNotFoundException(
      IVssRequestContext requestContext,
      SqlException ex,
      SqlError sqlError)
      : this(TeamFoundationServiceException.ExtractString(sqlError, "fileName"))
    {
      this.EventId = TeamFoundationEventId.ResourceStreamNotFoundException;
    }
  }
}
