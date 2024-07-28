// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.MachineManagement.WebApi.MachinePoolStates
// Assembly: Microsoft.VisualStudio.Services.MachineManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0CB85E58-B74B-46EE-B86D-9E028F77476B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.MachineManagement.WebApi.dll

namespace Microsoft.VisualStudio.Services.MachineManagement.WebApi
{
  public static class MachinePoolStates
  {
    public static readonly string Initializing = nameof (Initializing);
    public static readonly string Initialized = nameof (Initialized);
    public static readonly string Creating = nameof (Creating);
    public static readonly string Ready = nameof (Ready);
    public static readonly string Updating = nameof (Updating);
    public static readonly string Deleting = nameof (Deleting);
    public static readonly string Deleted = nameof (Deleted);
  }
}
