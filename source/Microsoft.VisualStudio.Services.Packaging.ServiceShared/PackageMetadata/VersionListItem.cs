// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata.VersionListItem
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ItemStore.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata
{
  public class VersionListItem : StoredItem
  {
    private const string PublishedVersionsDetailsKey = "PublishedVersionsDetails";
    private const string PublishedVersionsKey = "PublishedVersions";
    private const string VersionListItemType = "VersionList";
    private const string MetadataFormatVersionKey = "MetadataFormatVersion";

    public VersionListItem()
      : base("VersionList")
    {
      this.MetadataFormatVersion = 1;
    }

    public VersionListItem(IItemData data)
      : base(data, "VersionList")
    {
      this.MetadataFormatVersion = 1;
    }

    public IEnumerable<string> PublishedVersions
    {
      get
      {
        string s = this.Data[nameof (PublishedVersions)];
        if (s == null)
          return Enumerable.Empty<string>();
        try
        {
          s = Encoding.UTF8.GetString(CompressionHelper.InflateByteArray(System.Convert.FromBase64String(s)));
        }
        catch (FormatException ex)
        {
        }
        catch (InvalidDataException ex)
        {
        }
        return JsonConvert.DeserializeObject<IEnumerable<string>>(s);
      }
      set
      {
        ArgumentUtility.CheckForNull<IEnumerable<string>>(value, nameof (PublishedVersions));
        this.Data[nameof (PublishedVersions)] = System.Convert.ToBase64String(CompressionHelper.DeflateByteArray(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject((object) value))));
      }
    }

    public IDictionary<string, VersionDetails> PublishedVersionDetails
    {
      get
      {
        string str = this.Data["PublishedVersionsDetails"];
        return str == null ? (IDictionary<string, VersionDetails>) new Dictionary<string, VersionDetails>() : JsonConvert.DeserializeObject<IDictionary<string, VersionDetails>>(str);
      }
      set
      {
        ArgumentUtility.CheckForNull<IDictionary<string, VersionDetails>>(value, nameof (PublishedVersionDetails));
        this.Data["PublishedVersionsDetails"] = JsonConvert.SerializeObject((object) value);
      }
    }

    private int MetadataFormatVersion
    {
      get => int.Parse(this.Data[nameof (MetadataFormatVersion)]);
      set => this.Data[nameof (MetadataFormatVersion)] = value.ToString();
    }
  }
}
