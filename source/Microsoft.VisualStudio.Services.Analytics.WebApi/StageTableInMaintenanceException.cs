// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.WebApi.StageTableInMaintenanceException
// Assembly: Microsoft.VisualStudio.Services.Analytics.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F858B048-FFE8-4E5F-8EBC-9B25DAC23DF8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Analytics.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Analytics.WebApi
{
  [Serializable]
  public class StageTableInMaintenanceException : StageExceptionBase
  {
    public StageTableInMaintenanceException(string table, string reason)
      : base(AnalyticsWebApiResources.StageTableInMaintenanceException((object) table, (object) reason))
    {
    }

    public StageTableInMaintenanceException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    protected StageTableInMaintenanceException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
