// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.OnPrem.IAnalyticsStateService
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Analytics.WebApi.Contracts;
using System;

namespace Microsoft.VisualStudio.Services.Analytics.OnPrem
{
  [DefaultServiceImplementation(typeof (AnalyticsStateService))]
  public interface IAnalyticsStateService : IVssFrameworkService
  {
    AnalyticsState GetState(IVssRequestContext requestContext, bool bypassCache = false);

    DateTimeOffset GetChangedDate(IVssRequestContext requestContext);

    void Enable(IVssRequestContext requestContext);

    void Pause(IVssRequestContext requestContext);

    void Delete(IVssRequestContext requestContext);

    void Disable(IVssRequestContext requestContext);

    void MigrateFromExtension(IVssRequestContext requestContext, bool isInstalled, bool isDisabled);
  }
}
