Imports System.IO

Public Class RegistryKeyUtilities
    Public Delegate Sub LogEventDelegate(ByVal message As String)
    Public Event LogEvent As LogEventDelegate

    Public Function GetDotNETFolder(ByVal version As String) As String
        Return GetDotNETFolder(version, RegistryKey64.RegWow64Options.KEY_WOW64_32KEY)
    End Function

    Public Function GetDotNETFolder(ByVal version As String, ByVal bitness As RegistryKey64.RegWow64Options) As String
        Dim regKey As RegistryKeyLib.RegistryKey64 = RegistryKeyLib.RegistryKey64.OpenKey(Microsoft.Win32.RegistryHive.LocalMachine, "Software\Microsoft\.NETFramework\", False, bitness)
        If regKey Is Nothing Then
            Throw New Exception("Could not read the .NET framework folder location from the registry...")
        End If

        Dim installRoot As String = CStr(regKey.GetValue("InstallRoot"))

        If (Not installRoot.EndsWith("\")) Then installRoot &= "\"
        regKey = regKey.OpenSubKey("policy")

        For Each subKeyName As String In regKey.GetSubKeyNames()
            If (subKeyName = version) Then
                installRoot &= subKeyName

                Dim subKey As RegistryKeyLib.RegistryKey64 = regKey.OpenSubKey(subKeyName)
                Dim values() As String = subKey.GetValueNames()

                RaiseLogEvent("    values.Count = " & values.Length & vbCrLf)

                If (values.Length > 0) Then
                    installRoot &= "." & values(0) & "\"
                    RaiseLogEvent(".Net folder = " & installRoot)

                    Return installRoot
                Else
                    RaiseLogEvent("No values found for subKey " & subKeyName)
                End If
            End If
        Next

        Throw New FileNotFoundException("Failed to find .NET folder")
    End Function

    Public Sub RaiseLogEvent(ByVal message As String)
        RaiseEvent LogEvent(message)
    End Sub
End Class
