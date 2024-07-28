// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ClientAdditionalRouteParameterAttribute
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
  public sealed class ClientAdditionalRouteParameterAttribute : Attribute
  {
    public ClientAdditionalRouteParameterAttribute(
      Type parameterType,
      string parameterName,
      int order,
      string parameterDescription = null)
    {
      this.ParameterType = parameterType;
      this.ParameterName = parameterName;
      this.ParameterDescription = parameterDescription;
      this.Order = order;
    }

    public Type ParameterType { get; }

    public string ParameterName { get; }

    public string ParameterDescription { get; }

    public int Order { get; }
  }
}
