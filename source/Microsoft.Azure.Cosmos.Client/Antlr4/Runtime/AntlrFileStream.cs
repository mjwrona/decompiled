// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.AntlrFileStream
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System.IO;
using System.Text;

namespace Antlr4.Runtime
{
  internal class AntlrFileStream : AntlrInputStream
  {
    protected internal string fileName;

    public AntlrFileStream(string fileName)
      : this(fileName, (Encoding) null)
    {
    }

    public AntlrFileStream(string fileName, Encoding encoding)
    {
      this.fileName = fileName;
      this.Load(fileName, encoding);
    }

    public virtual void Load(string fileName, Encoding encoding)
    {
      if (fileName == null)
        return;
      this.data = (encoding == null ? File.ReadAllText(fileName) : File.ReadAllText(fileName, encoding)).ToCharArray();
      this.n = this.data.Length;
    }

    public override string SourceName => this.fileName;
  }
}
