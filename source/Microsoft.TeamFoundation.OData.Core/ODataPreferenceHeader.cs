// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ODataPreferenceHeader
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.OData
{
  public class ODataPreferenceHeader
  {
    private const string ReturnPreferenceTokenName = "return";
    private const string ReturnRepresentationPreferenceTokenValue = "representation";
    private const string ReturnMinimalPreferenceTokenValue = "minimal";
    private const string ODataAnnotationPreferenceToken = "odata.include-annotations";
    private const string RespondAsyncPreferenceToken = "respond-async";
    private const string WaitPreferenceTokenName = "wait";
    private const string ODataContinueOnErrorPreferenceToken = "odata.continue-on-error";
    private const string ODataMaxPageSizePreferenceToken = "odata.maxpagesize";
    private const string ODataTrackChangesPreferenceToken = "odata.track-changes";
    private const string OmitValuesPreferenceToken = "omit-values";
    private const string PreferHeaderName = "Prefer";
    private const string PreferenceAppliedHeaderName = "Preference-Applied";
    private static readonly KeyValuePair<string, string>[] EmptyParameters = new KeyValuePair<string, string>[0];
    private static readonly HttpHeaderValueElement ContinueOnErrorPreference = new HttpHeaderValueElement("odata.continue-on-error", (string) null, (IEnumerable<KeyValuePair<string, string>>) ODataPreferenceHeader.EmptyParameters);
    private static readonly HttpHeaderValueElement ReturnMinimalPreference = new HttpHeaderValueElement("return", "minimal", (IEnumerable<KeyValuePair<string, string>>) ODataPreferenceHeader.EmptyParameters);
    private static readonly HttpHeaderValueElement ReturnRepresentationPreference = new HttpHeaderValueElement("return", "representation", (IEnumerable<KeyValuePair<string, string>>) ODataPreferenceHeader.EmptyParameters);
    private static readonly HttpHeaderValueElement RespondAsyncPreference = new HttpHeaderValueElement("respond-async", (string) null, (IEnumerable<KeyValuePair<string, string>>) ODataPreferenceHeader.EmptyParameters);
    private static readonly HttpHeaderValueElement TrackChangesPreference = new HttpHeaderValueElement("odata.track-changes", (string) null, (IEnumerable<KeyValuePair<string, string>>) ODataPreferenceHeader.EmptyParameters);
    private readonly ODataMessage message;
    private readonly string preferenceHeaderName;
    private HttpHeaderValue preferences;

    internal ODataPreferenceHeader(IODataRequestMessage requestMessage)
    {
      this.message = (ODataMessage) new ODataRequestMessage(requestMessage, true, false, -1L);
      this.preferenceHeaderName = "Prefer";
    }

    internal ODataPreferenceHeader(IODataResponseMessage responseMessage)
    {
      this.message = (ODataMessage) new ODataResponseMessage(responseMessage, true, false, -1L);
      this.preferenceHeaderName = "Preference-Applied";
    }

    public bool? ReturnContent
    {
      get
      {
        HttpHeaderValueElement headerValueElement = this.Get("return");
        if (headerValueElement != null && headerValueElement.Value != null)
        {
          if (headerValueElement.Value.ToLowerInvariant().Equals("representation"))
            return new bool?(true);
          if (headerValueElement.Value.ToLowerInvariant().Equals("minimal"))
            return new bool?(false);
        }
        return new bool?();
      }
      set
      {
        this.Clear("return");
        bool? nullable1 = value;
        bool flag1 = true;
        if (nullable1.GetValueOrDefault() == flag1 & nullable1.HasValue)
          this.Set(ODataPreferenceHeader.ReturnRepresentationPreference);
        bool? nullable2 = value;
        bool flag2 = false;
        if (!(nullable2.GetValueOrDefault() == flag2 & nullable2.HasValue))
          return;
        this.Set(ODataPreferenceHeader.ReturnMinimalPreference);
      }
    }

    public string AnnotationFilter
    {
      get
      {
        HttpHeaderValueElement headerValueElement = this.Get("odata.include-annotations");
        if (headerValueElement == null || headerValueElement.Value == null)
          return (string) null;
        return headerValueElement.Value.Trim('"');
      }
      set
      {
        ExceptionUtils.CheckArgumentStringNotEmpty(value, nameof (AnnotationFilter));
        if (value == null)
          this.Clear("odata.include-annotations");
        else
          this.Set(new HttpHeaderValueElement("odata.include-annotations", ODataPreferenceHeader.AddQuotes(value), (IEnumerable<KeyValuePair<string, string>>) ODataPreferenceHeader.EmptyParameters));
      }
    }

    public string OmitValues
    {
      get => this.Get("omit-values")?.Value;
      set
      {
        ExceptionUtils.CheckArgumentStringNotEmpty(value, nameof (OmitValues));
        if (!string.Equals(value.Trim().ToLowerInvariant(), "nulls", StringComparison.Ordinal))
          return;
        this.Set(new HttpHeaderValueElement("omit-values", "nulls", (IEnumerable<KeyValuePair<string, string>>) ODataPreferenceHeader.EmptyParameters));
      }
    }

    public bool RespondAsync
    {
      get => this.Get("respond-async") != null;
      set
      {
        if (value)
          this.Set(ODataPreferenceHeader.RespondAsyncPreference);
        else
          this.Clear("respond-async");
      }
    }

    public int? Wait
    {
      get
      {
        HttpHeaderValueElement headerValueElement = this.Get("wait");
        if (headerValueElement == null || headerValueElement.Value == null)
          return new int?();
        int result;
        if (int.TryParse(headerValueElement.Value, out result))
          return new int?(result);
        throw new ODataException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Invalid value '{0}' for {1} preference header found. The {1} preference header requires an integer value.", new object[2]
        {
          (object) headerValueElement.Value,
          (object) "wait"
        }));
      }
      set
      {
        if (value.HasValue)
          this.Set(new HttpHeaderValueElement("wait", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}", new object[1]
          {
            (object) value
          }), (IEnumerable<KeyValuePair<string, string>>) ODataPreferenceHeader.EmptyParameters));
        else
          this.Clear("wait");
      }
    }

    public bool ContinueOnError
    {
      get => this.Get("odata.continue-on-error") != null;
      set
      {
        if (value)
          this.Set(ODataPreferenceHeader.ContinueOnErrorPreference);
        else
          this.Clear("odata.continue-on-error");
      }
    }

    public int? MaxPageSize
    {
      get
      {
        HttpHeaderValueElement headerValueElement = this.Get("odata.maxpagesize");
        if (headerValueElement == null || headerValueElement.Value == null)
          return new int?();
        int result;
        if (int.TryParse(headerValueElement.Value, out result))
          return new int?(result);
        throw new ODataException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Invalid value '{0}' for {1} preference header found. The {1} preference header requires an integer value.", new object[2]
        {
          (object) headerValueElement.Value,
          (object) "odata.maxpagesize"
        }));
      }
      set
      {
        if (value.HasValue)
          this.Set(new HttpHeaderValueElement("odata.maxpagesize", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}", new object[1]
          {
            (object) value.Value
          }), (IEnumerable<KeyValuePair<string, string>>) ODataPreferenceHeader.EmptyParameters));
        else
          this.Clear("odata.maxpagesize");
      }
    }

    public bool TrackChanges
    {
      get => this.Get("odata.track-changes") != null;
      set
      {
        if (value)
          this.Set(ODataPreferenceHeader.TrackChangesPreference);
        else
          this.Clear("odata.track-changes");
      }
    }

    private HttpHeaderValue Preferences => this.preferences ?? (this.preferences = this.ParsePreferences());

    protected void Clear(string preference)
    {
      if (!this.Preferences.Remove(preference))
        return;
      this.SetPreferencesToMessageHeader();
    }

    protected void Set(HttpHeaderValueElement preference)
    {
      this.Preferences[preference.Name] = preference;
      this.SetPreferencesToMessageHeader();
    }

    protected HttpHeaderValueElement Get(string preferenceName)
    {
      HttpHeaderValueElement headerValueElement;
      return !this.Preferences.TryGetValue(preferenceName, out headerValueElement) ? (HttpHeaderValueElement) null : headerValueElement;
    }

    private static string AddQuotes(string text) => "\"" + text + "\"";

    private HttpHeaderValue ParsePreferences() => HttpHeaderValueLexer.Create(this.preferenceHeaderName, this.message.GetHeader(this.preferenceHeaderName)).ToHttpHeaderValue();

    private void SetPreferencesToMessageHeader() => this.message.SetHeader(this.preferenceHeaderName, this.Preferences.ToString());
  }
}
