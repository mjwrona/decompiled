// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.CoreField
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  public static class CoreField
  {
    public const int Id = -3;
    public const int Rev = 8;
    public const int Title = 1;
    public const int Description = 52;
    public const int WorkItemType = 25;
    public const int TeamProject = -42;
    public const int State = 2;
    public const int Reason = 22;
    public const int CreatedBy = 33;
    public const int AssignedTo = 24;
    public const int ChangedBy = 9;
    public const int ChangedDate = -4;
    public const int CreatedDate = 32;
    public const int RevisedDate = -5;
    public const int AuthorizedDate = 3;
    public const int AuthorizedAs = -1;
    public const int History = 54;
    public const int AreaPath = -7;
    public const int AreaId = -2;
    public const int IterationPath = -105;
    public const int IterationId = -104;
    public const int NodeName = -12;
    public const int RelatedLinkCount = 75;
    public const int RemoteLinkCount = -34;
    public const int HyperLinkCount = -32;
    public const int AttachedFileCount = -31;
    public const int ExternalLinkCount = -57;
    public const int CommentCount = -33;
    public const int LinkType = 100;
    public const int Parent = -35;
    public const int Watermark = 7;
    private static int[] m_all = new int[31]
    {
      3,
      -2,
      -7,
      -57,
      24,
      -31,
      -1,
      9,
      -4,
      33,
      32,
      52,
      -33,
      54,
      -32,
      -3,
      -104,
      -105,
      100,
      -12,
      -35,
      22,
      75,
      -34,
      8,
      -5,
      2,
      -42,
      1,
      7,
      25
    };

    public static IEnumerable<int> All => (IEnumerable<int>) CoreField.m_all;
  }
}
