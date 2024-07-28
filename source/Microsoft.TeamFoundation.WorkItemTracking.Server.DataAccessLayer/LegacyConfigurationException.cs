// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.LegacyConfigurationException
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  [Serializable]
  public class LegacyConfigurationException : LegacyServerException
  {
    public LegacyConfigurationException()
    {
    }

    public LegacyConfigurationException(string message)
      : base(message)
    {
    }

    public LegacyConfigurationException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    protected LegacyConfigurationException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }

    public LegacyConfigurationException(string message, int id)
      : base(message, id)
    {
    }

    public LegacyConfigurationException(string message, int id, Exception innerException)
      : base(message, id, innerException)
    {
    }

    protected override void SetEventId() => this.EventId = TeamFoundationEventId.ConfigurationException;
  }
}
