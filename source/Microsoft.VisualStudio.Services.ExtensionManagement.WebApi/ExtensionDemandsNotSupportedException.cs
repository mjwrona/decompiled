// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionDemandsNotSupportedException
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A4FCC2C3-B106-43A6-A409-E4BF8CFC545C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.WebApi
{
  [Serializable]
  public class ExtensionDemandsNotSupportedException : VssServiceException
  {
    public ExtensionDemandsNotSupportedException(
      string publisherName,
      string extensionName,
      IEnumerable<string> demandsErrors)
      : base(demandsErrors == null ? ExtensionManagementResources.ExtensionDemandsNotSupportedWithoutReasonException() : ExtensionManagementResources.ExtensionDemandsNotSupportedException((object) string.Join("\n", demandsErrors.Select<string, string>((Func<string, string>) (e => ExtensionManagementResources.ExtensionDemandErrorFormat((object) e))))))
    {
    }
  }
}
