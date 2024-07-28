// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.Model.SourceWorkflow
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure;

namespace Microsoft.VisualStudio.Services.Analytics.Model
{
  public enum SourceWorkflow : byte
  {
    [LocalizedDisplayName("ENUM_TYPE_SOURCE_WORK_FLOW_BUILD", false)] Build = 1,
    [LocalizedDisplayName("ENUM_TYPE_SOURCE_WORK_FLOW_RELEASE", false)] Release = 2,
    [LocalizedDisplayName("ENUM_TYPE_SOURCE_WORK_FLOW_MANUAL", false)] Manual = 3,
  }
}
