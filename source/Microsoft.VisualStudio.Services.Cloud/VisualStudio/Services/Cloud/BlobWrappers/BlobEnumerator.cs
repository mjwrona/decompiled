// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.BlobWrappers.BlobEnumerator
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Azure.Storage.Blob;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Cloud.BlobWrappers
{
  public class BlobEnumerator : IEnumerator<ICloudBlobWrapper>, IDisposable, IEnumerator
  {
    private IEnumerator<ICloudBlob> m_enumerator;

    public BlobEnumerator(IEnumerable<IListBlobItem> list) => this.m_enumerator = list.OfType<ICloudBlob>().GetEnumerator();

    public ICloudBlobWrapper Current => (ICloudBlobWrapper) new CloudBlobWrapper(this.m_enumerator.Current);

    object IEnumerator.Current => (object) new CloudBlobWrapper(this.m_enumerator.Current);

    public void Dispose() => this.m_enumerator.Dispose();

    public bool MoveNext() => this.m_enumerator.MoveNext();

    public void Reset() => this.m_enumerator.Reset();
  }
}
