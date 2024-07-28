// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.Dependents
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;

namespace Antlr4.Runtime
{
  [Flags]
  internal enum Dependents
  {
    None = 0,
    Self = 1,
    Parents = 2,
    Children = 4,
    Ancestors = 8,
    Descendants = 16, // 0x00000010
    Siblings = 32, // 0x00000020
    PreceedingSiblings = 64, // 0x00000040
    FollowingSiblings = 128, // 0x00000080
    Preceeding = 256, // 0x00000100
    Following = 512, // 0x00000200
  }
}
