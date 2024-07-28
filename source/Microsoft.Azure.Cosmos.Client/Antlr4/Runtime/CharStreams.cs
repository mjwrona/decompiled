// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.CharStreams
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System.IO;
using System.Text;

namespace Antlr4.Runtime
{
  internal static class CharStreams
  {
    public static ICharStream fromPath(string path) => CharStreams.fromPath(path, Encoding.UTF8);

    public static ICharStream fromPath(string path, Encoding encoding)
    {
      CodePointCharStream codePointCharStream = new CodePointCharStream(File.ReadAllText(path, encoding));
      codePointCharStream.name = path;
      return (ICharStream) codePointCharStream;
    }

    public static ICharStream fromTextReader(TextReader textReader)
    {
      try
      {
        return (ICharStream) new CodePointCharStream(textReader.ReadToEnd());
      }
      finally
      {
        textReader.Dispose();
      }
    }

    public static ICharStream fromStream(Stream stream) => CharStreams.fromStream(stream, Encoding.UTF8);

    public static ICharStream fromStream(Stream stream, Encoding encoding)
    {
      using (StreamReader streamReader = new StreamReader(stream, encoding, false))
        return CharStreams.fromTextReader((TextReader) streamReader);
    }

    public static ICharStream fromstring(string s) => (ICharStream) new CodePointCharStream(s);
  }
}
