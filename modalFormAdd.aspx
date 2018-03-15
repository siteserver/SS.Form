<%@ Page Language="C#" Inherits="SS.Form.Pages.ModalFormAdd" %>
  <!DOCTYPE html>
  <html style="background:#fff">

  <head>
    <meta charset="utf-8">
    <link href="assets/plugin-utils/css/bootstrap.min.css" rel="stylesheet" type="text/css" />
    <link href="assets/plugin-utils/css/plugin-utils.css" rel="stylesheet" type="text/css" />
    <link href="assets/plugin-utils/css/font-awesome.min.css" rel="stylesheet" type="text/css" />
    <link href="assets/plugin-utils/css/ionicons.min.css" rel="stylesheet" type="text/css" />
  </head>

  <body style="padding: 0;background:#fff">
    <div style="padding: 20px 0;">

      <div class="container">
        <form id="form" runat="server" class="form-horizontal">

          <asp:Literal id="LtlMessage" runat="server"></asp:Literal>

          <div class="form-group">
            <label class="col-md-3 control-label">名称</label>
            <div class="col-md-6">
              <asp:TextBox class="form-control" Columns="25" MaxLength="50" id="TbTitle" runat="server" />
            </div>
            <div class="col-md-3">
              <asp:RequiredFieldValidator ControlToValidate="TbTitle" errorMessage=" *" foreColor="red" display="Dynamic" runat="server"
              />
              <asp:RegularExpressionValidator runat="server" ControlToValidate="TbTitle" ValidationExpression="[^',]+" errorMessage=" *"
                foreColor="red" display="Dynamic" />
            </div>
          </div>
          <div class="form-group">
            <label class="col-md-3 control-label">说明</label>
            <div class="col-md-6">
              <asp:TextBox class="form-control" TextMode="Multiline" Columns="25" MaxLength="50" id="TbDescription" runat="server" />
            </div>
            <div class="col-md-3">
            </div>
          </div>

          <div class="modal-footer">
            <asp:Button class="btn btn-primary" onclick="Submit_OnClick" runat="server" Text="保 存"></asp:Button>
            <button type="button" class="btn btn-default m-l-10" onclick="window.parent.layer.closeAll()">取 消</button>
          </div>


        </form>
      </div>
    </div>
  </body>

  </html>
  <script src="assets/plugin-utils/js/jquery.min.js"></script>
  <script src="assets/plugin-utils/js/bootstrap.min.js"></script>
  <script src="assets/layer/layer.min.js" type="text/javascript"></script>
  <script src="assets/sweetalert/sweetalert.min.js" type="text/javascript"></script>