// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.Offline.LocalMetadataTableLock
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client.Internal;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;

namespace Microsoft.TeamFoundation.Framework.Client.Offline
{
  internal class LocalMetadataTableLock : IDisposable
  {
    private Mutex m_mutex;
    private bool m_holdsMutex;
    private string m_mutexName;
    private string m_yieldRequestMutexName;
    private int m_retryCount;
    private Func<bool> m_spinDelegate;
    private static MutexSecurity s_mutexSecurity = new MutexSecurity();
    private const string c_globalNamespacePrefix = "Global\\";

    static LocalMetadataTableLock() => LocalMetadataTableLock.s_mutexSecurity.AddAccessRule(new MutexAccessRule((IdentityReference) new SecurityIdentifier(WellKnownSidType.WorldSid, (SecurityIdentifier) null), MutexRights.FullControl, AccessControlType.Allow));

    public LocalMetadataTableLock(string fileName)
      : this(fileName, 600, false, (Func<bool>) null)
    {
    }

    public LocalMetadataTableLock(string fileName, bool requestYield)
      : this(fileName, 600, requestYield, (Func<bool>) null)
    {
    }

    public LocalMetadataTableLock(string fileName, bool requestYield, Func<bool> spinDelegate)
      : this(fileName, 600, requestYield, spinDelegate)
    {
    }

    public LocalMetadataTableLock(string fileName, int retryCount, bool requestYield)
      : this(fileName, retryCount, requestYield, (Func<bool>) null)
    {
    }

    public LocalMetadataTableLock(
      string fileName,
      int retryCount,
      bool requestYield,
      Func<bool> spinDelegate)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(fileName, nameof (fileName));
      this.m_mutexName = "Global\\" + fileName.Replace(Path.DirectorySeparatorChar, '_');
      this.m_yieldRequestMutexName = this.m_mutexName + ";yield";
      this.m_retryCount = retryCount;
      this.m_spinDelegate = spinDelegate;
      bool createdNew;
      this.m_mutex = new Mutex(false, this.m_mutexName, out createdNew, LocalMetadataTableLock.s_mutexSecurity);
      this.Acquire(requestYield);
    }

    private void Acquire(bool requestYield)
    {
      int num = 0;
      bool flag1 = false;
      Mutex mutex = (Mutex) null;
      try
      {
        if (this.m_mutex.WaitOne(0, false))
          this.m_holdsMutex = true;
        else if (requestYield)
        {
          bool createdNew;
          mutex = new Mutex(false, this.m_yieldRequestMutexName, out createdNew, LocalMetadataTableLock.s_mutexSecurity);
          try
          {
            if (mutex.WaitOne(0, false))
              flag1 = true;
          }
          catch (AbandonedMutexException ex)
          {
            flag1 = true;
          }
        }
        while (!this.m_holdsMutex && num++ <= this.m_retryCount)
        {
          if (this.m_spinDelegate != null)
          {
            bool flag2 = false;
            try
            {
              flag2 = this.m_spinDelegate();
            }
            catch (Exception ex)
            {
              TeamFoundationTrace.TraceAndDebugFailException(ex);
            }
            if (flag2)
              throw new SpinDelegateSucceededException();
          }
          try
          {
            if (this.m_mutex.WaitOne(200, false))
              this.m_holdsMutex = true;
            else if (requestYield)
            {
              if (!flag1)
              {
                try
                {
                  if (mutex.WaitOne(0, false))
                    flag1 = true;
                }
                catch (AbandonedMutexException ex)
                {
                  flag1 = true;
                }
              }
            }
          }
          catch (AbandonedMutexException ex)
          {
            this.m_holdsMutex = true;
          }
        }
        if (!this.m_holdsMutex)
          throw new TimeoutException(ClientResources.LocalMetadataTableMutexTimeout());
      }
      catch (AbandonedMutexException ex)
      {
        this.m_holdsMutex = true;
      }
      finally
      {
        if (mutex != null)
        {
          if (flag1)
            mutex.ReleaseMutex();
          mutex.Close();
        }
      }
    }

    public void Dispose()
    {
      if (this.m_mutex == null)
        return;
      if (this.m_holdsMutex)
        this.m_mutex.ReleaseMutex();
      this.m_mutex.Close();
    }

    public bool IsYieldRequested()
    {
      bool flag = false;
      bool createdNew;
      Mutex mutex = new Mutex(false, this.m_yieldRequestMutexName, out createdNew, LocalMetadataTableLock.s_mutexSecurity);
      try
      {
        try
        {
          if (mutex.WaitOne(0, false))
            flag = true;
        }
        catch (AbandonedMutexException ex)
        {
          flag = true;
        }
        return !flag;
      }
      finally
      {
        if (mutex != null)
        {
          if (flag)
            mutex.ReleaseMutex();
          mutex.Close();
        }
      }
    }

    public void Yield()
    {
      if (this.m_mutex == null)
        return;
      if (this.m_holdsMutex)
      {
        this.m_mutex.ReleaseMutex();
        this.m_holdsMutex = false;
      }
      this.Acquire(true);
    }
  }
}
