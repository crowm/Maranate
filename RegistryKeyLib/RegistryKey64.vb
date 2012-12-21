Imports System.Runtime.InteropServices
Imports Microsoft.Win32
Imports System.Text

Public Class RegistryKey64
    Implements IDisposable

    Private _handle As New UIntPtr
    Private _hive As RegistryHive
    Private _subKey As String
    Private _bitness As RegWow64Options

    Private Const KEY_WOW64_64KEY As Integer = &H100
    Private Const KEY_WOW64_32KEY As Integer = &H200
    Private Const KEY_ALL_ACCESS As Integer = &HF003F
    Private Const KEY_READ As Integer = &H20019
    Private Const KEY_READWRITE As Integer = &H2001F

    Private Const MAX_REG_NAME_SIZE As Integer = 16383

    Public Enum RegWow64Options As Integer
        None = 0
        KEY_WOW64_64KEY = &H100
        KEY_WOW64_32KEY = &H200
    End Enum

#Region "interop"
    Private Declare Auto Function RegOpenKeyEx Lib "advapi32.dll" ( _
        ByVal hKey As RegistryHive, _
        ByVal lpSubKey As String, _
        ByVal ulOptions As Integer, _
        ByVal samDesired As Integer, _
        ByRef phkResult As UInt32 _
    ) As Integer

    Private Declare Auto Function RegCreateKeyEx Lib "advapi32.dll" ( _
        ByVal hKey As RegistryHive, _
        ByVal lpSubKey As String, _
        ByVal lpReserved As IntPtr, _
        ByVal lpClass As IntPtr, _
        ByVal ulOptions As Integer, _
        ByVal samDesired As Integer, _
        ByVal lpSecurityAttributes As IntPtr, _
        ByRef phkResult As UInt32, _
        ByRef lpdwDisposition As RegCreateResult _
    ) As Integer

    Private Declare Auto Function RegDeleteKeyEx Lib "advapi32.dll" ( _
        ByVal hKey As RegistryHive, _
        ByVal lpSubKey As String, _
        ByVal samDesired As Integer, _
        ByVal lpReserved As IntPtr _
    ) As Integer

    Private Declare Auto Function RegDeleteKey Lib "advapi32.dll" ( _
        ByVal hKey As RegistryHive, _
        ByVal lpSubKey As String _
    ) As Integer

    Private Declare Function RegCloseKey Lib "advapi32.dll" (ByVal hKey As UIntPtr) As Integer

    Private Declare Auto Function RegEnumKeyEx Lib "Advapi32" ( _
        ByVal hKey As UIntPtr, _
        ByVal dwIndex As Integer, _
        ByVal lpName As StringBuilder, _
        ByRef lpcName As Integer, _
        ByVal lpReserved As IntPtr, _
        ByVal lpClass As IntPtr, _
        ByVal lpcClass As IntPtr, _
        ByVal lpftLastWriteTime As IntPtr _
    ) As Integer

    Private Declare Auto Function RegEnumValue Lib "Advapi32" ( _
        ByVal hKey As UIntPtr, _
        ByVal dwIndex As Integer, _
        ByVal lpValueName As StringBuilder, _
        ByRef lpcValueName As Integer, _
        ByVal lpReserved As IntPtr, _
        ByVal lpType As IntPtr, _
        ByVal lpData As IntPtr, _
        ByVal lpcbData As IntPtr _
    ) As Integer

    <DllImport("advapi32.dll", CharSet:=CharSet.Unicode, SetLastError:=True)> _
    Private Shared Function RegQueryValueExW( _
        ByVal hKey As UIntPtr, _
        ByVal lpValueName As String, _
        ByVal lpReserved As Integer, _
        ByRef lpType As Integer, _
        ByVal lpData As Byte(), _
        ByRef lpcbData As Integer) As Integer
    End Function

    <DllImport("advapi32.dll", CharSet:=CharSet.Unicode, SetLastError:=True)> _
    Friend Shared Function RegSetValueExW( _
    ByVal hKey As UIntPtr, _
    ByVal lpValueName As String, _
    ByVal lpReserved As Integer, _
    ByVal lpType As Integer, _
    ByVal lpData As Byte(), _
    ByVal lpcbData As Integer) As Integer
    End Function

    <DllImport("advapi32.dll", CharSet:=CharSet.Unicode, SetLastError:=True)> _
    Private Shared Function RegDeleteValue( _
        ByVal hKey As UIntPtr, _
        ByVal lpValueName As String) As Integer
    End Function


    Declare Function GetCurrentProcess Lib "kernel32" () As IntPtr
    'Declare Function GetModuleHandle Lib "kernel32" (ByVal lpModuleName As String) As IntPtr
    <DllImport("kernel32.dll", CharSet:=CharSet.Ansi, SetLastError:=True, EntryPoint:="GetModuleHandleA")> _
    Private Shared Function GetModuleHandle(ByVal lpModuleName As String) As IntPtr
    End Function

    Private Declare Function GetProcAddress Lib "kernel32" ( _
        ByVal hModule As IntPtr, _
        ByVal lpProcName As String _
    ) As Integer

    <DllImport("Kernel32.dll", SetLastError:=True, CallingConvention:=CallingConvention.Winapi)> _
    Private Shared Function IsWow64Process( _
        ByVal hProcess As IntPtr, _
        ByRef lpSystemInfo As Boolean) _
        As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function

    Private Enum RegistryValueKind
        [None] = 0
        [String] = 1
        [ExpandString] = 2
        [Binary] = 3
        [DWord] = 4
        [DWordBigEndian] = 5
        [Link] = 6
        [MultiString] = 7
        [ResourceList] = 8
        [FullResourceDescriptor] = 9
        [ResourceRequirementsList] = 10
        [QWord] = 11
    End Enum

    Private Enum RegCreateResult
        CreatedNewKey = 1
        OpenedExistingKey = 2
    End Enum

