// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Authorization.Server.IDefinitionExtensionService
// Assembly: Microsoft.Azure.Pipelines.Authorization.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 22B31FF9-0E6B-45B0-A4F8-77598802CAB3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Authorization.Server.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.Azure.Pipelines.Authorization.Server
{
  [DefaultServiceImplementation(typeof (DefinitionExtensionService))]
  public interface IDefinitionExtensionService : IVssFrameworkService
  {
    IDefinitionPlugin GetDefinitionPlugin();

    IPipelineDefinitionPlugin GetPipelineDefinitionPlugin();
  }
}
