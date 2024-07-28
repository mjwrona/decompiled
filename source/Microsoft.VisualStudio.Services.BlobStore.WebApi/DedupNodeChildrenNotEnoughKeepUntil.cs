// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.WebApi.DedupNodeChildrenNotEnoughKeepUntil
// Assembly: Microsoft.VisualStudio.Services.BlobStore.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4500AC57-FBCC-4F18-B11F-F661A75E4A46
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.WebApi.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.BlobStore.WebApi
{
  [JsonObject(MemberSerialization.OptIn)]
  [Serializable]
  public struct DedupNodeChildrenNotEnoughKeepUntil
  {
    [JsonProperty(PropertyName = "InsufficientKeepUntil", Required = Required.Always)]
    public readonly DedupIdentifier[] InsufficientKeepUntil;
    [JsonProperty(PropertyName = "Receipts", Required = Required.Always)]
    private readonly Dictionary<string, KeepUntilReceipt> receipts;

    public Dictionary<DedupIdentifier, KeepUntilReceipt> Receipts => this.receipts.ToDictionary<KeyValuePair<string, KeepUntilReceipt>, DedupIdentifier, KeepUntilReceipt>((Func<KeyValuePair<string, KeepUntilReceipt>, DedupIdentifier>) (kvp => DedupIdentifier.Create(kvp.Key)), (Func<KeyValuePair<string, KeepUntilReceipt>, KeepUntilReceipt>) (kvp => kvp.Value));

    public DedupNodeChildrenNotEnoughKeepUntil(
      DedupIdentifier[] insufficientKeepUntil,
      Dictionary<DedupIdentifier, KeepUntilReceipt> receipts)
    {
      this.InsufficientKeepUntil = insufficientKeepUntil;
      this.receipts = receipts.ToDictionary<KeyValuePair<DedupIdentifier, KeepUntilReceipt>, string, KeepUntilReceipt>((Func<KeyValuePair<DedupIdentifier, KeepUntilReceipt>, string>) (kvp => kvp.Key.ValueString), (Func<KeyValuePair<DedupIdentifier, KeepUntilReceipt>, KeepUntilReceipt>) (kvp => kvp.Value));
    }
  }
}
