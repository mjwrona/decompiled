// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.TfsGitRef
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System;

namespace Microsoft.TeamFoundation.Git.Server
{
  public class TfsGitRef
  {
    public TfsGitRef(
      string name,
      Sha1Id objectId,
      bool isDefaultBranch = false,
      Guid? isLockedById = null,
      Guid? creatorId = null)
    {
      this.Name = name;
      this.ObjectId = objectId;
      this.IsDefaultBranch = isDefaultBranch;
      this.IsLockedById = isLockedById;
      this.CreatorId = creatorId;
    }

    public string Name { get; }

    public Sha1Id ObjectId { get; }

    public bool IsDefaultBranch { get; }

    public Guid? IsLockedById { get; }

    public Guid? CreatorId { get; }

    public override string ToString()
    {
      string[] strArray = new string[11]
      {
        base.ToString(),
        " Ref: ",
        this.Name,
        " ObjectId: ",
        this.ObjectId.ToString(),
        " IsDefaultBranch: ",
        this.IsDefaultBranch.ToString(),
        " IsLockedById: ",
        null,
        null,
        null
      };
      Guid? nullable;
      string str1;
      if (!this.IsLockedById.HasValue)
      {
        str1 = "null";
      }
      else
      {
        nullable = this.IsLockedById;
        str1 = nullable.ToString();
      }
      strArray[8] = str1;
      strArray[9] = " CreatorId: ";
      nullable = this.CreatorId;
      string str2;
      if (!nullable.HasValue)
      {
        str2 = "null";
      }
      else
      {
        nullable = this.CreatorId;
        str2 = nullable.ToString();
      }
      strArray[10] = str2;
      return string.Concat(strArray);
    }
  }
}
