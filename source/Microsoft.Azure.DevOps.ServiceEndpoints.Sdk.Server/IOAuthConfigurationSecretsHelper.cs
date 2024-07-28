// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.IOAuthConfigurationSecretsHelper
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 002C83BC-B53E-470A-8038-76E47B5E5BF3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server
{
  public interface IOAuthConfigurationSecretsHelper
  {
    void StoreSecrets(
      IVssRequestContext requestContext,
      Guid configurationId,
      string clientId,
      string clientSecret);

    void DeleteSecrets(IVssRequestContext requestContext, Guid configurationId);

    string ReadSecrets(IVssRequestContext requestContext, Guid configurationId, string clientId);
  }
}
