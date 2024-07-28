// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TCMServiceMigration.TCMServiceMigration
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server.TCMServiceMigration
{
  public class TCMServiceMigration
  {
    internal static IEnumerable<KeyValuePair<string, int>> GetMigrationThreshold(
      TestManagementRequestContext context)
    {
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
        return managementDatabase.GetTCMServiceMigrationThreshold();
    }

    internal static IEnumerable<KeyValuePair<string, int>> GetMigrationThresholdFromRegistry(
      TestManagementRequestContext context)
    {
      IVssRegistryService service = context.RequestContext.GetService<IVssRegistryService>();
      return (IEnumerable<KeyValuePair<string, int>>) new List<KeyValuePair<string, int>>()
      {
        new KeyValuePair<string, int>("TestRunThreshold", service.GetValue<int>(context.RequestContext, (RegistryQuery) "/Service/TestManagement/Settings/TcmServiceTestRunIdThreshold", 2)),
        new KeyValuePair<string, int>("TestAttachmentThreshold", service.GetValue<int>(context.RequestContext, (RegistryQuery) "/Service/TestManagement/Settings/TcmServiceTestAttachmentIdThreshold", 2))
      };
    }
  }
}
