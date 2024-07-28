// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.SourceVersionInfo
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Common.Internal;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build2.Server
{
  [DataContract]
  public class SourceVersionInfo
  {
    private const int MaxStringLength = 300;
    private const int MaxOverallSize = 1000;

    public SourceVersionInfo(string message, string authorDisplayName, string authorAvatarUrl)
    {
      this.Message = StringUtil.TruncateToFirstLine(message, 300, out bool _);
      this.AuthorDisplayName = StringUtil.Truncate(authorDisplayName, 300, true);
      string message1 = this.Message;
      int length1 = message1 != null ? message1.Length : 0;
      string authorDisplayName1 = this.AuthorDisplayName;
      int length2 = authorDisplayName1 != null ? authorDisplayName1.Length : 0;
      if (length1 + length2 + (authorAvatarUrl != null ? authorAvatarUrl.Length : 0) > 1000)
        return;
      this.AuthorAvatarUrl = authorAvatarUrl;
    }

    public SourceVersionInfo()
    {
    }

    [DataMember(EmitDefaultValue = false, Name = "message")]
    public string Message { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "name")]
    public string AuthorDisplayName { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "avatar")]
    public string AuthorAvatarUrl { get; set; }
  }
}
