// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebApi.GitConflictJsonConverter
// Assembly: Microsoft.TeamFoundation.SourceControl.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 663B2C57-EC74-4E67-8BD7-7AC09B503305
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.SourceControl.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json.Linq;
using System;

namespace Microsoft.TeamFoundation.SourceControl.WebApi
{
  public class GitConflictJsonConverter : VssJsonCreationConverter<GitConflict>
  {
    protected override GitConflict Create(Type objectType, JObject jsonObject)
    {
      JToken jtoken = jsonObject["conflictType"];
      switch (jtoken != null ? jtoken.ToObject<GitConflictType>() : GitConflictType.None)
      {
        case GitConflictType.AddAdd:
          return (GitConflict) new GitConflictAddAdd();
        case GitConflictType.AddRename:
          return (GitConflict) new GitConflictAddRename();
        case GitConflictType.DeleteEdit:
          return (GitConflict) new GitConflictDeleteEdit();
        case GitConflictType.DeleteRename:
          return (GitConflict) new GitConflictDeleteRename();
        case GitConflictType.DirectoryFile:
          return (GitConflict) new GitConflictDirectoryFile();
        case GitConflictType.EditDelete:
          return (GitConflict) new GitConflictEditDelete();
        case GitConflictType.EditEdit:
          return (GitConflict) new GitConflictEditEdit();
        case GitConflictType.FileDirectory:
          return (GitConflict) new GitConflictFileDirectory();
        case GitConflictType.Rename1to2:
          return (GitConflict) new GitConflictRename1to2();
        case GitConflictType.Rename2to1:
          return (GitConflict) new GitConflictRename2to1();
        case GitConflictType.RenameAdd:
          return (GitConflict) new GitConflictRenameAdd();
        case GitConflictType.RenameDelete:
          return (GitConflict) new GitConflictRenameDelete();
        case GitConflictType.RenameRename:
          return (GitConflict) new GitConflictRenameRename();
        default:
          return new GitConflict();
      }
    }
  }
}
