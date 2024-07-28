// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Egress.EgressCheckpoint
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using System;
using System.Text;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Egress
{
  public static class EgressCheckpoint
  {
    private const char DefaultCheckpointDelimiter = ':';
    private const char DefaultLogPathDelimiter = '/';

    public static string RenderCheckpoint(
      string service,
      long logBlobCount,
      DateTimeOffset startTime)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(string.Format("{0}/{1}/{2:D2}/{3:D2}/{4:D2}00:{5}", (object) service, (object) startTime.Year, (object) startTime.Month, (object) startTime.Day, (object) startTime.Hour, (object) logBlobCount));
      string checkpoint = stringBuilder.ToString();
      return !EgressCheckpoint.IsValidCheckpoint(checkpoint) ? (string) null : checkpoint;
    }

    public static bool IsValidCheckpoint(string checkpoint)
    {
      if (string.IsNullOrWhiteSpace(checkpoint))
        return false;
      string[] strArray1 = checkpoint.Split(':');
      if (strArray1.Length != 2)
        return false;
      string[] strArray2 = strArray1[0].Split('/');
      if (strArray2.Length != 5 || !Enum.TryParse<EgressConstants.AzureBlobStorageLogs>(strArray2[0], true, out EgressConstants.AzureBlobStorageLogs _))
        return false;
      return DateTimeOffset.TryParse(strArray2[1] + "-" + strArray2[2] + "-" + strArray2[3] + " " + strArray2[4].Substring(0, 2) + ":00:00.0", out DateTimeOffset _);
    }

    public static int GetLogBlobProcessedCount(string checkpoint)
    {
      if (string.IsNullOrWhiteSpace(checkpoint) || !EgressCheckpoint.IsValidCheckpoint(checkpoint))
        return 0;
      return int.Parse(checkpoint.Split(':')[1]);
    }

    public static bool TryGetDateTime(string checkpoint, out DateTimeOffset dateTime)
    {
      dateTime = new DateTimeOffset();
      if (string.IsNullOrWhiteSpace(checkpoint) || !EgressCheckpoint.IsValidCheckpoint(checkpoint))
        return false;
      string[] strArray = checkpoint.Split(':')[0].Split('/');
      return DateTimeOffset.TryParse(strArray[1] + "-" + strArray[2] + "-" + strArray[3] + " " + strArray[4].Substring(0, 2) + ":00:00.0", out dateTime);
    }
  }
}
