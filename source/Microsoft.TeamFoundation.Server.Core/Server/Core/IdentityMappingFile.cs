// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.IdentityMappingFile
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Microsoft.TeamFoundation.Server.Core
{
  internal class IdentityMappingFile : IDisposable
  {
    private const string cCloudIdentity = "CloudIdentity";
    private const string cDisplayName = "DisplayName";
    private const string cLocalIdentity = "LocalIdentity";
    private const int cColumnCount = 3;
    private const char cSeparatorChar = ',';
    private const char cEscapeCharacter = '"';
    private const char cLineFeed = '\n';
    private const char cCarriageReturn = '\r';
    private bool m_isEndOfFile;
    private StreamReader m_reader;
    private StreamWriter m_writer;

    public IdentityMappingFile(string file, FileAccess fileAccess)
    {
      if (fileAccess == FileAccess.Read)
      {
        this.m_reader = File.Exists(file) ? new StreamReader((Stream) new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) : throw new FileNotFoundException(FrameworkResources.IdentityMappingFileNotFound((object) file), file);
      }
      else
      {
        if (fileAccess != FileAccess.Write)
          throw new InvalidOperationException(FrameworkResources.ErrorMappingFileAccessMode());
        this.m_writer = new StreamWriter((Stream) new FileStream(file, FileMode.Create, FileAccess.Write));
        this.WriteLine("CloudIdentity", "DisplayName", "LocalIdentity");
      }
    }

    public IdentityMappingCollection ReadFile()
    {
      if (this.m_reader == null)
        throw new InvalidOperationException(FrameworkResources.ErrorMappingFileNotOpenForRead());
      IdentityMappingCollection mappingCollection = new IdentityMappingCollection();
      int num = 1;
      while (true)
      {
        try
        {
          IList<string> fields;
          this.ReadLine(out fields);
          if (!this.IsBlankLine(fields))
          {
            if (!this.IsHeader(fields))
              mappingCollection.Add(new IdentityMappingEntry(fields[0], fields[1], fields[2]));
          }
          else if (this.m_isEndOfFile)
            break;
          ++num;
        }
        catch (Exception ex)
        {
          throw new FileFormatException(FrameworkResources.ErrorParsingCsvFile((object) num, (object) ex.Message), ex);
        }
      }
      return mappingCollection;
    }

    public void WriteLine(params string[] fields)
    {
      if (fields.Length < 3)
        throw new FileFormatException(FrameworkResources.ErrorAtLeastThreeFields());
      if (this.m_writer == null)
        throw new InvalidOperationException(FrameworkResources.ErrorMappingFileNotOpenForWrite());
      for (int index = 0; index < fields.Length; ++index)
      {
        if (fields[index].Contains<char>(','))
        {
          this.m_writer.Write('"');
          this.m_writer.Write(fields[index]);
          this.m_writer.Write('"');
        }
        else
          this.m_writer.Write(fields[index]);
        if (index + 1 < fields.Length)
          this.m_writer.Write(',');
      }
      this.m_writer.WriteLine();
    }

    private bool IsHeader(IList<string> fields) => string.Equals(this.NormalizeHeader(fields[0]), "CloudIdentity", StringComparison.OrdinalIgnoreCase) && string.Equals(this.NormalizeHeader(fields[1]), "DisplayName", StringComparison.OrdinalIgnoreCase) && string.Equals(this.NormalizeHeader(fields[2]), "LocalIdentity", StringComparison.OrdinalIgnoreCase);

    private string NormalizeHeader(string header) => header.Trim().Replace(" ", "");

    internal bool ReadLine(out IList<string> fields)
    {
      fields = (IList<string>) null;
      IList<string> stringList = (IList<string>) new List<string>();
      string fieldValue;
      bool isEndOfLine;
      while (this.ReadField(out fieldValue, out isEndOfLine))
      {
        stringList.Add(fieldValue);
        if (isEndOfLine)
          break;
      }
      fields = stringList;
      if (stringList.Count > 0 && stringList.Count < 3)
      {
        for (int count = stringList.Count; count < 3; ++count)
          stringList.Add(string.Empty);
      }
      return stringList.Count > 0;
    }

    internal bool ReadField(out string fieldValue, out bool isEndOfLine)
    {
      StringBuilder stringBuilder = new StringBuilder(30);
      bool flag1 = false;
      isEndOfLine = false;
      bool flag2 = false;
      while (true)
      {
        int c = this.m_reader.Read();
        if (!this.IsEndofFile(c))
        {
          switch (c)
          {
            case 34:
              flag2 = true;
              if (this.m_reader.Peek() == 34)
              {
                stringBuilder.Append((char) this.m_reader.Read());
                continue;
              }
              flag1 = !flag1;
              continue;
            case 44:
              flag2 = true;
              if (flag1)
              {
                stringBuilder.Append((char) c);
                continue;
              }
              goto label_11;
            default:
              if (!this.IsEndOfLine(c))
              {
                flag2 = true;
                stringBuilder.Append((char) c);
                continue;
              }
              goto label_9;
          }
        }
        else
          goto label_11;
      }
label_9:
      flag2 = true;
      isEndOfLine = true;
label_11:
      fieldValue = stringBuilder.ToString().Trim();
      return flag2;
    }

    private bool IsBlankLine(IList<string> fields) => fields.Count == 0 || !fields.Any<string>((Func<string, bool>) (s => !string.IsNullOrEmpty(s)));

    private bool IsEndofFile(int c)
    {
      this.m_isEndOfFile = c < 0;
      return this.m_isEndOfFile;
    }

    private bool IsEndOfLine(int c)
    {
      bool flag = this.IsEndOfLineChar(c);
      if (flag)
      {
        while (this.IsEndOfLineChar(this.m_reader.Peek()))
          this.m_reader.Read();
      }
      return flag;
    }

    private bool IsEndOfLineChar(int c) => c == 10 || c == 13;

    public void Dispose() => this.Dispose(true);

    private void Dispose(bool disposing)
    {
      if (!disposing)
        return;
      if (this.m_reader != null)
      {
        this.m_reader.Dispose();
        this.m_reader = (StreamReader) null;
      }
      if (this.m_writer == null)
        return;
      this.m_writer.Flush();
      this.m_writer.Dispose();
      this.m_writer = (StreamWriter) null;
    }
  }
}
