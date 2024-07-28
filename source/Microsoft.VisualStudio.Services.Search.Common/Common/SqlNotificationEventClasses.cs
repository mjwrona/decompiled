// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.SqlNotificationEventClasses
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using System;

namespace Microsoft.VisualStudio.Services.Search.Common
{
  public static class SqlNotificationEventClasses
  {
    public static readonly Guid DocumentContractTypeChanged = new Guid("0D5D61B7-0F15-4831-9AFE-E3F1C8685218");
    public static readonly Guid QueryScopingCacheInvalidated = new Guid("C6486E8D-6DDF-40CC-A518-9D6DA86F40F2");
    public static readonly Guid WITFieldDefinitionUpdated = new Guid("A53C941F-45DF-4053-B390-14D0CC74AED3");
  }
}
