// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.Entities.ContentHashBranchListDictionary
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Search.Common.Entities
{
  [CollectionDataContract(Name = "ContentHashBranchListDictionary", Namespace = "", ItemName = "ContentHashBranchListMapping")]
  [Serializable]
  public class ContentHashBranchListDictionary : Dictionary<string, Branches>
  {
    public ContentHashBranchListDictionary()
    {
    }

    protected ContentHashBranchListDictionary(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }

    public override bool Equals(object obj)
    {
      if (!(obj is ContentHashBranchListDictionary branchListDictionary) || this.Count != branchListDictionary.Count)
        return false;
      foreach (string key in this.Keys)
      {
        if (!branchListDictionary.ContainsKey(key) || !this[key].Equals((object) branchListDictionary[key]))
          return false;
      }
      return true;
    }

    public override int GetHashCode() => base.GetHashCode();
  }
}
