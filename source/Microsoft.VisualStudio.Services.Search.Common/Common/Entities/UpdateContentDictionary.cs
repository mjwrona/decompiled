// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.Entities.UpdateContentDictionary
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Search.Common.Entities
{
  [CollectionDataContract(Name = "UpdateContentDictionary", Namespace = "", ItemName = "UpdateContentMetadata")]
  [Serializable]
  public class UpdateContentDictionary : Dictionary<string, UpdateTypeDictionary>
  {
    public UpdateContentDictionary()
    {
    }

    protected UpdateContentDictionary(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }

    public override bool Equals(object obj)
    {
      if (!(obj is UpdateContentDictionary contentDictionary) || this.Count != contentDictionary.Count)
        return false;
      foreach (string key in this.Keys)
      {
        if (!contentDictionary.ContainsKey(key) || !this[key].Equals((object) contentDictionary[key]))
          return false;
      }
      return true;
    }

    public override int GetHashCode() => base.GetHashCode();
  }
}
