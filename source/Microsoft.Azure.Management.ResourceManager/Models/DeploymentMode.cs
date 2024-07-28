// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Management.ResourceManager.Models.DeploymentMode
// Assembly: Microsoft.Azure.Management.ResourceManager, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: ABBAD935-2366-4053-A43B-1C3AE5FDB3D3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Management.ResourceManager.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace Microsoft.Azure.Management.ResourceManager.Models
{
  [JsonConverter(typeof (StringEnumConverter))]
  public enum DeploymentMode
  {
    [EnumMember(Value = "Incremental")] Incremental,
    [EnumMember(Value = "Complete")] Complete,
  }
}
