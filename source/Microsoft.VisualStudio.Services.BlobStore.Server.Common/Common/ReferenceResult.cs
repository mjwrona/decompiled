// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Common.ReferenceResult
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB48D0BF-32A2-483C-A1D4-2F10DEBB3D56
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.Common.dll

using Microsoft.VisualStudio.Services.BlobStore.Common;
using System;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Common
{
  public struct ReferenceResult
  {
    public readonly BlobReference Reference;
    public readonly bool Success;
    public readonly DateTime? KeepUntilToCache;

    public ReferenceResult(BlobReference reference, bool success, DateTime? keepUntilToCache)
    {
      this.Reference = reference;
      this.Success = success;
      this.KeepUntilToCache = keepUntilToCache;
    }
  }
}
