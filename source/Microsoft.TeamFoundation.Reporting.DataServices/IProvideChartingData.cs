// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Reporting.DataServices.IProvideChartingData
// Assembly: Microsoft.TeamFoundation.Reporting.DataServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0871DF71-209E-4628-905A-D95195A70FEC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Reporting.DataServices.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Reporting.DataServices.Services;

namespace Microsoft.TeamFoundation.Reporting.DataServices
{
  public interface IProvideChartingData
  {
    bool IsEnabled(IVssRequestContext requestContext);

    IDataServicesSecurityProvider GetSecurityProvider(IVssRequestContext requestContext);

    IDataServiceCapabilityProvider GetCapabilityProvider(IVssRequestContext requestContext);

    IDataTransformPipeline GetTransformProvider(IVssRequestContext requestContext);
  }
}
