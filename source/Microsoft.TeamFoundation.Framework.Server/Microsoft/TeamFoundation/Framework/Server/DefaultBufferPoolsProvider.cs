// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DefaultBufferPoolsProvider
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class DefaultBufferPoolsProvider : BufferPoolsProvider
  {
    internal static readonly BufferPoolsProvider Instance = (BufferPoolsProvider) new DefaultBufferPoolsProvider();

    private DefaultBufferPoolsProvider()
    {
    }

    protected override BufferPool[] CreateBufferPools() => OSDetails.IsClient ? new BufferPool[11]
    {
      new BufferPool(2048, 2),
      new BufferPool(4096, 2),
      new BufferPool(8192, 2),
      new BufferPool(16384, 2),
      new BufferPool(32768, 2),
      new BufferPool(65536, 2),
      new BufferPool(131072, 1),
      new BufferPool(262144, 1),
      new BufferPool(524288, 1),
      new BufferPool(1048576, 1),
      new BufferPool(2097152, 1)
    } : new BufferPool[11]
    {
      new BufferPool(2048, 32),
      new BufferPool(4096, 32),
      new BufferPool(8192, 32),
      new BufferPool(16384, 32),
      new BufferPool(32768, 64),
      new BufferPool(65536, 32),
      new BufferPool(131072, 8),
      new BufferPool(262144, 8),
      new BufferPool(524288, 8),
      new BufferPool(1048576, 8),
      new BufferPool(2097152, 4)
    };
  }
}
