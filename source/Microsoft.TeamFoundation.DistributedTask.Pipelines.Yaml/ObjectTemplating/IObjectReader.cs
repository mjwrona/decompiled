// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.IObjectReader
// Assembly: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2DC134C9-663D-46C7-A414-3ADCC50BB112
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Tokens;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating
{
  internal interface IObjectReader
  {
    bool AllowScalar(out int? line, out int? column, out string scalar, out ScalarStyle? style);

    bool AllowSequenceStart(out int? line, out int? column);

    bool AllowSequenceEnd();

    bool AllowMappingStart(out int? line, out int? column);

    bool AllowMappingEnd();

    void ValidateStart();

    void ValidateEnd();
  }
}
