// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Azure.MemoryBlobContainer
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7823E4AE-BEB6-4A7C-9914-276DEAE1FB1F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Azure.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Content.Server.Azure
{
  public sealed class MemoryBlobContainer
  {
    public readonly MemoryBlobAccount Account;
    public readonly string Name;
    public readonly Dictionary<string, MemoryBlob> Blobs = new Dictionary<string, MemoryBlob>();

    public MemoryBlobContainer(MemoryBlobAccount account, string containerName)
    {
      this.Account = account;
      this.Name = containerName;
    }

    public void UseBlob(string blobName, Func<MemoryBlob, MemoryBlob> action) => this.Account.UseContainer(this.Name, (Func<MemoryBlobContainer, MemoryBlobContainer>) (container =>
    {
      MemoryBlob memoryBlob1;
      this.Blobs.TryGetValue(blobName, out memoryBlob1);
      MemoryBlob memoryBlob2 = action(memoryBlob1);
      if (memoryBlob1 != null && memoryBlob2 == null)
        this.Blobs.Remove(blobName);
      else if (memoryBlob2 != null)
        this.Blobs[blobName] = memoryBlob2;
      return container;
    }));

    internal bool IsEmpty => !this.Blobs.Any<KeyValuePair<string, MemoryBlob>>();
  }
}
