// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.PackageExtraction.FullPathTarEntryFactory
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;
using System.Text;

namespace Microsoft.VisualStudio.Services.Npm.Server.PackageExtraction
{
  internal class FullPathTarEntryFactory : TarInputStream.IEntryFactory
  {
    internal TarInputStream.EntryFactoryAdapter adapter = new TarInputStream.EntryFactoryAdapter(GZipConstants.Encoding);

    public TarEntry CreateEntry(string name) => this.adapter.CreateEntry(name);

    public TarEntry CreateEntry(byte[] headerBuffer)
    {
      TarEntry entry = this.adapter.CreateEntry(headerBuffer);
      if (entry.TarHeader.Magic == "ustar")
      {
        StringBuilder stringBuilder = new StringBuilder(155);
        int index;
        for (index = 345; index < 500 && headerBuffer[index] != (byte) 0; ++index)
          stringBuilder.Append((char) headerBuffer[index]);
        string str = stringBuilder.ToString();
        if (index > 345 && !entry.Name.StartsWith(str))
          entry.Name = str.ToString() + "/" + entry.Name;
      }
      return entry;
    }

    public TarEntry CreateEntryFromFile(string fileName) => this.adapter.CreateEntryFromFile(fileName);
  }
}
