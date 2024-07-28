// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.FeatureManagement.ContributedFeatureInvalidScopeException
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A4FCC2C3-B106-43A6-A409-E4BF8CFC545C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.FeatureManagement
{
  [Serializable]
  public class ContributedFeatureInvalidScopeException : VssServiceException
  {
    public ContributedFeatureInvalidScopeException(string message)
      : base(message)
    {
    }
  }
}
