Imports System.Runtime.InteropServices

Public Class Form1
    Private remainingWorkTime As Integer = 25 * 60 ' 25 minutes
    Private remainingBreakTime As Integer = 5 * 60 ' 5 minutes
    Private isTimerPaused As Boolean = False
    Private isWorkTimerActive As Boolean = True

    ' Declare constants
    Private Const SND_SYNC As Integer = &H0 ' Play synchronously (the function will not return until the sound finishes playing)
    Private Const SND_ASYNC As Integer = &H1 ' Play asynchronously
    Private Const SND_FILENAME As Integer = &H20000 ' The parameter is a filename

    ' Import the PlaySound function from coredll.dll
    <DllImport("coredll.dll", EntryPoint:="PlaySound", SetLastError:=True)> _
    Private Shared Function PlaySound(ByVal szSound As String, ByVal hMod As IntPtr, ByVal flags As Integer) As Boolean
    End Function

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        ' Check if any timer is currently active
        If timerWork.Enabled Or timerBreak.Enabled Then
            ' Pause the active timer
            timerWork.Enabled = False
            timerBreak.Enabled = False
            isTimerPaused = True
        Else
            ' If the timer is paused, decide which timer to resume
            If isTimerPaused Then
                timerWork.Enabled = isWorkTimerActive
                timerBreak.Enabled = Not isWorkTimerActive
                isTimerPaused = False
            Else
                ' If no timer is paused, start the work timer
                isWorkTimerActive = True ' Start with work timer
                timerWork.Enabled = True
                timerBreak.Enabled = False
            End If
        End If
        UpdateTimerLabel(If(isWorkTimerActive, remainingWorkTime, remainingBreakTime))
        PlayMySound("hai.wav") ' Adjust the sound file name as needed for this event
    End Sub

    Private Sub timerWork_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles timerWork.Tick
        If remainingWorkTime > 0 Then
            remainingWorkTime -= 1
            UpdateTimerLabel(remainingWorkTime)
        Else
            PlayMySound("hard.wav") ' Use appropriate sound for work timer end
            ' Switch to break timer
            isWorkTimerActive = False
            timerWork.Enabled = False
            timerBreak.Enabled = True
            remainingWorkTime = 25 * 60 ' Reset for next cycle
        End If
    End Sub

    Private Sub timerBreak_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles timerBreak.Tick
        If remainingBreakTime > 0 Then
            remainingBreakTime -= 1
            UpdateTimerLabel(remainingBreakTime)
        Else
            ' End of break timer, start vibration
            PlayMySound("nice.wav")
            ' Switch to work timer
            isWorkTimerActive = True
            timerBreak.Enabled = False
            timerWork.Enabled = True
            remainingBreakTime = 5 * 60 ' Reset for next cycle
        End If
    End Sub



    Private Sub PlayMySound(ByVal soundFileName As String)
        Dim basePath As String = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase)
        If basePath.StartsWith("file:\\") Then
            basePath = New Uri(basePath).LocalPath
        End If
        Dim soundPath As String = System.IO.Path.Combine(basePath, soundFileName)
        System.Diagnostics.Debug.WriteLine("Attempting to play sound from: " + soundPath)
        PlaySound(soundPath, IntPtr.Zero, SND_ASYNC Or SND_FILENAME)
    End Sub




    Private Sub UpdateTimerLabel(ByVal seconds As Integer)
        Label1.Text = String.Format("{0}:{1:00}", seconds \ 60, seconds Mod 60)
        Button1.Text = If(isTimerPaused Or (Not timerWork.Enabled And Not timerBreak.Enabled), "Start", "Pause")
    End Sub


End Class
