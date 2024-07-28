// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestEnvironmentsHelper
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using System;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class TestEnvironmentsHelper : TfsRestApiHelper, ITestEnvironmentsHelper
  {
    internal TestEnvironmentsHelper(TestManagementRequestContext requestContext)
      : base(requestContext)
    {
    }

    public virtual Microsoft.TeamFoundation.TestManagement.WebApi.TestEnvironment GetTestEnvironment(
      string projectName,
      Guid environmentId)
    {
      this.RequestContext.TraceInfo("RestLayer", "EnvironmentsHelper.GetTestEnvironment projectName = {0}, environment = {1}", (object) projectName, (object) environmentId);
      return (Microsoft.TeamFoundation.TestManagement.WebApi.TestEnvironment) null;
    }
  }
}
