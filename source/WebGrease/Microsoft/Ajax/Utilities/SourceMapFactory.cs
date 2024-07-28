// Decompiled with JetBrains decompiler
// Type: Microsoft.Ajax.Utilities.SourceMapFactory
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.IO;

namespace Microsoft.Ajax.Utilities
{
  public static class SourceMapFactory
  {
    public static ISourceMap Create(TextWriter writer, string implementationName)
    {
      ISourceMap sourceMap = (ISourceMap) null;
      if (string.Compare(implementationName, V3SourceMap.ImplementationName, StringComparison.OrdinalIgnoreCase) == 0)
        sourceMap = (ISourceMap) new V3SourceMap(writer);
      else if (string.Compare(implementationName, ScriptSharpSourceMap.ImplementationName, StringComparison.OrdinalIgnoreCase) == 0)
        sourceMap = (ISourceMap) new ScriptSharpSourceMap(writer);
      return sourceMap;
    }
  }
}
