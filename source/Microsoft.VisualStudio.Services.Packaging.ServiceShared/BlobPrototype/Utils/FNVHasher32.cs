// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Utils.FNVHasher32
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Utils
{
  public class FNVHasher32 : IHasher<uint>
  {
    private const uint FnvOffset32 = 2166136261;
    private const uint FnvPrime32 = 16777619;

    public static FNVHasher32 Instance { get; } = new FNVHasher32();

    public uint Hash(string input)
    {
      int length = input.Length;
      uint num = 2166136261;
      for (int index = 0; index < length; ++index)
        num = (uint) (((int) num ^ (int) input[index]) * 16777619);
      return num;
    }
  }
}
