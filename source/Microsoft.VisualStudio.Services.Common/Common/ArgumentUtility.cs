// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.ArgumentUtility
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Microsoft.VisualStudio.Services.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class ArgumentUtility
  {
    private static readonly Regex s_emailPattern = new Regex("^([a-z0-9.!#$%&'*+/=?^_`{|}~-]+)@((\\[[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.)|(([a-zA-Z0-9\\-]+\\.)+))([a-z]{2,63}|[0-9]{1,3})(\\]?)$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CheckForNull<T>(T var, string varName) where T : class
    {
      if ((object) var != null)
        return;
      ArgumentUtility.ThrowArgumentNullException(varName);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CheckForNull<T>(T? var, string varName) where T : struct
    {
      if (var.HasValue)
        return;
      ArgumentUtility.ThrowArgumentNullException(varName);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CheckGenericForNull(object var, string varName)
    {
      if (var != null)
        return;
      ArgumentUtility.ThrowArgumentNullException(varName);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CheckForNull<T>(T var, string varName, string expectedServiceArea) where T : class
    {
      if ((object) var != null)
        return;
      ArgumentUtility.ThrowArgumentNullException(varName, expectedServiceArea);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CheckForNull<T>(T? var, string varName, string expectedServiceArea) where T : struct
    {
      if (var.HasValue)
        return;
      ArgumentUtility.ThrowArgumentNullException(varName, expectedServiceArea);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CheckGenericForNull(object var, string varName, string expectedServiceArea)
    {
      if (var != null)
        return;
      ArgumentUtility.ThrowArgumentNullException(varName, expectedServiceArea);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CheckStringForNullOrEmpty(string stringVar, string stringVarName)
    {
      ArgumentUtility.CheckForNull<string>(stringVar, stringVarName);
      if (stringVar.Length != 0)
        return;
      ArgumentUtility.ThrowEmptyStringNotAllowedException(stringVarName);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CheckStringForNullOrEmpty(
      string stringVar,
      string stringVarName,
      string expectedServiceArea)
    {
      ArgumentUtility.CheckForNull<string>(stringVar, stringVarName, expectedServiceArea);
      if (stringVar.Length != 0)
        return;
      ArgumentUtility.ThrowEmptyStringNotAllowedException(stringVarName, expectedServiceArea);
    }

    public static void CheckForNonnegativeInt(int var, string varName) => ArgumentUtility.CheckForNonnegativeInt(var, varName, (string) null);

    public static void CheckForNonnegativeInt(int var, string varName, string expectedServiceArea)
    {
      if (var < 0)
        throw new ArgumentOutOfRangeException(varName).Expected(expectedServiceArea);
    }

    public static void CheckForNonPositiveInt(int var, string varName) => ArgumentUtility.CheckForNonPositiveInt(var, varName, (string) null);

    public static void CheckForNonPositiveInt(int var, string varName, string expectedServiceArea)
    {
      if (var <= 0)
        throw new ArgumentOutOfRangeException(varName).Expected(expectedServiceArea);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CheckStringForNullOrWhiteSpace(string stringVar, string stringVarName)
    {
      ArgumentUtility.CheckForNull<string>(stringVar, stringVarName);
      if (!string.IsNullOrWhiteSpace(stringVar))
        return;
      ArgumentUtility.ThrowEmptyOrWhiteSpaceStringNotAllowedException(stringVarName);
    }

    public static void CheckStringForNullOrWhiteSpace(
      string stringVar,
      string stringVarName,
      string expectedServiceArea)
    {
      ArgumentUtility.CheckForNull<string>(stringVar, stringVarName, expectedServiceArea);
      if (!string.IsNullOrWhiteSpace(stringVar))
        return;
      ArgumentUtility.ThrowEmptyOrWhiteSpaceStringNotAllowedException(stringVarName, expectedServiceArea);
    }

    public static void CheckStringLength(
      string stringVar,
      string stringVarName,
      int maxLength,
      int minLength = 0,
      string expectedServiceArea = null)
    {
      ArgumentUtility.CheckForNull<string>(stringVar, stringVarName, expectedServiceArea);
      if (stringVar.Length < minLength || stringVar.Length > maxLength)
        throw new ArgumentException(CommonResources.StringLengthNotAllowed((object) stringVarName, (object) minLength, (object) maxLength), stringVarName).Expected(expectedServiceArea);
    }

    public static void CheckCollectionForMaxLength<T>(
      ICollection<T> collection,
      string collectionName,
      int maxLength)
    {
      if (collection != null && collection.Count > maxLength)
        throw new ArgumentException(CommonResources.CollectionSizeLimitExceeded((object) collectionName, (object) maxLength));
    }

    public static void CheckCollectionForOutOfRange<T>(
      ICollection<T> collection,
      string collectionName,
      int minLength,
      int maxLength)
    {
      ArgumentUtility.CheckForNull<ICollection<T>>(collection, collectionName);
      if (collection.Count < minLength || collection.Count > maxLength)
        throw new ArgumentOutOfRangeException(collectionName, (object) collection, CommonResources.OutOfRange((object) collection));
    }

    public static void CheckEnumerableForNullOrEmpty(IEnumerable enumerable, string enumerableName) => ArgumentUtility.CheckEnumerableForNullOrEmpty(enumerable, enumerableName, (string) null);

    public static void CheckEnumerableForNullOrEmpty(
      IEnumerable enumerable,
      string enumerableName,
      string expectedServiceArea)
    {
      ArgumentUtility.CheckForNull<IEnumerable>(enumerable, enumerableName, expectedServiceArea);
      if (!enumerable.GetEnumerator().MoveNext())
        throw new ArgumentException(CommonResources.EmptyCollectionNotAllowed(), enumerableName).Expected(expectedServiceArea);
    }

    public static void CheckEnumerableForNullElement(IEnumerable enumerable, string enumerableName) => ArgumentUtility.CheckEnumerableForNullElement(enumerable, enumerableName, (string) null);

    public static void CheckEnumerableForNullElement(
      IEnumerable enumerable,
      string enumerableName,
      string expectedServiceArea)
    {
      ArgumentUtility.CheckForNull<IEnumerable>(enumerable, enumerableName, expectedServiceArea);
      foreach (object obj in enumerable)
      {
        if (obj == null)
          throw new ArgumentException(CommonResources.NullElementNotAllowedInCollection(), enumerableName).Expected(expectedServiceArea);
      }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CheckForEmptyGuid(Guid guid, string varName)
    {
      if (!(guid == Guid.Empty))
        return;
      ArgumentUtility.ThrowEmptyGuidNotAllowedException(varName);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CheckForEmptyGuid(Guid? guid, string varName)
    {
      ArgumentUtility.CheckForNull<Guid>(guid, varName);
      Guid? nullable = guid;
      Guid empty = Guid.Empty;
      if ((nullable.HasValue ? (nullable.HasValue ? (nullable.GetValueOrDefault() == empty ? 1 : 0) : 1) : 0) == 0)
        return;
      ArgumentUtility.ThrowEmptyGuidNotAllowedException(varName);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CheckForEmptyGuid(Guid guid, string varName, string expectedServiceArea)
    {
      if (!(guid == Guid.Empty))
        return;
      ArgumentUtility.ThrowEmptyGuidNotAllowedException(varName, expectedServiceArea);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CheckForEmptyGuid(Guid? guid, string varName, string expectedServiceArea)
    {
      ArgumentUtility.CheckForNull<Guid>(guid, varName, expectedServiceArea);
      Guid? nullable = guid;
      Guid empty = Guid.Empty;
      if ((nullable.HasValue ? (nullable.HasValue ? (nullable.GetValueOrDefault() == empty ? 1 : 0) : 1) : 0) == 0)
        return;
      ArgumentUtility.ThrowEmptyGuidNotAllowedException(varName, expectedServiceArea);
    }

    public static void CheckForMultipleBits(int value, string varName) => ArgumentUtility.CheckForMultipleBits(value, varName, (string) null);

    public static void CheckForMultipleBits(int value, string varName, string expectedServiceArea)
    {
      if (value == 0 || (value & value - 1) != 0)
        throw new ArgumentException(CommonResources.SingleBitRequired((object) varName), varName).Expected(expectedServiceArea);
    }

    public static void CheckForDefault<T>(T value, string varName) => ArgumentUtility.CheckForDefault<T>(value, varName, (string) null);

    public static void CheckForDefault<T>(T value, string varName, string expectedServiceArea)
    {
      if (EqualityComparer<T>.Default.Equals(value, default (T)))
        throw new ArgumentException(CommonResources.DefaultValueNotAllowed((object) varName), varName).Expected(expectedServiceArea);
    }

    public static bool IsIllegalInputCharacter(char c, bool allowCrLf = false) => ArgumentUtility.IsIllegalInputCharacter(c, allowCrLf, OSDetails.IsChineseOS);

    public static bool IsIllegalInputCharacter(char c, bool allowCrLf, bool allowGB18030)
    {
      if (allowCrLf && (c == '\r' || c == '\n'))
        return false;
      UnicodeCategory unicodeCategory = char.GetUnicodeCategory(c);
      switch (unicodeCategory)
      {
        case UnicodeCategory.LineSeparator:
        case UnicodeCategory.ParagraphSeparator:
        case UnicodeCategory.Control:
          return true;
        case UnicodeCategory.OtherNotAssigned:
          return !allowGB18030 || c < '龦' || c > '\u9FEF';
        default:
          return unicodeCategory == UnicodeCategory.Format;
      }
    }

    public static string ReplaceIllegalCharacters(string str, char replaceWith, bool allowCrLf = false)
    {
      if (ArgumentUtility.IsIllegalInputCharacter(replaceWith, allowCrLf))
        throw new ArgumentException(CommonResources.VssInvalidUnicodeCharacter((object) (int) replaceWith), nameof (replaceWith));
      if (string.IsNullOrEmpty(str))
        return str;
      char[] charArray = str.ToCharArray();
      for (int index = 0; index < charArray.Length; ++index)
      {
        if (ArgumentUtility.IsIllegalInputCharacter(charArray[index], allowCrLf))
          charArray[index] = replaceWith;
      }
      return new string(charArray);
    }

    public static void CheckStringForInvalidCharacters(string stringVar, string stringVarName) => ArgumentUtility.CheckStringForInvalidCharacters(stringVar, stringVarName, false, (string) null);

    public static void CheckStringForInvalidCharacters(
      string stringVar,
      string stringVarName,
      string expectedServiceArea)
    {
      ArgumentUtility.CheckStringForInvalidCharacters(stringVar, stringVarName, false, expectedServiceArea);
    }

    public static void CheckStringForInvalidCharacters(
      string stringVar,
      string stringVarName,
      bool allowCrLf)
    {
      ArgumentUtility.CheckStringForInvalidCharacters(stringVar, stringVarName, allowCrLf, OSDetails.IsChineseOS, (string) null);
    }

    public static void CheckStringForInvalidCharacters(
      string stringVar,
      string stringVarName,
      bool allowCrLf,
      string expectedServiceArea)
    {
      ArgumentUtility.CheckStringForInvalidCharacters(stringVar, stringVarName, allowCrLf, OSDetails.IsChineseOS, expectedServiceArea);
    }

    public static void CheckStringForInvalidCharacters(
      string stringVar,
      string stringVarName,
      bool allowCrLf,
      bool allowGB18030,
      string expectedServiceArea)
    {
      ArgumentUtility.CheckForNull<string>(stringVar, stringVarName);
      for (int index = 0; index < stringVar.Length; ++index)
      {
        if (ArgumentUtility.IsIllegalInputCharacter(stringVar[index], allowCrLf, allowGB18030))
          throw new ArgumentException(CommonResources.VssInvalidUnicodeCharacter((object) (int) stringVar[index]), stringVarName).Expected(expectedServiceArea);
      }
    }

    public static void CheckStringForInvalidCharacters(
      string stringVar,
      string stringVarName,
      char[] invalidCharacters)
    {
      ArgumentUtility.CheckStringForInvalidCharacters(stringVar, stringVarName, invalidCharacters, (string) null);
    }

    public static void CheckStringForInvalidCharacters(
      string stringVar,
      string stringVarName,
      char[] invalidCharacters,
      string expectedServiceArea)
    {
      ArgumentUtility.CheckForNull<string>(stringVar, stringVarName);
      for (int index = 0; index < invalidCharacters.Length; ++index)
      {
        if (stringVar.IndexOf(invalidCharacters[index]) >= 0)
          throw new ArgumentException(CommonResources.StringContainsInvalidCharacters((object) invalidCharacters[index]), stringVarName).Expected(expectedServiceArea);
      }
    }

    public static void CheckStringForInvalidSqlEscapeCharacters(
      string stringVar,
      string stringVarName,
      string expectedServiceArea = null)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(stringVar, stringVarName);
      for (int index = 0; index < stringVar.Length - 1; ++index)
      {
        if (stringVar[index] == '\\')
        {
          switch (stringVar[++index])
          {
            case '*':
            case '?':
            case '\\':
              continue;
            default:
              throw new ArgumentException(CommonResources.StringContainsInvalidCharacters((object) '\\'), stringVarName).Expected(expectedServiceArea);
          }
        }
      }
    }

    public static void CheckBoundsInclusive(int value, int minValue, int maxValue, string varName) => ArgumentUtility.CheckBoundsInclusive(value, minValue, maxValue, varName, (string) null);

    public static void CheckBoundsInclusive(
      int value,
      int minValue,
      int maxValue,
      string varName,
      string expectedServiceArea)
    {
      if (value < minValue || value > maxValue)
        throw new ArgumentOutOfRangeException(varName, CommonResources.ValueOutOfRange((object) value, (object) varName, (object) minValue, (object) maxValue)).Expected(expectedServiceArea);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CheckForOutOfRange<T>(T var, string varName, T minimum) where T : IComparable<T> => ArgumentUtility.CheckForOutOfRange<T>(var, varName, minimum, (string) null);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CheckForOutOfRange<T>(
      T var,
      string varName,
      T minimum,
      string expectedServiceArea)
      where T : IComparable<T>
    {
      ArgumentUtility.CheckGenericForNull((object) var, varName, expectedServiceArea);
      if (var.CompareTo(minimum) < 0)
        throw new ArgumentOutOfRangeException(varName, (object) var, CommonResources.OutOfRange((object) var)).Expected(expectedServiceArea);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CheckForOutOfRange<T>(T var, string varName, T minimum, T maximum) where T : IComparable<T> => ArgumentUtility.CheckForOutOfRange<T>(var, varName, minimum, maximum, (string) null);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CheckForOutOfRange<T>(
      T var,
      string varName,
      T minimum,
      T maximum,
      string expectedServiceArea)
      where T : IComparable<T>
    {
      if ((object) var == null)
        ArgumentUtility.ThrowArgumentNullException(varName, expectedServiceArea);
      if (var.CompareTo(minimum) < 0 || var.CompareTo(maximum) > 0)
        throw new ArgumentOutOfRangeException(varName, (object) var, CommonResources.OutOfRange((object) var)).Expected(expectedServiceArea);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CheckForOutOfRange(int var, string varName, int minimum) => ArgumentUtility.CheckForOutOfRange(var, varName, minimum, (string) null);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CheckForOutOfRange(
      int var,
      string varName,
      int minimum,
      string expectedServiceArea)
    {
      if (var < minimum)
        throw new ArgumentOutOfRangeException(varName, (object) var, CommonResources.OutOfRange((object) var)).Expected(expectedServiceArea);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CheckForOutOfRange(int var, string varName, int minimum, int maximum) => ArgumentUtility.CheckForOutOfRange(var, varName, minimum, maximum, (string) null);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CheckForOutOfRange(
      int var,
      string varName,
      int minimum,
      int maximum,
      string expectedServiceArea)
    {
      if (var < minimum || var > maximum)
        throw new ArgumentOutOfRangeException(varName, (object) var, CommonResources.OutOfRange((object) var)).Expected(expectedServiceArea);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CheckForOutOfRange(long var, string varName, long minimum) => ArgumentUtility.CheckForOutOfRange(var, varName, minimum, (string) null);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CheckForOutOfRange(
      long var,
      string varName,
      long minimum,
      string expectedServiceArea)
    {
      if (var < minimum)
        throw new ArgumentOutOfRangeException(varName, (object) var, CommonResources.OutOfRange((object) var)).Expected(expectedServiceArea);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CheckForOutOfRange(long var, string varName, long minimum, long maximum) => ArgumentUtility.CheckForOutOfRange(var, varName, minimum, maximum, (string) null);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CheckForOutOfRange(
      long var,
      string varName,
      long minimum,
      long maximum,
      string expectedServiceArea)
    {
      if (var < minimum || var > maximum)
        throw new ArgumentOutOfRangeException(varName, (object) var, CommonResources.OutOfRange((object) var)).Expected(expectedServiceArea);
    }

    public static void CheckForDateTimeRange(
      DateTime var,
      string varName,
      DateTime minimum,
      DateTime maximum)
    {
      ArgumentUtility.CheckForDateTimeRange(var, varName, minimum, maximum, (string) null);
    }

    public static void CheckForDateTimeRange(
      DateTime var,
      string varName,
      DateTime minimum,
      DateTime maximum,
      string expectedServiceArea)
    {
      if (var < minimum || var > maximum)
        throw new ArgumentOutOfRangeException(varName, (object) var, CommonResources.OutOfRange((object) var)).Expected(expectedServiceArea);
    }

    public static void CheckGreaterThanOrEqualToZero(float value, string valueName) => ArgumentUtility.CheckGreaterThanOrEqualToZero(value, valueName, (string) null);

    public static void CheckGreaterThanOrEqualToZero(
      float value,
      string valueName,
      string expectedServiceArea)
    {
      if ((double) value < 0.0)
        throw new ArgumentException(CommonResources.ValueMustBeGreaterThanZero(), valueName).Expected(expectedServiceArea);
    }

    public static void CheckGreaterThanZero(float value, string valueName) => ArgumentUtility.CheckGreaterThanZero(value, valueName, (string) null);

    public static void CheckGreaterThanZero(
      float value,
      string valueName,
      string expectedServiceArea)
    {
      if ((double) value <= 0.0)
        throw new ArgumentException(CommonResources.ValueMustBeGreaterThanZero(), valueName).Expected(expectedServiceArea);
    }

    public static void EnsureIsNull(object var, string varName) => ArgumentUtility.EnsureIsNull(var, varName, (string) null);

    public static void EnsureIsNull(object var, string varName, string expectedServiceArea)
    {
      if (var != null)
        throw new ArgumentException(CommonResources.NullValueNecessary((object) varName)).Expected(expectedServiceArea);
    }

    public static void CheckStringCasing(string stringVar, string varName, bool checkForLowercase = true) => ArgumentUtility.CheckStringCasing(stringVar, varName, checkForLowercase, (string) null);

    public static void CheckStringCasing(
      string stringVar,
      string varName,
      bool checkForLowercase = true,
      string expectedServiceArea = null)
    {
      foreach (char c in stringVar)
      {
        if (char.IsLetter(c) && char.IsLower(c) == !checkForLowercase)
          throw new ArgumentException(checkForLowercase ? CommonResources.LowercaseStringRequired((object) varName) : CommonResources.UppercaseStringRequired((object) varName)).Expected(expectedServiceArea);
      }
    }

    public static void CheckEnumerableForEmpty(IEnumerable enumerable, string enumerableName) => ArgumentUtility.CheckEnumerableForEmpty(enumerable, enumerableName, (string) null);

    public static void CheckEnumerableForEmpty(
      IEnumerable enumerable,
      string enumerableName,
      string expectedServiceArea)
    {
      if (enumerable != null && !enumerable.GetEnumerator().MoveNext())
        throw new ArgumentException(CommonResources.EmptyArrayNotAllowed(), enumerableName).Expected(expectedServiceArea);
    }

    public static void CheckStringExactLength(string stringVar, int length, string stringVarName) => ArgumentUtility.CheckStringExactLength(stringVar, length, stringVarName, (string) null);

    public static void CheckStringExactLength(
      string stringVar,
      int length,
      string stringVarName,
      string expectedServiceArea)
    {
      ArgumentUtility.CheckForNull<string>(stringVar, stringVarName, expectedServiceArea);
      if (stringVar.Length != length)
        throw new ArgumentException(CommonResources.StringLengthNotMatch((object) length), stringVarName).Expected(expectedServiceArea);
    }

    public static void CheckForBothStringsNullOrEmpty(
      string var1,
      string varName1,
      string var2,
      string varName2)
    {
      ArgumentUtility.CheckForBothStringsNullOrEmpty(var1, varName1, var2, varName2, (string) null);
    }

    public static void CheckForBothStringsNullOrEmpty(
      string var1,
      string varName1,
      string var2,
      string varName2,
      string expectedServiceArea)
    {
      if (string.IsNullOrEmpty(var1) && string.IsNullOrEmpty(var2))
        throw new ArgumentException(CommonResources.BothStringsCannotBeNull((object) varName1, (object) varName2)).Expected(expectedServiceArea);
    }

    public static void CheckStringForAnyWhiteSpace(string stringVar, string stringVarName) => ArgumentUtility.CheckStringForAnyWhiteSpace(stringVar, stringVarName, (string) null);

    public static void CheckStringForAnyWhiteSpace(
      string stringVar,
      string stringVarName,
      string expectedServiceArea)
    {
      if (stringVar == null)
        return;
      for (int index = 0; index < stringVar.Length; ++index)
      {
        if (char.IsWhiteSpace(stringVar[index]))
          throw new ArgumentException(CommonResources.WhiteSpaceNotAllowed(), stringVarName).Expected(expectedServiceArea);
      }
    }

    public static void CheckType<T>(object var, string varName, string typeName) => ArgumentUtility.CheckType<T>(var, varName, typeName, (string) null);

    public static void CheckType<T>(
      object var,
      string varName,
      string typeName,
      string expectedServiceArea)
    {
      if (!(var is T))
        throw new ArgumentException(CommonResources.UnexpectedType((object) varName, (object) typeName)).Expected(expectedServiceArea);
    }

    public static void CheckForDefinedEnum<TEnum>(TEnum value, string enumVarName) where TEnum : struct => ArgumentUtility.CheckForDefinedEnum<TEnum>(value, enumVarName, (string) null);

    public static void CheckForDefinedEnum<TEnum>(
      TEnum value,
      string enumVarName,
      string expectedServiceArea)
      where TEnum : struct
    {
      if (!typeof (TEnum).IsEnumDefined((object) value))
        throw new InvalidEnumArgumentException(enumVarName, (int) (ValueType) value, typeof (TEnum)).Expected(expectedServiceArea);
    }

    public static bool IsValidEmailAddress(string emailAddress) => ArgumentUtility.s_emailPattern.IsMatch(emailAddress);

    public static void CheckEmailAddress(string stringVar, string stringVarName) => ArgumentUtility.CheckEmailAddress(stringVar, stringVarName, (string) null);

    public static void CheckEmailAddress(
      string stringVar,
      string stringVarName,
      string expectedServiceArea)
    {
      if (!ArgumentUtility.IsValidEmailAddress(stringVar))
        throw new ArgumentException(CommonResources.InvalidEmailAddressError(), stringVarName).Expected(expectedServiceArea);
    }

    public static void CheckIsValidURI(string uriString, UriKind uriKind, string stringVarName)
    {
      if (!Uri.IsWellFormedUriString(uriString, uriKind))
        throw new ArgumentException(CommonResources.InvalidUriError((object) uriKind), stringVarName);
    }

    public static void CheckValueEqualsToInfinity(float value, string valueName) => ArgumentUtility.CheckValueEqualsToInfinity(value, valueName, (string) null);

    public static void CheckBool(bool actualValue, bool expectedValue, string varName)
    {
      if (actualValue != expectedValue)
        throw new ArgumentException(CommonResources.UnexpectedBoolValue((object) varName, (object) expectedValue, (object) actualValue));
    }

    public static void CheckValueEqualsToInfinity(
      float value,
      string valueName,
      string expectedServiceArea)
    {
      if (float.IsInfinity(value))
        throw new ArgumentException(CommonResources.ValueEqualsToInfinity(), valueName).Expected(expectedServiceArea);
    }

    public static bool IsInvalidString(string strIn) => ArgumentUtility.IsInvalidString(strIn, false, OSDetails.IsChineseOS);

    public static bool IsInvalidString(string strIn, bool allowCrLf) => ArgumentUtility.IsInvalidString(strIn, allowCrLf, OSDetails.IsChineseOS);

    public static bool IsInvalidString(string strIn, bool allowCrLf, bool allowGB18030)
    {
      ArgumentUtility.CheckForNull<string>(strIn, nameof (strIn));
      foreach (char c in strIn)
      {
        if (ArgumentUtility.IsIllegalInputCharacter(c, allowCrLf, allowGB18030))
          return true;
      }
      return ArgumentUtility.HasMismatchedSurrogates(strIn);
    }

    public static bool HasSurrogates(string strIn)
    {
      for (int index = 0; index < strIn.Length; ++index)
      {
        if (char.IsSurrogate(strIn[index]))
          return true;
      }
      return false;
    }

    public static bool HasMismatchedSurrogates(string strIn)
    {
      for (int index = 0; index < strIn.Length; ++index)
      {
        char c = strIn[index];
        if (char.IsLowSurrogate(c))
          return true;
        if (char.IsHighSurrogate(c))
        {
          if (!char.IsSurrogatePair(strIn, index))
            return true;
          ++index;
        }
      }
      return false;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ThrowArgumentNullException(string paramName) => throw new ArgumentNullException(paramName);

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ThrowArgumentNullException(string paramName, string expectedServiceArea) => throw new ArgumentNullException(paramName).Expected(expectedServiceArea);

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void ThrowEmptyStringNotAllowedException(string paramName) => throw new ArgumentException(CommonResources.EmptyStringNotAllowed(), paramName);

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void ThrowEmptyStringNotAllowedException(
      string paramName,
      string expectedServiceArea)
    {
      throw new ArgumentException(CommonResources.EmptyStringNotAllowed(), paramName).Expected(expectedServiceArea);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void ThrowEmptyOrWhiteSpaceStringNotAllowedException(string paramName) => throw new ArgumentException(CommonResources.EmptyOrWhiteSpaceStringNotAllowed(), paramName);

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void ThrowEmptyOrWhiteSpaceStringNotAllowedException(
      string paramName,
      string expectedServiceArea)
    {
      throw new ArgumentException(CommonResources.EmptyOrWhiteSpaceStringNotAllowed(), paramName).Expected(expectedServiceArea);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void ThrowEmptyGuidNotAllowedException(string paramName) => throw new ArgumentException(CommonResources.EmptyGuidNotAllowed((object) paramName), paramName);

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void ThrowEmptyGuidNotAllowedException(
      string paramName,
      string expectedServiceArea)
    {
      throw new ArgumentException(CommonResources.EmptyGuidNotAllowed((object) paramName), paramName).Expected(expectedServiceArea);
    }
  }
}
