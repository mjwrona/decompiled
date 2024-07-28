// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Web.Models.VSSearchResult
// Assembly: Microsoft.VisualStudio.Services.Gallery.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 17D36576-2EF3-4ABC-94BA-AF7891D15A3A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Web.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Gallery.Web.Models
{
  public class VSSearchResult
  {
    public bool AllowDiscussions;
    public VSAuthorInfo Author;
    public int CostCategory;
    public DateTime DatePublished;
    public DateTime DateUpdatedl;
    public int Downloads;
    public List<VSFileInfo> Files;
    public bool IsOfficialContribution;
    public bool IsPublished;
    public bool IsReferralLink;
    public string Link;
    public string Name;
    public int PrimaryLanguage;
    public int Raters;
    public double Rating;
    public string Summary;
    public List<VSTag> TagGroups;
    public string Thumbnail;
    public string Title;
    public string VsixId;
  }
}
