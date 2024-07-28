// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.AntlrInputStream
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System.IO;

namespace Antlr4.Runtime
{
  internal class AntlrInputStream : BaseInputCharStream
  {
    protected internal char[] data;

    public AntlrInputStream()
    {
    }

    public AntlrInputStream(string input)
    {
      this.data = input.ToCharArray();
      this.n = input.Length;
    }

    public AntlrInputStream(char[] data, int numberOfActualCharsInArray)
    {
      this.data = data;
      this.n = numberOfActualCharsInArray;
    }

    public AntlrInputStream(TextReader r)
      : this(r, 1024, 1024)
    {
    }

    public AntlrInputStream(TextReader r, int initialSize)
      : this(r, initialSize, 1024)
    {
    }

    public AntlrInputStream(TextReader r, int initialSize, int readChunkSize) => this.Load(r, initialSize, readChunkSize);

    public AntlrInputStream(Stream input)
      : this((TextReader) new StreamReader(input), 1024)
    {
    }

    public AntlrInputStream(Stream input, int initialSize)
      : this((TextReader) new StreamReader(input), initialSize)
    {
    }

    public AntlrInputStream(Stream input, int initialSize, int readChunkSize)
      : this((TextReader) new StreamReader(input), initialSize, readChunkSize)
    {
    }

    public virtual void Load(TextReader r, int size, int readChunkSize)
    {
      if (r == null)
        return;
      this.data = r.ReadToEnd().ToCharArray();
      this.n = this.data.Length;
    }

    protected override int ValueAt(int i) => (int) this.data[i];

    protected override string ConvertDataToString(int start, int count) => new string(this.data, start, count);
  }
}
