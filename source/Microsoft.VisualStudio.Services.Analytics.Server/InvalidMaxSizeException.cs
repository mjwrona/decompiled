// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.InvalidMaxSizeException
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.VisualStudio.Services.Analytics
{
  public class InvalidMaxSizeException : VssServiceException
  {
    public InvalidMaxSizeException(int maxMaxSize)
      : this(maxMaxSize, (Exception) null)
    {
    }

    public InvalidMaxSizeException(int maxMaxSize, Exception innerException)
      : base(AnalyticsResources.MAX_SIZE_HEADER_INVALID((object) maxMaxSize), innerException)
    {
    }
  }
}
