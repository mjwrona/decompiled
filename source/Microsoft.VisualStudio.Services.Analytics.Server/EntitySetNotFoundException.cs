// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.EntitySetNotFoundException
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.VisualStudio.Services.Analytics
{
  public class EntitySetNotFoundException : VssServiceException
  {
    public EntitySetNotFoundException(string entitySetName)
      : this(entitySetName, (Exception) null)
    {
    }

    public EntitySetNotFoundException(string entitySetName, Exception innerException)
      : base(AnalyticsResources.ENTITY_SET_NOT_FOUND((object) entitySetName), innerException)
    {
    }
  }
}
