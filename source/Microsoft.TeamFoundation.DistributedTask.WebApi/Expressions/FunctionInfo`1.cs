// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Expressions.FunctionInfo`1
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

namespace Microsoft.TeamFoundation.DistributedTask.Expressions
{
  public class FunctionInfo<T> : IFunctionInfo where T : FunctionNode, new()
  {
    public FunctionInfo(string name, int minParameters, int maxParameters)
    {
      this.Name = name;
      this.MinParameters = minParameters;
      this.MaxParameters = maxParameters;
    }

    public string Name { get; }

    public int MinParameters { get; }

    public int MaxParameters { get; }

    public FunctionNode CreateNode() => (FunctionNode) new T();
  }
}
