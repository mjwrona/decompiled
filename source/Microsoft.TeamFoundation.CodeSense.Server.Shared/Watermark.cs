// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CodeSense.Server.Watermark
// Assembly: Microsoft.TeamFoundation.CodeSense.Server.Shared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 548902A5-AE61-4BC7-8D52-315B40AB5900
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.CodeSense.Server.Shared.dll

using System;

namespace Microsoft.TeamFoundation.CodeSense.Server
{
  public class Watermark
  {
    private const string LowWatermarkRegistryKey = "/Service/CodeSense/Watermark/Low";
    private const string HighWatermarkRegistryKey = "/Service/CodeSense/Watermark/High";
    private const string MidWatermarkRegistryKey = "/Service/CodeSense/Watermark/Mid";
    private const char RegistrySeparator = '|';

    public Watermark(WatermarkKind kind)
      : this(kind, 0, 0)
    {
    }

    public Watermark(WatermarkKind kind, int changesetId)
      : this(kind, changesetId, 0)
    {
    }

    public Watermark(WatermarkKind kind, int changesetId, int retryCount)
    {
      this.Kind = kind;
      this.ChangesetId = changesetId;
      this.RetryCount = retryCount;
    }

    private Watermark()
    {
    }

    public int ChangesetId { get; private set; }

    public WatermarkKind Kind { get; private set; }

    public int RetryCount { get; private set; }

    internal string RegistryKey
    {
      get
      {
        switch (this.Kind)
        {
          case WatermarkKind.Low:
            return "/Service/CodeSense/Watermark/Low";
          case WatermarkKind.High:
            return "/Service/CodeSense/Watermark/High";
          case WatermarkKind.FLI:
            return "/Service/CodeSense/Watermark/Mid";
          default:
            return (string) null;
        }
      }
    }

    internal string Description => Enum.GetName(typeof (WatermarkKind), (object) this.Kind);

    public override string ToString() => string.Join('|'.ToString(), (object) this.Kind, (object) this.ChangesetId, (object) this.RetryCount);

    internal static Watermark FromString(string str)
    {
      if (string.IsNullOrEmpty(str))
        return (Watermark) null;
      try
      {
        string[] strArray = str.Split(new char[1]{ '|' }, 3);
        int kind = (int) Enum.Parse(typeof (WatermarkKind), strArray[0]);
        int num1 = int.Parse(strArray[1]);
        int num2 = int.Parse(strArray[2]);
        int changesetId = num1;
        int retryCount = num2;
        return new Watermark((WatermarkKind) kind, changesetId, retryCount);
      }
      catch
      {
        return (Watermark) null;
      }
    }
  }
}
