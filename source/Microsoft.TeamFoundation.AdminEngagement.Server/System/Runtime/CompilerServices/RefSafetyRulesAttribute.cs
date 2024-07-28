// Decompiled with JetBrains decompiler
// Type: System.Runtime.CompilerServices.RefSafetyRulesAttribute
// Assembly: Microsoft.TeamFoundation.AdminEngagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4DC53F57-597F-449E-A165-8D6CA5E396C1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.AdminEngagement.Server.dll

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
