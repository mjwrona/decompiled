// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.TeamFoundationContextExtensions
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.Client
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class TeamFoundationContextExtensions
  {
    public static string DomainName(this ITeamFoundationContext context) => !context.HasCollection ? (string) null : context.TeamProjectCollection.Name;

    public static string DomainUri(this ITeamFoundationContext context) => !context.HasCollection ? (string) null : context.TeamProjectCollection.Uri.AbsoluteUri;

    public static string ProjectUri(this ITeamFoundationContext context) => !context.HasTeamProject ? (string) null : context.TeamProjectUri.AbsoluteUri;

    public static Guid TeamProjectGuid(this ITeamFoundationContext context) => context.HasTeamProject ? new Guid(LinkingUtilities.DecodeUri(context.TeamProjectUri.AbsoluteUri).ToolSpecificId) : Guid.Empty;
  }
}
