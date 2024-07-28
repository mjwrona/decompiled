// Decompiled with JetBrains decompiler
// Type: Nest.InputContainer
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class InputContainer : IInputContainer, IDescriptor
  {
    internal InputContainer()
    {
    }

    public InputContainer(InputBase input)
    {
      input.ThrowIfNull<InputBase>(nameof (input));
      input.WrapInContainer((IInputContainer) this);
    }

    IChainInput IInputContainer.Chain { get; set; }

    IHttpInput IInputContainer.Http { get; set; }

    ISearchInput IInputContainer.Search { get; set; }

    ISimpleInput IInputContainer.Simple { get; set; }

    public static implicit operator InputContainer(InputBase input) => input != null ? new InputContainer(input) : (InputContainer) null;
  }
}
