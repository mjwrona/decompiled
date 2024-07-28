// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.MaxDacpacSizeExceededException
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.WebApi
{
  [Serializable]
  public class MaxDacpacSizeExceededException : DataImportException
  {
    public MaxDacpacSizeExceededException(string dacpacSizeInGB, string thresholdSizeInGB)
      : base(DataImportResources.MaxDacpacSizeExceededException((object) dacpacSizeInGB, (object) thresholdSizeInGB))
    {
      this.MarkAsUserErrorServicingOrchestrationException(this.Message);
    }

    public MaxDacpacSizeExceededException(string message)
      : base(message)
    {
      this.MarkAsUserErrorServicingOrchestrationException(message);
    }

    public MaxDacpacSizeExceededException(string message, Exception innerException)
      : base(message, innerException)
    {
      this.MarkAsUserErrorServicingOrchestrationException(message);
    }

    protected MaxDacpacSizeExceededException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
      this.MarkAsUserErrorServicingOrchestrationException();
    }
  }
}
