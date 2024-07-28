// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Security.Server.SecurityNamespacesController
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Security.Server
{
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "Security", ResourceName = "SecurityNamespaces")]
  public class SecurityNamespacesController : TfsApiController
  {
    [HttpGet]
    [ClientExample("GET__securitynamespaces_.json", "All security namespaces", null, null)]
    [ClientExample("GET__securitynamespaces__securityNamespaceId__.json", "Get the specified security namespace", null, null)]
    public IQueryable<Microsoft.VisualStudio.Services.Security.SecurityNamespaceDescription> QuerySecurityNamespaces(
      Guid securityNamespaceId = default (Guid),
      bool localOnly = false)
    {
      IEnumerable<Microsoft.TeamFoundation.Framework.Server.SecurityNamespaceDescription> descriptions;
      if (localOnly)
      {
        this.TfsRequestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(this.TfsRequestContext, FrameworkSecurity.FrameworkNamespaceId).CheckPermission(this.TfsRequestContext, FrameworkSecurity.FrameworkNamespaceToken, 1);
        IEnumerable<Microsoft.TeamFoundation.Framework.Server.SecurityNamespaceDescription> list;
        using (SecurityComponent component = this.TfsRequestContext.CreateComponent<SecurityComponent>("Default"))
          list = (IEnumerable<Microsoft.TeamFoundation.Framework.Server.SecurityNamespaceDescription>) component.QuerySecurityNamespaces(securityNamespaceId).OfType<SecurityComponent.LocalNamespaceDescription>().Select<SecurityComponent.LocalNamespaceDescription, Microsoft.TeamFoundation.Framework.Server.SecurityNamespaceDescription>((Func<SecurityComponent.LocalNamespaceDescription, Microsoft.TeamFoundation.Framework.Server.SecurityNamespaceDescription>) (s => s.ToPublicType())).ToList<Microsoft.TeamFoundation.Framework.Server.SecurityNamespaceDescription>();
        IVssRequestContext vssRequestContext = this.TfsRequestContext.To(TeamFoundationHostType.Deployment);
        SecurityNamespaceTemplateService service = vssRequestContext.GetService<SecurityNamespaceTemplateService>();
        long sequenceId;
        IEnumerable<NamespaceDescription> source;
        if (!(securityNamespaceId == Guid.Empty))
          source = (IEnumerable<NamespaceDescription>) new NamespaceDescription[1]
          {
            service.GetSecurityNamespaceTemplate(vssRequestContext, this.TfsRequestContext.ServiceHost.HostType, securityNamespaceId, out sequenceId)
          };
        else
          source = service.GetNamespaceTemplatesByHostType(vssRequestContext, this.TfsRequestContext.ServiceHost.HostType, out sequenceId).Values;
        IDictionary<Guid, Microsoft.TeamFoundation.Framework.Server.SecurityNamespaceDescription> dictionary = (IDictionary<Guid, Microsoft.TeamFoundation.Framework.Server.SecurityNamespaceDescription>) new Dictionary<Guid, Microsoft.TeamFoundation.Framework.Server.SecurityNamespaceDescription>();
        foreach (Microsoft.TeamFoundation.Framework.Server.SecurityNamespaceDescription namespaceDescription in source.OfType<Microsoft.TeamFoundation.Framework.Server.SecurityNamespaceDescription>())
          dictionary.Add(namespaceDescription.NamespaceId, namespaceDescription);
        foreach (Microsoft.TeamFoundation.Framework.Server.SecurityNamespaceDescription namespaceDescription in list)
        {
          if (!dictionary.ContainsKey(namespaceDescription.NamespaceId))
            dictionary.Add(namespaceDescription.NamespaceId, namespaceDescription);
        }
        descriptions = (IEnumerable<Microsoft.TeamFoundation.Framework.Server.SecurityNamespaceDescription>) dictionary.Values;
      }
      else
      {
        ITeamFoundationSecurityService service = (ITeamFoundationSecurityService) this.TfsRequestContext.GetService<SecuredTeamFoundationSecurityService>();
        if (Guid.Empty == securityNamespaceId)
        {
          descriptions = service.GetSecurityNamespaces(this.TfsRequestContext).Select<IVssSecurityNamespace, Microsoft.TeamFoundation.Framework.Server.SecurityNamespaceDescription>((Func<IVssSecurityNamespace, Microsoft.TeamFoundation.Framework.Server.SecurityNamespaceDescription>) (s => s.Description));
        }
        else
        {
          IVssSecurityNamespace securityNamespace = service.GetSecurityNamespace(this.TfsRequestContext, securityNamespaceId);
          if (securityNamespace != null)
            descriptions = (IEnumerable<Microsoft.TeamFoundation.Framework.Server.SecurityNamespaceDescription>) new Microsoft.TeamFoundation.Framework.Server.SecurityNamespaceDescription[1]
            {
              securityNamespace.Description
            };
          else
            descriptions = (IEnumerable<Microsoft.TeamFoundation.Framework.Server.SecurityNamespaceDescription>) Array.Empty<Microsoft.TeamFoundation.Framework.Server.SecurityNamespaceDescription>();
        }
      }
      return SecurityConverter.Convert(descriptions).AsQueryable<Microsoft.VisualStudio.Services.Security.SecurityNamespaceDescription>();
    }

    [HttpPost]
    [ClientInternalUseOnly(true)]
    public void SetInheritFlag(Guid securityNamespaceId, JObject container)
    {
      ArgumentUtility.CheckForEmptyGuid(securityNamespaceId, nameof (securityNamespaceId));
      ArgumentUtility.CheckForNull<JObject>(container, nameof (container));
      JToken jtoken1 = container["token"] ?? (JToken) new JProperty("token", (object[]) null);
      JToken jtoken2 = container["inherit"] ?? (JToken) new JProperty("inherit", (object) bool.TrueString);
      string str = jtoken1.ToObject<string>();
      bool inherit = jtoken2.ToObject<bool>();
      ArgumentUtility.CheckForNull<string>(str, "token");
      (this.TfsRequestContext.GetService<SecuredTeamFoundationSecurityService>().GetSecurityNamespace(this.TfsRequestContext, securityNamespaceId) ?? throw new InvalidSecurityNamespaceException(securityNamespaceId)).SetInheritFlag(this.TfsRequestContext, str, inherit);
    }

    public override string TraceArea => "SecurityService";

    public override string ActivityLogArea => "Framework";
  }
}
