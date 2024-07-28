// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement.JobMapEntrySerializer
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using System;
using System.IO;
using System.Text;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement
{
  public class JobMapEntrySerializer : ISerializer<FeedJobMapEntry>
  {
    private readonly ISerializer<FeedJobMapEntry> legacySerializer;

    public JobMapEntrySerializer(ISerializer<FeedJobMapEntry> legacySerializer) => this.legacySerializer = legacySerializer;

    public FeedJobMapEntry Deserialize(Stream s)
    {
      string input = Encoding.UTF8.GetString(((MemoryStream) s).ToArray());
      if (input.Length == 36)
        return new FeedJobMapEntry()
        {
          JobId = Guid.Parse(input)
        };
      s.Position = 0L;
      return this.legacySerializer.Deserialize(s);
    }

    public byte[] Serialize(FeedJobMapEntry input) => Encoding.UTF8.GetBytes(input.JobId.ToString());
  }
}
