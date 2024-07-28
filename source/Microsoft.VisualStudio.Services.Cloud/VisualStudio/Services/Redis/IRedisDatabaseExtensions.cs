// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Redis.IRedisDatabaseExtensions
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using System;

namespace Microsoft.VisualStudio.Services.Redis
{
  internal static class IRedisDatabaseExtensions
  {
    public static Microsoft.VisualStudio.Services.Redis.V1.IRedisDatabaseInternal Internal(
      this Microsoft.VisualStudio.Services.Redis.V1.IRedisDatabase database)
    {
      return database is Microsoft.VisualStudio.Services.Redis.V1.IRedisDatabaseInternal databaseInternal ? databaseInternal : throw new InvalidOperationException();
    }

    public static Microsoft.VisualStudio.Services.Redis.V2.IRedisDatabaseInternal Internal(
      this Microsoft.VisualStudio.Services.Redis.V2.IRedisDatabase database)
    {
      return database is Microsoft.VisualStudio.Services.Redis.V2.IRedisDatabaseInternal databaseInternal ? databaseInternal : throw new InvalidOperationException();
    }
  }
}
