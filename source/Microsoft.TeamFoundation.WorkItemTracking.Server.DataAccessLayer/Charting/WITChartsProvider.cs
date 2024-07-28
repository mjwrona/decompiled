// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Charting.WITChartsProvider
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Reporting.DataServices;
using Microsoft.TeamFoundation.Reporting.DataServices.Services;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Charting
{
  internal class WITChartsProvider : ChartProviderBase
  {
    public override IDataServiceCapabilityProvider GetCapabilityProvider(
      IVssRequestContext requestContext)
    {
      return FactoryHelper.InstantiateService<IDataServiceCapabilityProvider, WitDataServiceCapabilityProvider>(requestContext);
    }

    public override IDataServicesSecurityProvider GetSecurityProvider(
      IVssRequestContext requestContext)
    {
      return FactoryHelper.InstantiateService<IDataServicesSecurityProvider, WitSecurityProvider>(requestContext);
    }

    public override IDataTransformPipeline GetTransformProvider(IVssRequestContext requestContext)
    {
      WitUnifiedTransformPipeline<DatedWorkItemFieldData> transformProvider = new WitUnifiedTransformPipeline<DatedWorkItemFieldData>();
      transformProvider.RequestContext = requestContext;
      WitProviderInterpreter providerInterpreter = new WitProviderInterpreter();
      transformProvider.DataInterpreter = (IInterpretTimedData<DatedWorkItemFieldData>) providerInterpreter;
      transformProvider.DataProvider = (IProvideFilteredData<DatedWorkItemFieldData>) providerInterpreter;
      transformProvider.QueryInterpreter = (IInterpretQueryText) providerInterpreter;
      return (IDataTransformPipeline) transformProvider;
    }
  }
}
