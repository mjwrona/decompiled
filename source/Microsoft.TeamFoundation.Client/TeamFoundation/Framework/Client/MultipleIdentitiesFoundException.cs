// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.MultipleIdentitiesFoundException
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client.Internal;
using Microsoft.TeamFoundation.Framework.Common;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text;

namespace Microsoft.TeamFoundation.Framework.Client
{
  [Serializable]
  public class MultipleIdentitiesFoundException : TeamFoundationIdentityServiceException
  {
    private string[] m_displayNames;
    private string[] m_uniqueNames;

    public MultipleIdentitiesFoundException(
      string factorValue,
      TeamFoundationIdentity[] matchingIdentities)
      : base(MultipleIdentitiesFoundException.BuildExceptionMessage(factorValue, matchingIdentities))
    {
      this.FactorValue = factorValue;
      this.MatchingIdentities = matchingIdentities;
    }

    private static string BuildExceptionMessage(
      string factorValue,
      TeamFoundationIdentity[] matchingIdentities)
    {
      StringBuilder stringBuilder = new StringBuilder();
      foreach (TeamFoundationIdentity matchingIdentity in matchingIdentities)
        stringBuilder.AppendLine(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, "- {0} ({1})", (object) matchingIdentity.DisplayName, (object) matchingIdentity.UniqueName));
      return ClientResources.MultipleIdentitiesFoundMessage((object) factorValue, (object) stringBuilder.ToString());
    }

    public string FactorValue { get; private set; }

    public TeamFoundationIdentity[] MatchingIdentities { get; private set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public string[] MatchingIdentityDisplayNames
    {
      get
      {
        if (this.m_displayNames == null)
        {
          object[] property = this.GetProperty<object[]>(MultipleIdentitiesFoundExceptionProperties.DisplayNames);
          if (property != null)
          {
            string[] strArray = new string[property.Length];
            for (int index = 0; index < property.Length; ++index)
              strArray[index] = (string) property[index];
            this.m_displayNames = strArray;
          }
          else
            this.m_displayNames = Array.Empty<string>();
        }
        return this.m_displayNames;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public string[] MatchingIdentityUniqueNames
    {
      get
      {
        if (this.m_uniqueNames == null)
        {
          object[] property = this.GetProperty<object[]>(MultipleIdentitiesFoundExceptionProperties.UniqueNames);
          if (property != null)
          {
            string[] strArray = new string[property.Length];
            for (int index = 0; index < property.Length; ++index)
              strArray[index] = (string) property[index];
            this.m_uniqueNames = strArray;
          }
          else
            this.m_uniqueNames = Array.Empty<string>();
        }
        return this.m_uniqueNames;
      }
    }

    public MultipleIdentitiesFoundException(string message)
      : base(message)
    {
    }

    public MultipleIdentitiesFoundException(string message, Exception ex)
      : base(message, ex)
    {
    }

    protected MultipleIdentitiesFoundException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
