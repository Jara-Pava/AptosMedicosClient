<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SinAcceso.aspx.cs" Inherits="DesarrollosQAS.SinAcceso" %>

<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Sin Acceso</title>
    <link rel="shortcut icon" href="~/Images/techint.ico" type="image/vnd.microsoft.icon" />
    <style>
        body {
            font-family: 'Segoe UI', Tahoma, Geneva, sans-serif;
            background-color: #f5f5f5;
            margin: 0;
            padding: 0;
            display: flex;
            justify-content: center;
            align-items: center;
            min-height: 100vh;
        }

        .container {
            background: white;
            border-radius: 8px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.1);
            padding: 50px 40px;
            text-align: center;
            max-width: 500px;
            width: 90%;
        }

        .icon-lock {
            font-size: 64px;
            color: #dc3545;
            margin-bottom: 20px;
        }

        h1 {
            color: #353943;
            font-size: 28px;
            margin-bottom: 10px;
        }

        .message {
            color: #666;
            font-size: 16px;
            line-height: 1.6;
            margin-bottom: 10px;
        }

        .user-info {
            background-color: #f8f9fa;
            border: 1px solid #e9ecef;
            border-radius: 4px;
            padding: 12px;
            margin: 20px 0;
            color: #353943;
            font-weight: bold;
        }

        .footer-text {
            color: #999;
            font-size: 13px;
            margin-top: 30px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <div class="icon-lock">&#128274;</div>
            <h1>Acceso Denegado</h1>
            <p class="message">
                Su usuario de red no se encuentra registrado o activo en el sistema.
                <br />
                Contacte al administrador para solicitar acceso.
            </p>
            <div class="user-info">
                Usuario: <asp:Label ID="lblUsuario" runat="server" />
            </div>
            <p class="footer-text">
                Si cree que esto es un error, comuníquese con el área de sistemas.
            </p>
        </div>
    </form>
</body>
</html>