//
// Copyright (c)  Microsoft Corporation.  All rights reserved.
//
//
// This source code is licensed under Microsoft Shared Source License
// Version 1.0 for Windows CE.
// For a copy of the license visit http://go.microsoft.com/?linkid=2933443.
//

using System;
using System.Collections;
using System.Text;
using Microsoft.WindowsMobile.SharedSource.Utilities;

namespace Microsoft.WindowsMobile.SharedSource.Utilities
{
    /// <summary>
    /// Helper class to work with the system registry
    /// </summary>
	public sealed class Registry
	{
		public class RegistryException : Exception
		{
			public RegistryException() : base() { }
			public RegistryException(string message) : base(message) { }
			public RegistryException(string message, Exception inner) : base(message, inner) { }
		}

		public enum HKey
		{
			CLASSES_ROOT = unchecked((int)0x80000000),
			CURRENT_USER = unchecked((int)0x80000001),
			LOCAL_MACHINE = unchecked((int)0x80000002),
			USERS = unchecked((int)0x80000003)
		}

		public enum ErrorCodes : int
		{
			ERROR_SUCCESS = 0,
			ERROR_MORE_DATA = 234
		}

		public enum CreateKeyOptions : int
		{
			REG_OPTION_NON_VOLATILE = 0,
			REG_OPTION_VOLATILE = 1
		}

		public static IntPtr GetRootKey(HKey key)
		{
			return new IntPtr((int)key);
		}

		public enum ValueType : int
		{
			REG_STRING = 1,
			REG_BINARY = 3,
			REG_DWORD = 4,
			REG_DWORD_LITTLE_ENDIAN = 4,
			REG_DWORD_BIG_ENDIAN = 5,
			REG_MULTI_SZ = 7
		}

		public static void DeleteKey(IntPtr rootKey, string subKey)
		{
			int result = SafeNativeMethods.RegDeleteKey(rootKey, subKey);
			if (result != 0)
			{
				throw new RegistryException("Error Deleting Key");
			}
		}

		public static void CloseKey(IntPtr key)
		{
			int result = SafeNativeMethods.RegCloseKey(key);
			if (result != 0)
			{
				throw new RegistryException("Error Closing Key");
			}
		}

		public static void SetValue(IntPtr key, string valueName, string stringValue)
		{
			byte[] bytes;

			if (stringValue.Length == 0)
			{
				bytes = new byte[2];
				bytes[0] = 0;
				bytes[1] = 0;
			}
			else
			{
				bytes = new byte[System.Text.UnicodeEncoding.Unicode.GetByteCount(stringValue) + 2];
				System.Text.UnicodeEncoding.Unicode.GetBytes(stringValue, 0, stringValue.Length, bytes, 0);
				bytes[bytes.Length - 2] = 0;
				bytes[bytes.Length - 1] = 0;
			}

			SetValue(key, valueName, ValueType.REG_STRING, bytes);
		}

		public static void SetValue(IntPtr key, string valueName, int dwordValue)
		{
			byte[] bytes = System.BitConverter.GetBytes((int)dwordValue);

			SetValue(key, valueName, ValueType.REG_DWORD, bytes);
		}

		public static void SetValue(IntPtr key, string valueName, byte[] bytes)
		{
			SetValue(key, valueName, ValueType.REG_BINARY, bytes);
		}

		public static void SetValue(IntPtr key, string valueName, ValueType type, byte[] data)
		{
			int result = SafeNativeMethods.RegSetValueEx(key, valueName, 0, (int)type, data, data.Length);

			if (result != 0)
			{
				throw new RegistryException("Error Creating Key");
			}
		}

		public static IntPtr OpenKey(IntPtr rootKey, string subkey)
		{
			IntPtr newKey = new IntPtr(0);

			// Create a default non-volatile key under the specified rootkey
			int result = SafeNativeMethods.RegOpenKeyEx(rootKey, subkey, 0, 0, ref newKey);

			if (result != 0)
			{
				newKey = IntPtr.Zero;
			}

			return newKey;
		}

		public static IntPtr CreateKey(IntPtr rootKey, string subkey, bool isVolatile)
		{

			IntPtr newKey = new IntPtr(0);
			IntPtr disposition = new IntPtr(0);
			int options;
			if (isVolatile)
			{
				options = (int)CreateKeyOptions.REG_OPTION_VOLATILE;
			}
			else
			{
				options = (int)CreateKeyOptions.REG_OPTION_NON_VOLATILE;
			}

			// Create a default non-volatile key under the specified rootkey
			int result = SafeNativeMethods.RegCreateKeyEx(rootKey, subkey, 0, null, options,
				0, new IntPtr(0), ref newKey, ref disposition);

			if (result != 0)
			{
				newKey = IntPtr.Zero;
			}

			return newKey;
		}

		public static IntPtr CreateKey(IntPtr rootKey, string subkey)
		{
			return CreateKey(rootKey, subkey, false);
		}

