// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Provenance.SessionItem
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ItemStore.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Provenance
{
  public class SessionItem : StoredItem
  {
    public const string SessionItemType = "SessionItem";
    private const string SessionNameKey = "sessionName";
    private const string FeedIdKey = "feedId";
    private const string SessionMetadataKey = "metadata";
    private const string SourceTypeKey = "sourceType";
    private const string SessionCreatorKey = "sessionCreatorVSID";
    private const string MetadataFormatVersionKey = "metadataFormatVersion";

    public SessionItem()
      : base(nameof (SessionItem))
    {
      this.MetadataFormatVersion = 1;
    }

    public SessionItem(IItemData data)
      : base(data, nameof (SessionItem))
    {
      this.MetadataFormatVersion = 1;
    }

    public string SessionName
    {
      get => this.Data["sessionName"];
      set => this.Data["sessionName"] = value;
    }

    public Guid? SessionCreator
    {
      get
      {
        Guid result;
        return !Guid.TryParse(this.Data["sessionCreatorVSID"], out result) ? new Guid?() : new Guid?(result);
      }
      set => this.Data["sessionCreatorVSID"] = value?.ToString();
    }

    public string Feed
    {
      get => this.Data["feedId"];
      set => this.Data["feedId"] = value;
    }

    public IDictionary<string, string> SessionMetadata
    {
      get
      {
        string str = this.Data["metadata"];
        return str == null ? (IDictionary<string, string>) new Dictionary<string, string>() : JsonConvert.DeserializeObject<IDictionary<string, string>>(str);
      }
      set
      {
        ArgumentUtility.CheckForNull<IDictionary<string, string>>(value, "SessionMetadataKey");
        this.Data["metadata"] = JsonConvert.SerializeObject((object) value);
      }
    }

    public string SourceType
    {
      get => this.Data["sourceType"];
      set => this.Data["sourceType"] = value;
    }

    private int MetadataFormatVersion
    {
      get => int.Parse(this.Data["metadataFormatVersion"]);
      set => this.Data["metadataFormatVersion"] = value.ToString();
    }
  }
}
