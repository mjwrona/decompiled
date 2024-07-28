// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.DefinitionQueryOptions
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using System;

namespace Microsoft.TeamFoundation.Build2.Server
{
  [Flags]
  public enum DefinitionQueryOptions
  {
    None = 0,
    Options = 1,
    Repository = 2,
    Triggers = 4,
    Process = 8,
    Variables = 16, // 0x00000010
    Demands = 32, // 0x00000020
    RetentionPolicy = 64, // 0x00000040
    ProcessParameters = 128, // 0x00000080
    All = ProcessParameters | RetentionPolicy | Demands | Variables | Process | Triggers | Repository | Options, // 0x000000FF
  }
}
