// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ODataParameterWriter
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System.Threading.Tasks;

namespace Microsoft.OData
{
  public abstract class ODataParameterWriter
  {
    public abstract void WriteStart();

    public abstract Task WriteStartAsync();

    public abstract void WriteValue(string parameterName, object parameterValue);

    public abstract Task WriteValueAsync(string parameterName, object parameterValue);

    public abstract ODataCollectionWriter CreateCollectionWriter(string parameterName);

    public abstract Task<ODataCollectionWriter> CreateCollectionWriterAsync(string parameterName);

    public abstract ODataWriter CreateResourceWriter(string parameterName);

    public abstract Task<ODataWriter> CreateResourceWriterAsync(string parameterName);

    public abstract ODataWriter CreateResourceSetWriter(string parameterName);

    public abstract Task<ODataWriter> CreateResourceSetWriterAsync(string parameterName);

    public abstract void WriteEnd();

    public abstract Task WriteEndAsync();

    public abstract void Flush();

    public abstract Task FlushAsync();
  }
}
