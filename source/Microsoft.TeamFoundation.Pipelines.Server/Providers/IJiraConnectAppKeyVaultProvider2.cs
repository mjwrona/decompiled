// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.Providers.IJiraConnectAppKeyVaultProvider2
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CloudConfiguration;

namespace Microsoft.TeamFoundation.Pipelines.Server.Providers
{
  [DefaultServiceImplementation(typeof (JiraConnectAppKeyVaultProvider2))]
  public interface IJiraConnectAppKeyVaultProvider2 : IVssFrameworkService
  {
    SecretString SetSecret(IVssRequestContext requestContext, string key, string value);

    string GetSecret(IVssRequestContext requestContext, string key);

    SecretChangedResult DeleteSecret(IVssRequestContext requestContext, string key);

    bool PurgeSecret(IVssRequestContext requestContext, string key);
  }
}
