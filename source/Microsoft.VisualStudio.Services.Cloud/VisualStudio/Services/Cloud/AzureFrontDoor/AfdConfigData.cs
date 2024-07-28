// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AzureFrontDoor.AfdConfigData
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Cloud.AzureFrontDoor
{
  public class AfdConfigData
  {
    public string Tenant { get; set; }

    public string Partner { get; set; }

    public List<string> Hosts { get; set; }

    public List<string> Paths { get; set; }

    public string EndpointName { get; set; }

    public string EndpointAddress { get; set; }

    public string EndpointPool { get; set; }

    public string LookupServiceEndpointPool { get; set; }

    public string LookupServiceEndpoint { get; set; }

    public string ForwardingRouteName { get; set; }

    public string HttpsRedirectRouteName { get; set; }

    public string HttpsRedirectDestinationPath { get; set; }

    public string HealthCheckEndpoint { get; set; }

    public string LookupServiceHealthCheckEndpoint { get; set; }

    public int? HealthProbeInterval { get; set; }

    public bool AlertsEnabled { get; set; }

    public bool CreateAlerts { get; set; }

    public string ReliabilityAlertName { get; set; }

    public double ReliabilityAlertThreshold { get; set; }

    public bool UseWebSockets { get; set; }

    public bool UseHttpsRedirectRoutes { get; set; }

    public bool UseEndpointLookupRouteKey { get; set; }

    public string GetRouteName(bool httpsRedirect) => !httpsRedirect ? this.ForwardingRouteName : this.HttpsRedirectRouteName;
  }
}
