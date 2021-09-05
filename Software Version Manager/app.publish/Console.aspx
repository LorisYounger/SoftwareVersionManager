<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Console.aspx.cs" Inherits="SoftwareVersion.Manager.Console" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <asp:Literal ID="LHeader" runat="server"></asp:Literal>
    <style>
        /*登陆页面*/
        .CenterBox {
            margin-top: 20px;
            margin-left: 0;
            margin-bottom: 20px;
            padding: 26px 24px 46px;
            font-weight: 400;
            overflow: hidden;
            background: #fff;
            margin: auto;
            width: 500px;
            border: 1px solid #ccd0d4;
            box-shadow: 0 1px 3px rgba(0,0,0,.04);
        }

        .errorbox {
            background: #e53935;
            color: #ffebee;
            font-weight: bolder;
            border: 1px solid #ba000d;
            box-shadow: 0 1px 3px rgba(0,0,0,.04);
            margin: auto;
            text-align: center;
        }

        .singlelineinput {
            font-size: 24px;
            line-height: 1.33333333;
            width: 100%;
            border-width: .0625rem;
            padding: .1875rem .3125rem;
            margin: 0 6px 16px 0;
            min-height: 40px;
            max-height: none;
        }

        .BoxLable {
            font-size: 14px;
            line-height: 1.5;
            display: inline-block;
            margin-bottom: 3px;
        }
    </style>
</head>
<body>
    <form id="formConsole" runat="server">
        <asp:TextBox runat="server" ID="MasterKey" Style="display: none"></asp:TextBox>
        <div id="divlogin" runat="server"  visible="false">
            <h1 style="text-align: center">登录</h1>
            <p id="errorboxlogin" runat="server" class="errorbox" visible="false"></p>
            <p class="BoxLable">用户名或电子邮件地址</p>
            <asp:TextBox runat="server" ID="usernamelogin" class="singlelineinput"></asp:TextBox>
            <p class="BoxLable">密码</p>
            <asp:TextBox runat="server" ID="passwordlogin" TextMode="Password" class="singlelineinput"></asp:TextBox>
            <p class="BoxLable">请计算: </p>
            <asp:Label runat="server" Text="0+0=" ID="CalloginKey"></asp:Label>
            <asp:TextBox runat="server" ID="checkloginkey" class="singlelineinput"></asp:TextBox>
            <br />
            <asp:Button runat="server" Text="登陆" ID="buttonlogin" OnClick="buttonlogin_Click" />
            <br />
            <br />
            <a href="?Action=Register">获得账户</a>
            <a href="?Action=Forget" style="text-align: right">忘记密码</a>
        </div>
        <div id="divregister" runat="server" visible="false">
        <h1 style="text-align: center">注册</h1>
        <p id="errorboxregister" runat="server" class="errorbox" visible="false"></p>
        <p class="BoxLable">用户名</p>
        <asp:TextBox runat="server" ID="usernamereg" class="singlelineinput"></asp:TextBox>
        <p class="BoxLable">电子邮件</p>
        <asp:TextBox runat="server" ID="emailreg" class="singlelineinput" onchange="emailregtextchange()"></asp:TextBox>
        <p class="BoxLable">密码</p>
        <asp:TextBox runat="server" ID="passwordreg" TextMode="Password" class="singlelineinput"></asp:TextBox>
        <p class="BoxLable">请计算: </p>
        <asp:Label runat="server" Text="0+0=" ID="CalregistKey"></asp:Label>
        <asp:TextBox runat="server" ID="checkregisterkey" class="singlelineinput"></asp:TextBox>
        <br/>
        <div id="emailcheck" runat="server" style="display: none" visible="false">
            <p class="BoxLable">邮箱验证码</p>
            <button id="bottonsendregemail" style="float: right; font-size: 50%;" onclick="sendregemail()" type="button">获取验证码</button>
            <asp:TextBox runat="server" ID="emailcode" class="singlelineinput"></asp:TextBox>
            <br/>
        </div>
        <asp:Button runat="server" Text="注册" ID="buttonregister" />
        <br />
        <br />
        <a href="?Action=Login">已有账户?立即登陆</a>
        <a href="?Action=Forget" style="text-align: right">忘记密码?通过邮件找回</a>

    </div>
    </form>
</body>
</html>
