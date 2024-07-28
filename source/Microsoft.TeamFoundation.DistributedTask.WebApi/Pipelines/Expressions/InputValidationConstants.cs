// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Expressions.InputValidationConstants
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.TeamFoundation.DistributedTask.Expressions;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Expressions
{
  internal static class InputValidationConstants
  {
    public static readonly string IsEmail = "isEmail";
    public static readonly string IsInRange = "isInRange";
    public static readonly string IsIPv4Address = "isIPv4Address";
    public static readonly string IsSha1 = "isSha1";
    public static readonly string IsUrl = "isUrl";
    public static readonly string IsMatch = "isMatch";
    public static readonly string Length = "length";
    public static readonly IFunctionInfo[] Functions = new IFunctionInfo[7]
    {
      (IFunctionInfo) new FunctionInfo<IsEmailNode>(InputValidationConstants.IsEmail, IsEmailNode.minParameters, IsEmailNode.maxParameters),
      (IFunctionInfo) new FunctionInfo<IsInRangeNode>(InputValidationConstants.IsInRange, IsInRangeNode.minParameters, IsInRangeNode.maxParameters),
      (IFunctionInfo) new FunctionInfo<IsIPv4AddressNode>(InputValidationConstants.IsIPv4Address, IsIPv4AddressNode.minParameters, IsIPv4AddressNode.maxParameters),
      (IFunctionInfo) new FunctionInfo<IsMatchNode>(InputValidationConstants.IsMatch, IsMatchNode.minParameters, IsMatchNode.maxParameters),
      (IFunctionInfo) new FunctionInfo<IsSHA1Node>(InputValidationConstants.IsSha1, IsSHA1Node.minParameters, IsSHA1Node.maxParameters),
      (IFunctionInfo) new FunctionInfo<IsUrlNode>(InputValidationConstants.IsUrl, IsUrlNode.minParameters, IsUrlNode.maxParameters),
      (IFunctionInfo) new FunctionInfo<LengthNode>(InputValidationConstants.Length, LengthNode.minParameters, LengthNode.maxParameters)
    };
    public static readonly INamedValueInfo[] NamedValues = new INamedValueInfo[1]
    {
      (INamedValueInfo) new NamedValueInfo<InputValueNode>("value")
    };
  }
}
