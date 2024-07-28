// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.WebApi.AnalyticsViewSchemaGenerationFailedException
// Assembly: Microsoft.VisualStudio.Services.Analytics.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F858B048-FFE8-4E5F-8EBC-9B25DAC23DF8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Analytics.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.VisualStudio.Services.Analytics.WebApi
{
  [Serializable]
  public class AnalyticsViewSchemaGenerationFailedException : VssServiceException
  {
    public AnalyticsViewSchemaGenerationFailedException(string message)
      : base(message)
    {
    }

    public AnalyticsViewSchemaGenerationFailedException(string message, Exception innerException)
      : base(message, innerException)
    {
    }
  }
}
