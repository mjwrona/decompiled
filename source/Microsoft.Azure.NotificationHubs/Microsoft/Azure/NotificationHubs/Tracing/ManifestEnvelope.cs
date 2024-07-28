// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Tracing.ManifestEnvelope
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

namespace Microsoft.Azure.NotificationHubs.Tracing
{
  internal struct ManifestEnvelope
  {
    public const int MaxChunkSize = 65280;
    public ManifestEnvelope.ManifestFormats Format;
    public byte MajorVersion;
    public byte MinorVersion;
    public byte Magic;
    public ushort TotalChunks;
    public ushort ChunkNumber;

    public enum ManifestFormats : byte
    {
      SimpleXmlFormat = 1,
    }
  }
}
