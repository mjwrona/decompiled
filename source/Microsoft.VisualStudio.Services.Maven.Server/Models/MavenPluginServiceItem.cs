// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Models.MavenPluginServiceItem
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ItemStore.Common;
using Microsoft.VisualStudio.Services.Maven.Server.Models.Xml;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Maven.Server.Models
{
  public class MavenPluginServiceItem : StoredItem
  {
    public MavenPluginServiceItem()
      : base(nameof (MavenPluginServiceItem))
    {
    }

    public MavenPluginServiceItem(IItemData data)
      : base(data, nameof (MavenPluginServiceItem))
    {
    }

    public MavenPluginServiceItem(IEnumerable<MavenPluginItem> plugins)
      : this()
    {
      ArgumentUtility.CheckForNull<IEnumerable<MavenPluginItem>>(plugins, nameof (plugins));
      this.Plugins = plugins.ToList<MavenPluginItem>();
    }

    public List<MavenPluginItem> Plugins
    {
      get
      {
        string str = this.Data[nameof (Plugins)];
        return !string.IsNullOrWhiteSpace(str) ? JsonConvert.DeserializeObject<List<MavenPluginItem>>(str) : (List<MavenPluginItem>) null;
      }
      set
      {
        if (value == null)
          return;
        this.Data[nameof (Plugins)] = JsonConvert.SerializeObject((object) value);
      }
    }
  }
}