#End Region

    Private Sub New(ByVal hive As RegistryHive, ByVal subKey As String, ByVal bitness As RegWow64Options)
        _hive = hive
        _subKey = subKey
        _bitness = bitness
    End Sub

    Public Sub Close()
        If _handle.Equals(UIntPtr.Zero) = False Then
            RegCloseKey(_handle)
            _handle = New UIntPtr
        End If
        _subKey = ""
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        Close()
    End Sub

#Region "Shared Methods"
    Public Shared Function CreateKey(ByVal hKey As RegistryHive, ByVal subKey As String) As RegistryKey64
        Return CreateKey(hKey, subKey, True, RegWow64Options.None)
    End Function

    Public Shared Function CreateKey(ByVal hKey As RegistryHive, ByVal subKey As String, ByVal writable As Boolean) As RegistryKey64
        Return CreateKey(hKey, subKey, writable, RegWow64Options.None)
    End Function

    Public Shared Function CreateKey(ByVal hKey As RegistryHive, ByVal subKey As String, ByVal writable As Boolean, ByVal bitness As RegWow64Options) As RegistryKey64
        Dim rk As New RegistryKey64(hKey, subKey, bitness)

        Dim samDesired As Integer = bitness

        If (writable) Then
            samDesired = samDesired Or KEY_READWRITE
        Else
            samDesired = samDesired Or KEY_READ
        End If
        Dim handle As UInt32
        Dim disposition As RegCreateResult
        Dim result As Integer = RegCreateKeyEx(hKey, subKey, IntPtr.Zero, IntPtr.Zero, 0, samDesired, IntPtr.Zero, handle, disposition)
        If (result <> 0) Then
            Dim errorstr As String = New System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error()).Message
            Return Nothing
        End If

        rk._handle = New UIntPtr(handle)
        Return rk
    End Function

    Public Shared Function OpenKey(ByVal hKey As RegistryHive, ByVal subKey As String) As RegistryKey64
        Return OpenKey(hKey, subKey, False, RegWow64Options.None)
    End Function

    Public Shared Function OpenKey(ByVal hKey As RegistryHive, ByVal subKey As String, ByVal writable As Boolean) As RegistryKey64
        Return OpenKey(hKey, subKey, writable, RegWow64Options.None)
    End Function

    Public Shared Function OpenKey(ByVal hKey As RegistryHive, ByVal subKey As String, ByVal writable As Boolean, ByVal bitness As RegWow64Options) As RegistryKey64
        Dim rk As New RegistryKey64(hKey, subKey, bitness)

        Dim samDesired As Integer = bitness

        If (writable) Then
            samDesired = samDesired Or KEY_READWRITE
        Else
            samDesired = samDesired Or KEY_READ
        End If
        Dim handle As UInt32
        Dim result As Integer = RegOpenKeyEx(hKey, subKey, 0, samDesired, handle)
        If (result <> 0) Then
            Dim errorstr As String = New System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error()).Message
            Return Nothing
        End If

        rk._handle = New UIntPtr(handle)
        Return rk
    End Function

    Public Shared Sub DeleteKey(ByVal hKey As RegistryHive, ByVal subKey As String)
        DeleteKey(hKey, subKey, RegWow64Options.None)
    End Sub

    Public Shared Sub DeleteKey(ByVal hKey As RegistryHive, ByVal subKey As String, ByVal bitness As RegWow64Options)
        Dim rk As New RegistryKey64(hKey, subKey, bitness)

        Dim samDesired As Integer = bitness

        Dim result As Integer
        Dim osVersion As String = Environment.OSVersion.Version.Major & "." & Environment.OSVersion.Version.Minor

        If osVersion.Equals("5.1") And bitness = RegWow64Options.KEY_WOW64_32KEY Then
            ' 32 bit XP doesn't support the RegDeleteKeyEx method in the
            ' advapi32.dll file. So we'll use the basic RegDeleteKey instead
            result = RegDeleteKey(hKey, subKey)
        Else
            result = RegDeleteKeyEx(hKey, subKey, samDesired, IntPtr.Zero)
        End If

        If (result <> 0) Then
            Dim errorstr As String = New System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error()).Message
            Return
        End If
    End Sub
