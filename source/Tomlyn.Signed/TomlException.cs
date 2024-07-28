// Decompiled with JetBrains decompiler
// Type: Tomlyn.TomlException
// Assembly: Tomlyn.Signed, Version=0.16.0.0, Culture=neutral, PublicKeyToken=925c35bbd70fa682
// MVID: 8155B7FF-9444-4ADB-B8A6-687FA556DA39
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Tomlyn.Signed.dll

using System;
using Tomlyn.Syntax;


#nullable enable
namespace Tomlyn
{
  public class TomlException : Exception
  {
    public TomlException(DiagnosticsBag diagnostics)
      : base(diagnostics.ToString())
    {
      this.Diagnostics = diagnostics;
    }

    public DiagnosticsBag Diagnostics { get; }
  }
}
