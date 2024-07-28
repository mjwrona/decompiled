// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Provenance.SessionIdSessionRequestProvider
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.CrossProtocol.WebApi.Types;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Provenance
{
  public class SessionIdSessionRequestProvider : ISessionRequestProvider
  {
    private readonly IVssRequestContext requestContext;

    public SessionIdSessionRequestProvider(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public Task<(bool Success, SessionRequest Request)> TryGetSessionRequest()
    {
      object obj;
      return this.requestContext.Items.TryGetValue("Packaging.Provenance.Session", out obj) && obj is SessionKey sessionKey ? Task.FromResult<(bool, SessionRequest)>((true, this.requestContext.GetService<ISessionMetadataService>().GetSessionMetadata(this.requestContext, sessionKey))) : Task.FromResult<(bool, SessionRequest)>((false, (SessionRequest) null));
    }
  }
}
