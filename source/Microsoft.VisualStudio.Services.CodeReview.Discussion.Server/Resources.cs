// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Discussion.Server.Resources
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Discussion.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FFC5EC6C-1B94-4299-8BA9-787264C21330
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.CodeReview.Discussion.Server.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.CodeReview.Discussion.Server
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  public class Resources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal Resources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static ResourceManager ResourceManager
    {
      get
      {
        if (Microsoft.VisualStudio.Services.CodeReview.Discussion.Server.Resources.resourceMan == null)
          Microsoft.VisualStudio.Services.CodeReview.Discussion.Server.Resources.resourceMan = new ResourceManager("Microsoft.VisualStudio.Services.CodeReview.Discussion.Server.Resources", typeof (Microsoft.VisualStudio.Services.CodeReview.Discussion.Server.Resources).Assembly);
        return Microsoft.VisualStudio.Services.CodeReview.Discussion.Server.Resources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static CultureInfo Culture
    {
      get => Microsoft.VisualStudio.Services.CodeReview.Discussion.Server.Resources.resourceCulture;
      set => Microsoft.VisualStudio.Services.CodeReview.Discussion.Server.Resources.resourceCulture = value;
    }

    public static string ArgumentInvalidError => Microsoft.VisualStudio.Services.CodeReview.Discussion.Server.Resources.ResourceManager.GetString(nameof (ArgumentInvalidError), Microsoft.VisualStudio.Services.CodeReview.Discussion.Server.Resources.resourceCulture);

    public static string ArgumentNullError => Microsoft.VisualStudio.Services.CodeReview.Discussion.Server.Resources.ResourceManager.GetString(nameof (ArgumentNullError), Microsoft.VisualStudio.Services.CodeReview.Discussion.Server.Resources.resourceCulture);

    public static string ArtifactUriParseError => Microsoft.VisualStudio.Services.CodeReview.Discussion.Server.Resources.ResourceManager.GetString(nameof (ArtifactUriParseError), Microsoft.VisualStudio.Services.CodeReview.Discussion.Server.Resources.resourceCulture);

    public static string CommentAuthorMustBeRequester => Microsoft.VisualStudio.Services.CodeReview.Discussion.Server.Resources.ResourceManager.GetString(nameof (CommentAuthorMustBeRequester), Microsoft.VisualStudio.Services.CodeReview.Discussion.Server.Resources.resourceCulture);

    public static string CommentAuthorNotFound => Microsoft.VisualStudio.Services.CodeReview.Discussion.Server.Resources.ResourceManager.GetString(nameof (CommentAuthorNotFound), Microsoft.VisualStudio.Services.CodeReview.Discussion.Server.Resources.resourceCulture);

    public static string CommentCanNotBeUpdated => Microsoft.VisualStudio.Services.CodeReview.Discussion.Server.Resources.ResourceManager.GetString(nameof (CommentCanNotBeUpdated), Microsoft.VisualStudio.Services.CodeReview.Discussion.Server.Resources.resourceCulture);

    public static string CommentNotFound => Microsoft.VisualStudio.Services.CodeReview.Discussion.Server.Resources.ResourceManager.GetString(nameof (CommentNotFound), Microsoft.VisualStudio.Services.CodeReview.Discussion.Server.Resources.resourceCulture);

    public static string DiscussionCommentWithTooMuchContent => Microsoft.VisualStudio.Services.CodeReview.Discussion.Server.Resources.ResourceManager.GetString(nameof (DiscussionCommentWithTooMuchContent), Microsoft.VisualStudio.Services.CodeReview.Discussion.Server.Resources.resourceCulture);

    public static string DiscussionsAndCommentsMismatch => Microsoft.VisualStudio.Services.CodeReview.Discussion.Server.Resources.ResourceManager.GetString(nameof (DiscussionsAndCommentsMismatch), Microsoft.VisualStudio.Services.CodeReview.Discussion.Server.Resources.resourceCulture);

    public static string DiscussionThreadNotFound => Microsoft.VisualStudio.Services.CodeReview.Discussion.Server.Resources.ResourceManager.GetString(nameof (DiscussionThreadNotFound), Microsoft.VisualStudio.Services.CodeReview.Discussion.Server.Resources.resourceCulture);

    public static string FieldNameCommentAgeInMinutes => Microsoft.VisualStudio.Services.CodeReview.Discussion.Server.Resources.ResourceManager.GetString(nameof (FieldNameCommentAgeInMinutes), Microsoft.VisualStudio.Services.CodeReview.Discussion.Server.Resources.resourceCulture);

    public static string FieldNameCommentDeleted => Microsoft.VisualStudio.Services.CodeReview.Discussion.Server.Resources.ResourceManager.GetString(nameof (FieldNameCommentDeleted), Microsoft.VisualStudio.Services.CodeReview.Discussion.Server.Resources.resourceCulture);

    public static string FieldNameCommenter => Microsoft.VisualStudio.Services.CodeReview.Discussion.Server.Resources.ResourceManager.GetString(nameof (FieldNameCommenter), Microsoft.VisualStudio.Services.CodeReview.Discussion.Server.Resources.resourceCulture);

    public static string FieldNameCommenters => Microsoft.VisualStudio.Services.CodeReview.Discussion.Server.Resources.ResourceManager.GetString(nameof (FieldNameCommenters), Microsoft.VisualStudio.Services.CodeReview.Discussion.Server.Resources.resourceCulture);

    public static string FieldNameCRAuthor => Microsoft.VisualStudio.Services.CodeReview.Discussion.Server.Resources.ResourceManager.GetString(nameof (FieldNameCRAuthor), Microsoft.VisualStudio.Services.CodeReview.Discussion.Server.Resources.resourceCulture);

    public static string FieldValueDeleted => Microsoft.VisualStudio.Services.CodeReview.Discussion.Server.Resources.ResourceManager.GetString(nameof (FieldValueDeleted), Microsoft.VisualStudio.Services.CodeReview.Discussion.Server.Resources.resourceCulture);

    public static string FieldValueNotDeleted => Microsoft.VisualStudio.Services.CodeReview.Discussion.Server.Resources.ResourceManager.GetString(nameof (FieldValueNotDeleted), Microsoft.VisualStudio.Services.CodeReview.Discussion.Server.Resources.resourceCulture);

    public static string InvalidNewDiscussionId => Microsoft.VisualStudio.Services.CodeReview.Discussion.Server.Resources.ResourceManager.GetString(nameof (InvalidNewDiscussionId), Microsoft.VisualStudio.Services.CodeReview.Discussion.Server.Resources.resourceCulture);

    public static string InvalidVSOIdentitiesAreFound => Microsoft.VisualStudio.Services.CodeReview.Discussion.Server.Resources.ResourceManager.GetString(nameof (InvalidVSOIdentitiesAreFound), Microsoft.VisualStudio.Services.CodeReview.Discussion.Server.Resources.resourceCulture);

    public static string MaxDiscussionThreadCountException => Microsoft.VisualStudio.Services.CodeReview.Discussion.Server.Resources.ResourceManager.GetString(nameof (MaxDiscussionThreadCountException), Microsoft.VisualStudio.Services.CodeReview.Discussion.Server.Resources.resourceCulture);

    public static string NewDiscussionPOSTError => Microsoft.VisualStudio.Services.CodeReview.Discussion.Server.Resources.ResourceManager.GetString(nameof (NewDiscussionPOSTError), Microsoft.VisualStudio.Services.CodeReview.Discussion.Server.Resources.resourceCulture);

    public static string OpenIn => Microsoft.VisualStudio.Services.CodeReview.Discussion.Server.Resources.ResourceManager.GetString(nameof (OpenIn), Microsoft.VisualStudio.Services.CodeReview.Discussion.Server.Resources.resourceCulture);

    public static string UserIdentitiesMustBeUnique => Microsoft.VisualStudio.Services.CodeReview.Discussion.Server.Resources.ResourceManager.GetString(nameof (UserIdentitiesMustBeUnique), Microsoft.VisualStudio.Services.CodeReview.Discussion.Server.Resources.resourceCulture);

    public static string UserIdMustBeAGuid => Microsoft.VisualStudio.Services.CodeReview.Discussion.Server.Resources.ResourceManager.GetString(nameof (UserIdMustBeAGuid), Microsoft.VisualStudio.Services.CodeReview.Discussion.Server.Resources.resourceCulture);

    public static string UserNotFound => Microsoft.VisualStudio.Services.CodeReview.Discussion.Server.Resources.ResourceManager.GetString(nameof (UserNotFound), Microsoft.VisualStudio.Services.CodeReview.Discussion.Server.Resources.resourceCulture);
  }
}
