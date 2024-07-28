// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.Entities.UpdateTypeDictionary
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Stores.TreeStore;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Search.Common.Entities
{
  [CollectionDataContract(Name = "UpdateTypeDictionary", Namespace = "", ItemName = "Metadata")]
  [Serializable]
  public class UpdateTypeDictionary : Dictionary<MetaDataStoreUpdateType, List<BranchInfo>>
  {
    public UpdateTypeDictionary()
    {
    }

    protected UpdateTypeDictionary(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
      this.ContentSize = (long) info.GetValue(nameof (ContentSize), typeof (long));
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      base.GetObjectData(info, context);
      info.AddValue("ContentSize", (object) this.ContentSize, typeof (long));
    }

    public long ContentSize { get; set; }

    public override bool Equals(object obj)
    {
      if (!(obj is UpdateTypeDictionary updateTypeDictionary) || this.Count != updateTypeDictionary.Count)
        return false;
      foreach (MetaDataStoreUpdateType key in this.Keys)
      {
        if (!updateTypeDictionary.ContainsKey(key) || this[key].Count != updateTypeDictionary[key].Count)
          return false;
        foreach (BranchInfo branchInfo1 in this[key])
        {
          BranchInfo branchInfo = branchInfo1;
          if (!updateTypeDictionary[key].Exists((Predicate<BranchInfo>) (x => x.BranchName == branchInfo.BranchName)))
            return false;
        }
      }
      return true;
    }

    public override int GetHashCode() => base.GetHashCode();
  }
}
