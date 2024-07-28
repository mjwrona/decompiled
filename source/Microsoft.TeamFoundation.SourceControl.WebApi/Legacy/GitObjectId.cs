// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.GitObjectId
// Assembly: Microsoft.TeamFoundation.SourceControl.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 663B2C57-EC74-4E67-8BD7-7AC09B503305
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.SourceControl.WebApi.dll

using System;
using System.Runtime.Serialization;
using System.Text;

namespace Microsoft.TeamFoundation.SourceControl.WebApi.Legacy
{
  [DataContract]
  public class GitObjectId : VersionControlSecuredObject
  {
    private const int c_abbreviatedIdLengthInBytes = 4;

    public GitObjectId()
    {
    }

    public GitObjectId(string objectId)
      : this(GitHelpers.ObjectIdFromString(objectId))
    {
    }

    public GitObjectId(byte[] objectId)
    {
      this.ObjectId = objectId;
      this.Full = GitObjectId.StringFromObjectId(objectId);
      this.Short = GitObjectId.Abbreviate(objectId);
    }

    public byte[] ObjectId { get; private set; }

    [DataMember(Name = "full", EmitDefaultValue = false)]
    public string Full { get; set; }

    [DataMember(Name = "short", EmitDefaultValue = false)]
    public string Short { get; set; }

    public static string Abbreviate(byte[] commitId)
    {
      StringBuilder stringBuilder = new StringBuilder();
      if (commitId != null)
      {
        int num1 = Math.Min(commitId.Length, 4);
        for (int index = 0; index < num1; ++index)
        {
          int num2 = (int) commitId[index];
          char ch1 = (char) ((num2 >> 4 & 15) + 48);
          char ch2 = (char) ((num2 & 15) + 48);
          stringBuilder.Append(ch1 >= ':' ? (char) ((uint) ch1 + 39U) : ch1);
          stringBuilder.Append(ch2 >= ':' ? (char) ((uint) ch2 + 39U) : ch2);
        }
      }
      return stringBuilder.ToString();
    }

    private static string StringFromObjectId(byte[] oid)
    {
      if (oid == null)
        throw new ArgumentNullException(nameof (oid));
      if (oid.Length != 20)
        throw new ArgumentException("An object ID must be 20 bytes in length.", nameof (oid));
      StringBuilder stringBuilder = new StringBuilder(40);
      for (int index = 0; index < 20; ++index)
      {
        int num = (int) oid[index];
        char ch1 = (char) ((num >> 4 & 15) + 48);
        char ch2 = (char) ((num & 15) + 48);
        stringBuilder.Append(ch1 >= ':' ? (char) ((uint) ch1 + 39U) : ch1);
        stringBuilder.Append(ch2 >= ':' ? (char) ((uint) ch2 + 39U) : ch2);
      }
      return stringBuilder.ToString();
    }
  }
}
