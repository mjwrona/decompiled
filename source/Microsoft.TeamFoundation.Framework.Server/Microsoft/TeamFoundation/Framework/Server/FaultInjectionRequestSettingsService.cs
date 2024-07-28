// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.FaultInjectionRequestSettingsService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class FaultInjectionRequestSettingsService : 
    IFaultInjectionRequestSettingsService,
    IVssFrameworkService
  {
    private const string c_requestContextItemKey = "RequestScopedFaults";
    private const string s_layer = "FaultInjectionRequestSettings";

    public void ServiceStart(IVssRequestContext systemRequestContext) => systemRequestContext.CheckHostedDeployment();

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public FaultDefinition[] GetFaults(
      IVssRequestContext requestContext,
      string faultPoint,
      string faultType)
    {
      try
      {
        if (this.AreRequestScopedFaultsEnabled(requestContext))
        {
          FaultDefinitionCollection definitionCollection;
          if (requestContext.TryGetItem<FaultDefinitionCollection>("RequestScopedFaults", out definitionCollection))
          {
            FaultDefinition[] faults = definitionCollection.GetFaults(faultPoint, faultType);
            if (faults != null)
            {
              if (faults.Length != 0)
                return faults;
            }
          }
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceAlways(1440011, TraceLevel.Warning, "VisualStudio.Services.FaultInjectionService", "FaultInjectionRequestSettings", "Error loading fault definition from request context.  Error message: " + ex.ToString());
      }
      return Array.Empty<FaultDefinition>();
    }

    public string GetSerializedFaultSettings(IVssRequestContext requestContext)
    {
      if (this.AreRequestScopedFaultsEnabled(requestContext))
      {
        try
        {
          FaultDefinitionCollection definitionCollection;
          if (requestContext.TryGetItem<FaultDefinitionCollection>("RequestScopedFaults", out definitionCollection))
            return definitionCollection.Serialize();
        }
        catch (Exception ex)
        {
          requestContext.TraceAlways(1440013, TraceLevel.Warning, "VisualStudio.Services.FaultInjectionService", "FaultInjectionRequestSettings", "Error serializing fault definitions from request.  Error message: " + ex.ToString());
        }
      }
      return (string) null;
    }

    public void SetFaultInjectionSettingsForRequest(
      IVssRequestContext requestContext,
      string injectionSettings)
    {
      if (!this.AreRequestScopedFaultsEnabled(requestContext))
        return;
      try
      {
        FaultDefinition[] faultDefinitionArray = JsonConvert.DeserializeObject<FaultDefinition[]>(injectionSettings);
        FaultDefinitionCollection definitionCollection = new FaultDefinitionCollection();
        definitionCollection.AddRange((IEnumerable<FaultDefinition>) faultDefinitionArray);
        requestContext.Items["RequestScopedFaults"] = (object) definitionCollection;
      }
      catch (Exception ex)
      {
        requestContext.TraceAlways(1440012, TraceLevel.Warning, "VisualStudio.Services.FaultInjectionService", "FaultInjectionRequestSettings", "Error deserializing fault definitions from request.  Error message: " + ex.ToString());
      }
    }

    public bool AreRequestScopedFaultsEnabled(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("VisualStudio.Services.RequestScopedFaultInjection.Enabled");
  }
}
