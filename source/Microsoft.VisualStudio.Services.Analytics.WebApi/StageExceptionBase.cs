// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.WebApi.StageExceptionBase
// Assembly: Microsoft.VisualStudio.Services.Analytics.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F858B048-FFE8-4E5F-8EBC-9B25DAC23DF8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Analytics.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Analytics.WebApi
{
  [Serializable]
  public abstract class StageExceptionBase : VssServiceException
  {
    public StageExceptionBase(string message)
      : base(message)
    {
    }

    public StageExceptionBase(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    protected StageExceptionBase(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
