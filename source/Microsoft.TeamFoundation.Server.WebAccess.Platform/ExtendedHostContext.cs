// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.ExtendedHostContext
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Organization;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  [DataContract]
  public class ExtendedHostContext : HostContext
  {
    private bool? m_isAccountAadBacked;
    private System.Uri m_hostUri;

    public ExtendedHostContext(IVssRequestContext requestContext)
      : base(requestContext)
    {
      this.HostType = (ContextHostType) requestContext.ServiceHost.HostType;
      if (!requestContext.ExecutionEnvironment.IsHostedDeployment || this.HostType != (ContextHostType.Deployment | ContextHostType.Application))
        return;
      this.HostType = ContextHostType.Deployment;
    }

    public ExtendedHostContext()
    {
    }

    [DataMember(EmitDefaultValue = false, Order = 50)]
    public ContextHostType HostType { get; set; }

    [DataMember(EmitDefaultValue = false, Order = 60)]
    public string Scheme => this.HostUri?.Scheme;

    [DataMember(EmitDefaultValue = false, Order = 70)]
    public string Authority => this.HostUri?.Authority;

    [DataMember(EmitDefaultValue = false, Name = "isAADAccount")]
    public bool IsAadBacked
    {
      get
      {
        if (!this.m_isAccountAadBacked.HasValue)
        {
          bool flag = false;
          if (this.m_requestContext != null && this.m_requestContext.ExecutionEnvironment.IsHostedDeployment && this.m_requestContext.ServiceHost != null && !this.m_requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
          {
            using (PerformanceTimer.StartMeasure(this.m_requestContext, "HostContext.IsAadBacked"))
            {
              try
              {
                flag = this.m_requestContext.IsOrganizationAadBacked();
              }
              catch (Exception ex)
              {
                this.m_requestContext.TraceException(520007, "WebAccess", TfsTraceLayers.Controller, ex);
              }
            }
          }
          this.m_isAccountAadBacked = new bool?(flag);
        }
        return this.m_isAccountAadBacked.Value;
      }
      set => this.m_isAccountAadBacked = new bool?(value);
    }

    private System.Uri HostUri
    {
      get
      {
        System.Uri result;
        if (this.m_hostUri == (System.Uri) null && !string.IsNullOrEmpty(this.Uri) && System.Uri.TryCreate(this.Uri, UriKind.Absolute, out result))
          this.m_hostUri = result;
        return this.m_hostUri;
      }
    }
  }
}
