// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TfsTestManagementService
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public abstract class TfsTestManagementService : TeamFoundationTestManagementService
  {
    protected TfsTestManagementService()
    {
    }

    protected TfsTestManagementService(TestManagementRequestContext requestContext)
      : base(requestContext)
    {
    }

    protected override TestManagementRequestContext GetTestManagementRequestContext(
      IVssRequestContext context)
    {
      return this.GetTestManagementRequestContext() ?? (TestManagementRequestContext) new TfsTestManagementRequestContext(context);
    }

    protected virtual TfsTestManagementRequestContext GetTfsTestManagementRequestContext(
      IVssRequestContext context)
    {
      return this.GetTestManagementRequestContext(context) as TfsTestManagementRequestContext;
    }
  }
}
