<%@ Page Language="C#" Inherits="SS.Form.Pages.PageFields" %>
  <!DOCTYPE html>
  <html>

  <head>
    <meta charset="utf-8">
    <link href="assets/plugin-utils/css/bootstrap.min.css" rel="stylesheet" type="text/css" />
    <link href="assets/plugin-utils/css/plugin-utils.css" rel="stylesheet" type="text/css" />
    <link href="assets/plugin-utils/css/font-awesome.min.css" rel="stylesheet" type="text/css" />
    <link href="assets/plugin-utils/css/ionicons.min.css" rel="stylesheet" type="text/css" />
  </head>

  <body>

    <form runat="server">

      <header id="topnav">
        <div class="navbar-custom">
          <div class="container">
            <div id="navigation">
              <ul class="navigation-menu">
                <li class="has-submenu">
                  <a href="<%=PageLogsUrl%>">
                    <i class="ion-compose"></i>
                    数据列表
                  </a>
                </li>
                <li class="has-submenu">
                  <a href="<%=PageResultsUrl%>">
                    <i class="ion-compose"></i>
                    数据统计
                  </a>
                </li>
                <li class="has-submenu active">
                  <a href="<%=PageFieldsUrl%>">
                    <i class="ion-compose"></i>
                    字段管理
                  </a>
                </li>
                <li class="has-submenu">
                  <a href="<%=PageSettingsUrl%>">
                    <i class="ion-compose"></i>
                    选项设置
                  </a>
                </li>
                <li class="has-submenu">
                  <asp:LinkButton id="LbTemplate" onclick="LbTemplate_Click" runat="server">
                    <i class="ion-compose"></i>
                    自定义模板
                  </asp:LinkButton>
                </li>
                <li class="has-submenu">
                  <a href="<%=ReturnUrl%>">
                    <i class="ion-ios-undo"></i>
                    返回列表
                  </a>
                </li>
              </ul>
            </div>
          </div>
        </div>
      </header>

      <!-- container start -->
      <div class="container" style="margin-top: 70px;">
        <div class="m-b-25"></div>
        <div class="row">
          <div class="col-sm-12">
            <div class="card-box">
              <h4 class="text-dark  header-title m-t-0">提交表单管理</h4>
              <p class="text-muted m-b-25 font-13"></p>
              <asp:Literal id="LtlMessage" runat="server" />

              <asp:dataGrid id="DgContents" showHeader="true" AutoGenerateColumns="false" HeaderStyle-CssClass="info thead" CssClass="table m-0"
                gridlines="none" runat="server">
                <Columns>
                  <asp:TemplateColumn HeaderText="标题">
                    <ItemTemplate>
                      <asp:Literal ID="ltlTitle" runat="server"></asp:Literal>
                    </ItemTemplate>
                  </asp:TemplateColumn>
                  <asp:TemplateColumn HeaderText="描述">
                    <ItemTemplate>
                      <asp:Literal ID="ltlDescription" runat="server"></asp:Literal>
                    </ItemTemplate>
                  </asp:TemplateColumn>
                  <asp:TemplateColumn HeaderText="表单提交类型">
                    <ItemTemplate>
                      <asp:Literal ID="ltlFieldType" runat="server"></asp:Literal>
                    </ItemTemplate>
                    <ItemStyle Width="120" />
                  </asp:TemplateColumn>
                  <asp:TemplateColumn HeaderText="验证规则">
                    <ItemTemplate>
                      <asp:Literal ID="ltlValidate" runat="server"></asp:Literal>
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="left" />
                  </asp:TemplateColumn>
                  <asp:TemplateColumn HeaderText="操作">
                    <ItemTemplate>
                      <asp:Literal ID="ltlActions" runat="server"></asp:Literal>
                    </ItemTemplate>
                    <ItemStyle Width="300" />
                  </asp:TemplateColumn>
                </Columns>
              </asp:dataGrid>

              <div class="m-b-25"></div>

              <asp:Button class="btn btn-success" id="BtnAddField" Text="新增字段" runat="server" />
              <asp:Button class="btn" id="BtnAddFields" Text="批量新增字段" runat="server" />
              <asp:Button class="btn" id="BtnImport" Text="导 入" runat="server" />
              <asp:Button class="btn" id="BtnExport" Text="导 出" runat="server" />

            </div>
          </div>
        </div>
      </div>
      <!-- container end -->

      <!-- modal add start -->
      <asp:PlaceHolder id="PhModalAdd" visible="false" runat="server">
        <div id="modalAdd" class="modal fade" style="display: none;">
          <div class="modal-dialog" style="width:80%;">
            <div class="modal-content">
              <div class="modal-header">
                <button type="button" class="close" onClick="$('#modalAdd').modal('hide');return false;" aria-hidden="true">×</button>
                <h4 class="modal-title" id="modalLabel">
                  <asp:Literal id="LtlModalAddTitle" runat="server"></asp:Literal>
                </h4>
              </div>
              <div class="modal-body">
                <asp:Literal id="LtlModalAddMessage" runat="server"></asp:Literal>

                <div class="form-horizontal">

                  <div class="form-group">
                    <label class="col-md-3 control-label">标题</label>
                    <div class="col-md-6">
                      <asp:TextBox class="form-control" Columns="25" MaxLength="50" id="TbTitle" runat="server" />
                    </div>
                    <div class="col-md-3">
                      <asp:RequiredFieldValidator ControlToValidate="TbTitle" errorMessage=" *" foreColor="red" display="Dynamic" runat="server"
                      />
                      <asp:RegularExpressionValidator runat="server" ControlToValidate="TbTitle" ValidationExpression="[^']+" errorMessage=" *"
                        foreColor="red" display="Dynamic" />
                    </div>
                  </div>

                  <div class="form-group">
                    <label class="col-md-3 control-label">描述</label>
                    <div class="col-md-6">
                      <asp:TextBox class="form-control" Columns="25" MaxLength="50" id="TbDescription" runat="server" />
                    </div>
                    <div class="col-md-3">
                      <asp:RegularExpressionValidator runat="server" ControlToValidate="TbDescription" ValidationExpression="[^']+" errorMessage=" *"
                        foreColor="red" display="Dynamic" />
                    </div>
                  </div>

                  <div class="form-group">
                    <label class="col-md-3 control-label">提示信息</label>
                    <div class="col-md-6">
                      <asp:TextBox class="form-control" Columns="25" MaxLength="50" id="TbPlaceHolder" runat="server" />
                    </div>
                    <div class="col-md-3">
                    </div>
                  </div>

                  <div class="form-group">
                    <label class="col-md-3 control-label">表单提交类型</label>
                    <div class="col-md-6">
                      <asp:DropDownList class="form-control" ID="DdlFieldType" OnSelectedIndexChanged="ReFresh" AutoPostBack="true" runat="server"></asp:DropDownList>
                    </div>
                    <div class="col-md-3">
                    </div>
                  </div>

                  <asp:PlaceHolder ID="PhIsSelectField" runat="server">

                    <div class="form-group">
                      <label class="col-md-3 control-label">设置选项</label>
                      <div class="col-md-6">
                        <asp:DropDownList class="form-control" ID="DdlIsRapid" OnSelectedIndexChanged="ReFresh" AutoPostBack="true" runat="server">
                          <asp:ListItem Text="快速设置" Value="True" Selected="True" />
                          <asp:ListItem Text="详细设置" Value="False" />
                        </asp:DropDownList>
                      </div>
                      <div class="col-md-3"></div>
                    </div>

                    <asp:PlaceHolder ID="PhRapid" runat="server">
                      <div class="form-group">
                        <label class="col-md-3 control-label">选项可选值</label>
                        <div class="col-md-6">
                          <asp:TextBox TextMode="MultiLine" class="form-control" Columns="60" id="TbRapidValues" runat="server" />
                          <span class="help-block">英文","分隔，如：“选项1,选项2”</span>
                        </div>
                        <div class="col-md-3">
                          <asp:RequiredFieldValidator ControlToValidate="TbRapidValues" errorMessage=" *" foreColor="red" display="Dynamic" runat="server"
                          />
                        </div>
                      </div>
                    </asp:PlaceHolder>

                    <asp:PlaceHolder ID="PhItemCount" runat="server">
                      <div class="form-group">
                        <label class="col-md-3 control-label">共有选项</label>
                        <div class="col-md-6">
                          <asp:TextBox class="form-control" id="TbItemCount" runat="server" />
                        </div>
                        <div class="col-md-3">
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
                        <label class="col-md-3 control-label">选项可选值</label>
                        <div class="col-md-6">
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
                        <div class="col-md-3">
                        </div>
                      </div>
                    </asp:PlaceHolder>

                  </asp:PlaceHolder>

                </div>

              </div>
              <div class="modal-footer">
                <asp:Button class="btn btn-primary" onclick="Add_OnClick" runat="server" Text="保 存"></asp:Button>
                <button type="button" class="btn btn-default" onClick="$('#modalAdd').modal('hide');return false;">取 消</button>
              </div>
            </div>
          </div>
        </div>
      </asp:PlaceHolder>
      <!-- modal add end -->

      <!-- modal validate start -->
      <asp:PlaceHolder id="PhModalValidate" visible="false" runat="server">
        <div id="modalValidate" class="modal fade" style="display: none;">
          <div class="modal-dialog" style="width:80%;">
            <div class="modal-content">
              <div class="modal-header">
                <button type="button" class="close" onClick="$('#modalValidate').modal('hide');return false;" aria-hidden="true">×</button>
                <h4 class="modal-title" id="modalLabel">
                  设置验证规则
                </h4>
              </div>
              <div class="modal-body">
                <asp:Literal id="LtlModalValidateMessage" runat="server"></asp:Literal>

                <div class="form-horizontal">

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

                </div>

              </div>
              <div class="modal-footer">
                <asp:Button class="btn btn-primary" onclick="BtnValidate_OnClick" runat="server" Text="保 存"></asp:Button>
                <button type="button" class="btn btn-default" onClick="$('#modalValidate').modal('hide');return false;">取 消</button>
              </div>
            </div>
          </div>
        </div>
      </asp:PlaceHolder>
      <!-- modal validate end -->

      <asp:Literal id="LtlScript" runat="server"></asp:Literal>
    </form>
  </body>

  </html>
  <script src="assets/plugin-utils/js/jquery.min.js"></script>
  <script src="assets/plugin-utils/js/bootstrap.min.js"></script>