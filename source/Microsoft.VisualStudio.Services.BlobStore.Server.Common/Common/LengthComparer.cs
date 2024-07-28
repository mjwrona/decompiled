// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Common.LengthComparer
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB48D0BF-32A2-483C-A1D4-2F10DEBB3D56
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.Common.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Common
{
  internal class LengthComparer : IComparer<string>
  {
    internal static LengthComparer Instance = new LengthComparer();

    public int Compare(string x, string y) => x.Length - y.Length;
  }
}
