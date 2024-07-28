// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Core.WebApi.TaggingWebApiConstants
// Assembly: Microsoft.TeamFoundation.Core.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3A82A796-05AB-42F0-97D0-CB8516E08665
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Core.WebApi.dll

using System;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.Core.WebApi
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class TaggingWebApiConstants
  {
    public const string TaggingAreaId = "1F131D7F-CFBB-4EC9-B358-FB4E8341CE59";
    public const string TaggingAreaName = "Tagging";
    public const string TagsResource = "tags";
    public const string ScopesResource = "scopes";
    public const string TagIdUrlPlaceholder = "tagId";
    public const string ScopeIdUrlPlaceholder = "scopeId";
    public static readonly Guid TagsLocationId = new Guid("CF333E53-8825-4D68-8877-6EEB6BF98E2D");
  }
}
