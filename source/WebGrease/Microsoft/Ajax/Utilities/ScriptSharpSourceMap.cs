// Decompiled with JetBrains decompiler
// Type: Microsoft.Ajax.Utilities.ScriptSharpSourceMap
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Xml;

namespace Microsoft.Ajax.Utilities
{
  public sealed class ScriptSharpSourceMap : ISourceMap, IDisposable
  {
    private readonly XmlWriter m_writer;
    private string m_currentPackagePath;
    private string m_mapPath;
    private Dictionary<string, int> m_sourceFileIndexMap = new Dictionary<string, int>();
    private int currentIndex;
    private int m_lineOffset;
    private int m_columnOffset;

    public string SourceRoot { get; set; }

    public bool SafeHeader { get; set; }

    public static string ImplementationName => "XML";

    public string Name => ScriptSharpSourceMap.ImplementationName;

    public ScriptSharpSourceMap(TextWriter writer)
    {
      if (writer == null)
        throw new ArgumentNullException(nameof (writer));
      XmlWriterSettings settings = new XmlWriterSettings()
      {
        CloseOutput = true,
        Indent = true
      };
      this.m_writer = XmlWriter.Create(writer, settings);
      this.m_writer.WriteStartDocument();
      this.m_writer.WriteStartElement("map");
      ScriptSharpSourceMap.JavaScriptSymbol.WriteHeadersTo(this.m_writer);
      this.m_writer.WriteStartElement("scriptFiles");
    }

    public void StartPackage(string sourcePath, string mapPath)
    {
      this.m_currentPackagePath = sourcePath;
      this.m_mapPath = mapPath;
      this.m_writer.WriteStartElement("scriptFile");
      this.m_writer.WriteAttributeString("path", ScriptSharpSourceMap.MakeRelative(sourcePath, this.m_mapPath) ?? string.Empty);
    }

