// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Server.ISupportedExtensionService
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 57F50803-C5C4-41A9-A26F-AD293D563111
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ExtensionManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Server
{
  [DefaultServiceImplementation(typeof (SupportedExtensionService))]
  internal interface ISupportedExtensionService : IVssFrameworkService
  {
    bool TryGetSupportedVersions(
      IVssRequestContext requestContext,
      string key,
      out IDictionary<string, SupportedExtension> versions);

    IDictionary<string, SupportedExtension> FetchSupportedVersions(
      IVssRequestContext requestContext,
      string versionCheckUri,
      out bool isCheckSuccessful);
  }
}
