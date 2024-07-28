// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Azure.MemoryBlobAccount
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7823E4AE-BEB6-4A7C-9914-276DEAE1FB1F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Azure.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Content.Server.Azure
{
  public sealed class MemoryBlobAccount
  {
    public static readonly MemoryBlobAccount Global = new MemoryBlobAccount("global");
    public readonly string Name;
    private readonly Dictionary<string, MemoryBlobContainer> Containers = new Dictionary<string, MemoryBlobContainer>();

    public MemoryBlobAccount(string accountName) => this.Name = accountName;

    public void Reset()
    {
      lock (this.Containers)
        this.Containers.Clear();
    }

    public void UseContainer(
      string containerName,
      Func<MemoryBlobContainer, MemoryBlobContainer> action)
    {
      lock (this.Containers)
      {
        MemoryBlobContainer memoryBlobContainer;
        this.Containers.TryGetValue(containerName, out memoryBlobContainer);
        this.Containers[containerName] = action(memoryBlobContainer);
      }
    }

    public bool IsEmpty
    {
      get
      {
        lock (this.Containers)
          return this.Containers.All<KeyValuePair<string, MemoryBlobContainer>>((Func<KeyValuePair<string, MemoryBlobContainer>, bool>) (c => c.Value.IsEmpty));
      }
    }
  }
}
