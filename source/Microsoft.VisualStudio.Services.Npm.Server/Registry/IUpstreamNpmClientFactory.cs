// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.Registry.IUpstreamNpmClientFactory
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Npm.Server.Registry
{
  public interface IUpstreamNpmClientFactory
  {
    Task<IUpstreamNpmClient> GetUpstreamClient(
      IVssRequestContext requestContext,
      UpstreamSource source);
  }
}
