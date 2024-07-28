// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.IBuildConfiguration
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public interface IBuildConfiguration
  {
    BuildConfiguration Query(IVssRequestContext context, int buildConfigurationId);

    BuildConfiguration Query(IVssRequestContext context, Guid projectId, int buildConfigurationId);

    IList<string> Query(
      IVssRequestContext context,
      Guid projectId,
      bool? queryDeletedBuild = false,
      int pageSize = 1000);
  }
}
