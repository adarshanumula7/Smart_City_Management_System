﻿Imports System.Data.SqlClient
Imports System.IO
Imports System.Web
Imports MySql.Data.MySqlClient
Imports Mysqlx
Imports Mysqlx.XDevAPI.Relational
Imports Org.BouncyCastle.Crypto.Prng

Public Class TransportVehicleRegReq
    Public Property uid As Integer = 13
    Public Property u_name As String = "Adarsh Anumula"
    Private payClicked As Boolean = False
    Private pdfBytes As Byte()
    Private row1 As Integer = 0
    Dim id As String
    Private imageBytes As Byte()
    Private Sub DataGridView1_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellContentClick
        ' Check if the clicked cell is in the "Column3" column and not a header cell
        If e.ColumnIndex = DataGridView1.Columns("Column3").Index AndAlso e.RowIndex >= 0 Then
            ' Assuming you have a PDFViewerForm with a PictureBox control named PictureBox1 on it
            row1 = e.RowIndex
            Dim cellValue As Object = DataGridView1.Rows(row1).Cells(0).Value
            id = cellValue
            'Retrieve the PDF bytes from the database
            Using Con As MySqlConnection = Globals.GetDBConnection()
                Con.Open()
                Dim query As String = "SELECT inv_pdf FROM vehicle_reg WHERE vehicle_id = @a"
                Using command As New MySqlCommand(query, Con)
                    command.Parameters.AddWithValue("@a", id)
                    ' Execute the query and read the PDF data
                    Dim pdfData As Byte() = Nothing
                    Dim result As Object = command.ExecuteScalar()

                    If result IsNot DBNull.Value Then
                        pdfData = DirectCast(result, Byte())
                    End If
                    ' Check if the PDF data is not null
                    If pdfData IsNot Nothing Then
                        ' Save the PDF data to a temporary file with .tmp extension
                        Dim tempFilePath As String = Path.GetTempFileName()

                        ' Rename the file with .pdf extension
                        Dim pdfFilePath As String = Path.ChangeExtension(tempFilePath, ".pdf")
                        File.WriteAllBytes(pdfFilePath, pdfData)

                        ' Open the PDF file using the default web browser
                        Process.Start(pdfFilePath)
                    Else
                        ' If no PDF data is found in the database
                        MessageBox.Show("No PDF found in the database.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    End If
                End Using
            End Using
        ElseIf e.ColumnIndex = DataGridView1.Columns("Column4").Index AndAlso e.RowIndex >= 0 Then
            row1 = e.RowIndex
            Dim cellValue As Object = DataGridView1.Rows(row1).Cells(0).Value
            id = cellValue
            Dim Con = Globals.GetDBConnection()
            Con.Open()
            Dim query As String = "SELECT inv_pdf FROM vehicle_reg WHERE vehicle_id = @a"
            Using command As New MySqlCommand(query, Con)
                command.Parameters.AddWithValue("@a", id)
                ' Execute the query and read the image data
                Dim imageData As Byte() = Nothing
                Dim result As Object = command.ExecuteScalar()

                If result IsNot DBNull.Value Then
                    imageData = DirectCast(result, Byte())
                End If

                ' Check if the image data is not null
                If imageData IsNot Nothing AndAlso imageData.Length > 0 Then
                    Try
                        ' Convert the byte array back to an Image
                        Using ms As New MemoryStream(imageData)
                            Dim vehicleImage As Image = Image.FromStream(ms)
                            ' Set the value of the corresponding DataGridView cell to the image data
                            DataGridView1.Rows(row1).Cells(3).Value = vehicleImage
                        End Using
                    Catch ex As Exception
                        ' If an error occurs while processing the image, display an error message
                        MessageBox.Show("Error processing image: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    End Try
                Else
                    ' If no image data is found in the database
                    MessageBox.Show("No image found in the database.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If
            End Using
            Con.Close()

        End If
    End Sub

    Private Function ImageToByteArray(image As Image) As Byte()
        Using ms As New MemoryStream()
            image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg)
            Return ms.ToArray()
        End Using
    End Function

    Private Sub LoadandBindDataGridView()
        'Get connection from globals
        Dim Con = Globals.GetDBConnection()
        Dim reader As MySqlDataReader
        Dim cmd As MySqlCommand

        Try
            Con.Open()

        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

        cmd = New MySqlCommand("SELECT uid, vehicle_type as vehicle_type_ID, vehicle_ID, vehicle_pic, inv_pdf, status, inv_id FROM vehicle_reg where uid = @a ", Con)
        cmd.Parameters.AddWithValue("@a", uid)
        reader = cmd.ExecuteReader
        ' Create a DataTable to store the data
        Dim dataTable As New DataTable()

        'Fill the DataTable with data from the SQL table
        dataTable.Load(reader)
        reader.Close()

        Dim NewColumn As New DataColumn("vehicle_type", GetType(String))

        ' Add the new column to the DataTable
        dataTable.Columns.Add(NewColumn)
        If dataTable.Rows.Count > 0 Then
            For Each row As DataRow In dataTable.Rows
                Dim id As Integer = If(Not IsDBNull(row("vehicle_type_ID")), Convert.ToInt32(row("vehicle_type_ID")), "")
                Dim name As String = (TransportGlobals.GetVehicleType(id)).ToString()
                row("vehicle_type") = name
            Next
            ' Iterate through the rows to handle empty image data
            For Each row As DataGridViewRow In DataGridView1.Rows
                Dim imageCell As DataGridViewImageCell = row.Cells("Column4")

                ' Check if the image cell value is DBNull or Nothing
                If IsDBNull(imageCell.Value) OrElse imageCell.Value Is Nothing Then
                    ' Set a placeholder image or leave the cell blank
                    ' For example, you can set a blank image
                    Dim blankImage As New Bitmap(1, 1)
                    imageCell.Value = blankImage
                End If
            Next
        End If

        Dim filteredRows1() As DataRow = dataTable.Select($"status = 'approved'")
        Dim filteredRows1List As New List(Of DataRow)()
        ' Create a new DataTable to store the filtered results
        Dim dataTable1 As DataTable = dataTable.Clone()

        ' Copy filtered rows to the new DataTable
        For Each row As DataRow In filteredRows1
            dataTable1.ImportRow(row)
        Next

        'IMP: Specify the Column Mappings from DataGridView to SQL Table
        DataGridView1.AutoGenerateColumns = False
        DataGridView1.Columns(0).DataPropertyName = "vehicle_ID"
        DataGridView1.Columns(1).DataPropertyName = "vehicle_type"
        ' Bind the data to DataGridView
        DataGridView1.DataSource = dataTable1

        ' Load user data
        Dim userDataQuery As String = "SELECT name, age, address FROM users WHERE user_id = @UserId"
        Using userDataCmd As New MySqlCommand(userDataQuery, Con)
            userDataCmd.Parameters.AddWithValue("@UserId", uid)
            Using userDatareader As MySqlDataReader = userDataCmd.ExecuteReader()
                If userDatareader.Read() Then
                    Nametb.Text = u_name
                    Agetb.Text = If(Not IsDBNull(userDatareader("age")), userDatareader("age").ToString(), "NULL")
                    Addresstb.Text = If(Not IsDBNull(userDatareader("address")), userDatareader("address").ToString(), "NULL")
                Else
                    MessageBox.Show("No user data is available", "", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If
            End Using
        End Using

        'if pay button clicked 
        If payClicked Then
            Dim insertStatement As String = "INSERT INTO vehicle_reg (vehicle_id, inv_pdf, vehicle_pic, vehicle_type, status, uid,inv_id) 
                                                    VALUES (@Value1, @Value2, @Value3, @Value4, @Value5, @Value6, @Value7) 
                                                    ;
                                                    "
            If VTypeCb.SelectedItem IsNot Nothing Then
                Dim vtype As String = VTypeCb.SelectedItem.ToString()
                Dim vTypeId As Integer = TransportGlobals.GetVehicleTypeID(vtype)
                Dim invid As Integer = Convert.ToInt32(Inv_idtb.Text)

                Dim filteredRows2() As DataRow = dataTable.Select($"vehicle_type_ID = {vTypeId} and inv_id = {invid} ")
                Dim filteredRows2List As New List(Of DataRow)()
                ' Create a new DataTable to store the filtered results
                Dim filteredDataTable As DataTable = dataTable.Clone()
                ' Copy filtered rows to the new DataTable
                For Each row As DataRow In filteredRows2
                    filteredDataTable.ImportRow(row)
                Next
                Using command As New MySqlCommand(insertStatement, Con)
                    command.Parameters.AddWithValue("@Value2", pdfBytes)
                    ' Convert the image in the picture box to a byte array
                    Dim image As Image = PictureBox1.Image
                    Dim backgroundImage As Image = My.Resources.icons8_car_100 ' Replace "ImageName" with the actual name of your image resource

                    Dim imageByteArray As Byte() = ImageToByteArray(image)
                    command.Parameters.AddWithValue("@Value3", imageByteArray)
                    command.Parameters.AddWithValue("@Value4", vTypeId)
                    command.Parameters.AddWithValue("@Value6", uid)
                    command.Parameters.AddWithValue("@Value7", invid)
                    If filteredDataTable.Rows.Count > 0 Then
                        MessageBox.Show("User has already registered a vehicle with this vehicle type and invoice id ")
                        Exit Sub
                    Else
                        command.Parameters.AddWithValue("@Value1", GenerateRandomId())
                    End If
                    command.Parameters.AddWithValue("@Value5", "requested")
                    command.ExecuteNonQuery()
                End Using
            End If
            payClicked = False
        End If

        Con.Close()
    End Sub

    Private Shared RandomGenerator As New Random()
    Private Function GenerateRandomId() As String
        Dim prefix As Integer
        Dim postfix As Integer
        Dim id As String
        Dim idExists As Boolean
        Do
            ' Generate random two-digit number for the prefix
            prefix = RandomGenerator.Next(10, 100)
            ' Generate random four-digit number for the postfix
            postfix = RandomGenerator.Next(100, 1000)
            ' Concatenate the prefix and postfix to form the ID
            id = "AS-" & prefix.ToString("00") & "-" & postfix.ToString("000")
            ' Check if the generated ID exists in the database
            idExists = CheckIdExists(id)
        Loop While idExists

        Return id
    End Function

    Private Function CheckIdExists(id As String) As Boolean
        'Get connection from globals
        Dim Con = Globals.GetDBConnection()
        'Dim connection As New MySqlConnection
        Dim command As New MySqlCommand("SELECT COUNT(*) FROM vehicle_reg WHERE vehicle_id = @id", Con)
        Try
            Con.Open()
            command.Parameters.AddWithValue("@id", id)
            Dim count As Integer = Convert.ToInt32(command.ExecuteScalar())
            Return count > 0
        Catch ex As Exception
            MessageBox.Show("Error checking ID existence: " & ex.Message)
            Return True ' Assume ID exists to avoid inserting duplicates (you may handle this differently based on your application's requirements)
        Finally
            Con.Close()
        End Try
    End Function

    Private Sub TransportationInnerScreen_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        LoadandBindDataGridView()
        ' Populate ComboBox with vehicle types until GetVehicleType returns non-null
        Dim vtypeid As Integer = 1
        Dim vehicleType As String = TransportGlobals.GetVehicleType(vtypeid)
        While vehicleType <> "Unknown"
            VTypeCb.Items.Add(vehicleType)
            vtypeid += 1
            vehicleType = TransportGlobals.GetVehicleType(vtypeid)
        End While
        VTypeCb.SelectedIndex = -1
        Inv_idtb.Text = ""
        Inv_pdftb.Text = ""
        PictureBox1.Image = Nothing
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs)
        payClicked = True
        If Not String.IsNullOrEmpty(Inv_idtb.Text) Then
            If VTypeCb.SelectedItem IsNot Nothing Then
                If PictureBox1.Image IsNot Nothing Then
                    If Not String.IsNullOrEmpty(Inv_pdftb.Text) Then
                        LoadandBindDataGridView()
                        VTypeCb.SelectedIndex = -1
                        Inv_idtb.Text = ""
                        Inv_pdftb.Text = ""
                        PictureBox1.Image = Nothing
                        MessageBox.Show("Payment request will be sent. ", "Registration fee Payment", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    Else
                        MessageBox.Show("Give Invoice pdf to proceed", "", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    End If
                Else
                    MessageBox.Show("Give Vehicle picture to proceed", "", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If
            Else
                MessageBox.Show("Select a vehicle type to proceed", "", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
        Else
            MessageBox.Show("Give invoice id to proceed", "", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If
    End Sub


    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        VTypeCb.SelectedIndex = -1
        Inv_idtb.Text = ""
        Inv_pdftb.Text = ""
        PictureBox1.Image = Nothing
        MessageBox.Show("Registration will be cancelled ", "", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs)
        Dim openFileDialog As New OpenFileDialog With {
            .Filter = "PDF Files (*.pdf)|*.pdf"
        }
        If openFileDialog.ShowDialog() = DialogResult.OK Then
            Dim pdfFilePath As String = openFileDialog.FileName
            ' Check if the selected file is a PDF file
            If pdfFilePath.ToLower().EndsWith(".pdf") Then
                Try
                    ' Read the PDF file into a byte array
                    pdfBytes = File.ReadAllBytes(pdfFilePath)
                    Inv_pdftb.Text = pdfFilePath
                Catch ex As Exception
                    MessageBox.Show("Error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try
            Else
                MessageBox.Show("Please select a PDF file.", "Invalid File", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
        End If

    End Sub

    Private Sub Button5_Click_1(sender As Object, e As EventArgs) Handles Vehicle_picbtn.Click
        Dim openFileDialog As New OpenFileDialog()

        openFileDialog.Filter = "Image Files (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png"
        If openFileDialog.ShowDialog() = DialogResult.OK Then
            ' Get the selected file path
            Dim imagePath As String = openFileDialog.FileName
            ' Display the selected image in a PictureBox control
            PictureBox1.Image = Image.FromFile(imagePath)
            ' Convert the image to a byte array
            imageBytes = File.ReadAllBytes(imagePath)

        End If

    End Sub

End Class
