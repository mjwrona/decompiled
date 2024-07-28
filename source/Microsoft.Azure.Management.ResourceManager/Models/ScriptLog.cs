// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Management.ResourceManager.Models.ScriptLog
// Assembly: Microsoft.Azure.Management.ResourceManager, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: ABBAD935-2366-4053-A43B-1C3AE5FDB3D3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Management.ResourceManager.dll

using Microsoft.Rest.Serialization;
using Newtonsoft.Json;

namespace Microsoft.Azure.Management.ResourceManager.Models
{
  [JsonTransformation]
  public class ScriptLog : AzureResourceBase
  {
    public ScriptLog()
    {
    }

    public ScriptLog(string id = null, string name = null, string type = null, string log = null)
      : base(id, name, type)
    {
      this.Log = log;
    }

    [JsonProperty(PropertyName = "properties.log")]
    public string Log { get; private set; }
  }
}
