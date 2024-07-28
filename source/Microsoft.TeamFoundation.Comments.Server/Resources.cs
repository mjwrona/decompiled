// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Comments.Server.Resources
// Assembly: Microsoft.TeamFoundation.Comments.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CBA40CC5-9694-4582-97B5-1660FA9D4307
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Comments.Server.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.TeamFoundation.Comments.Server
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  internal class Resources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal Resources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static ResourceManager ResourceManager
    {
      get
      {
        if (Microsoft.TeamFoundation.Comments.Server.Resources.resourceMan == null)
          Microsoft.TeamFoundation.Comments.Server.Resources.resourceMan = new ResourceManager("Microsoft.TeamFoundation.Comments.Server.Resources", typeof (Microsoft.TeamFoundation.Comments.Server.Resources).Assembly);
        return Microsoft.TeamFoundation.Comments.Server.Resources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get => Microsoft.TeamFoundation.Comments.Server.Resources.resourceCulture;
      set => Microsoft.TeamFoundation.Comments.Server.Resources.resourceCulture = value;
    }

    internal static string CommentAttachmentNotFoundMessage => Microsoft.TeamFoundation.Comments.Server.Resources.ResourceManager.GetString(nameof (CommentAttachmentNotFoundMessage), Microsoft.TeamFoundation.Comments.Server.Resources.resourceCulture);

    internal static string CommentDeleteException => Microsoft.TeamFoundation.Comments.Server.Resources.ResourceManager.GetString(nameof (CommentDeleteException), Microsoft.TeamFoundation.Comments.Server.Resources.resourceCulture);

    internal static string CommentNotFoundMessage => Microsoft.TeamFoundation.Comments.Server.Resources.ResourceManager.GetString(nameof (CommentNotFoundMessage), Microsoft.TeamFoundation.Comments.Server.Resources.resourceCulture);

    internal static string CommentProviderNotRegisteredMessage => Microsoft.TeamFoundation.Comments.Server.Resources.ResourceManager.GetString(nameof (CommentProviderNotRegisteredMessage), Microsoft.TeamFoundation.Comments.Server.Resources.resourceCulture);

    internal static string CommentThreadingNotSupportedException => Microsoft.TeamFoundation.Comments.Server.Resources.ResourceManager.GetString(nameof (CommentThreadingNotSupportedException), Microsoft.TeamFoundation.Comments.Server.Resources.resourceCulture);

    internal static string CommentUpdateException => Microsoft.TeamFoundation.Comments.Server.Resources.ResourceManager.GetString(nameof (CommentUpdateException), Microsoft.TeamFoundation.Comments.Server.Resources.resourceCulture);

    internal static string CommentVersionNotFoundMessage => Microsoft.TeamFoundation.Comments.Server.Resources.ResourceManager.GetString(nameof (CommentVersionNotFoundMessage), Microsoft.TeamFoundation.Comments.Server.Resources.resourceCulture);

    internal static string InvalidArtifactIdFormatMessage => Microsoft.TeamFoundation.Comments.Server.Resources.ResourceManager.GetString(nameof (InvalidArtifactIdFormatMessage), Microsoft.TeamFoundation.Comments.Server.Resources.resourceCulture);

    internal static string InvalidParentSpecified => Microsoft.TeamFoundation.Comments.Server.Resources.ResourceManager.GetString(nameof (InvalidParentSpecified), Microsoft.TeamFoundation.Comments.Server.Resources.resourceCulture);

    internal static string InvalidProjectId => Microsoft.TeamFoundation.Comments.Server.Resources.ResourceManager.GetString(nameof (InvalidProjectId), Microsoft.TeamFoundation.Comments.Server.Resources.resourceCulture);
  }
}
