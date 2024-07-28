// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.AzureSdk.HttpMessageWrapper
// Assembly: Microsoft.TeamFoundation.DistributedTask.AzureSdk, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 84D2B88A-971A-412D-9BB4-BAAD1599A5AE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.DistributedTask.AzureSdk.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.AzureSdk
{
  [DataContract]
  public abstract class HttpMessageWrapper
  {
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Content { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public IDictionary<string, IEnumerable<string>> Headers { get; } = (IDictionary<string, IEnumerable<string>>) new Dictionary<string, IEnumerable<string>>();
  }
}
