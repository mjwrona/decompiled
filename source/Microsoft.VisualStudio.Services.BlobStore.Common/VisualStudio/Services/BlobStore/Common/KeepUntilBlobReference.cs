// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Common.KeepUntilBlobReference
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FAFB0281-5CF2-4D3F-992C-49FBB9BEC906
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.BlobStore.Common.dll

using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.BlobStore.Common
{
  [DataContract]
  [Serializable]
  public struct KeepUntilBlobReference : 
    IEquatable<KeepUntilBlobReference>,
    IComparable<KeepUntilBlobReference>
  {
    public static string KeepUntilFormat = "yyyy-MM-ddTHH:mm:ss'Z'";

    public KeepUntilBlobReference(DateTimeOffset date)
      : this(date.UtcDateTime)
    {
    }

    public KeepUntilBlobReference(DateTime date)
    {
      this.KeepUntil = date;
      long num = this.KeepUntil.Ticks % 10000000L;
      if (num > 0L)
        this.KeepUntil = this.KeepUntil.AddTicks(10000000L - num);
      this.Validate();
    }

    public KeepUntilBlobReference(string dateString) => this.KeepUntil = KeepUntilBlobReference.ParseDate(dateString);

    public static KeepUntilBlobReference Parse(string dateString) => new KeepUntilBlobReference(KeepUntilBlobReference.ParseDate(dateString));

    public static DateTime ParseDate(string dateString)
    {
      try
      {
        return DateTime.ParseExact(dateString, KeepUntilBlobReference.KeepUntilFormat, (IFormatProvider) CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal);
      }
      catch
      {
        throw new ArgumentException("Failed to parse date string " + dateString + "; keepUntil dates must be formated as " + KeepUntilBlobReference.KeepUntilFormat);
      }
    }

    [DataMember]
    public DateTime KeepUntil { get; private set; }

    public bool Equals(KeepUntilBlobReference other) => DateTime.Equals(this.KeepUntil, other.KeepUntil);

    public override bool Equals(object obj) => obj is KeepUntilBlobReference other && this.Equals(other);

    public static bool operator ==(KeepUntilBlobReference r1, KeepUntilBlobReference r2) => r1.Equals(r2);

    public static bool operator !=(KeepUntilBlobReference r1, KeepUntilBlobReference r2) => !(r1 == r2);

    public override int GetHashCode() => this.KeepUntil.GetHashCode();

    public string KeepUntilString => this.KeepUntil.ToString(KeepUntilBlobReference.KeepUntilFormat, (IFormatProvider) CultureInfo.InvariantCulture);

    public void Validate()
    {
      DateTime keepUntil = this.KeepUntil;
      if (this.KeepUntil.Kind != DateTimeKind.Utc)
        throw new ArgumentNullException("KeepUntil");
    }

    [System.Runtime.Serialization.OnSerialized]
    private void OnSerialized(StreamingContext context) => this.Validate();

    [System.Runtime.Serialization.OnDeserialized]
    private void OnDeserialized(StreamingContext context) => this.Validate();

    public override string ToString() => "KeepUntil:" + this.KeepUntilString;

    public int CompareTo(KeepUntilBlobReference other) => this.KeepUntil.CompareTo(other.KeepUntil);
  }
}
