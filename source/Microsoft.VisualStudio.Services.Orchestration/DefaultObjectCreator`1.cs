// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Orchestration.DefaultObjectCreator`1
// Assembly: Microsoft.VisualStudio.Services.Orchestration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C0C603F4-BE31-455B-860A-9FD3B046611C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Orchestration.dll

using System;

namespace Microsoft.VisualStudio.Services.Orchestration
{
  public class DefaultObjectCreator<T> : ObjectCreator<T>
  {
    private Type prototype;
    private T instance;

    public DefaultObjectCreator(Type type)
    {
      this.prototype = type;
      this.Initialize((object) type);
    }

    public DefaultObjectCreator(T instance)
    {
      this.instance = instance;
      this.Initialize((object) instance);
    }

    public override T Create() => this.prototype != (Type) null ? (T) Activator.CreateInstance(this.prototype) : this.instance;

    private void Initialize(object obj)
    {
      this.Name = NameVersionHelper.GetDefaultName(obj);
      this.Version = NameVersionHelper.GetDefaultVersion(obj);
    }
  }
}
