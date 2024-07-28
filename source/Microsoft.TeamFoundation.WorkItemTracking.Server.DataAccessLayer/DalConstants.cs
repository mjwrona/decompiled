// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DalConstants
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using System.Runtime.InteropServices;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class DalConstants
  {
    public const int IdentityVsidComponentVersion = 42;

    private DalConstants()
    {
    }

    [StructLayout(LayoutKind.Sequential, Size = 1)]
    internal struct SyncBisGroupsAndUsers
    {
      [StructLayout(LayoutKind.Sequential, Size = 1)]
      internal struct StoredProcs
      {
        public const string SyncIdentity = "SyncWithBIS_SyncIdentity";
        public const string UpdateSequenceId1 = "UpdateSequenceId1";
      }
    }

    [StructLayout(LayoutKind.Sequential, Size = 1)]
    internal struct InverseQueries
    {
      [StructLayout(LayoutKind.Sequential, Size = 1)]
      internal struct StoredProcs
      {
        public const string GetWorkItemDetails = "GetWorkItemDetail";
        public const string GetWorkItemDetailsByReferenceUri = "GetWorkItemDetailByReferenceUri";
        public const string GetWorkItemDetailsByLinkFilter = "GetWorkItemDetailsByLinkFilter";
      }
    }
  }
}