		public static object GetValue(IntPtr rootKey, string subKey, string valueName)
		{
			IntPtr key = Registry.OpenKey(rootKey, subKey);
			object result = GetValue(key, valueName);
			Registry.CloseKey(key);

			return result;
		}

		public static object GetValue(IntPtr key, string valueName)
		{
			object result = null;
			int size = 0;
			Type type;
			int typeValue = 0;
			try
			{
				GetValueType(key, valueName, out type, out size);
				byte[] data = new byte[size];

				int hr = SafeNativeMethods.RegQueryValueEx(key, valueName, 0, ref typeValue, data, ref size);
				if (hr == (int)ErrorCodes.ERROR_SUCCESS)
				{
					if (type == typeof(int))
					{
						result = BitConverter.ToInt32(data, 0);
					}
					else if (type == typeof(string))
					{
						result = System.Text.UnicodeEncoding.Unicode.GetString(data, 0, (int)size).TrimEnd(new char[] { '\0' });
					}
					else if (type == typeof(byte[]))
					{
						result = data;
					}
				}
			}
			catch (RegistryException)
			{
				// we will just return null;
			}

			return result;
		}

		public static void GetValueType(IntPtr rootKey, string subKey, string valueName, out Type type, out int size)
		{
			IntPtr key = Registry.OpenKey(rootKey, subKey);
			GetValueType(key, valueName, out type, out size);
			Registry.CloseKey(key);
		}

		public static void GetValueType(IntPtr key, string valueName, out Type type, out int size)
		{
			int typeValue = 0;
			byte[] data = null;
			size = 0;
			int result = SafeNativeMethods.RegQueryValueEx(key, valueName, 0, ref typeValue, data, ref size);

			if (result != (int)ErrorCodes.ERROR_SUCCESS &&
				result != (int)ErrorCodes.ERROR_MORE_DATA)
			{
				throw new RegistryException();
			}

			switch (typeValue)
			{
				case (int)ValueType.REG_DWORD:
				case (int)ValueType.REG_DWORD_BIG_ENDIAN:
					type = typeof(int);
					break;
				case (int)ValueType.REG_STRING:
				case (int)ValueType.REG_MULTI_SZ:
					type = typeof(string);
					break;
				case (int)ValueType.REG_BINARY:
				default:
					type = typeof(byte[]);
					break;
			}
		}

		public static ArrayList GetSubKeyNames(IntPtr key)
		{
			int index = 0;
			int charCount = 256;
			StringBuilder keyName = new StringBuilder(256);
			ErrorCodes resultCode = ErrorCodes.ERROR_SUCCESS;
			ArrayList result = new ArrayList();

			do
			{
				resultCode = (ErrorCodes) SafeNativeMethods.RegEnumKeyEx(key, index, keyName, ref charCount, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);

				if (resultCode == ErrorCodes.ERROR_SUCCESS)
				{
					index++;

					charCount = 256;

					result.Add(keyName.ToString());
				}
			} while (resultCode == ErrorCodes.ERROR_SUCCESS);

			return result;
		}
	}

	public sealed class SafeNativeMethods
	{
		#region Registry PInvoke
		[System.Runtime.InteropServices.DllImport("coredll.dll")]
		public extern static int RegOpenKeyEx(IntPtr hKey, string lpSubKey, int ulOptions,
			int samDesired, ref IntPtr phkResult);

		[System.Runtime.InteropServices.DllImport("coredll.dll")]
		public extern static int RegCloseKey(IntPtr hKey);

		[System.Runtime.InteropServices.DllImport("coredll.dll")]
		public extern static int RegDeleteKey(IntPtr hKey, string lpSubKey);

		[System.Runtime.InteropServices.DllImport("coredll.dll")]
		public extern static int RegCreateKeyEx(IntPtr hKey, string lpSubKey, int reserved,
			string lpClass, int dwOptions, int samDesired, IntPtr lpSecurityAttributes,
			ref IntPtr phkResult, ref IntPtr lpdwDisposition);

		[System.Runtime.InteropServices.DllImport("coredll.dll")]
		public extern static int RegSetValueEx(IntPtr hKey, string lpValueName, int reserved,
			int dwType, byte[] lpData, int dbData);

		[System.Runtime.InteropServices.DllImport("coredll.dll")]
		public extern static int RegQueryValueEx(IntPtr hKey, string lpValueName, int reserved,
			ref int dwType, byte[] lpData, ref int lpcbData);

		[System.Runtime.InteropServices.DllImport("coredll.dll")]
		public extern static int RegEnumKeyEx(IntPtr hKey, int dwIndex, StringBuilder lpName,
			ref int lpcName, IntPtr lpReserved, IntPtr lpClass, IntPtr lpcbClass,
			IntPtr lpftLastWriteTime);
		#endregion
	}
}
