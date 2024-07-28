// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestExecution.Server.IPropertyServiceHelper
// Assembly: Microsoft.VisualStudio.Services.TE.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BC2680F-A5FB-41BE-A4CF-F78BF7AC3E02
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.TE.Server.Common.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestExecution.Server
{
  [CLSCompliant(false)]
  public interface IPropertyServiceHelper
  {
    void RegisterInternalArtifactKind(TestExecutionRequestContext context, Guid artifactKind);

    void AddOrUpdate(
      TestExecutionRequestContext context,
      Guid artifactKind,
      int artifactId,
      IDictionary<string, object> properties);

    void AddOrUpdate(
      TestExecutionRequestContext context,
      Guid artifactKind,
      string artifactId,
      IDictionary<string, object> properties);

    IDictionary<string, object> Get(
      TestExecutionRequestContext context,
      Guid artifactKind,
      int artifactId,
      IEnumerable<string> propertyNameFilters);

    IDictionary<string, object> Get(
      TestExecutionRequestContext context,
      Guid artifactKind,
      string artifactId,
      IEnumerable<string> propertyNameFilters);

    void Delete(TestExecutionRequestContext context, Guid artifactKind, int artifactId);

    void Delete(TestExecutionRequestContext context, Guid artifactKind, string artifactId);
  }
}
