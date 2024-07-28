// Decompiled with JetBrains decompiler
// Type: System.Runtime.CompilerServices.RefSafetyRulesAttribute
// Assembly: Microsoft.TeamFoundation.ConnectedService.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0D040FD2-0366-4FA8-B2F4-4380C0B19F54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.ConnectedService.WebApi.dll

using Microsoft.CodeAnalysis;
using System.Runtime.InteropServices;

namespace System.Runtime.CompilerServices
{
  [CompilerGenerated]
  [Embedded]
  [AttributeUsage(AttributeTargets.Module, AllowMultiple = false, Inherited = false)]
  internal sealed class RefSafetyRulesAttribute : Attribute
  {
    public readonly int Version;

    public RefSafetyRulesAttribute([In] int obj0) => this.Version = obj0;
  }
}
