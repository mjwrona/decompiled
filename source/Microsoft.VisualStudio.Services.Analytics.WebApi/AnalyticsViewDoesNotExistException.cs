// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.WebApi.AnalyticsViewDoesNotExistException
// Assembly: Microsoft.VisualStudio.Services.Analytics.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F858B048-FFE8-4E5F-8EBC-9B25DAC23DF8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Analytics.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Analytics.WebApi
{
  [Serializable]
  public class AnalyticsViewDoesNotExistException : VssServiceException
  {
    public AnalyticsViewDoesNotExistException(Guid viewId)
      : base(AnalyticsWebApiResources.AnalyticsViewDoesNotExistException((object) viewId))
    {
    }

    public AnalyticsViewDoesNotExistException(Guid viewId, Exception innerException)
      : base(AnalyticsWebApiResources.AnalyticsViewDoesNotExistException((object) viewId), innerException)
    {
    }

    protected AnalyticsViewDoesNotExistException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
