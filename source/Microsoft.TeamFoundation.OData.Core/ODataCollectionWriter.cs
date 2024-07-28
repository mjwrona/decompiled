// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ODataCollectionWriter
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System.Threading.Tasks;

namespace Microsoft.OData
{
  public abstract class ODataCollectionWriter
  {
    public abstract void WriteStart(ODataCollectionStart collectionStart);

    public abstract Task WriteStartAsync(ODataCollectionStart collectionStart);

    public abstract void WriteItem(object item);

    public abstract Task WriteItemAsync(object item);

    public abstract void WriteEnd();

    public abstract Task WriteEndAsync();

    public abstract void Flush();

    public abstract Task FlushAsync();
  }
}
