// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.FaultInjectionRegistrySettingsService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class FaultInjectionRegistrySettingsService : 
    IFaultInjectionRegistrySettingsService,
    IVssFrameworkService
  {
    private FaultDefinitionCollection m_faults;
    private const string s_FaultInjectionServiceRegistry = "/Service/FaultInjectionService";
    private const string s_layer = "FaultInjectionRegistrySettings";

    public void ServiceStart(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(1440008, "VisualStudio.Services.FaultInjectionService", "FaultInjectionRegistrySettings", nameof (ServiceStart));
      try
      {
        requestContext.CheckHostedDeployment();
        requestContext.CheckDeploymentRequestContext();
        requestContext.GetService<IVssRegistryService>().RegisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged), true, "/Service/FaultInjectionService/...");
        this.LoadSettingsFromRegistry(requestContext);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1440009, "VisualStudio.Services.FaultInjectionService", "FaultInjectionRegistrySettings", ex);
      }
      finally
      {
        requestContext.TraceLeave(1440010, "VisualStudio.Services.FaultInjectionService", "FaultInjectionRegistrySettings", nameof (ServiceStart));
      }
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public FaultDefinition[] GetFaults(
      IVssRequestContext requestContext,
      string faultPoint,
      string faultType)
    {
      return this.m_faults.GetFaults(faultPoint, faultType);
    }

    public string AddFaultDefinition(
      IVssRequestContext requestContext,
      string faultPoint,
      string faultType,
      FaultFilter filter,
      Dictionary<string, object> faultSettings)
    {
      requestContext.CheckDeploymentRequestContext();
      ArgumentUtility.CheckStringForNullOrWhiteSpace(faultPoint, nameof (faultPoint));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(faultType, nameof (faultType));
      ArgumentUtility.CheckForNull<FaultFilter>(filter, nameof (filter));
      ArgumentUtility.CheckForNull<Dictionary<string, object>>(faultSettings, nameof (faultSettings));
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      Guid guid = Guid.NewGuid();
      JObject faultSettings1 = JObject.FromObject((object) faultSettings);
      string str1 = new FaultDefinition(faultPoint, faultType, filter, faultSettings1).Serialize();
      IVssRequestContext requestContext1 = requestContext;
      string path = "/Service/FaultInjectionService/" + guid.ToString();
      string str2 = str1;
      service.SetValue<string>(requestContext1, path, str2);
      return guid.ToString();
    }

    public string AddFaultDefinition<T>(
      IVssRequestContext requestContext,
      string faultPoint,
      string faultType,
      FaultFilter filter,
      T faultSettings)
      where T : class, IFaultSettings
    {
      requestContext.CheckDeploymentRequestContext();
      ArgumentUtility.CheckStringForNullOrWhiteSpace(faultPoint, nameof (faultPoint));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(faultType, nameof (faultType));
      ArgumentUtility.CheckForNull<FaultFilter>(filter, nameof (filter));
      ArgumentUtility.CheckForNull<T>(faultSettings, nameof (faultSettings));
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      JObject faultSettings1 = JObject.FromObject((object) faultSettings);
      Guid guid = Guid.NewGuid();
      string str1 = new FaultDefinition(faultPoint, faultType, filter, faultSettings1).Serialize();
      IVssRequestContext requestContext1 = requestContext;
      string path = "/Service/FaultInjectionService/" + guid.ToString();
      string str2 = str1;
      service.SetValue<string>(requestContext1, path, str2);
      return guid.ToString();
    }

    public void ClearAllFaultDefinitions(IVssRequestContext requestContext)
    {
      requestContext.CheckDeploymentRequestContext();
      requestContext.GetService<IVssRegistryService>().DeleteEntries(requestContext, "/Service/FaultInjectionService/**");
    }

    public void DeleteSingleFaultDefinition(IVssRequestContext requestContext, string faultId)
    {
      requestContext.CheckDeploymentRequestContext();
      ArgumentUtility.CheckStringForNullOrWhiteSpace(faultId, nameof (faultId));
      requestContext.GetService<IVssRegistryService>().DeleteEntries(requestContext, "/Service/FaultInjectionService/" + faultId);
    }

    public string[] GetAllRegisteredFaultDefinitionIds(IVssRequestContext requestContext)
    {
      requestContext.CheckDeploymentRequestContext();
      return requestContext.GetService<IVssRegistryService>().ReadEntries(requestContext, (RegistryQuery) "/Service/FaultInjectionService/**").Select<RegistryEntry, string>((Func<RegistryEntry, string>) (e => e.Name)).ToArray<string>();
    }

    private void OnRegistryChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      try
      {
        requestContext.TraceEnter(1440005, "VisualStudio.Services.FaultInjectionService", "FaultInjectionRegistrySettings", nameof (OnRegistryChanged));
        this.LoadSettingsFromRegistry(requestContext);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1440006, "VisualStudio.Services.FaultInjectionService", "FaultInjectionRegistrySettings", ex);
      }
      finally
      {
        requestContext.TraceLeave(1440007, "VisualStudio.Services.FaultInjectionService", "FaultInjectionRegistrySettings", nameof (OnRegistryChanged));
      }
    }

    internal void LoadSettingsFromRegistry(IVssRequestContext requestContext)
    {
      FaultDefinitionCollection definitionCollection = new FaultDefinitionCollection();
      foreach (RegistryEntry readEntry in requestContext.GetService<IVssRegistryService>().ReadEntries(requestContext, (RegistryQuery) "/Service/FaultInjectionService/**"))
      {
        try
        {
          FaultDefinition fault = JsonConvert.DeserializeObject<FaultDefinition>(readEntry.Value);
          definitionCollection.Add(fault);
        }
        catch (Exception ex)
        {
          requestContext.TraceAlways(1440004, TraceLevel.Warning, "VisualStudio.Services.FaultInjectionService", "FaultInjectionRegistrySettings", "Error loading fault definition from registry.  Error message: " + ex.ToString());
        }
      }
      this.m_faults = definitionCollection;
    }
  }
}
