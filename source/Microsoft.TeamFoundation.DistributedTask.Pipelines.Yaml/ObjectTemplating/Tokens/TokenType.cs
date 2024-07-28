// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Tokens.TokenType
// Assembly: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2DC134C9-663D-46C7-A414-3ADCC50BB112
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.dll

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Tokens
{
  internal static class TokenType
  {
    internal const int Literal = 0;
    internal const int Sequence = 1;
    internal const int Mapping = 2;
    internal const int BasicExpression = 3;
    internal const int InsertExpression = 4;
    internal const int IfExpression = 5;
    internal const int ElseIfExpression = 6;
    internal const int ElseExpression = 7;
    internal const int EachExpression = 8;
  }
}
