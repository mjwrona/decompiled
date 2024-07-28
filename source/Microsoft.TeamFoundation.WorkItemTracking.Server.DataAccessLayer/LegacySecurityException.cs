// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.LegacySecurityException
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  [Serializable]
  public class LegacySecurityException : LegacyServerException
  {
    public LegacySecurityException()
    {
    }

    public LegacySecurityException(string message)
      : base(message)
    {
    }

    public LegacySecurityException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    protected LegacySecurityException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }

    public LegacySecurityException(string message, int id)
      : base(message, id, true)
    {
    }

    public LegacySecurityException(string message, int id, Exception innerException)
      : base(message, id, true, innerException)
    {
    }

    protected override void SetEventId() => this.EventId = TeamFoundationEventId.SecurityException;
  }
}
