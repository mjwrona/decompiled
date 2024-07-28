// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Discussion.Server.CustomSqlError
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Discussion.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FFC5EC6C-1B94-4299-8BA9-787264C21330
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.CodeReview.Discussion.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.ComponentModel;

namespace Microsoft.VisualStudio.Services.CodeReview.Discussion.Server
{
  [ClientEditorBrowsable(EditorBrowsableState.Never)]
  public static class CustomSqlError
  {
    public const int SqlServerDefaultUserMessage = 50000;
    public static readonly int GenericError = 1050001;
    public const int DatabaseSaveFailure = 1050002;
    public const int DatabaseQueryFailure = 1050003;
    public const int DiscussionNotFound = 1050004;
    public const int CommentNotFound = 1050005;
    public const int CommentCannotBeUpdated = 1050006;
  }
}
