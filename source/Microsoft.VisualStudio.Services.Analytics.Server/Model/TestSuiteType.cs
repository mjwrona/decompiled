// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.Model.TestSuiteType
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure;

namespace Microsoft.VisualStudio.Services.Analytics.Model
{
  public enum TestSuiteType : byte
  {
    [LocalizedDisplayName("ENUM_TYPE_TEST_SUITE_TYPE_NONE", false)] None,
    [LocalizedDisplayName("ENUM_TYPE_TEST_SUITE_TYPE_QUERY_BASED", false)] QueryBased,
    [LocalizedDisplayName("ENUM_TYPE_TEST_SUITE_TYPE_STATIC", false)] Static,
    [LocalizedDisplayName("ENUM_TYPE_TEST_SUITE_TYPE_REQUIREMENT_BASED", false)] RequirementBased,
  }
}
