// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.ClientStorage.IVssClientStorage
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System.ComponentModel;

namespace Microsoft.VisualStudio.Services.Common.ClientStorage
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public interface IVssClientStorage : IVssClientStorageReader, IVssClientStorageWriter
  {
    string PathKeyCombine(params string[] paths);

    char PathSeparator { get; }
  }
}
