// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.IClientSettingService
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts;

namespace Microsoft.VisualStudio.Services.BlobStore.Server
{
  [DefaultServiceImplementation(typeof (ClientSettingService))]
  public interface IClientSettingService : IVssFrameworkService
  {
    ClientSettingsInfo GetSettings(IVssRequestContext requestContext, Client toolName);

    void SetSettings(IVssRequestContext requestContext, ClientSettingsInfo settings);

    void DeleteSettings(IVssRequestContext requestContext, ClientSettingsInfo settings);

    void SetValue(
      IVssRequestContext requestContext,
      Client toolName,
      ClientSettingKey key,
      string value);
  }
}
