// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.FormExtensions.PageExtensions
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout;
using System;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.FormExtensions
{
  public static class PageExtensions
  {
    public static Section SectionFromIndex(this Page page, int index)
    {
      string sectionName = PageExtensions.SectionNameFromIndex(index);
      return page.Children.FirstOrDefault<Section>((Func<Section, bool>) (s => s.Id == sectionName));
    }

    public static string SectionNameFromIndex(int index) => string.Format("Section{0}", (object) (index + 1));
  }
}
