﻿Public Class Ed_RoleSelect

    Private Sub Ed_RoleSelect_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ' Add the event handlers to the button
        AddHandler Button1.MouseEnter, AddressOf Button1_MouseEnter
        AddHandler Button1.MouseLeave, AddressOf Button1_MouseLeave
        AddHandler Button2.MouseEnter, AddressOf Button2_MouseEnter
        AddHandler Button2.MouseLeave, AddressOf Button2_MouseLeave
        AddHandler Button3.MouseEnter, AddressOf Button3_MouseEnter
        AddHandler Button3.MouseLeave, AddressOf Button3_MouseLeave
        Me.AcceptButton = Nothing
    End Sub
    Private Sub Button1_MouseEnter(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.MouseEnter
        ' Set the button color on mouse enter
        Button1.BackColor = Color.FromArgb(&H48, &H6F, &H94)
    End Sub

    Private Sub Button1_MouseLeave(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.MouseLeave
        ' Restore the original button color on mouse leave
        Button1.BackColor = Color.FromArgb(&HC4, &HE0, &HE5)
    End Sub

    Private Sub Button2_MouseEnter(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.MouseEnter
        ' Set the button color on mouse enter
        Button2.BackColor = Color.FromArgb(&H48, &H6F, &H94)
    End Sub

    Private Sub Button2_MouseLeave(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.MouseLeave
        ' Restore the original button color on mouse leave
        Button2.BackColor = Color.FromArgb(&HC4, &HE0, &HE5)
    End Sub

    Private Sub Button3_MouseEnter(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.MouseEnter
        ' Set the button color on mouse enter
        Button3.BackColor = Color.FromArgb(&H48, &H6F, &H94)
    End Sub

    Private Sub Button3_MouseLeave(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.MouseLeave
        ' Restore the original button color on mouse leave
        Button3.BackColor = Color.FromArgb(&HC4, &HE0, &HE5)
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Ed_GlobalDashboard.OpenFormInGlobalEdPanel(Ed_StudentDashboard)
    End Sub


    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
         Ed_GlobalDashboard.OpenFormInGlobalEdPanel(Ed_TeacherDashboard)
    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        Ed_GlobalDashboard.OpenFormInGlobalEdPanel(Ed_AdminDashboard)
    End Sub

    Private Sub childformPanel_Paint(sender As Object, e As PaintEventArgs) Handles childformPanel.Paint

    End Sub
End Class
