<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="CombinedTryIt.Default" %>
<%@ OutputCache Duration="20" VaryByParam="*" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Assignment5</title>
    <!--<link href="CSS/StyleSheet.css" rel="stylesheet" type="text/css" />-->
    <style type ="text/css">
        #Input {
            background-color: rgb(144,198,226);
            border-radius:10px;
            border: 1px solid #000;
            width:250px;
            height:100%;
            
        }

        body {
            background-color:rgb(144,198,226);
        }
        header {
            background-color: rgb(0,89,134);
            padding: 30px;
            text-align: left;
            font-size: 35px;
            color: white;
        }
        #ComName {
            padding-right: 50px;
            font-size:20px;
        }
        {
            box-sizing: border-box;
        }
        .column {
            float: left;
            width: 75%;
        }
        .column2 {
            float: left;
            width: 25%;
        }
        .column3 {
            float: left;
            width: 16%;
            margin:1px;
        }

        .row:after {
            content: "";
            display: table;
            clear: both;
        }
        .alert {
          padding: 20px;
          background-color: #f44336;
          color: white;
          display:none;
        }
        #middle {
            height:500px;
        }

        .closebtn {
          margin-left: 15px;
          color: white;
          font-weight: bold;
          float: right;
          font-size: 22px;
          line-height: 20px;
          cursor: pointer;
          transition: 0.3s;
        }
        footer {
            background-color: rgb(0,89,134);
            padding: 30px;
            text-align: left;
            font-size: 15px;
            color: white;
        
        }

        .closebtn:hover {
          color: black;
        }
        .box {
            background-color: rgb(0,89,134);
            padding: 10px;
            text-align: center;
            font-size: 15px;
            color: white;
            border-width:1px;
            border-radius:8px;
            margin:5px;
        }
        .day {
            background-color:rgb(144,198,226);
            padding: 10px;
            text-align: center;
            font-size: 15px;
            color: white;
            border-width:1px;
            border-radius:8px;
            margin:5px;
        }
        #Day1 {
            white-space: pre-wrap;
        }
        #Day2 {
            white-space: pre-wrap;
        }
        #Day3 {
            white-space: pre-wrap;
        }
        #Day4 {
            white-space: pre-wrap;
        }
        #Day5 {
            white-space: pre-wrap;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <header> 
            <asp:Image ID="Image1" runat="server" Height="61px" ImageUrl="Compass.png" Width="62px" />
            <label id="ComName">Compass Weather Co.</label>
            <center><asp:TextBox ID="Input" runat="server"></asp:TextBox>
            <asp:ImageButton ID="ImageButton1" runat="server"  ImageUrl="search.png" OnClick="ImageButton1_Click" Height="30px" Width="30px" />
            </center></header>
            
        <div id="Err" class="alert" runat="server"><span class="closebtn" onclick="this.parentElement.style.display='none';">&times;</span> Invalid Zipcode!</div>
        <div class="row">
        <div class="column">
            <div class ="box">
            <h2>Five Day Forecast</h2>
            <asp:Label ID="Location" runat="server" Text=""></asp:Label>
            <div class="row">
                <div class="column3 day">
                    <center>
                    <asp:Image ID="ImgDay1" runat="server" />
                        <br />
                    <asp:Label ID="Day1" runat="server" Text=""></asp:Label>
                    </center>  
                </div>
                <div class="column3 day">
                    <center>
                    <asp:Image ID="ImgDay2" runat="server" />
                        <br />
                    <asp:Label ID="Day2" runat="server" Text=""></asp:Label>
                    </center>  
                </div>
                <div class="column3 day">
                    <center>
                    <asp:Image ID="ImgDay3" runat="server" />
                        <br />
                    <asp:Label ID="Day3" runat="server" Text=""></asp:Label>
                    </center>  
                </div>
                <div class="column3 day" >
                    <center>
                    <asp:Image ID="ImgDay4" runat="server" />
                        <br />
                    <asp:Label ID="Day4" runat="server" Text=""></asp:Label>
                    </center>  
                </div>
                <div class="column3 day">
                    <center>
                    <asp:Image ID="ImgDay5" runat="server" />
                        <br />
                    <asp:Label ID="Day5" runat="server" Text=""></asp:Label>
                    </center>  
                </div>
            </div>
            </div>
        </div>
        <div class="column2">
            <div class="box">
                <h2>Annual Solar:</h2>
                <p>Ghi: <asp:Label ID="ghi" runat="server" Text=""></asp:Label></p>
                <p>Dni: <asp:Label ID="dni" runat="server" Text=""></asp:Label></p>
                <p>Lat_Tilt: <asp:Label ID="tilt" runat="server" Text=""></asp:Label></p>
            </div>
            <div class="box">
                <h2>Today's Pollution:</h2>
                <p>Air Quality: <asp:Label ID="AQ" runat="server" Text=""></asp:Label></p>
            </div>
        </div>
    </div>
        <footer><p>By: Maryannette Diaz, 2022</p><p>Credit For Icons: <asp:HyperLink ID="HyperLink1" runat="server">https://icons8.com</asp:HyperLink>
            </p></footer>
    </form>
</body>
</html>
