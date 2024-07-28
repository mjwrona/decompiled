// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.ICheckEventService
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Build2.Server
{
  [DefaultServiceImplementation(typeof (CheckEventService))]
  public interface ICheckEventService : IVssFrameworkService
  {
    BuildCheckEvent AddCheckEvent(IVssRequestContext requestContext, BuildCheckEvent checkEvent);

    List<BuildCheckEvent> AddCheckEvents(
      IVssRequestContext requestContext,
      List<BuildCheckEvent> checkEvents);

    CheckEventResults GetCheckEvents(IVssRequestContext requestContext, int? maxCount);

    List<BuildCheckEvent> UpateCheckEventStatus(
      IVssRequestContext requestContext,
      List<BuildCheckEvent> checkEvents);

    void CleanupCheckEvents(IVssRequestContext requestContext);
  }
}
