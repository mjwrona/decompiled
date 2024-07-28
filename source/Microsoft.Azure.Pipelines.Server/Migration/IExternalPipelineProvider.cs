// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Server.Migration.IExternalPipelineProvider
// Assembly: Microsoft.Azure.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC20940E-746B-4985-9936-F8ACD7ADA1DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Server.dll

using Microsoft.Azure.Pipelines.Server.ObjectModel;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Microsoft.Azure.Pipelines.Server.Migration
{
  [InheritedExport]
  public interface IExternalPipelineProvider
  {
    IReadOnlyList<Pipeline> GetPipelines(
      IVssRequestContext requestContext,
      Guid projectId,
      QueryPipelinesParameters queryParameters,
      PipelineOrderByExpression orderBy,
      int count);

    Pipeline GetPipeline(
      IVssRequestContext requestContext,
      Guid projectId,
      int pipelineId,
      int? pipelineRevision);

    Pipeline CreatePipeline(
      IVssRequestContext requestContext,
      Guid projectId,
      CreatePipelineParameters parameters);
  }
}
