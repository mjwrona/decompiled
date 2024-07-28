// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Common.Migration.IServiceHostAccess
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 203E0171-FB50-4FDE-9B1F-EFC6366423BC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Common.dll

namespace Microsoft.VisualStudio.Services.Content.Server.Common.Migration
{
  public interface IServiceHostAccess
  {
    bool IsProduction { get; }

    int GetRegistry(string path, int defaultValue);

    bool GetRegistry(string path, bool defaultValue);

    void SetRegistry(string path, int value);

    void SetRegistry(string path, bool value);

    void DeleteRegistries(string[] paths);
  }
}
