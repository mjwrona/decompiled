// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.ILocalExtensionStoragePlugin
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3FE9DD3E-5758-4D1B-8056-ECAF8A4B7A77
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System.ComponentModel.Composition;
using System.IO;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server
{
  [InheritedExport]
  public interface ILocalExtensionStoragePlugin
  {
    void StoreAsset(
      IVssRequestContext requestContext,
      ITFLogger logger,
      string fullPath,
      string publisherName,
      string extensionName,
      string version,
      string language,
      Stream asset,
      bool isCdnEnabled,
      string cdnContainerName,
      string cdnStorageConnectionString);
  }
}
