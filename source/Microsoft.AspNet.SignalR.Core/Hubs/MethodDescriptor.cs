// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Hubs.MethodDescriptor
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using System;
using System.Collections.Generic;

namespace Microsoft.AspNet.SignalR.Hubs
{
  public class MethodDescriptor : Descriptor
  {
    public virtual Type ReturnType { get; set; }

    public virtual HubDescriptor Hub { get; set; }

    public virtual IList<ParameterDescriptor> Parameters { get; set; }

    public Type ProgressReportingType { get; set; }

    public virtual Func<IHub, object[], object> Invoker { get; set; }

    public virtual IEnumerable<Attribute> Attributes { get; set; }
  }
}
