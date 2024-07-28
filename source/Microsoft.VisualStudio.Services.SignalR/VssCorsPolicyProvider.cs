// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.SignalR.VssCorsPolicyProvider
// Assembly: Microsoft.VisualStudio.Services.SignalR, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: BD148864-3B8A-4D7D-BD16-EF04E9549DC9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.SignalR.dll

using Microsoft.Owin;
using Microsoft.Owin.Cors;
using System.Threading.Tasks;
using System.Web.Cors;

namespace Microsoft.VisualStudio.Services.SignalR
{
  public class VssCorsPolicyProvider : ICorsPolicyProvider
  {
    private CorsPolicy m_corsPolicy;

    public VssCorsPolicyProvider() => this.m_corsPolicy = new CorsPolicy()
    {
      AllowAnyHeader = true,
      AllowAnyMethod = true,
      AllowAnyOrigin = true,
      SupportsCredentials = true
    };

    public Task<CorsPolicy> GetCorsPolicyAsync(IOwinRequest request) => Task.FromResult<CorsPolicy>(this.m_corsPolicy);
  }
}
