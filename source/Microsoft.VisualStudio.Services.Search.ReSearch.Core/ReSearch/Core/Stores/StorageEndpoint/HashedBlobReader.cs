// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.ReSearch.Core.Stores.StorageEndpoint.HashedBlobReader
// Assembly: Microsoft.VisualStudio.Services.Search.ReSearch.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 657A74AE-F2A6-4615-BB2F-7FA1F961B173
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.ReSearch.Core.dll

using Microsoft.VisualStudio.Services.Search.Server.Storage.FileSystem.Definitions;

namespace Microsoft.VisualStudio.Services.Search.ReSearch.Core.Stores.StorageEndpoint
{
  public class HashedBlobReader : HashedBlob
  {
    public void Read(IFileReader reader, out byte[] blob, out bool isCompressed)
    {
      int num = (int) reader.ReadUInt32();
      int count = reader.ReadInt32();
      reader.ReadBytes(HashedBlob.HashLength);
      isCompressed = reader.ReadBoolean();
      blob = reader.ReadBytes(count);
    }
  }
}
