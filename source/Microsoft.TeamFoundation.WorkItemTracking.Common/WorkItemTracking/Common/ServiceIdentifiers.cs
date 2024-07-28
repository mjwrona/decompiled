// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Common.ServiceIdentifiers
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Microsoft.TeamFoundation.WorkItemTracking.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  [StructLayout(LayoutKind.Sequential, Size = 1)]
  public struct ServiceIdentifiers
  {
    public static readonly Guid WorkItem = new Guid("179b6a0b-a5be-43fc-879f-cfa2a43cd3d8");
    public static readonly Guid WorkItem2 = new Guid("7EDE8C17-7965-4AEE-874D-ED9B25276DEB");
    public static readonly Guid WorkItem3 = new Guid("CA87FA49-58C9-4089-8535-1299FA60EEBC");
    public static readonly Guid WorkItem4 = new Guid("B73B758B-C1CD-4043-B7AF-AE70E505916F");
    public static readonly Guid WorkItem5 = new Guid("4C5EB288-4C0A-4888-BB1B-742A4B5B706E");
    public static readonly Guid WorkItem6 = new Guid("A4ED4FBF-EB4A-467A-9DE6-13599C3F81DE");
    public static readonly Guid WorkItem7 = new Guid("BC9B27AA-EDA2-4FC9-AC3B-644BB7999C19");
    public static readonly Guid WorkItem8 = new Guid("1cc519db-7813-49eb-8db5-04003dd776e8");
    public static readonly Guid WorkItemAttachmentHandler = new Guid("F04F5BFC-FF3D-4EA2-BFC4-6FA485AD594E");
    public static readonly Guid ConfigurationServerUrl = new Guid("1e9d1b48-775a-49c8-af8f-d41a06e0cdb0");
  }
}
