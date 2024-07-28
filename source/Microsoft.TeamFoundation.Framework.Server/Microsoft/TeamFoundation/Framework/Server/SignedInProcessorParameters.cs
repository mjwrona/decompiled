// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SignedInProcessorParameters
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Compliance;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class SignedInProcessorParameters
  {
    private const string c_useRealmInsteadOfReplyTo = "VisualStudio.Services.Framework.SignedInControllerParameters.UseRealmInsteadOfReplyTo";
    private const string c_area = "Authentication";
    private const string c_layer = "SignedInControllerParameters";

    public string Domain { get; set; }

    public string Realm { get; set; }

    public Guid? HostId { get; set; }

    public bool RealmIsDeploymentHost { get; set; }

    public string ReplyTo { get; set; }

    public SecureFlowLocation RealmSignedInLocation { get; set; }

    public bool FinalHop { get; set; }

    public static SignedInProcessorParameters EvaluateParameters(
      IVssRequestContext requestContext,
      string realm,
      string reply_to,
      string protocol,
      string hid,
      Uri requestUri,
      SecureFlowLocation currentLocation)
    {
      requestContext.TraceEnter(1450796, "Authentication", "SignedInControllerParameters", nameof (EvaluateParameters));
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      bool flag1 = !string.IsNullOrWhiteSpace(realm);
      bool flag2 = false;
      string str1 = (string) null;
      Guid? nullable = new Guid?();
      SecureFlowLocation location = (SecureFlowLocation) null;
      Guid result;
      if (!string.IsNullOrEmpty(hid) && Guid.TryParse(hid, out result))
        nullable = new Guid?(result);
      SecureFlowLocation defaultLocation = SecureFlowLocation.GetDefaultLocation(vssRequestContext);
      string str2 = defaultLocation.ToString();
      if (SecureFlowLocation.TryCreate(requestContext, reply_to, SecureFlowLocation.GetDefaultLocation(requestContext), out location) == SecureFlowLocationState.Valid)
      {
        flag2 = true;
        reply_to = location.ToString();
      }
      string parentDomain = vssRequestContext.GetService<IVssRegistryService>().GetValue<string>(vssRequestContext, in ConfigurationConstants.DevOpsRootDomain);
      bool flag3 = flag1 && requestContext.IsFeatureEnabled("VisualStudio.Services.Framework.SignedInControllerParameters.UseRealmInsteadOfReplyTo");
      bool flag4 = flag2 && !string.IsNullOrEmpty(parentDomain) && (flag3 ? realm : location.Host).IsSubdomainOf(parentDomain);
      bool flag5 = false;
      if (!flag2)
      {
        location = defaultLocation;
        reply_to = str2;
        flag5 = true;
        realm = (string) null;
        flag1 = true;
        requestContext.Trace(5500102, TraceLevel.Warning, "Authentication", "SignedInControllerParameters", string.Format("SignedIn Request does not have a valid reply_to parameter. Redirecting to deployment level mapping."));
      }
      else if (flag4)
      {
        flag5 = true;
      }
      else
      {
        foreach (ServiceInstance serviceInstance in (IEnumerable<ServiceInstance>) vssRequestContext.GetService<IInstanceManagementService>().GetServiceInstances(vssRequestContext))
        {
          List<string> source = new List<string>();
          if (serviceInstance.PublicUri != (Uri) null)
            source.Add(serviceInstance.PublicUri.Authority);
          source.AddRange(serviceInstance.AdditionalMappings.Select<LocationMapping, string>((Func<LocationMapping, string>) (m => new Uri(m.Location).Authority)));
          if (source.Any<string>((Func<string, bool>) (r => r.Equals(realm, StringComparison.OrdinalIgnoreCase))))
          {
            flag5 = true;
            break;
          }
        }
      }
      if (!flag5 && realm != null)
      {
        if (((IEnumerable<string>) realm.Split('.')).Take<string>(2).Any<string>((Func<string, bool>) (s => string.Equals(s, "app", StringComparison.OrdinalIgnoreCase))) || realm.StartsWith("azurecommerce.", StringComparison.OrdinalIgnoreCase))
          flag5 = true;
      }
      if (!flag5)
      {
        if (!flag1)
        {
          reply_to = str2;
          location = defaultLocation;
          flag5 = true;
          nullable = new Guid?();
          requestContext.Trace(5500101, TraceLevel.Error, "Authentication", "SignedInControllerParameters", string.Format("SignedIn Request does not have a realm parameter. Assuming this is a deployment-level signin. Reply_to: {0}", (object) reply_to));
        }
        else if (requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) && str2.Contains(realm))
          flag5 = true;
      }
      bool flag6 = false;
      if (flag4)
      {
        if (currentLocation.Host.IsSubdomainOf(parentDomain))
          flag6 = true;
      }
      else
        flag6 = realm == null || realm.Contains(currentLocation.Host);
      AccessMapping accessMapping = vssRequestContext.GetService<ILocationService>().GetAccessMapping(vssRequestContext, AccessMappingConstants.ServiceDomainMappingMoniker);
      IVssRequestContext requestContext1 = requestContext;
      string realm1 = realm;
      string protocol1 = protocol;
      Guid? hostId = nullable;
      SecureFlowLocation secureReplyTo = location;
      Uri devopsDomainUri;
      if (accessMapping == null)
      {
        devopsDomainUri = (Uri) null;
      }
      else
      {
        string accessPoint = accessMapping.AccessPoint;
        devopsDomainUri = accessPoint != null ? accessPoint.AsUri() : (Uri) null;
      }
      int num = flag4 ? 1 : 0;
      SecureFlowLocation secureFlowLocation = SignedInProcessorParameters.BuildSignedInLocation(requestContext1, realm1, protocol1, hostId, secureReplyTo, devopsDomainUri, num != 0);
      requestContext.TraceLeave(1450799, "Authentication", "SignedInControllerParameters", nameof (EvaluateParameters));
      return new SignedInProcessorParameters()
      {
        Domain = str1,
        Realm = realm,
        RealmIsDeploymentHost = flag5,
        HostId = nullable,
        ReplyTo = reply_to,
        RealmSignedInLocation = secureFlowLocation,
        FinalHop = flag6
      };
    }

    public SecureFlowLocation BuildSignedInLocation(
      IVssRequestContext requestContext,
      string protocol,
      SecureFlowLocation secureReplyTo)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      AccessMapping accessMapping = vssRequestContext.GetService<ILocationService>().GetAccessMapping(vssRequestContext, AccessMappingConstants.ServiceDomainMappingMoniker);
      string parentDomain = vssRequestContext.GetService<IVssRegistryService>().GetValue<string>(vssRequestContext, in ConfigurationConstants.DevOpsRootDomain);
      bool flag = !string.IsNullOrEmpty(parentDomain) && secureReplyTo.Host.IsSubdomainOf(parentDomain);
      IVssRequestContext requestContext1 = requestContext;
      string realm = this.Realm;
      string protocol1 = protocol;
      Guid? hostId = this.HostId;
      SecureFlowLocation secureReplyTo1 = secureReplyTo;
      Uri devopsDomainUri;
      if (accessMapping == null)
      {
        devopsDomainUri = (Uri) null;
      }
      else
      {
        string accessPoint = accessMapping.AccessPoint;
        devopsDomainUri = accessPoint != null ? accessPoint.AsUri() : (Uri) null;
      }
      int num = flag ? 1 : 0;
      return SignedInProcessorParameters.BuildSignedInLocation(requestContext1, realm, protocol1, hostId, secureReplyTo1, devopsDomainUri, num != 0);
    }

    private static SecureFlowLocation BuildSignedInLocation(
      IVssRequestContext requestContext,
      string realm,
      string protocol,
      Guid? hostId,
      SecureFlowLocation secureReplyTo,
      Uri devopsDomainUri,
      bool isDevOpsDomainSignIn)
    {
      string str = secureReplyTo.ToString();
      UriBuilder uriBuilder = new UriBuilder(str)
      {
        Query = "realm=" + realm + "&reply_to=" + Uri.EscapeDataString(str) + "&protocol=" + protocol
      };
      if (isDevOpsDomainSignIn)
      {
        uriBuilder.Host = devopsDomainUri.Host;
        uriBuilder.Path = !hostId.HasValue ? PathUtility.Combine(devopsDomainUri.AbsolutePath, "_signedin") : PathUtility.Combine(devopsDomainUri.AbsolutePath, string.Format("/{0}/{1}/_signedin", (object) "serviceHosts", (object) hostId.Value));
      }
      else
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        string path1 = vssRequestContext.GetService<IVssRegistryService>().GetValue(vssRequestContext, in ConfigurationConstants.TestEnvironmentQuery);
        uriBuilder.Path = PathUtility.Combine(path1, "_signedin");
      }
      SecureFlowLocation location;
      int num = (int) SecureFlowLocation.TryCreate(requestContext, uriBuilder.Uri, SecureFlowLocation.GetDefaultLocation(requestContext), out location);
      return location;
    }

    internal static Uri RemoveSentitiveQueryParams(IVssRequestContext requestContext, Uri url)
    {
      requestContext.TraceEnter(1450800, "Authentication", "SignedInControllerParameters", nameof (RemoveSentitiveQueryParams));
      NameValueCollection queryString = url.ParseQueryString();
      queryString.Remove("code");
      queryString.Remove("id_token");
      queryString.Remove("session_state");
      Uri uri = new Uri(url.GetLeftPart(UriPartial.Path));
      requestContext.TraceLeave(1450804, "Authentication", "SignedInControllerParameters", nameof (RemoveSentitiveQueryParams));
      NameValueCollection queryValues = queryString;
      return uri.AppendQuery(queryValues);
    }
  }
}
