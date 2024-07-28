// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.ISecretsProvider
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.ComponentModel.Composition;

namespace Microsoft.TeamFoundation.Build2.Server
{
  [InheritedExport]
  public interface ISecretsProvider
  {
    bool AllowSecrets(
      IVssRequestContext requestContext,
      BuildData build,
      BuildDefinition definition);

    void AnonymizeServiceEndpoint(
      IVssRequestContext requestContext,
      ServiceEndpoint serviceEndpoint);

    bool AllowServiceEndpoint(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid serviceEndpointId);
  }
}
