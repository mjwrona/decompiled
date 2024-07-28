// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.WebApi.Types.API.PyPiBatchOperationType
// Assembly: Microsoft.VisualStudio.Services.PyPi.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 17E1C323-94FE-4FF1-903A-ED51BA3159D2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.PyPi.WebApi.Types.API
{
  [DataContract]
  [ClientIncludeModel]
  public enum PyPiBatchOperationType
  {
    [DataMember] Promote,
    [DataMember] Delete,
    [DataMember] PermanentDelete,
    [DataMember] RestoreToFeed,
  }
}
