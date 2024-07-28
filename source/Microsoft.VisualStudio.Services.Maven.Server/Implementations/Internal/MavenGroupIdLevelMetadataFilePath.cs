// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Implementations.Internal.MavenGroupIdLevelMetadataFilePath
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.VisualStudio.Services.Maven.Server.Implementations.Internal
{
  public class MavenGroupIdLevelMetadataFilePath : 
    MavenMetadataFilePath,
    IMavenGroupIdLevelMetadataFilePath,
    IMavenMetadataFilePath,
    IMavenFilePath
  {
    public string GroupId { get; }

    public MavenGroupIdLevelMetadataFilePath(string groupId, string fileName)
      : base(fileName)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(groupId, nameof (groupId));
      this.GroupId = groupId;
    }

    public override string FullName
    {
      get
      {
        string originalPath = this.OriginalPath;
        if (originalPath != null)
          return originalPath;
        return this.BuildPath(this.GroupId, this.FileName);
      }
    }
  }
}
