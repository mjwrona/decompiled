// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Common.LsaPolicy
// Assembly: Microsoft.TeamFoundation.Build.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AD9C54FA-787C-49B8-AA73-C4A6EF8CE391
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build.Common.dll

using Microsoft.TeamFoundation.Common.Internal;
using System;

namespace Microsoft.TeamFoundation.Build.Common
{
  internal class LsaPolicy : IDisposable
  {
    public IntPtr Handle { get; set; }

    public LsaPolicy()
    {
      NativeMethods.LSA_UNICODE_STRING SystemName = new NativeMethods.LSA_UNICODE_STRING();
      NativeMethods.LSA_OBJECT_ATTRIBUTES ObjectAttributes = new NativeMethods.LSA_OBJECT_ATTRIBUTES()
      {
        Length = 0,
        RootDirectory = IntPtr.Zero,
        Attributes = 0,
        SecurityDescriptor = IntPtr.Zero,
        SecurityQualityOfService = IntPtr.Zero
      };
      IntPtr PolicyHandle = IntPtr.Zero;
      if (NativeMethods.LsaOpenPolicy(ref SystemName, ref ObjectAttributes, NativeMethods.LSA_POLICY_ALL_ACCESS, out PolicyHandle) != 0U || PolicyHandle == IntPtr.Zero)
        throw new Exception("OpenLsaFailed");
      this.Handle = PolicyHandle;
    }

    void IDisposable.Dispose()
    {
      NativeMethods.LsaClose(this.Handle);
      GC.SuppressFinalize((object) this);
    }
  }
}
