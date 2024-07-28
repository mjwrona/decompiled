// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Hubs.DefaultParameterResolver
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using Microsoft.AspNet.SignalR.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.AspNet.SignalR.Hubs
{
  public class DefaultParameterResolver : IParameterResolver
  {
    public virtual object ResolveParameter(ParameterDescriptor descriptor, IJsonValue value)
    {
      if (descriptor == null)
        throw new ArgumentNullException(nameof (descriptor));
      if (value == null)
        throw new ArgumentNullException(nameof (value));
      return value.GetType() == descriptor.ParameterType ? (object) value : value.ConvertTo(descriptor.ParameterType);
    }

    public virtual IList<object> ResolveMethodParameters(
      MethodDescriptor method,
      IList<IJsonValue> values)
    {
      if (method == null)
        throw new ArgumentNullException(nameof (method));
      return (IList<object>) method.Parameters.Zip<ParameterDescriptor, IJsonValue, object>((IEnumerable<IJsonValue>) values, new Func<ParameterDescriptor, IJsonValue, object>(this.ResolveParameter)).ToArray<object>();
    }
  }
}
