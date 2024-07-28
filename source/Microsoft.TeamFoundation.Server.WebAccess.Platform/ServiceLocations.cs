// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.ServiceLocations
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  [DataContract]
  public class ServiceLocations : WebSdkMetadata
  {
    private WebContext m_webContext;
    private const string s_area = "ContributionService";
    private const string s_layer = "ServiceLocations";

    public ServiceLocations(WebContext webContext) => this.m_webContext = webContext;

    public ServiceLocations()
    {
    }

    [DataMember]
    public Dictionary<Guid, Dictionary<ContextHostType, string>> Locations { get; private set; }

    public void Add(Guid serviceInstanceId, TeamFoundationHostType hostType)
    {
      if (this.m_webContext == null || !this.m_webContext.IsHosted || hostType > this.m_webContext.TfsRequestContext.ServiceHost.HostType)
        return;
      IVssRequestContext tfsRequestContext = this.m_webContext.TfsRequestContext;
      if (!tfsRequestContext.ServiceHost.Is(hostType))
        tfsRequestContext = tfsRequestContext.To(hostType);
      ILocationService service = tfsRequestContext.GetService<ILocationService>();
      try
      {
        string accessMappingMoniker = AccessMappingConstants.ClientAccessMappingMoniker;
        if (tfsRequestContext.IsFeatureEnabled("VisualStudio.Services.WebPlatform.UsePublicAccessMappingMoniker"))
          accessMappingMoniker = AccessMappingConstants.PublicAccessMappingMoniker;
        string locationServiceUrl = service.GetLocationServiceUrl(tfsRequestContext, serviceInstanceId, accessMappingMoniker);
        if (locationServiceUrl == null)
          return;
        if (this.Locations == null)
          this.Locations = new Dictionary<Guid, Dictionary<ContextHostType, string>>();
        Dictionary<ContextHostType, string> dictionary;
        if (!this.Locations.TryGetValue(serviceInstanceId, out dictionary))
        {
          dictionary = new Dictionary<ContextHostType, string>();
          this.Locations.Add(serviceInstanceId, dictionary);
        }
        dictionary[(ContextHostType) hostType] = locationServiceUrl;
      }
      catch (VssServiceResponseException ex)
      {
        tfsRequestContext.TraceException(10013514, "ContributionService", nameof (ServiceLocations), (Exception) ex);
      }
    }
  }
}
