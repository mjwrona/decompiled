// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Ssh.Server.SshServerNativeMethods
// Assembly: Microsoft.TeamFoundation.Ssh.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9CD799E8-FCEB-494E-B317-B5A120D16BCC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Ssh.Server.dll

using System.Runtime.InteropServices;

namespace Microsoft.TeamFoundation.Ssh.Server
{
  internal class SshServerNativeMethods
  {
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool SetInformationJobObject(
      [In] JobHandle jobHandle,
      [In] JobObjectInfoClass jobObjectInfoClass,
      [In] ref ProcessJobObject.CpuJobObjectInfo jobObjectInfo,
      [In] int jobObjectInfoLength);

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    public static extern bool SetInformationJobObject(
      [In] JobHandle jobObjectHandle,
      [In] JobObjectInfoClass jobObjectInfoClass,
      [In] ref ProcessJobObject.JOBOBJECT_EXTENDED_LIMIT_INFORMATION info,
      [In] int infoLength);
  }
}
