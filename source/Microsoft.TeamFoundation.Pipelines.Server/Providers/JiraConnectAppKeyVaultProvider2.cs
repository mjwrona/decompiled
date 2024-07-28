// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.Providers.JiraConnectAppKeyVaultProvider2
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CloudConfiguration;

namespace Microsoft.TeamFoundation.Pipelines.Server.Providers
{
  public class JiraConnectAppKeyVaultProvider2 : 
    IJiraConnectAppKeyVaultProvider2,
    IVssFrameworkService
  {
    public SecretString SetSecret(IVssRequestContext requestContext, string key, string value) => new KeyVaultProvider2(requestContext).SetSecret(key, value);

    public string GetSecret(IVssRequestContext requestContext, string key) => new KeyVaultProvider2(requestContext).GetSecret(key);

    public SecretChangedResult DeleteSecret(IVssRequestContext requestContext, string key) => new KeyVaultProvider2(requestContext).DeleteSecret(key);

    public bool PurgeSecret(IVssRequestContext requestContext, string key) => new KeyVaultProvider2(requestContext).PurgeSecret(key);

    public void ServiceStart(IVssRequestContext requestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext requestContext)
    {
    }
  }
}
