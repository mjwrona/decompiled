// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.KustoQueryFailedException
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.VisualStudio.Services.Cloud
{
  [Serializable]
  public class KustoQueryFailedException : VssServiceException
  {
    public KustoQueryFailedException()
    {
    }

    public KustoQueryFailedException(string message)
      : base(message)
    {
    }

    public KustoQueryFailedException(string message, Exception innerException)
      : base(message, innerException)
    {
    }
  }
}
