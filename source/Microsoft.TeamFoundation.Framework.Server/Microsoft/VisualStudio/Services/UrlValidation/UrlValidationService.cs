// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.UrlValidation.UrlValidationService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.NameResolution;
using Microsoft.VisualStudio.Services.NameResolution.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.UrlValidation
{
  public class UrlValidationService : IUrlValidationService, IVssFrameworkService
  {
    private const string s_area = "Authentication";
    private const string s_layer = "UrlValidationService";
    private static readonly RegistryQuery _vstsRootDomainQuery = new RegistryQuery("/Configuration/SignedIn/VstsRootDomain");
    private static readonly RegistryQuery _devOpsRootDomainQuery = new RegistryQuery("/Configuration/SignedIn/DevOpsRootDomain");
    private static readonly RegistryQuery _whitelistedDomainsQuery = new RegistryQuery("/Configuration/UrlValidation/Whitelist");
    private const string _vstsRootDomainDefault = "visualstudio.com";
    private const string _vstsRootDomainDevFabricDefault = "vsts.me";
    private const string _devOpsRootDomainDefault = "dev.azure.com";
    private const string _devOpsRootDomainDevFabricDefault = "codedev.ms";
    private const string _whitelistDefault = "cdn1,cdn2,cdn3,elstest,sps,tfsodata,deployment,teamfoundation,teamfoundationserver,ww,api,auditservice,azboards,azchatops,azdevchatops,azdevopscommerce,azminicom,azents,careers,codesmarts,commvtwo,dev,dl,docs,e,elstest,exchange,explore,forums,githubapp,githubapps,i3,insightsportal,intellitrace,internetexplorer,jscript,liveshare,lync,media,offer,pipelines,pipelinesapp,premium,professional,promo,reg,skydrive,scaleUnits,secretscan,serviceDeployments,serviceHosts,sqlazure,ssh,start,status,statusalt1,status-alt1,support,tfsapp,ultimate,userext,video,vscatalog,vsdevprofile,vsdscops,vsevidence,vslicense,vsnotify,vsodata,vspolicy,vssps,webmatrix,x-boards,x-pipes,x-pipes-ppe,x-ibizacd";
    private string _vstsRootDomain;
    private string _devOpsRootDomain;
    private HashSet<string> _serviceNames;
    private HashSet<string> _whitelistedDomains;

    public void ServiceStart(IVssRequestContext requestContext)
    {
      if (requestContext.ExecutionEnvironment.IsDevFabricDeployment)
      {
        this._vstsRootDomain = this.GetRootDomain(requestContext, UrlValidationService._vstsRootDomainQuery, "vsts.me");
        this._devOpsRootDomain = this.GetRootDomain(requestContext, UrlValidationService._devOpsRootDomainQuery, "codedev.ms");
      }
      else
      {
        this._vstsRootDomain = this.GetRootDomain(requestContext, UrlValidationService._vstsRootDomainQuery, "visualstudio.com");
        this._devOpsRootDomain = this.GetRootDomain(requestContext, UrlValidationService._devOpsRootDomainQuery, "dev.azure.com");
      }
      this._whitelistedDomains = this.GetWhiteListedDomains(requestContext, UrlValidationService._whitelistedDomainsQuery);
      this._serviceNames = this.GetServiceNames(requestContext);
    }

    public void ServiceEnd(IVssRequestContext requestContext)
    {
    }

    public bool IsValidUrl(IVssRequestContext requestContext, string url)
    {
      try
      {
        requestContext.TraceEnter(45333, "Authentication", nameof (UrlValidationService), nameof (IsValidUrl));
        ArgumentUtility.CheckStringForNullOrEmpty(url, nameof (url));
        string host = new Uri(url).Host;
        if (host.EndsWith(this._devOpsRootDomain))
          return true;
        if (!host.EndsWith(this._vstsRootDomain))
          return false;
        string[] strArray = host.Split(new char[1]{ '.' }, StringSplitOptions.RemoveEmptyEntries);
        if (strArray.Length <= 2)
          return false;
        string str = strArray[strArray.Length - 3];
        IVssRequestContext context = requestContext.To(TeamFoundationHostType.Deployment);
        IInternalNameResolutionService service = context.GetService<IInternalNameResolutionService>();
        IVssRequestContext requestContext1 = context;
        List<string> namespaces = new List<string>();
        namespaces.Add("Collection");
        namespaces.Add("Deployment");
        string name = str;
        int num1 = service.QueryFirstEntry(requestContext1, (IReadOnlyList<string>) namespaces, name, (Predicate<NameResolutionEntry>) (x => x.IsEnabled)) != null ? 1 : 0;
        bool flag1 = this._serviceNames.Contains(str);
        bool flag2 = this._whitelistedDomains.Contains(str);
        int num2 = flag1 ? 1 : 0;
        return (num1 | num2 | (flag2 ? 1 : 0)) != 0;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(45334, "Authentication", nameof (UrlValidationService), ex);
        return false;
      }
      finally
      {
        requestContext.TraceLeave(45333, "Authentication", nameof (UrlValidationService), nameof (IsValidUrl));
      }
    }

    private HashSet<string> GetServiceNames(IVssRequestContext requestContext)
    {
      HashSet<string> serviceNames = new HashSet<string>();
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      foreach (ServiceInstanceType serviceInstanceType in (IEnumerable<ServiceInstanceType>) vssRequestContext.GetService<IInstanceManagementService>().GetServiceInstanceTypes(vssRequestContext))
      {
        if (!(serviceInstanceType.InstanceType == ServiceInstanceTypes.TFS))
        {
          LocationMapping locationMapping = serviceInstanceType.LocationMappings.Where<LocationMapping>((Func<LocationMapping, bool>) (x => StringComparer.OrdinalIgnoreCase.Equals(x.AccessMappingMoniker, AccessMappingConstants.RootDomainMappingMoniker))).FirstOrDefault<LocationMapping>();
          if (locationMapping != null)
          {
            string host = new Uri(locationMapping.Location).Host;
            string[] strArray = host.Split(new char[1]
            {
              '.'
            }, StringSplitOptions.RemoveEmptyEntries);
            if (strArray.Length == 3)
            {
              string str = strArray[0];
              serviceNames.Add(str);
            }
            else
              requestContext.Trace(45335, TraceLevel.Info, "Authentication", nameof (UrlValidationService), "Encountered service domain not composed of 3 subdomains: " + host);
          }
        }
      }
      serviceNames.Add(this.GetSpsServiceName(requestContext));
      return serviceNames;
    }

    private string GetSpsServiceName(IVssRequestContext requestContext)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      return new Uri(vssRequestContext.GetService<ILocationService>().GetLocationData(vssRequestContext, ServiceInstanceTypes.SPS).GetAccessMapping(vssRequestContext, AccessMappingConstants.RootDomainMappingMoniker).AccessPoint).Host.Split(new char[1]
      {
        '.'
      }, StringSplitOptions.RemoveEmptyEntries)[0];
    }

    private string GetRootDomain(
      IVssRequestContext requestContext,
      RegistryQuery rootDomainRegistryQuery,
      string defaultValue)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      return vssRequestContext.GetService<IVssRegistryService>().GetValue<string>(vssRequestContext, in rootDomainRegistryQuery, defaultValue);
    }

    private HashSet<string> GetWhiteListedDomains(
      IVssRequestContext requestContext,
      RegistryQuery whitelistedDomainsRegistryQuery)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      return new HashSet<string>((IEnumerable<string>) vssRequestContext.GetService<IVssRegistryService>().GetValue<string>(vssRequestContext, in whitelistedDomainsRegistryQuery, "cdn1,cdn2,cdn3,elstest,sps,tfsodata,deployment,teamfoundation,teamfoundationserver,ww,api,auditservice,azboards,azchatops,azdevchatops,azdevopscommerce,azminicom,azents,careers,codesmarts,commvtwo,dev,dl,docs,e,elstest,exchange,explore,forums,githubapp,githubapps,i3,insightsportal,intellitrace,internetexplorer,jscript,liveshare,lync,media,offer,pipelines,pipelinesapp,premium,professional,promo,reg,skydrive,scaleUnits,secretscan,serviceDeployments,serviceHosts,sqlazure,ssh,start,status,statusalt1,status-alt1,support,tfsapp,ultimate,userext,video,vscatalog,vsdevprofile,vsdscops,vsevidence,vslicense,vsnotify,vsodata,vspolicy,vssps,webmatrix,x-boards,x-pipes,x-pipes-ppe,x-ibizacd").Split(new char[1]
      {
        ','
      }, StringSplitOptions.RemoveEmptyEntries));
    }
  }
}
