// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemViewerConstants
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 929F0284-16B2-4277-9F4A-B615689A77D1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices.dll

using System.Runtime.InteropServices;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  [StructLayout(LayoutKind.Sequential, Size = 1)]
  internal struct WorkItemViewerConstants
  {
    public const string TemplateExtension = ".xsl";
    public const string TransformDir = "v1.0\\Transforms";
    public const string DefaultXslFile = "WorkItem.xsl";
    public const string IdQueryParam = "artifactMoniker";
    public const string RevQueryParam = "rev";
  }
}
