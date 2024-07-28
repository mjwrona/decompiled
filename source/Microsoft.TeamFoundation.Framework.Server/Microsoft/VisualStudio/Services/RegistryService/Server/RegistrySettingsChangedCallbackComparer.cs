// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.RegistryService.Server.RegistrySettingsChangedCallbackComparer
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.RegistryService.Server
{
  public class RegistrySettingsChangedCallbackComparer : 
    IEqualityComparer<RegistrySettingsChangedCallback>
  {
    public static readonly IEqualityComparer<RegistrySettingsChangedCallback> Instance = (IEqualityComparer<RegistrySettingsChangedCallback>) new RegistrySettingsChangedCallbackComparer();

    public bool Equals(RegistrySettingsChangedCallback x, RegistrySettingsChangedCallback y) => x.Equals((object) y);

    public int GetHashCode(RegistrySettingsChangedCallback obj) => obj.Target != null ? obj.Target.GetHashCode() : obj.GetHashCode();
  }
}
