// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server.Interfaces.IScanService
// Assembly: Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AEA81E2B-AB47-44C0-8043-66C0E1018997
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.VisualStudio.Services.DevSecOps.WebApi;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server.Interfaces
{
  public interface IScanService : IVssFrameworkService
  {
    Task<IEnumerable<ContainerResult>> ScanAsync(
      IVssRequestContext requestContext,
      Container batchContainer,
      bool returnViolations,
      bool forwardToAdvancedSecurity,
      ClientTraceData summaryCtData);

    ScanConfiguration GetScanConfiguration();
  }
}
