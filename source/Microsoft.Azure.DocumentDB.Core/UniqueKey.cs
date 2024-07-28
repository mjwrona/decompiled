// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.UniqueKey
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Newtonsoft.Json;
using System.Collections.ObjectModel;

namespace Microsoft.Azure.Documents
{
  public sealed class UniqueKey : JsonSerializable
  {
    private Collection<string> paths;

    [JsonProperty(PropertyName = "paths")]
    public Collection<string> Paths
    {
      get
      {
        if (this.paths == null)
        {
          this.paths = this.GetValue<Collection<string>>("paths");
          if (this.paths == null)
            this.paths = new Collection<string>();
        }
        return this.paths;
      }
      set
      {
        this.paths = value;
        this.SetValue("paths", (object) value);
      }
    }

    internal override void Validate()
    {
      base.Validate();
      this.GetValue<Collection<string>>("paths");
    }

    internal override void OnSave()
    {
      if (this.paths == null)
        return;
      this.SetValue("paths", (object) this.paths);
    }
  }
}