#End Region


    Public Function CreateSubKey(ByVal subKey As String) As RegistryKey64
        Return CreateSubKey(subKey, True)
    End Function

    Public Function CreateSubKey(ByVal subKey As String, ByVal writable As Boolean) As RegistryKey64
        Return CreateKey(_hive, _subKey & "\" & subKey, writable, _bitness)
    End Function

    Public Function OpenSubKey(ByVal subKey As String) As RegistryKey64
        Return OpenSubKey(subKey, False)
    End Function

    Public Function OpenSubKey(ByVal subKey As String, ByVal writable As Boolean) As RegistryKey64
        Return OpenKey(_hive, _subKey & "\" & subKey, writable, _bitness)
    End Function

    Public Sub DeleteSubKey(ByVal subKey As String)
        DeleteKey(_hive, _subKey & "\" & subKey, _bitness)
    End Sub

    Public Function GetSubKeyNames() As String()
        Dim i, ret, NameSize As Integer
        Dim sc As New ArrayList
        Dim sb As New StringBuilder(MAX_REG_NAME_SIZE + 1)

        ' quick sanity check
        If _handle.Equals(UIntPtr.Zero) Then
            Throw New ApplicationException("Cannot access a closed registry key")
        End If

        Do
            NameSize = MAX_REG_NAME_SIZE + 1
            ret = RegEnumKeyEx(_handle, i, sb, NameSize, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero)
            If ret <> 0 Then
                Exit Do
            End If
            sc.Add(sb.ToString)
            i += 1
        Loop

        Return sc.ToArray(GetType(String))
    End Function

    Public Function GetValueNames() As String()
        Dim i, ret, NameSize As Integer
        Dim sc As New ArrayList
        Dim sb As New StringBuilder(MAX_REG_NAME_SIZE + 1)

        ' quick sanity check
        If _handle.Equals(UIntPtr.Zero) Then
            Throw New ApplicationException("Cannot access a closed registry key")
        End If

        Do
            NameSize = MAX_REG_NAME_SIZE + 1
            ret = RegEnumValue(_handle, i, sb, NameSize, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero)
            If ret <> 0 Then
                Exit Do
            End If
            sc.Add(sb.ToString)
            i += 1
        Loop

        Return sc.ToArray(GetType(String))
    End Function

    Public Function GetValue(ByVal name As String) As Object
        Return GetValue(name, Nothing)
    End Function

    Public Function GetValue(ByVal name As String, ByVal defaultValue As Object) As Object

        ' quick sanity check
        If _handle.Equals(UIntPtr.Zero) Then
            Throw New ApplicationException("Cannot access a closed registry key")
        End If

        Dim result As Integer
        Dim keyType As Integer
        'Dim keyValue As StringBuilder
        Dim keyByteValue As Byte()
        Dim keyValueLength As Integer

        result = RegQueryValueExW(_handle, name, Nothing, keyType, Nothing, keyValueLength)
        If (result <> 0) Then
            Dim strError As String = New ComponentModel.Win32Exception(Marshal.GetLastWin32Error()).Message
            Return defaultValue
        End If
        'keyValue = New StringBuilder(keyValueLength)
        ReDim keyByteValue(keyValueLength - 1)

        result = RegQueryValueExW(_handle, name, Nothing, keyType, keyByteValue, keyValueLength)
        If (result <> 0) Then
            Dim strError As String = New ComponentModel.Win32Exception(Marshal.GetLastWin32Error()).Message
            Return defaultValue
        End If

        Dim RegType As RegistryValueKind = keyType

        Select Case RegType
            Case RegistryValueKind.None
                Return defaultValue

            Case RegistryValueKind.String
                If (keyValueLength < 2) Then Return ""
                Return Encoding.Unicode.GetString(keyByteValue, 0, keyValueLength - 2)

            Case RegistryValueKind.ExpandString
                If (keyValueLength < 2) Then Return ""
                Dim str As String = Encoding.Unicode.GetString(keyByteValue, 0, keyValueLength - 2)
                Return Environment.ExpandEnvironmentVariables(str)

            Case RegistryValueKind.Binary
                Return keyByteValue

            Case RegistryValueKind.DWord
                If (keyValueLength < 4) Then Return 0
                Dim val As UInteger = 0
                val += keyByteValue(3)
                val = val << 8
                val += keyByteValue(2)
                val = val << 8
                val += keyByteValue(1)
                val = val << 8
                val += keyByteValue(0)
                Return val

            Case RegistryValueKind.DWordBigEndian
                Return keyByteValue

            Case RegistryValueKind.Link
                Return defaultValue

            Case RegistryValueKind.MultiString
                Dim strings As New ArrayList
                Dim index As Integer = 0
                Dim lastIndex As Integer = 0
                While (index < keyValueLength)
                    While (index < keyValueLength) AndAlso (keyByteValue(index) <> 0)
                        index += 2
                    End While

                    Dim str As String = Encoding.Unicode.GetString(keyByteValue, lastIndex, index - lastIndex)
                    If (str.Length > 0) Then
                        strings.Add(str)
                    End If
                    index += 2
                    lastIndex = index
                End While

                Return strings.ToArray(GetType(String))

            Case RegistryValueKind.ResourceList
                Return defaultValue
            Case RegistryValueKind.FullResourceDescriptor
                Return defaultValue
            Case RegistryValueKind.ResourceRequirementsList
                Return defaultValue

            Case RegistryValueKind.QWord
                If (keyValueLength < 8) Then Return 0
                Dim val As ULong = 0
                val += keyByteValue(7)
                val = val << 8
                val += keyByteValue(6)
                val = val << 8
                val += keyByteValue(5)
                val = val << 8
                val += keyByteValue(4)
                val = val << 8
                val += keyByteValue(3)
                val = val << 8
                val += keyByteValue(2)
                val = val << 8
                val += keyByteValue(1)
                val = val << 8
                val += keyByteValue(0)
                Return val

        End Select

        Return defaultValue
    End Function

    Public Sub SetValue(ByVal Name As String, ByVal Value As Object)
        Dim ret As Integer
        Dim data As Byte()
        Dim RegType As RegistryValueKind

        ' So we have to figure out the type
        Select Case Value.GetType.ToString
            Case "System.String"
                RegType = RegistryValueKind.String
            Case "System.Int32"
                RegType = RegistryValueKind.DWord
            Case "System.Int64"
                RegType = RegistryValueKind.QWord
            Case "System.String[]"
                RegType = RegistryValueKind.MultiString
            Case "System.Byte[]"
                RegType = RegistryValueKind.Binary
            Case Else
                ' convert it to a string
                RegType = RegistryValueKind.String
                Value = Value.ToString
        End Select

        Select Case RegType
            Case RegistryValueKind.Binary
                data = DirectCast(Value, System.Byte())

            Case RegistryValueKind.DWord
                Dim temp As Int32 = CType(Value, Int32)
                ReDim data(4 - 1)
                data(0) = temp And &HFF
                temp = temp >> 8
                data(1) = temp And &HFF
                temp = temp >> 8
                data(2) = temp And &HFF
                temp = temp >> 8
                data(3) = temp And &HFF

            Case RegistryValueKind.ExpandString
                Dim temp As String = CStr(Value)
                data = Encoding.Unicode.GetBytes(temp)

            Case RegistryValueKind.MultiString
                Dim temp, lines() As String
                Dim index As Integer
                lines = DirectCast(Value, System.String())

                ' Calculate the total size, including the terminating null
                Dim size As Integer = 0
                For Each temp In lines
                    size += Encoding.Unicode.GetByteCount(temp) + 2
                Next
                size += 2
                ReDim data(size - 1)

                index = 0
                For Each temp In lines
                    Encoding.Unicode.GetBytes(temp, 0, temp.Length, data, index)
                    index += Encoding.Unicode.GetByteCount(temp) + 2
                Next

            Case RegistryValueKind.QWord
                Dim temp As Int64 = CType(Value, Long)
                ReDim data(8 - 1)
                data(0) = temp And &HFF
                temp = temp >> 8
                data(1) = temp And &HFF
                temp = temp >> 8
                data(2) = temp And &HFF
                temp = temp >> 8
                data(3) = temp And &HFF
                temp = temp >> 8
                data(4) = temp And &HFF
                temp = temp >> 8
                data(5) = temp And &HFF
                temp = temp >> 8
                data(6) = temp And &HFF
                temp = temp >> 8
                data(7) = temp And &HFF

            Case RegistryValueKind.String
                Dim temp As String = CStr(Value)
                data = Encoding.Unicode.GetBytes(temp)

            Case Else
                Throw New ApplicationException("Registry type of " & RegType & " is not supported")
        End Select

        ' let's do it!
        ret = RegSetValueExW(_handle, Name, 0, RegType, data, data.Length)
        If ret <> 0 Then
            Throw New System.ComponentModel.Win32Exception(ret)
        End If

    End Sub

    Public Sub DeleteValue(ByVal Name As String)
        Dim ret As Integer
        ret = RegDeleteValue(_handle, Name)
        If ret <> 0 Then
            Throw New System.ComponentModel.Win32Exception(ret)
        End If
    End Sub

    'Checks if the IsWow64Process function exists on this OS, then calls it.
    Public Shared Function Is64BitOperatingSystem() As Boolean
        If (Marshal.SizeOf(GetType(IntPtr)) = 8) Then
            Return True
        End If

        Return Is32BitOn64BitOperatingSystem()
    End Function

    Public Shared Function Is32BitOn64BitOperatingSystem() As Boolean
        Dim modHandle As IntPtr = GetModuleHandle("Kernel32.dll")

        Dim procAddress As Integer = GetProcAddress(modHandle, "IsWow64Process")
        If procAddress <= 0 Then
            Return False
        End If

        Dim procHandle As IntPtr = System.Diagnostics.Process.GetCurrentProcess().Handle

        Dim retVal As Boolean
        If IsWow64Process(procHandle, retVal) Then
            Return retVal
        Else
            Return False
        End If
    End Function

End Class
