// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Hubs.NullMethodDescriptor
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.AspNet.SignalR.Hubs
{
  public class NullMethodDescriptor : MethodDescriptor
  {
    private static readonly IEnumerable<Attribute> _attributes = (IEnumerable<Attribute>) new List<Attribute>();
    private static readonly IList<ParameterDescriptor> _parameters = (IList<ParameterDescriptor>) new List<ParameterDescriptor>();
    private readonly string _methodName;
    private readonly IEnumerable<MethodDescriptor> _availableMethods;

    public NullMethodDescriptor(
      HubDescriptor descriptor,
      string methodName,
      IEnumerable<MethodDescriptor> availableMethods)
    {
      this._methodName = methodName;
      this._availableMethods = availableMethods;
      this.Hub = descriptor;
    }

    public override Func<IHub, object[], object> Invoker => (Func<IHub, object[], object>) ((emptyHub, emptyParameters) =>
    {
      IEnumerable<string> array = (IEnumerable<string>) this.GetAvailableMethodSignatures().ToArray<string>();
      string format;
      if (!array.Any<string>())
        format = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_MethodCouldNotBeResolved, new object[1]
        {
          (object) this._methodName
        });
      else
        format = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_MethodCouldNotBeResolvedCandidates, new object[2]
        {
          (object) this._methodName,
          (object) ("\n" + string.Join("\n", array))
        });
      throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, format));
    });

    private IEnumerable<string> GetAvailableMethodSignatures() => this._availableMethods.Select<MethodDescriptor, string>((Func<MethodDescriptor, string>) (m => m.Name + "(" + string.Join(", ", m.Parameters.Select<ParameterDescriptor, string>((Func<ParameterDescriptor, string>) (p => p.Name + ":" + p.ParameterType.Name))) + "):" + m.ReturnType.Name));

    public override IList<ParameterDescriptor> Parameters => NullMethodDescriptor._parameters;

    public override IEnumerable<Attribute> Attributes => NullMethodDescriptor._attributes;
  }
}
