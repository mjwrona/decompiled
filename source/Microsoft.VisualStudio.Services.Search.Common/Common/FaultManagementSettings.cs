// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.FaultManagementSettings
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.VisualStudio.Services.Search.Common
{
  public class FaultManagementSettings
  {
    public FaultManagementSettings()
    {
    }

    public FaultManagementSettings(IVssRequestContext requestContext)
    {
      this.MaxSuccessCount = requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/FaultHandlerSuccessCount");
      this.MaxFailureCount = requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/FaultHandlerFailureCount");
      this.MidFailureExpirationInSec = requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/FaultHandlerMidFailureExpirationInSec");
      this.CritFailureExpirationInSec = requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/FaultHandlerCritFailureExpirationInSec");
      this.MidFailureRequestLimit = requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/FaultHandlerMidFailureRequestLimit");
      this.CritFailureRequestLimit = requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/FaultHandlerCritFailureRequestLimit");
      this.MidFailureRequestDelayInSec = requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/FaultMediumSeverityJobDelayInSec");
      this.CritFailureRequestDelayInSec = requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/FaultCriticalSeverityJobDelayInSec");
    }

    public int MaxSuccessCount { get; set; }

    public int MaxFailureCount { get; set; }

    public int MidFailureExpirationInSec { get; set; }

    public int CritFailureExpirationInSec { get; set; }

    public int MidFailureRequestLimit { get; set; }

    public int CritFailureRequestLimit { get; set; }

    public int MidFailureRequestDelayInSec { get; set; }

    public int CritFailureRequestDelayInSec { get; set; }
  }
}
