// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Devops.Tags.Server.TagDefinitionComparer
// Assembly: Microsoft.Azure.Devops.Tags.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10FDC8E3-D1DB-4668-B2F2-04DAA10A7143
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Devops.Tags.Server.dll

using System.Collections.Generic;

namespace Microsoft.Azure.Devops.Tags.Server
{
  public class TagDefinitionComparer : IEqualityComparer<TagDefinition>
  {
    public bool Equals(TagDefinition tag1, TagDefinition tag2)
    {
      if (tag1 == null && tag2 == null)
        return true;
      return tag1 != null && tag2 != null && tag1.TagId == tag2.TagId;
    }

    public int GetHashCode(TagDefinition tag) => tag == null ? 0 : tag.TagId.GetHashCode();
  }
}
