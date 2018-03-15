<%@ Page Language="C#" Inherits="SS.Form.Pages.ModalFieldAdd" %>
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

          <div class="form-horizontal">

            <div class="form-group">
              <label class="col-md-2 control-label">标题</label>
              <div class="col-md-8">
                <asp:TextBox class="form-control" Columns="25" MaxLength="50" id="TbTitle" runat="server" />
              </div>
              <div class="col-md-2">
                <asp:RequiredFieldValidator ControlToValidate="TbTitle" errorMessage=" *" foreColor="red" display="Dynamic" runat="server"
                />
                <asp:RegularExpressionValidator runat="server" ControlToValidate="TbTitle" ValidationExpression="[^']+" errorMessage=" *"
                  foreColor="red" display="Dynamic" />
              </div>
            </div>

            <div class="form-group">
              <label class="col-md-2 control-label">描述</label>
              <div class="col-md-8">
                <asp:TextBox class="form-control" Columns="25" MaxLength="50" id="TbDescription" runat="server" />
              </div>
              <div class="col-md-2">
                <asp:RegularExpressionValidator runat="server" ControlToValidate="TbDescription" ValidationExpression="[^']+" errorMessage=" *"
                  foreColor="red" display="Dynamic" />
              </div>
            </div>

            <div class="form-group">
              <label class="col-md-2 control-label">提示信息</label>
              <div class="col-md-8">
                <asp:TextBox class="form-control" Columns="25" MaxLength="50" id="TbPlaceHolder" runat="server" />
              </div>
              <div class="col-md-2">
              </div>
            </div>

            <div class="form-group">
              <label class="col-md-2 control-label">表单提交类型</label>
              <div class="col-md-8">
                <asp:DropDownList class="form-control" ID="DdlFieldType" OnSelectedIndexChanged="ReFresh" AutoPostBack="true" runat="server"></asp:DropDownList>
              </div>
              <div class="col-md-2">
              </div>
            </div>

            <asp:PlaceHolder ID="PhIsSelectField" runat="server">

              <div class="form-group">
                <label class="col-md-2 control-label">设置选项</label>
                <div class="col-md-8">
                  <asp:DropDownList class="form-control" ID="DdlIsRapid" OnSelectedIndexChanged="ReFresh" AutoPostBack="true" runat="server">
                    <asp:ListItem Text="快速设置" Value="True" Selected="True" />
                    <asp:ListItem Text="详细设置" Value="False" />
                  </asp:DropDownList>
                </div>
                <div class="col-md-2"></div>
              </div>

              <asp:PlaceHolder ID="PhRapid" runat="server">
                <div class="form-group">
                  <label class="col-md-2 control-label">选项可选值</label>
                  <div class="col-md-8">
                    <asp:TextBox TextMode="MultiLine" class="form-control" Columns="60" id="TbRapidValues" runat="server" />
                    <span class="help-block">英文","分隔，如：“选项1,选项2”</span>
                  </div>
                  <div class="col-md-2">
                    <asp:RequiredFieldValidator ControlToValidate="TbRapidValues" errorMessage=" *" foreColor="red" display="Dynamic" runat="server"
                    />
                  </div>
                </div>
              </asp:PlaceHolder>

              <asp:PlaceHolder ID="PhItemCount" runat="server">
                <div class="form-group">
                  <label class="col-md-2 control-label">共有选项</label>
                  <div class="col-md-8">
                    <asp:TextBox class="form-control" id="TbItemCount" runat="server" />
                  </div>
                  <div class="col-md-2">
                    <asp:RequiredFieldValidator ControlToValidate="TbItemCount" errorMessage=" *" foreColor="red" display="Dynamic" runat="server"
                    />
                    <asp:Button class="btn" style="margin-bottom:0px;" id="SetCount" text="设 置" onclick="SetCount_OnClick" CausesValidation="false"
                      runat="server" />
                    <asp:RegularExpressionValidator ControlToValidate="TbItemCount" ValidationExpression="\d+" Display="Dynamic" ErrorMessage="此项必须为数字"
                      foreColor="red" runat="server" />
                  </div>
                </div>
              </asp:PlaceHolder>

              <asp:PlaceHolder ID="PhItems" runat="server">
                <div class="form-group">
                  <label class="col-md-2 control-label">选项可选值</label>
                  <div class="col-md-8">
                    <asp:Repeater ID="RptItems" runat="server">
                      <itemtemplate>

                        <div class="row">
                          <div class="col-xs-1">
                          </div>
                          <div class="col-xs-1">
                            <label>
                              <%# Container.ItemIndex + 1 %>：
                            </label>
                            <asp:RequiredFieldValidator ControlToValidate="TbValue" errorMessage=" *" foreColor="red" display="Dynamic" runat="server"
                            />
                          </div>
                          <div class="col-xs-6">
                            <asp:TextBox ID="TbValue" class="form-control" runat="server" Text='<%# DataBinder.Eval(Container.DataItem,"Value") %>'></asp:TextBox>

                          </div>
                          <div class="col-xs-4">
                            <asp:CheckBox ID="CbIsSelected" class="checkbox checkbox-primary" runat="server" Checked="False" Text="默认勾选"></asp:CheckBox>
                            <asp:CheckBox ID="CbIsExtras" class="checkbox checkbox-primary" runat="server" Checked="False" Text="勾选后需提交信息"></asp:CheckBox>
                          </div>
                        </div>
                        <hr />

                      </itemtemplate>
                    </asp:Repeater>
                  </div>
                  <div class="col-md-2">
                  </div>
                </div>
              </asp:PlaceHolder>

            </asp:PlaceHolder>

          </div>

          <div class="modal-footer">
            <asp:Button class="btn btn-primary" onclick="Add_OnClick" runat="server" Text="保 存"></asp:Button>
            <button type="button" class="btn btn-default m-l-10" onclick="window.parent.layer.closeAll()">取 消</button>
          </div>
          
        </form>
      </div>
    </div>
  </body>

  </html>
  <script src="assets/plugin-utils/js/jquery.min.js"></script>
  <script src="assets/plugin-utils/js/bootstrap.min.js"></script>