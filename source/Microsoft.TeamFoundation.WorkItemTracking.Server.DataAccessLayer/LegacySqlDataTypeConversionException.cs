// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.LegacySqlDataTypeConversionException
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  [Serializable]
  public class LegacySqlDataTypeConversionException : LegacyValidationException
  {
    public LegacySqlDataTypeConversionException()
    {
    }

    protected LegacySqlDataTypeConversionException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }

    public LegacySqlDataTypeConversionException(string message)
      : base(message, 602207)
    {
    }

    public LegacySqlDataTypeConversionException(string message, Exception innerException)
      : base(message, 602207, innerException)
    {
    }
  }
}
