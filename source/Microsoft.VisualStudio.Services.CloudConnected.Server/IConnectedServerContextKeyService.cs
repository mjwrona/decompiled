// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CloudConnected.IConnectedServerContextKeyService
// Assembly: Microsoft.VisualStudio.Services.CloudConnected.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6AAFC756-39E6-4247-9102-7DC33B035E4F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.CloudConnected.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.CloudConnected
{
  [DefaultServiceImplementation(typeof (ConnectedServerContextKeyService))]
  public interface IConnectedServerContextKeyService : IVssFrameworkService
  {
    string GetToken(IVssRequestContext requestContext, Dictionary<string, string> properties);

    bool IsValidAuthToken(IVssRequestContext requestContext, string token);

    void SaveAuthToken(IVssRequestContext requestContext, string token);
  }
}
