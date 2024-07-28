// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.DataImportException
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.WebApi
{
  [Serializable]
  public abstract class DataImportException : VssServiceException
  {
    public DataImportException(string message)
      : base(message)
    {
      this.MarkAsFatalServicingOrchestrationException();
    }

    public DataImportException(string message, Exception innerException)
      : base(message, innerException)
    {
      this.MarkAsFatalServicingOrchestrationException();
    }

    protected DataImportException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
      this.MarkAsFatalServicingOrchestrationException();
    }
  }
}
