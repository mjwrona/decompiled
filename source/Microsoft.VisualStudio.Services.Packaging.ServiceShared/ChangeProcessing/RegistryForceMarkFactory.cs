// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.ChangeProcessing.RegistryForceMarkFactory
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.ChangeProcessing
{
  public class RegistryForceMarkFactory : IForceMarkFactory
  {
    private const string ForceKey = "/Configuration/{0}/ChangeProcessing/ContinueOnAnyError";

    public bool ComputeForceMode(IVssRequestContext requestContext, Guid feedId)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      string str = string.Format("/Configuration/{0}/ChangeProcessing/ContinueOnAnyError", (object) feedId);
      if (!service.GetValue<bool>(requestContext, (RegistryQuery) str, true, false))
        return false;
      service.SetValue<bool>(requestContext, str, false);
      return true;
    }
  }
}
