<%@ Page Language="C#" Inherits="SS.Form.Pages.ModalView" %>
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

  <body style="padding: 0;background:#fff">
    <div style="padding: 20px 0;">

      <div class="container">
        <form id="form" runat="server" class="form-horizontal">

          <asp:Literal id="LtlMessage" runat="server" />

          <div class="form-horizontal">

            <asp:repeater id="RptContents" runat="server">
              <itemtemplate>
                <div class="form-group">
                  <label class="col-xs-3 text-right control-label">
                    <asp:Literal id="ltlTitle" runat="server"></asp:Literal>
                  </label>
                  <div class="col-xs-8 help-block">
                    <asp:Literal id="ltlValue" runat="server"></asp:Literal>
                  </div>
                  <div class="col-xs-1"></div>
                </div>
              </itemtemplate>
            </asp:repeater>

            <div class="form-group">
                <label class="col-xs-3 text-right control-label">
                    提交时间
                </label>
                <div class="col-xs-8 help-block">
                  <asp:Literal id="LtlAddDate" runat="server"></asp:Literal>
                </div>
                <div class="col-xs-1"></div>
              </div>

            <hr />

            <div class="form-group m-b-0">
              <div class="col-xs-12 text-right">
                <button type="button" class="btn btn-default m-l-10" onclick="window.parent.layer.closeAll()">关 闭</button>
              </div>
            </div>

          </div>

        </form>
      </div>

    </div>
  </body>

  </html>