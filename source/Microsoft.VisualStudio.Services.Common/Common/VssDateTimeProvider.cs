// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.VssDateTimeProvider
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.Common
{
  public class VssDateTimeProvider : IVssDateTimeProvider
  {
    public static readonly IVssDateTimeProvider DefaultProvider = (IVssDateTimeProvider) new VssDateTimeProvider.DefaultDateTimeProvider();

    public string Name { get; }

    public DateTime Now => this.Transform == null ? DateTime.Now : this.Transform.Now;

    public DateTime UtcNow => this.Transform == null ? DateTime.UtcNow : this.Transform.UtcNow;

    public DateTime Convert(DateTime time) => this.Transform == null ? time : this.Transform.Convert(time);

    public IDisposable SetOffset(TimeSpan offset) => this.SetTransform((VssDateTimeProvider.DateTimeTransform) new VssDateTimeProvider.FixedOffsetDateTimeTransform(this, offset));

    public IDisposable SetNow(DateTime now) => this.SetTransform((VssDateTimeProvider.DateTimeTransform) new VssDateTimeProvider.FixedNowDateTimeTransform(this, now));

    public VssDateTimeProvider(string name)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(name, nameof (name));
      this.Name = name;
    }

    private IDisposable SetTransform(VssDateTimeProvider.DateTimeTransform transform)
    {
      this.Transform = this.Transform == null ? transform : throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Cannot create more than one concurrent date time transform on the same date time provider. Callers MUST dispose the current transform before setting the new transform, ideally by wrapping in separate using statements.\nProvider: {0}, Current transform: {1}, New transform: {2}", (object) this.Name, (object) this.Transform, (object) transform));
      return (IDisposable) transform;
    }

    private VssDateTimeProvider.DateTimeTransform Transform { get; set; }

    private class DefaultDateTimeProvider : IVssDateTimeProvider
    {
      public string Name => nameof (DefaultDateTimeProvider);

      public DateTime Now => DateTime.Now;

      public DateTime UtcNow => DateTime.UtcNow;

      public DateTime Convert(DateTime time) => time;
    }

    private abstract class DateTimeTransform : IDisposable
    {
      public abstract DateTime Now { get; }

      public abstract DateTime UtcNow { get; }

      public abstract DateTime Convert(DateTime time);

      public DateTimeTransform(VssDateTimeProvider provider) => this.Provider = provider;

      public void Dispose()
      {
        if (this.Provider.Transform != this)
          return;
        this.Provider.Transform = (VssDateTimeProvider.DateTimeTransform) null;
      }

      private VssDateTimeProvider Provider { get; }
    }

    private class FixedOffsetDateTimeTransform : VssDateTimeProvider.DateTimeTransform
    {
      public override DateTime Now => DateTime.Now + this.Offset;

      public override DateTime UtcNow => DateTime.UtcNow + this.Offset;

      public override DateTime Convert(DateTime time) => time + this.Offset;

      public FixedOffsetDateTimeTransform(VssDateTimeProvider provider, TimeSpan offset)
        : base(provider)
      {
        this.Offset = offset;
      }

      public override string ToString() => "FixedOffsetDateTimeTransform offset: " + this.Offset.ToString();

      private TimeSpan Offset { get; }
    }

    private class FixedNowDateTimeTransform : VssDateTimeProvider.DateTimeTransform
    {
      public override DateTime Now { get; }

      public override DateTime UtcNow => this.Now.ToUniversalTime();

      public override DateTime Convert(DateTime time) => time + (this.Now - DateTime.Now);

      public FixedNowDateTimeTransform(VssDateTimeProvider provider, DateTime now)
        : base(provider)
      {
        this.Now = now;
      }

      public override string ToString() => "FixedNowDateTimeTransform now: " + this.Now.ToString();
    }
  }
}
