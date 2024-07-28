// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.ITeamFoundationTestManagementAfnStripService
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  [DefaultServiceImplementation(typeof (TeamFoundationTestManagementAfnStripService))]
  public interface ITeamFoundationTestManagementAfnStripService : IVssFrameworkService
  {
    AfnStrip CreateAfnStrip(
      TestManagementRequestContext context,
      Guid projectId,
      AfnStrip afnStrip);

    AfnStrip CreateAfnStrip(
      TestManagementRequestContext context,
      Guid projectId,
      int testCaseId,
      Stream stream);

    AfnStrip GetDefaultAfnStrip(
      TestManagementRequestContext context,
      Guid projectId,
      int testCaseId);

    List<AfnStrip> GetDefaultAfnStrips(
      TestManagementRequestContext context,
      Guid projectId,
      IList<int> testCaseId);

    void UpdateDefaultStrip(
      TestManagementRequestContext context,
      Guid projectId,
      IList<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.DefaultAfnStripBinding> bindings);

    void UpdateDefaultStrip(
      TestManagementRequestContext context,
      string projectName,
      IList<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.DefaultAfnStripBinding> bindings);
  }
}
