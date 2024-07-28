// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.WebApi.StageStreamThrottledException
// Assembly: Microsoft.VisualStudio.Services.Analytics.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F858B048-FFE8-4E5F-8EBC-9B25DAC23DF8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Analytics.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Analytics.WebApi
{
  [Serializable]
  public class StageStreamThrottledException : StageExceptionBase
  {
    public StageStreamThrottledException(
      string table,
      int providerShard,
      int stream,
      string reason)
      : base(AnalyticsWebApiResources.StageStreamThrottledException((object) table, (object) providerShard, (object) stream, (object) reason))
    {
    }

    public StageStreamThrottledException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    protected StageStreamThrottledException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
