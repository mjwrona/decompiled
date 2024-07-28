// Decompiled with JetBrains decompiler
// Type: Microsoft.Ajax.Utilities.V3SourceMap
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Microsoft.Ajax.Utilities
{
  public sealed class V3SourceMap : ISourceMap, IDisposable
  {
    private string m_minifiedPath;
    private string m_mapPath;
    private TextWriter m_writer;
    private int m_maxMinifiedLine;
    private bool m_hasProperty;
    private HashSet<string> m_sourceFiles;
    private List<string> m_sourceFileList;
    private HashSet<string> m_names;
    private List<string> m_nameList;
    private List<V3SourceMap.Segment> m_segments;
    private int m_lastDestinationLine;
    private int m_lastDestinationColumn;
    private int m_lastSourceLine;
    private int m_lastSourceColumn;
    private int m_lastFileIndex;
    private int m_lastNameIndex;
    private int m_lineOffset;
    private int m_columnOffset;
    private static string s_base64 = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";

    public string SourceRoot { get; set; }

    public bool SafeHeader { get; set; }

    public static string ImplementationName => "V3";

    public string Name => V3SourceMap.ImplementationName;

    public V3SourceMap(TextWriter writer)
    {
      this.m_writer = writer;
      this.m_sourceFiles = new HashSet<string>();
      this.m_sourceFileList = new List<string>();
      this.m_names = new HashSet<string>();
      this.m_nameList = new List<string>();
      this.m_segments = new List<V3SourceMap.Segment>();
      this.m_lastDestinationLine = -1;
      this.m_lastDestinationColumn = -1;
      this.m_lastSourceLine = -1;
      this.m_lastSourceColumn = -1;
      this.m_lastFileIndex = -1;
      this.m_lastNameIndex = -1;
      this.m_lineOffset = 0;
      this.m_columnOffset = 0;
    }

    public void StartPackage(string sourcePath, string mapPath)
    {
      this.m_minifiedPath = sourcePath;
      this.m_mapPath = mapPath;
    }

    public void EndPackage()
    {
    }

    public void NewLineInsertedInOutput()
    {
      this.m_columnOffset = 0;
      ++this.m_lineOffset;
    }

    public void EndOutputRun(int lineNumber, int columnPosition)
    {
      this.m_lineOffset += lineNumber;
      this.m_columnOffset += columnPosition;
    }

    public object StartSymbol(AstNode node, int startLine, int startColumn) => (object) null;

    public void MarkSegment(
      AstNode node,
      int startLine,
      int startColumn,
      string name,
      Context context)
    {
      if (startLine == int.MaxValue)
        throw new ArgumentOutOfRangeException(nameof (startLine));
      startLine += this.m_lineOffset;
      startColumn += this.m_columnOffset;
      if (!string.IsNullOrEmpty(name) && this.m_names.Add(name))
        this.m_nameList.Add(name);
      this.m_maxMinifiedLine = Math.Max(this.m_maxMinifiedLine, startLine);
      if (context != null && context.Document != null && context.Document.FileContext != null && this.m_sourceFiles.Add(context.Document.FileContext))
        this.m_sourceFileList.Add(V3SourceMap.MakeRelative(context.Document.FileContext, this.m_mapPath));
      this.m_segments.Add(this.CreateSegment(startLine + 1, startColumn, context == null || context.StartLineNumber < 1 ? -1 : context.StartLineNumber - 1, context == null || context.StartColumn < 0 ? -1 : context.StartColumn, context.IfNotNull<Context, string>((Func<Context, string>) (c => V3SourceMap.MakeRelative(c.Document.FileContext, this.m_mapPath))), name));
    }

    public void EndSymbol(object symbol, int endLine, int endColumn, string parentContext)
    {
    }

    public void EndFile(TextWriter writer, string newLine)
    {
      if (writer == null || this.m_mapPath.IsNullOrWhiteSpace())
        return;
      writer.Write(newLine);
      writer.Write("//# sourceMappingURL={0}", (object) V3SourceMap.MakeRelative(this.m_mapPath, this.m_minifiedPath));
      writer.Write(newLine);
    }

    public void Dispose()
    {
      if (this.m_writer == null)
        return;
      if (this.SafeHeader)
        this.m_writer.WriteLine(")]}'");
      this.m_writer.WriteLine("{");
      this.WriteProperty("version", 3);
      this.WriteProperty("file", V3SourceMap.MakeRelative(this.m_minifiedPath, this.m_mapPath));
      this.WriteProperty("lineCount", this.m_maxMinifiedLine + 1);
      this.WriteProperty("mappings", this.GenerateMappings((IList<string>) this.m_sourceFileList, (IList<string>) this.m_nameList));
      if (!this.SourceRoot.IsNullOrWhiteSpace())
        this.WriteProperty("sourceRoot", this.SourceRoot);
      this.WriteProperty("sources", (ICollection<string>) this.m_sourceFileList);
      this.WriteProperty("names", (ICollection<string>) this.m_nameList);
      this.m_writer.WriteLine();
      this.m_writer.WriteLine("}");
      this.m_writer.Close();
      this.m_writer = (TextWriter) null;
    }

    private V3SourceMap.Segment CreateSegment(
      int destinationLine,
      int destinationColumn,
      int sourceLine,
      int sourceColumn,
      string fileName,
      string symbolName)
    {
      V3SourceMap.Segment segment = new V3SourceMap.Segment()
      {
        DestinationLine = destinationLine,
        DestinationColumn = this.m_lastDestinationColumn < 0 || this.m_lastDestinationLine < destinationLine ? destinationColumn : destinationColumn - this.m_lastDestinationColumn,
        SourceLine = fileName == null ? -1 : (this.m_lastSourceLine < 0 ? sourceLine : sourceLine - this.m_lastSourceLine),
        SourceColumn = fileName == null ? -1 : (this.m_lastSourceColumn < 0 ? sourceColumn : sourceColumn - this.m_lastSourceColumn),
        FileName = fileName,
        SymbolName = symbolName
      };
      this.m_lastDestinationLine = destinationLine;
      this.m_lastDestinationColumn = destinationColumn;
      if (!string.IsNullOrEmpty(fileName))
      {
        this.m_lastSourceLine = sourceLine;
        this.m_lastSourceColumn = sourceColumn;
      }
      return segment;
    }

    private string GenerateMappings(IList<string> fileList, IList<string> nameList)
    {
      StringBuilder sb = new StringBuilder();
      int num = 1;
      foreach (V3SourceMap.Segment segment in this.m_segments)
      {
        if (num < segment.DestinationLine)
        {
          do
          {
            sb.Append(';');
          }
          while (++num < segment.DestinationLine);
        }
        else if (sb.Length > 0)
          sb.Append(',');
        this.EncodeNumbers(sb, segment, fileList, nameList);
      }
      return sb.ToString();
    }

    private void EncodeNumbers(
      StringBuilder sb,
      V3SourceMap.Segment segment,
      IList<string> files,
      IList<string> names)
    {
      V3SourceMap.EncodeNumber(sb, segment.DestinationColumn);
      if (segment.FileName.IsNullOrWhiteSpace())
        return;
      int num1 = files.IndexOf(segment.FileName);
      V3SourceMap.EncodeNumber(sb, this.m_lastFileIndex < 0 ? num1 : num1 - this.m_lastFileIndex);
      this.m_lastFileIndex = num1;
      V3SourceMap.EncodeNumber(sb, segment.SourceLine);
      V3SourceMap.EncodeNumber(sb, segment.SourceColumn);
      if (string.IsNullOrEmpty(segment.SymbolName))
        return;
      int num2 = names.IndexOf(segment.SymbolName);
      V3SourceMap.EncodeNumber(sb, this.m_lastNameIndex < 0 ? num2 : num2 - this.m_lastNameIndex);
      this.m_lastNameIndex = num2;
    }

    private static void EncodeNumber(StringBuilder sb, int value)
    {
      value = value < 0 ? -value << 1 | 1 : value << 1;
      do
      {
        int index = value & 31;
        value >>= 5;
        if (value > 0)
          index |= 32;
        sb.Append(V3SourceMap.s_base64[index]);
      }
      while (value > 0);
    }

    private static string MakeRelative(string path, string relativeFrom)
    {
      if (!path.IsNullOrWhiteSpace())
      {
        if (!relativeFrom.IsNullOrWhiteSpace())
        {
          try
          {
            return new Uri(V3SourceMap.Normalize(relativeFrom)).MakeRelativeUri(new Uri(V3SourceMap.Normalize(path))).ToString();
          }
          catch (UriFormatException ex)
          {
          }
        }
      }
      return path;
    }

    private static string Normalize(string path) => !Path.IsPathRooted(path) ? Path.Combine(Environment.CurrentDirectory, path) : path;

    private void WriteProperty(string name, int number)
    {
      this.WritePropertyStart(name);
      this.m_writer.Write(number.ToStringInvariant());
    }

    private void WriteProperty(string name, string text)
    {
      this.WritePropertyStart(name);
      this.OutputEscapedString(text ?? string.Empty);
    }

    private void WriteProperty(string name, ICollection<string> collection)
    {
      this.WritePropertyStart(name);
      this.m_writer.Write('[');
      bool flag = true;
      foreach (string text in (IEnumerable<string>) collection)
      {
        if (flag)
          flag = false;
        else
          this.m_writer.Write(',');
        this.OutputEscapedString(text);
      }
      this.m_writer.Write(']');
    }

    private void WritePropertyStart(string name)
    {
      if (this.m_hasProperty)
        this.m_writer.WriteLine(',');
      this.OutputEscapedString(name);
      this.m_writer.Write(':');
      this.m_hasProperty = true;
    }

    private void OutputEscapedString(string text)
    {
      this.m_writer.Write('"');
      for (int index = 0; index < text.Length; ++index)
      {
        char ch = text[index];
        switch (ch)
        {
          case '\b':
            this.m_writer.Write("\\b");
            break;
          case '\t':
            this.m_writer.Write("\\t");
            break;
          case '\n':
            this.m_writer.Write("\\n");
            break;
          case '\f':
            this.m_writer.Write("\\f");
            break;
          case '\r':
            this.m_writer.Write("\\r");
            break;
          case '"':
            this.m_writer.Write("\\\"");
            break;
          default:
            if (ch < ' ')
            {
              this.m_writer.Write("\\u{0:x4}", (object) (int) ch);
              break;
            }
            this.m_writer.Write(ch);
            break;
        }
      }
      this.m_writer.Write('"');
    }

    private class Segment
    {
      public int DestinationLine { get; set; }

      public int DestinationColumn { get; set; }

      public int SourceLine { get; set; }

      public int SourceColumn { get; set; }

      public string FileName { get; set; }

      public string SymbolName { get; set; }
    }
  }
}
