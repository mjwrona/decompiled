// Decompiled with JetBrains decompiler
// Type: System.Runtime.CompilerServices.RefSafetyRulesAttribute
// Assembly: Microsoft.VisualStudio.Services.PipelineCache.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E063C74A-FCE9-47BF-84C0-7143B7075032
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PipelineCache.Common.dll

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
