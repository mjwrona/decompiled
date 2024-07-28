// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ODataReader
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.OData
{
  public abstract class ODataReader
  {
    public abstract ODataReaderState State { get; }

    public abstract ODataItem Item { get; }

    public abstract bool Read();

    public virtual Stream CreateReadStream() => throw new NotImplementedException();

    public virtual TextReader CreateTextReader() => throw new NotImplementedException();

    public abstract Task<bool> ReadAsync();

    public virtual Task<Stream> CreateReadStreamAsync() => TaskUtils.GetTaskForSynchronousOperation<Stream>((Func<Stream>) (() => this.CreateReadStream()));

    public virtual Task<TextReader> CreateTextReaderAsync() => TaskUtils.GetTaskForSynchronousOperation<TextReader>((Func<TextReader>) (() => this.CreateTextReader()));
  }
}
