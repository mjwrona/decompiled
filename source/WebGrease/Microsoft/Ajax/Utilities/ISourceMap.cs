// Decompiled with JetBrains decompiler
// Type: Microsoft.Ajax.Utilities.ISourceMap
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.IO;

namespace Microsoft.Ajax.Utilities
{
  public interface ISourceMap : IDisposable
  {
    void StartPackage(string sourcePath, string mapPath);

    void EndPackage();

    object StartSymbol(AstNode node, int startLine, int startColumn);

    void MarkSegment(AstNode node, int startLine, int startColumn, string name, Context context);

    void EndSymbol(object symbol, int endLine, int endColumn, string parentContext);

    void EndOutputRun(int lineNumber, int columnPosition);

    void EndFile(TextWriter writer, string newLine);

    void NewLineInsertedInOutput();

    string Name { get; }

    string SourceRoot { get; set; }

    bool SafeHeader { get; set; }
  }
}
