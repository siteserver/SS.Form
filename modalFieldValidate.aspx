<%@ Page Language="C#" Inherits="SS.Form.Pages.ModalFieldValidate" %>
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
            <label class="col-md-3 control-label">是否为必填项</label>
            <div class="col-md-6">
              <asp:DropDownList id="DdlIsRequired" class="form-control" runat="server">
                <asp:ListItem Value="True" Text="是" />
                <asp:ListItem Value="False" Text="否" Selected="True" />
              </asp:DropDownList>
            </div>
            <div class="col-md-3">

            </div>
          </div>

          <asp:PlaceHolder ID="PhNum" runat="server">
            <div class="form-group">
              <label class="col-md-3 control-label">最小字符数</label>
              <div class="col-md-6">
                <asp:TextBox class="form-control" MaxLength="50" Text="0" id="TbMinNum" runat="server" />
              </div>
              <div class="col-md-3">
                个字符
                <asp:RegularExpressionValidator ControlToValidate="TbMinNum" ValidationExpression="\d+" Display="Dynamic" errorMessage=" *"
                  foreColor="red" runat="server" /> （0代表不限制）
              </div>
            </div>
            <div class="form-group">
              <label class="col-md-3 control-label">最大字符数</label>
              <div class="col-md-6">
                <asp:TextBox class="form-control" MaxLength="50" Text="0" id="TbMaxNum" runat="server" />
              </div>
              <div class="col-md-3">
                个字符
                <asp:RegularExpressionValidator ControlToValidate="TbMaxNum" ValidationExpression="\d+" Display="Dynamic" errorMessage=" *"
                  foreColor="red" runat="server" /> （0代表不限制）
              </div>
            </div>
          </asp:PlaceHolder>

          <asp:PlaceHolder ID="PhValidateType" runat="server">
            <div class="form-group">
              <label class="col-md-3 control-label">高级验证</label>
              <div class="col-md-6">
                <asp:DropDownList ID="DdlValidateType" class="form-control" runat="server"></asp:DropDownList>
              </div>
              <div class="col-md-3">

              </div>
            </div>
          </asp:PlaceHolder>

          <div class="modal-footer">
            <asp:Button class="btn btn-primary" onclick="BtnValidate_OnClick" runat="server" Text="保 存"></asp:Button>
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