// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.RegistryService.Server.RegistryCallbackEntry
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.RegistryService.Server
{
  public class RegistryCallbackEntry
  {
    public readonly RegistrySettingsChangedCallback Callback;
    public readonly Guid ServiceHostId;
    public readonly HashSet<RegistryQuery> Filters;
    public bool IsFallThru;

    public RegistryCallbackEntry(
      RegistrySettingsChangedCallback callback,
      Guid serviceHostId,
      bool isFallThru)
    {
      this.Callback = callback;
      this.ServiceHostId = serviceHostId;
      this.Filters = new HashSet<RegistryQuery>(RegistryQueryComparer.Instance);
      this.IsFallThru = isFallThru;
    }
  }
}
