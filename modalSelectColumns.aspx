<%@ Page Language="C#" Inherits="SS.Form.Pages.ModalSelectColumns" %>
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
  <script type="text/javascript" language="javascript">
    function checkAll(layer, bcheck) {
      for (var i = 0; i < layer.children.length; i++) {
        if (layer.children[i].children.length > 0) {
          checkAll(layer.children[i], bcheck);
        } else {
          if (layer.children[i].type == "checkbox") {
            layer.children[i].checked = bcheck;
          }
        }
      }
    }
  </script>
</head>

<body style="padding: 0;background:#fff">
  <div style="padding: 20px 0;">

    <div class="container">
      <form id="form" runat="server" class="form-horizontal">

        <asp:Literal id="LtlMessage" runat="server" />

        <div class="form-horizontal">

          <div class="form-group">
            <label class="col-xs-3 text-right control-label">需要显示的项</label>
            <div class="col-xs-9">
              <input type="checkbox" id="check_groups" onClick="checkAll(document.getElementById('Group'), this.checked);">
              <label for="check_groups">全选</label>
            </div>
          </div>

          <div class="form-group">
            <label class="col-xs-1 text-right control-label"></label>
            <div id="Group" class="col-xs-10">
              <asp:CheckBoxList ID="CblDisplayAttributes" RepeatColumns="1" RepeatDirection="Horizontal" class="checkbox checkbox-primary"
                runat="server" />
            </div>
            <div class="col-xs-1"></div>
          </div>

          <hr />

          <div class="form-group m-b-0">
            <div class="col-xs-12 text-right">
              <asp:Button class="btn btn-primary m-l-10" text="确 定" runat="server" onClick="Submit_OnClick" />
              <button type="button" class="btn btn-default m-l-10" onclick="window.parent.layer.closeAll()">取 消</button>
            </div>
          </div>

        </div>

      </form>
    </div>

  </div>
</body>

</html>