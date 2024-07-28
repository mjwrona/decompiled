// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.IRemotePropertyProvider
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3FE9DD3E-5758-4D1B-8056-ECAF8A4B7A77
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server
{
  [InheritedExport]
  public interface IRemotePropertyProvider
  {
    string Name { get; }

    Dictionary<string, object> GetProviderProperties(IVssRequestContext requestContext);
  }
}