    public void EndPackage()
    {
      if (this.m_currentPackagePath.IsNullOrWhiteSpace())
        return;
      using (FileStream inputStream = new FileStream(this.m_currentPackagePath, FileMode.Open))
      {
        using (MD5 md5 = MD5.Create())
        {
          byte[] hash = md5.ComputeHash((Stream) inputStream);
          this.m_writer.WriteStartElement("checksum");
          this.m_writer.WriteAttributeString("value", BitConverter.ToString(hash));
          this.m_writer.WriteEndElement();
          this.m_writer.WriteEndElement();
        }
      }
      this.m_currentPackagePath = (string) null;
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

    public object StartSymbol(AstNode node, int startLine, int startColumn) => node != null && !node.Context.Document.IsGenerated ? (object) ScriptSharpSourceMap.JavaScriptSymbol.StartNew(node, startLine + this.m_lineOffset, startColumn + this.m_columnOffset, this.GetSourceFileIndex(node.Context.Document.FileContext)) : (object) null;

    public void MarkSegment(
      AstNode node,
      int startLine,
      int startColumn,
      string name,
      Context context)
    {
      if (node == null || string.IsNullOrEmpty(name) || !(node is FunctionObject functionObject) || string.CompareOrdinal(name, functionObject.Binding.Name) != 0 || context == functionObject.Context)
        return;
      startLine += this.m_lineOffset;
      startColumn += this.m_columnOffset;
      ScriptSharpSourceMap.JavaScriptSymbol javaScriptSymbol = ScriptSharpSourceMap.JavaScriptSymbol.StartNew((AstNode) new Lookup(context)
      {
        Name = name
      }, startLine, startColumn, this.GetSourceFileIndex(functionObject.Context.Document.FileContext));
      javaScriptSymbol.End(startLine, startColumn + name.Length, name);
      javaScriptSymbol.WriteTo(this.m_writer);
    }

    public void EndSymbol(object symbol, int endLine, int endColumn, string parentContext)
    {
      if (symbol == null)
        return;
      endLine += this.m_lineOffset;
      endColumn += this.m_columnOffset;
      ScriptSharpSourceMap.JavaScriptSymbol javaScriptSymbol = (ScriptSharpSourceMap.JavaScriptSymbol) symbol;
      javaScriptSymbol.End(endLine, endColumn, parentContext);
      javaScriptSymbol.WriteTo(this.m_writer);
    }

    public void EndFile(TextWriter writer, string newLine)
    {
    }

    public void Dispose()
    {
      this.EndPackage();
      this.m_writer.WriteEndElement();
      this.m_writer.WriteStartElement("sourceFiles");
      foreach (KeyValuePair<string, int> sourceFileIndex in this.m_sourceFileIndexMap)
      {
        this.m_writer.WriteStartElement("sourceFile");
        this.m_writer.WriteAttributeString("id", sourceFileIndex.Value.ToStringInvariant());
        this.m_writer.WriteAttributeString("path", ScriptSharpSourceMap.MakeRelative(sourceFileIndex.Key, this.m_mapPath) ?? string.Empty);
        this.m_writer.WriteEndElement();
      }
      this.m_writer.WriteEndElement();
      this.m_writer.WriteEndElement();
      this.m_writer.WriteEndDocument();
      this.m_writer.Close();
    }

    private int GetSourceFileIndex(string fileName)
    {
      int sourceFileIndex;
      if (!this.m_sourceFileIndexMap.TryGetValue(fileName, out sourceFileIndex))
      {
        sourceFileIndex = ++this.currentIndex;
        this.m_sourceFileIndexMap.Add(fileName, sourceFileIndex);
      }
      return sourceFileIndex;
    }

    private static string MakeRelative(string path, string relativeFrom)
    {
      if (!path.IsNullOrWhiteSpace())
      {
        if (!relativeFrom.IsNullOrWhiteSpace())
        {
          try
          {
            return new Uri(ScriptSharpSourceMap.Normalize(relativeFrom)).MakeRelativeUri(new Uri(ScriptSharpSourceMap.Normalize(path))).ToString();
          }
          catch (UriFormatException ex)
          {
          }
        }
      }
      return path;
    }

    private static string Normalize(string path) => !Path.IsPathRooted(path) ? Path.Combine(Environment.CurrentDirectory, path) : path;

    private class JavaScriptSymbol
    {
      private const string SymbolDataFormat = "{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12}";
      private int m_startLine;
      private int m_endLine;
      private int m_startColumn;
      private int m_endColumn;
      private Context m_sourceContext;
      private int m_sourceFileId;
      private string m_symbolType;
      private string m_parentFunction;

      private JavaScriptSymbol()
      {
      }

      public static ScriptSharpSourceMap.JavaScriptSymbol StartNew(
        AstNode node,
        int startLine,
        int startColumn,
        int sourceFileId)
      {
        if (startLine == int.MaxValue)
          throw new ArgumentOutOfRangeException(nameof (startLine));
        if (startColumn == int.MaxValue)
          throw new ArgumentOutOfRangeException(nameof (startColumn));
        return new ScriptSharpSourceMap.JavaScriptSymbol()
        {
          m_startLine = startLine + 1,
          m_startColumn = startColumn + 1,
          m_sourceContext = node?.Context,
          m_symbolType = node != null ? node.GetType().Name : "[UNKNOWN]",
          m_sourceFileId = sourceFileId
        };
      }

      public void End(int endLine, int endColumn, string parentFunction)
      {
        if (endLine == int.MaxValue)
          throw new ArgumentOutOfRangeException(nameof (endLine));
        if (endColumn == int.MaxValue)
          throw new ArgumentOutOfRangeException(nameof (endColumn));
        this.m_endLine = endLine + 1;
        this.m_endColumn = endColumn + 1;
        this.m_parentFunction = parentFunction;
      }

      public static void WriteHeadersTo(XmlWriter writer)
      {
        if (writer == null)
          return;
        writer.WriteStartElement("headers");
        writer.WriteString("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12}".FormatInvariant((object) "DstStartLine", (object) "DstStartColumn", (object) "DstEndLine", (object) "DstEndColumn", (object) "SrcStartPosition", (object) "SrcEndPosition", (object) "SrcStartLine", (object) "SrcStartColumn", (object) "SrcEndLine", (object) "SrcEndColumn", (object) "SrcFileId", (object) "SymbolType", (object) "ParentFunction"));
        writer.WriteEndElement();
      }

      public void WriteTo(XmlWriter writer)
      {
        if (writer == null)
          return;
        writer.WriteStartElement("s");
        writer.WriteString("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12}".FormatInvariant((object) this.m_startLine, (object) this.m_startColumn, (object) this.m_endLine, (object) this.m_endColumn, (object) (this.m_sourceContext.StartPosition - this.m_sourceContext.SourceOffsetStart), (object) (this.m_sourceContext.EndPosition - this.m_sourceContext.SourceOffsetEnd), (object) this.m_sourceContext.StartLineNumber, (object) this.m_sourceContext.StartColumn, (object) this.m_sourceContext.EndLineNumber, (object) this.m_sourceContext.EndColumn, (object) this.m_sourceFileId, (object) this.m_symbolType, (object) this.m_parentFunction));
        writer.WriteEndElement();
      }
    }
  }
}
