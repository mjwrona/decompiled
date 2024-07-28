// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Configuration.INetFwRule
// Assembly: Microsoft.VisualStudio.Services.Configuration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AB461A1-8255-4EAB-B12B-E1D379571DC1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Configuration.dll

using System.Runtime.InteropServices;

namespace Microsoft.VisualStudio.Services.Configuration
{
  [Guid("AF230D27-BABA-4E42-ACED-F524F22CFCE2")]
  [TypeLibType(4160)]
  public interface INetFwRule
  {
    [DispId(1)]
    string Name { [DispId(1)] [return: MarshalAs(UnmanagedType.BStr)] get; [DispId(1)] [param: MarshalAs(UnmanagedType.BStr)] set; }

    [DispId(2)]
    string Description { [DispId(2)] [return: MarshalAs(UnmanagedType.BStr)] get; [DispId(2)] [param: MarshalAs(UnmanagedType.BStr)] set; }

    [DispId(3)]
    string ApplicationName { [DispId(3)] [return: MarshalAs(UnmanagedType.BStr)] get; [DispId(3)] [param: MarshalAs(UnmanagedType.BStr)] set; }

    [DispId(4)]
    string serviceName { [DispId(4)] [return: MarshalAs(UnmanagedType.BStr)] get; [DispId(4)] [param: MarshalAs(UnmanagedType.BStr)] set; }

    [DispId(5)]
    int Protocol { [DispId(5)] get; [DispId(5)] set; }

    [DispId(6)]
    string LocalPorts { [DispId(6)] [return: MarshalAs(UnmanagedType.BStr)] get; [DispId(6)] [param: MarshalAs(UnmanagedType.BStr)] set; }

    [DispId(7)]
    string RemotePorts { [DispId(7)] [return: MarshalAs(UnmanagedType.BStr)] get; [DispId(7)] [param: MarshalAs(UnmanagedType.BStr)] set; }

    [DispId(8)]
    string LocalAddresses { [DispId(8)] [return: MarshalAs(UnmanagedType.BStr)] get; [DispId(8)] [param: MarshalAs(UnmanagedType.BStr)] set; }

    [DispId(9)]
    string RemoteAddresses { [DispId(9)] [return: MarshalAs(UnmanagedType.BStr)] get; [DispId(9)] [param: MarshalAs(UnmanagedType.BStr)] set; }

    [DispId(10)]
    string IcmpTypesAndCodes { [DispId(10)] [return: MarshalAs(UnmanagedType.BStr)] get; [DispId(10)] [param: MarshalAs(UnmanagedType.BStr)] set; }

    [DispId(11)]
    NET_FW_RULE_DIRECTION_ Direction { [DispId(11)] get; [DispId(11)] set; }

    [DispId(12)]
    object Interfaces { [DispId(12)] [return: MarshalAs(UnmanagedType.Struct)] get; [DispId(12)] [param: MarshalAs(UnmanagedType.Struct)] set; }

    [DispId(13)]
    string InterfaceTypes { [DispId(13)] [return: MarshalAs(UnmanagedType.BStr)] get; [DispId(13)] [param: MarshalAs(UnmanagedType.BStr)] set; }

    [DispId(14)]
    bool Enabled { [DispId(14)] get; [DispId(14)] set; }

    [DispId(15)]
    string Grouping { [DispId(15)] [return: MarshalAs(UnmanagedType.BStr)] get; [DispId(15)] [param: MarshalAs(UnmanagedType.BStr)] set; }

    [DispId(16)]
    int Profiles { [DispId(16)] get; [DispId(16)] set; }

    [DispId(17)]
    bool EdgeTraversal { [DispId(17)] get; [DispId(17)] set; }

    [DispId(18)]
    NET_FW_ACTION_ Action { [DispId(18)] get; [DispId(18)] set; }
  }
}
