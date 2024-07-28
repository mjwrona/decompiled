// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Management.ResourceManager.Models.ChangeType
// Assembly: Microsoft.Azure.Management.ResourceManager, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: ABBAD935-2366-4053-A43B-1C3AE5FDB3D3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Management.ResourceManager.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace Microsoft.Azure.Management.ResourceManager.Models
{
  [JsonConverter(typeof (StringEnumConverter))]
  public enum ChangeType
  {
    [EnumMember(Value = "Create")] Create,
    [EnumMember(Value = "Delete")] Delete,
    [EnumMember(Value = "Ignore")] Ignore,
    [EnumMember(Value = "Deploy")] Deploy,
    [EnumMember(Value = "NoChange")] NoChange,
    [EnumMember(Value = "Modify")] Modify,
  }
}
