﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Reflection;
using Microsoft.CodeAnalysis.ErrorReporting;
using Microsoft.VisualStudio.Debugger;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.ExpressionEvaluator
{
    internal static class RegistryHelpers
    {
        private const string RegistryKey = @"Software\Microsoft\ExpressionEvaluator";

        internal static object? GetRegistryValue(string name)
        {
            try
            {
                // Microsoft.Win32.Registry is not supported on OneCore/CoreSystem,
                // so we have to check to see if it's there at runtime.
                var registryType = typeof(object).GetTypeInfo().Assembly.GetType("Microsoft.Win32.Registry");
                if (registryType != null)
                {
                    var hKeyCurrentUserField = registryType.GetTypeInfo().GetDeclaredField("CurrentUser");
                    if (hKeyCurrentUserField != null && hKeyCurrentUserField.IsStatic)
                    {
                        using var currentUserKey = (IDisposable)hKeyCurrentUserField.GetValue(null);
                        var openSubKeyMethod = currentUserKey.GetType().GetTypeInfo().GetDeclaredMethod("OpenSubKey", [typeof(string), typeof(bool)]);

                        using var eeKey = (IDisposable?)openSubKeyMethod?.Invoke(currentUserKey, new object[] { RegistryKey, /*writable*/ false });
                        if (eeKey != null)
                        {
                            var getValueMethod = eeKey.GetType().GetTypeInfo().GetDeclaredMethod("GetValue", [typeof(string)]);
                            return getValueMethod?.Invoke(eeKey, new object[] { name });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Assert(false, "Failure checking registry key: " + ex.ToString());
            }
            return null;
        }

        internal static bool GetBoolRegistryValue(string name)
        {
            var value = RegistryHelpers.GetRegistryValue(name);
            return value is int i && i == 1;
        }
    }

    internal static class ExpressionEvaluatorFatalError
    {
        private const string RegistryValue = "EnableFailFast";
        internal static bool IsFailFastEnabled = RegistryHelpers.GetBoolRegistryValue(RegistryValue);

        internal static bool CrashIfFailFastEnabled(Exception exception)
        {
            if (!IsFailFastEnabled)
            {
                return false;
            }

            if (exception is NotImplementedException)
            {
                // This is part of the dispatcher mechanism.  A NotImplementedException indicates
                // that another component should handle the call.
                return false;
            }

            if (exception is DkmException dkmException)
            {
                switch (dkmException.Code)
                {
                    case DkmExceptionCode.E_METADATA_UPDATE_DEADLOCK: // Metadata was updated while EE had component lock
                    case DkmExceptionCode.E_PROCESS_DESTROYED:
                    case DkmExceptionCode.E_XAPI_REMOTE_CLOSED:
                    case DkmExceptionCode.E_XAPI_REMOTE_DISCONNECTED:
                    case DkmExceptionCode.E_XAPI_COMPONENT_DLL_NOT_FOUND:
                        return false;
                }
            }

            return FatalError.ReportAndPropagate(exception);
        }
    }
}
