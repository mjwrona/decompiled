// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Infrastructure.PerformanceCounterAttribute
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using System;
using System.Diagnostics;

namespace Microsoft.AspNet.SignalR.Infrastructure
{
  [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
  internal sealed class PerformanceCounterAttribute : Attribute
  {
    public string Name { get; set; }

    public string Description { get; set; }

    public PerformanceCounterType CounterType { get; set; }
  }
}
