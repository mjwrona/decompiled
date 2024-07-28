// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Stores.TreeStore.WikiMetaDataStoreItemEncoder
// Assembly: Microsoft.VisualStudio.Services.Search.ReSearch.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 657A74AE-F2A6-4615-BB2F-7FA1F961B173
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.ReSearch.Core.dll

using Microsoft.VisualStudio.Services.Search.Server.Storage.FileSystem.Definitions;

namespace Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Stores.TreeStore
{
  public sealed class WikiMetaDataStoreItemEncoder : 
    BasicMetaDataStoreItemEncoder<WikiMetaDataStoreItem>
  {
    private string m_previousWikiId = string.Empty;
    private string m_previousWikiName = string.Empty;
    private string m_previousMappedPath = string.Empty;

    public override void Decode(IFileReader reader, WikiMetaDataStoreItem item)
    {
      base.Decode(reader, item);
      string wikiId = BasicMetaDataStoreItemEncoder<WikiMetaDataStoreItem>.ReadString(reader, ref this.m_previousWikiId);
      string wikiName = BasicMetaDataStoreItemEncoder<WikiMetaDataStoreItem>.ReadString(reader, ref this.m_previousWikiName);
      string mappedPath = BasicMetaDataStoreItemEncoder<WikiMetaDataStoreItem>.ReadString(reader, ref this.m_previousMappedPath);
      item.Initialize(wikiId, wikiName, mappedPath);
    }

    public override void Encode(IFileWriter writer, WikiMetaDataStoreItem item)
    {
      WikiMetaDataStoreItem metaDataStoreItem = item;
      base.Encode(writer, item);
      BasicMetaDataStoreItemEncoder<WikiMetaDataStoreItem>.WriteString(writer, ref this.m_previousWikiId, metaDataStoreItem.WikiId);
      BasicMetaDataStoreItemEncoder<WikiMetaDataStoreItem>.WriteString(writer, ref this.m_previousWikiName, metaDataStoreItem.WikiName);
      BasicMetaDataStoreItemEncoder<WikiMetaDataStoreItem>.WriteString(writer, ref this.m_previousMappedPath, metaDataStoreItem.MappedPath);
    }
  }
}
