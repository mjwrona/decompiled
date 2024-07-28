// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.DefaultTfsServiceHelper
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class DefaultTfsServiceHelper : ITfsServiceHelper
  {
    public DefaultTfsServiceHelper(IVssRequestContext context)
    {
    }

    public bool TryGetTestResultRetentionSettings(
      TestManagementRequestContext context,
      Guid projectId,
      out ResultRetentionSettings retentionSettings)
    {
      IResultRetentionSettingsService service = context.RequestContext.GetService<IResultRetentionSettingsService>();
      retentionSettings = service.Get(context, new GuidAndString(string.Empty, projectId));
      return true;
    }
  }
}
