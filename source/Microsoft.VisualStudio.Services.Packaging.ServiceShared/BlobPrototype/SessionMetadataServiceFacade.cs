// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.SessionMetadataServiceFacade
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.CrossProtocol.WebApi.Types;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Provenance;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public class SessionMetadataServiceFacade : ISessionMetadataServiceFacade
  {
    private readonly IVssRequestContext requestContext;

    public SessionMetadataServiceFacade(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public SessionRequest GetSessionMetadata(SessionKey sessionKey) => this.requestContext.GetService<ISessionMetadataService>().GetSessionMetadata(this.requestContext, sessionKey);

    public Task StoreSessionMetadataAsync(SessionKey sessionKey, SessionRequest metadata) => this.requestContext.GetService<ISessionMetadataService>().StoreSessionMetadataAsync(this.requestContext, sessionKey, metadata);
  }
}
