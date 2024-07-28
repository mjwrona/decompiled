// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.IHostReadOnlyModeService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [DefaultServiceImplementation(typeof (HostReadOnlyModeService))]
  public interface IHostReadOnlyModeService : IVssFrameworkService
  {
    void UpdateReadOnlyState(
      IVssRequestContext requestContext,
      Guid hostId,
      bool isReadOnly,
      bool waitForCompletion);

    bool IsReadOnly(IVssRequestContext requestContext, Guid hostId);
  }
}
