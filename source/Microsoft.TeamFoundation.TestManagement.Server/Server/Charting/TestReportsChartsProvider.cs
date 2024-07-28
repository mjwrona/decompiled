// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.Charting.TestReportsChartsProvider
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Reporting.DataServices;
using Microsoft.TeamFoundation.Reporting.DataServices.Services;

namespace Microsoft.TeamFoundation.TestManagement.Server.Charting
{
  internal class TestReportsChartsProvider : ChartProviderBase
  {
    public override IDataServiceCapabilityProvider GetCapabilityProvider(
      IVssRequestContext requestContext)
    {
      return FactoryHelper.InstantiateService<IDataServiceCapabilityProvider, TestExecutionCapabilityProvider>(requestContext);
    }

    public override IDataServicesSecurityProvider GetSecurityProvider(
      IVssRequestContext requestContext)
    {
      return FactoryHelper.InstantiateService<IDataServicesSecurityProvider, TestReportsSecurityProvider>(requestContext);
    }

    public override IDataTransformPipeline GetTransformProvider(IVssRequestContext requestContext) => (IDataTransformPipeline) new TestReportsDataTransformPipeline(requestContext);
  }
}
