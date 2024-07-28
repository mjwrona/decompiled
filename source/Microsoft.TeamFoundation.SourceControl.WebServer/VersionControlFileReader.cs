// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.VersionControlFileReader
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Diff;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  public static class VersionControlFileReader
  {
    public static string ReadFileContent(Stream contentStream, int fileEncoding = 0)
    {
      Encoding encoding = VersionControlFileReader.GetEncoding(fileEncoding);
      return (encoding == null ? (TextReader) new StreamReader(contentStream) : (TextReader) new StreamReader(contentStream, encoding)).ReadToEnd();
    }

    public static List<string> ReadFileLines(Stream contentStream, int fileEncoding = 0)
    {
      List<string> stringList = new List<string>();
      Encoding encoding = VersionControlFileReader.GetEncoding(fileEncoding);
      DiffLineTokenizer diffLineTokenizer = new DiffLineTokenizer(contentStream, encoding);
      EndOfLineTerminator ofLineTerminator = EndOfLineTerminator.None;
      EndOfLineTerminator endOfLine;
      for (string str = diffLineTokenizer.NextLineToken(out endOfLine); str != null; str = diffLineTokenizer.NextLineToken(out endOfLine))
      {
        ofLineTerminator = endOfLine;
        stringList.Add(str);
      }
      if (ofLineTerminator != EndOfLineTerminator.None)
        stringList.Add(string.Empty);
      return stringList;
    }

    public static int CountFileLines(Stream contentStream, int fileEncoding = 0)
    {
      Encoding encoding = VersionControlFileReader.GetEncoding(fileEncoding);
      DiffLineTokenizer diffLineTokenizer = new DiffLineTokenizer(contentStream, encoding);
      int num = 0;
      EndOfLineTerminator ofLineTerminator = EndOfLineTerminator.None;
      EndOfLineTerminator endOfLine;
      for (string str = diffLineTokenizer.NextLineToken(out endOfLine); str != null; str = diffLineTokenizer.NextLineToken(out endOfLine))
      {
        ofLineTerminator = endOfLine;
        ++num;
      }
      if (ofLineTerminator != EndOfLineTerminator.None)
        ++num;
      return num;
    }

    private static Encoding GetEncoding(int fileEncoding)
    {
      Encoding encoding = (Encoding) null;
      try
      {
        if (fileEncoding > 0)
          encoding = Encoding.GetEncoding(fileEncoding);
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(599999, TraceLevel.Error, WebApiConstants.TraceArea, WebApiTraceLayers.Controller, nameof (VersionControlFileReader), ex);
      }
      return encoding;
    }
  }
}
