// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TCMServiceMigration.TCMServiceMigrationHelper
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server.TCMServiceMigration
{
  internal class TCMServiceMigrationHelper : TfsRestApiHelper
  {
    internal TCMServiceMigrationHelper(TestManagementRequestContext requestContext)
      : base(requestContext)
    {
    }

    public IEnumerable<KeyValuePair<string, int>> GetMigrationThreshold()
    {
      this.AuthenticateS2SCall();
      return Microsoft.TeamFoundation.TestManagement.Server.TCMServiceMigration.TCMServiceMigration.GetMigrationThresholdFromRegistry((TestManagementRequestContext) this.TfsTestManagementRequestContext);
    }
  }
}
