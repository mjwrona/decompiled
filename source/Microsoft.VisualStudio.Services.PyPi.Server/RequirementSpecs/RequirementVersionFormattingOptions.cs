// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.RequirementSpecs.RequirementVersionFormattingOptions
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using System;

namespace Microsoft.VisualStudio.Services.PyPi.Server.RequirementSpecs
{
  [Flags]
  public enum RequirementVersionFormattingOptions
  {
    Default = 0,
    NoParenthesesAroundVersionList = 1,
  }
}
