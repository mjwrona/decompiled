// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.DefaultReplicator
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class DefaultReplicator : ITestManagementReplicator
  {
    public virtual ICssCache GetAreaPathsCache(TestManagementRequestContext context) => (ICssCache) new DefaultCssCache();

    public virtual ICssCache GetIterationsCache(TestManagementRequestContext context) => (ICssCache) new DefaultCssCache();

    public virtual void UpdateCss(TestManagementRequestContext context)
    {
    }

    public virtual void ForceUpdateCss(
      TestManagementRequestContext context,
      string lockResourcePrefix,
      int? lockTimeout)
    {
    }

    public virtual void RefreshCache(IVssRequestContext context, Guid eventClass, string eventData)
    {
    }
  }
}
