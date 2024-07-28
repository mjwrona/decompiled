// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.IncompleteUploadException
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Data.SqlClient;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [Serializable]
  public class IncompleteUploadException : TeamFoundationServiceException
  {
    public IncompleteUploadException() => this.EventId = TeamFoundationEventId.IncompleteUploadException;

    public IncompleteUploadException(string resourceName)
      : base(FrameworkResources.IncompleteUploadException((object) resourceName))
    {
      this.EventId = TeamFoundationEventId.IncompleteUploadException;
      this.LogException = true;
      this.LogLevel = EventLogEntryType.Warning;
    }

    public IncompleteUploadException(
      IVssRequestContext requestContext,
      SqlException ex,
      SqlError sqlError)
      : this(TeamFoundationServiceException.ExtractString(sqlError, "serverItem"))
    {
      this.EventId = TeamFoundationEventId.IncompleteUploadException;
    }
  }
}
