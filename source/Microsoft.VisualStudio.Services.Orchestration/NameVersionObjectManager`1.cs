// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Orchestration.NameVersionObjectManager`1
// Assembly: Microsoft.VisualStudio.Services.Orchestration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C0C603F4-BE31-455B-860A-9FD3B046611C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Orchestration.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Orchestration
{
  public class NameVersionObjectManager<T>
  {
    private readonly IDictionary<string, ObjectCreator<T>> creators;
    private readonly object thisLock = new object();

    public NameVersionObjectManager() => this.creators = (IDictionary<string, ObjectCreator<T>>) new Dictionary<string, ObjectCreator<T>>();

    public void Add(ObjectCreator<T> creator)
    {
      lock (this.thisLock)
      {
        string key = this.GetKey(creator.Name, creator.Version);
        if (this.creators.ContainsKey(key))
          throw new InvalidOperationException("Duplicate entry detected: " + creator.Name + " " + creator.Version);
        this.creators.Add(key, creator);
      }
    }

    public T GetObject(string name, string version)
    {
      string key = this.GetKey(name, version);
      lock (this.thisLock)
      {
        ObjectCreator<T> objectCreator = (ObjectCreator<T>) null;
        return this.creators.TryGetValue(key, out objectCreator) ? objectCreator.Create() : default (T);
      }
    }

    private string GetKey(string name, string version) => name + "_" + version;
  }
}
