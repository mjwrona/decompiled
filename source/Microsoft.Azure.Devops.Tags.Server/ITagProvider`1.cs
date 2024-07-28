// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Devops.Tags.Server.ITagProvider`1
// Assembly: Microsoft.Azure.Devops.Tags.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10FDC8E3-D1DB-4668-B2F2-04DAA10A7143
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Devops.Tags.Server.dll

using System.ComponentModel.Composition;

namespace Microsoft.Azure.Devops.Tags.Server
{
  [InheritedExport]
  public interface ITagProvider<T> : ITagProvider
  {
    ITagStorageProvider<T> StorageProvider { get; }

    ITagSecurityProvider<T> SecurityProvider { get; }
  }
}
