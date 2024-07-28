// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.ThreadSafeServiceContainer
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System;
using System.ComponentModel.Design;

namespace Microsoft.TeamFoundation.Client
{
  internal class ThreadSafeServiceContainer : IServiceContainer, IServiceProvider, IDisposable
  {
    private ServiceContainer m_serviceContainer;
    private object m_lock = new object();

    public ThreadSafeServiceContainer() => this.m_serviceContainer = new ServiceContainer();

    public void FlushServices()
    {
      lock (this.m_lock)
      {
        this.m_serviceContainer.Dispose();
        this.m_serviceContainer = new ServiceContainer();
      }
    }

    public void Dispose()
    {
      lock (this.m_lock)
        this.m_serviceContainer.Dispose();
    }

    public void AddService(Type serviceType, object serviceInstance)
    {
      lock (this.m_lock)
        this.m_serviceContainer.AddService(serviceType, serviceInstance);
    }

    public void AddService(Type serviceType, ServiceCreatorCallback callback)
    {
      lock (this.m_lock)
        this.m_serviceContainer.AddService(serviceType, callback);
    }

    public void AddService(Type serviceType, object serviceInstance, bool promote)
    {
      lock (this.m_lock)
        this.m_serviceContainer.AddService(serviceType, serviceInstance, promote);
    }

    public void AddService(Type serviceType, ServiceCreatorCallback callback, bool promote)
    {
      lock (this.m_lock)
        this.m_serviceContainer.AddService(serviceType, callback, promote);
    }

    public void RemoveService(Type serviceType)
    {
      lock (this.m_lock)
        this.m_serviceContainer.RemoveService(serviceType);
    }

    public void RemoveService(Type serviceType, bool promote)
    {
      lock (this.m_lock)
        this.m_serviceContainer.RemoveService(serviceType);
    }

    public object GetService(Type serviceType)
    {
      lock (this.m_lock)
        return this.m_serviceContainer.GetService(serviceType);
    }
  }
}
