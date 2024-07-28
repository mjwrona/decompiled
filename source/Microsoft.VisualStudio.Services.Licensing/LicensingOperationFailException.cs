// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.LicensingOperationFailException
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.VisualStudio.Services.Licensing
{
  [Serializable]
  public class LicensingOperationFailException : TeamFoundationServiceException
  {
    public LicensingOperationFailException()
      : base(LicensingResources.LicensingOperationFailed())
    {
    }

    public LicensingOperationFailException(string message)
      : base(message)
    {
    }

    public LicensingOperationFailException(string message, Exception innerException)
      : base(message, innerException)
    {
    }
  }
}
