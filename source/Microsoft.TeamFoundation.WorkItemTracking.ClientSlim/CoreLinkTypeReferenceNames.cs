// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Client.CoreLinkTypeReferenceNames
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.ClientSlim, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 946B9068-2299-475E-A3F8-BCA3E57420E0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.ClientSlim.dll

using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Client
{
  [GenerateAllConstants(null)]
  public static class CoreLinkTypeReferenceNames
  {
    public static readonly string Related = "System.LinkTypes.Related";
    public static readonly string Hierarchy = "System.LinkTypes.Hierarchy";
    public static readonly string Dependency = "System.LinkTypes.Dependency";
    public static readonly string Duplicate = "System.LinkTypes.Duplicate";
    private static string[] m_all = new string[4]
    {
      CoreLinkTypeReferenceNames.Related,
      CoreLinkTypeReferenceNames.Hierarchy,
      CoreLinkTypeReferenceNames.Dependency,
      CoreLinkTypeReferenceNames.Duplicate
    };

    public static IEnumerable<string> All => (IEnumerable<string>) CoreLinkTypeReferenceNames.m_all;
  }
}
