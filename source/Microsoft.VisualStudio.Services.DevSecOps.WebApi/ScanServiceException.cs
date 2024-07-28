// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DevSecOps.WebApi.ScanServiceException
// Assembly: Microsoft.VisualStudio.Services.DevSecOps.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 78BC9F0A-6262-4C96-81AF-14E523464B20
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DevSecOps.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.VisualStudio.Services.DevSecOps.WebApi
{
  [Serializable]
  public class ScanServiceException : VssServiceException
  {
    public ScanServiceException(string message)
      : base(message)
    {
    }

    public ScanServiceException(string message, Exception innerException)
      : base(message, innerException)
    {
    }
  }
}
