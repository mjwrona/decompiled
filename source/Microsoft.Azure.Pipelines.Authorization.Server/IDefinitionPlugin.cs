// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Authorization.Server.IDefinitionPlugin
// Assembly: Microsoft.Azure.Pipelines.Authorization.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 22B31FF9-0E6B-45B0-A4F8-77598802CAB3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Authorization.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

namespace Microsoft.Azure.Pipelines.Authorization.Server
{
  [InheritedExport]
  public interface IDefinitionPlugin
  {
    Task ValidateDefinitionAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      int definitionId);

    Task ValidateDefinitionsAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      List<int> definitionIds);

    Task<List<int>> FilterValidDefinitionIdsAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      List<int> definitionIds);

    Task<bool> IsYamlProcess(IVssRequestContext requestContext, Guid projectId, int definitionId);
  }
}
