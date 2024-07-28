// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PipelineCache.Common.SecurityDefintions
// Assembly: Microsoft.VisualStudio.Services.PipelineCache.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E063C74A-FCE9-47BF-84C0-7143B7075032
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PipelineCache.Common.dll

using System;

namespace Microsoft.VisualStudio.Services.PipelineCache.Common
{
  public static class SecurityDefintions
  {
    public static readonly Guid NamespaceId = new Guid("62A7AD6B-8B8D-426B-BA10-76A7090E94D5");
    public const string RootToken = "$";
    public const char PathSeparator = '/';

    public static string GetScopeToken(string scope) => string.Format("{0}{1}{2}", (object) "$", (object) '/', (object) scope);

    [Flags]
    public enum Permissions
    {
      Read = 1,
      Write = 2,
      All = Write | Read, // 0x00000003
    }
  }
}
