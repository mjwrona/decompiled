// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Checks.Server.IPipelineDetailsProvider
// Assembly: Microsoft.Azure.Pipelines.Checks.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D08C7285-654E-4A4D-BA46-748F0D1E96AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Checks.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Microsoft.Azure.Pipelines.Checks.Server
{
  [InheritedExport]
  public interface IPipelineDetailsProvider
  {
    IDictionary<int, string> GetPipelinesIdNameMapping(
      IVssRequestContext requestContext,
      Guid projectId);

    IDictionary<string, string> GetStagesNameMapping(
      IVssRequestContext requestContext,
      Guid projectId,
      int pipelineId);
  }
}
