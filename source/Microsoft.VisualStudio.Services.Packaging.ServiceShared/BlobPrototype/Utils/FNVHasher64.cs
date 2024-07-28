// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Utils.FNVHasher64
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Utils
{
  public class FNVHasher64 : IHasher<ulong>
  {
    private const ulong FnvOffset64 = 14695981039346656037;
    private const ulong FnvPrime64 = 1099511628211;

    public static FNVHasher64 Instance { get; } = new FNVHasher64();

    public ulong Hash(string input)
    {
      int length = input.Length;
      ulong num = 14695981039346656037;
      for (int index = 0; index < length; ++index)
        num = (ulong) (((long) num ^ (long) input[index]) * 1099511628211L);
      return num;
    }
  }
}
