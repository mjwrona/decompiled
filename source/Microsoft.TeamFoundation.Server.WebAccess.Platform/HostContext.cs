// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.HostContext
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  [DataContract]
  public class HostContext : ContextIdentifier
  {
    protected IVssRequestContext m_requestContext;
    private string m_hostUri;

    public HostContext(IVssRequestContext requestContext)
    {
      this.m_requestContext = requestContext;
      this.Id = requestContext.ServiceHost.InstanceId;
      this.Name = requestContext.ServiceHost.Name;
      this.RelativeUri = requestContext.VirtualPath();
    }

    public HostContext()
    {
    }

    [DataMember(EmitDefaultValue = false, Order = 30)]
    public string Uri
    {
      get
      {
        if (this.m_hostUri == null && this.m_requestContext != null)
          this.m_hostUri = this.m_requestContext.GetService<ILocationService>().GetLocationServiceUrl(this.m_requestContext, LocationServiceConstants.SelfReferenceIdentifier, AccessMappingConstants.ClientAccessMappingMoniker);
        return this.m_hostUri;
      }
      set => this.m_hostUri = value;
    }

    [DataMember(EmitDefaultValue = false, Order = 40)]
    public string RelativeUri { get; set; }
  }
}
