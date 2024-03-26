﻿Imports System.Data.SqlClient
Public Class Ed_Moodle_CourseContent
    Private RoomID As Integer
    Private callingPanel As Panel


    ' Constructor that accepts a Panel parameter
    Public Sub New(roomID As Integer, panel As Panel)
        InitializeComponent()
        roomID = roomID
        callingPanel = panel
    End Sub

    Private Sub Ed_Moodle_CourseContent_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim labels As Label() = New Label(21) {}

        ' Create labels and set properties
        For i As Integer = 0 To 20
            labels(i) = New Label()
            labels(i).Text = "Resource " & (i + 1)
            labels(i).AutoSize = False
            labels(i).Width = FlowLayoutPanel1.Width - 10
            labels(i).Height = 20 ' Adjust the height as needed
            labels(i).BackColor = Color.Transparent
            labels(i).BorderStyle = BorderStyle.None
            labels(i).Font = New Font("Cascadia Mono", 10) ' Set font to Cascadia Mono, size 10
            labels(i).ForeColor = Color.FromArgb(40, 68, 114) ' Set text color
            AddHandler labels(i).Click, AddressOf Label_Click ' Add click event handler
        Next


        ' Add labels to the FlowLayoutPanel
        For Each Label As Label In labels
            FlowLayoutPanel1.Controls.Add(Label)
        Next
    End Sub
    Private Sub Label_Click(sender As Object, e As EventArgs)
        Dim resourceForm As New Ed_Moodle_CourseResource(callingPanel, "Moodle")
        resourceForm.Name = "HELLO"
        Globals.viewChildForm(callingPanel, resourceForm)
    End Sub

    Private Sub Label1_Click(sender As Object, e As EventArgs) Handles Label1.Click

    End Sub
End Class