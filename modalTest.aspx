<%@ Page Language="C#" Inherits="SS.SMS.Pages.ModalTest" %>
  <!DOCTYPE html>
  <html style="background:#fff">

  <head>
    <meta charset="utf-8">
    <link href="assets/plugin-utils/css/bootstrap.min.css" rel="stylesheet" type="text/css" />
    <link href="assets/plugin-utils/css/plugin-utils.css" rel="stylesheet" type="text/css" />
    <link href="assets/plugin-utils/css/font-awesome.min.css" rel="stylesheet" type="text/css" />
    <link href="assets/plugin-utils/css/ionicons.min.css" rel="stylesheet" type="text/css" />
    <script src="assets/js/jquery.min.js" type="text/javascript"></script>
    <script src="assets/layer/layer.min.js" type="text/javascript"></script>
  </head>

  <body style="padding: 0;background:#fff" class="m-l-15 m-r-15 m-t-15">
    <form id="form" runat="server">

      <asp:Literal id="LtlMessage" runat="server" />

      <div class="form-group">
        <label class="col-form-label">接收短信手机号
          <asp:RequiredFieldValidator ControlToValidate="TbMobile" ErrorMessage=" *" ForeColor="red" Display="Dynamic" runat="server"
          />
          <asp:RegularExpressionValidator runat="server" ControlToValidate="TbMobile" ValidationExpression="[^']+" ErrorMessage=" *"
            ForeColor="red" Display="Dynamic" />
        </label>
        <asp:TextBox ID="TbMobile" class="form-control" runat="server" />
      </div>

      <div class="form-group">
        <label class="col-form-label">短信类型</label>
        <asp:DropDownList ID="DdlType" class="form-control" AutoPostBack="true" OnSelectedIndexChanged="DdlType_SelectedIndexChanged"
          runat="server"></asp:DropDownList>
      </div>

      <div class="form-group">
        <label class="col-form-label">模板Id
          <asp:RequiredFieldValidator ControlToValidate="TbTplId" ErrorMessage=" *" ForeColor="red" Display="Dynamic" runat="server"
          />
          <asp:RegularExpressionValidator runat="server" ControlToValidate="TbTplId" ValidationExpression="[^']+" ErrorMessage=" *"
            ForeColor="red" Display="Dynamic" />
        </label>
        <asp:TextBox ID="TbTplId" class="form-control" runat="server" />
      </div>

      <hr />

      <div class="form-group m-b-0">
        <div class="col-xs-12 text-right">
          <asp:Button class="btn btn-primary m-l-10" text="发 送" runat="server" onClick="BtnSubmit_OnClick" />
          <button type="button" class="btn btn-default m-l-10" onclick="window.parent.layer.closeAll()">关 闭</button>
        </div>
      </div>

    </form>
  </body>

  </html>