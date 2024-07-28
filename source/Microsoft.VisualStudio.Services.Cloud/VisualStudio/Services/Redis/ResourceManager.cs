// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Redis.ResourceManager
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Reflection;

namespace Microsoft.VisualStudio.Services.Redis
{
  internal class ResourceManager : IResourceManager
  {
    private const string c_luaResourceNamespace = "Microsoft.VisualStudio.Services.Cloud.Redis.Lua";
    private ConcurrentDictionary<string, string> m_scriptCache = new ConcurrentDictionary<string, string>();

    public string LoadLuaScript(IVssRequestContext requestContext, string script)
    {
      string orAdd = this.m_scriptCache.GetOrAdd(script, (Func<string, string>) (K =>
      {
        string name = "Microsoft.VisualStudio.Services.Cloud.Redis.Lua" + "." + K;
        using (StreamReader streamReader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(name)))
          return streamReader.ReadToEnd();
      }));
      return RedisConfiguration.IsClusterEnabled(requestContext) ? "if redis.replicate_commands then redis.replicate_commands() end" + Environment.NewLine + orAdd : orAdd;
    }

    public static ResourceManager Instance { get; } = new ResourceManager();
  }
}
