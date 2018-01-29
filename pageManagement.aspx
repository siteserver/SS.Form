<%@ Page Language="C#" Inherits="SS.Form.Pages.PageManagement" %>
  <!DOCTYPE html>
  <html>

  <head>
    <meta charset="utf-8">
    <link href="assets/plugin-utils/css/bootstrap.min.css" rel="stylesheet" type="text/css" />
    <link href="assets/plugin-utils/css/plugin-utils.css" rel="stylesheet" type="text/css" />
    <link href="assets/plugin-utils/css/font-awesome.min.css" rel="stylesheet" type="text/css" />
    <link href="assets/plugin-utils/css/ionicons.min.css" rel="stylesheet" type="text/css" />
    <script src="assets/plugin-utils/js/jquery.min.js"></script>
    <script src="assets/plugin-utils/js/bootstrap.min.js"></script>
  </head>

  <body>
    <form runat="server">

      <!-- container start -->
      <div class="container">
        <div class="m-b-25"></div>

        <div class="row">
          <div class="col-sm-12">
            <div class="card-box">
              <h4 class="text-dark  header-title m-t-0">表单管理</h4>
              <p class="text-muted m-b-25 font-13"></p>
              <asp:Literal id="LtlMessage" runat="server" />

              <asp:dataGrid id="DgContents" showHeader="true" AutoGenerateColumns="false" DataKeyField="Id" HeaderStyle-CssClass="info thead"
                CssClass="table m-0" gridlines="none" runat="server">
                <Columns>
                  <asp:TemplateColumn HeaderText="名称">
                    <ItemTemplate>
                      <asp:Literal ID="ltlTitle" runat="server"></asp:Literal>
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="left" />
                  </asp:TemplateColumn>
                  <asp:TemplateColumn HeaderText="说明">
                    <ItemTemplate>
                      <asp:Literal ID="ltlDescription" runat="server"></asp:Literal>
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="left" />
                  </asp:TemplateColumn>
                  <asp:TemplateColumn HeaderText="操作">
                    <ItemTemplate>
                      <asp:Literal ID="ltlActions" runat="server"></asp:Literal>
                    </ItemTemplate>
                    <ItemStyle Width="400" />
                  </asp:TemplateColumn>
                </Columns>
              </asp:dataGrid>

              <div class="m-b-25"></div>

              <asp:Button class="btn btn-success" id="BtnAddInput" Text="添加表单" runat="server" />
              <asp:Button class="btn" id="BtnImport" Text="导入表单" runat="server" />

            </div>
          </div>
        </div>
      </div>
      <!-- container end -->

      <asp:PlaceHolder id="PhModalAdd" runat="server" visible="false">

        <div id="modalAdd" class="modal fade">
          <div class="modal-dialog" style="width:80%;">
            <div class="modal-content">
              <div class="modal-header">
                <button type="button" class="close" onClick="$('#modalAdd').modal('hide');return false;" aria-hidden="true" aria-hidden="true">×</button>
                <h4 class="modal-title" id="modalLabel">
                  <asp:Literal id="LtlModalAddTitle" runat="server"></asp:Literal>
                </h4>
              </div>
              <div class="modal-body">
                <asp:Literal id="LtlModalAddMessage" runat="server"></asp:Literal>

                <div class="form-horizontal">
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

                </div>

              </div>
              <div class="modal-footer">
                <asp:Button class="btn btn-primary" onclick="Submit_OnClick" runat="server" Text="保 存"></asp:Button>
                <button type="button" class="btn btn-default" onClick="$('#modalAdd').modal('hide');return false;" aria-hidden="true">取 消</button>
              </div>
            </div>
          </div>
        </div>
        <script>
          $(document).ready(function () {
            $('#modalAdd').modal();
          });
        </script>
      </asp:PlaceHolder>

      <asp:PlaceHolder id="PhModalImport" runat="server" visible="false">
        <div id="modalImport" class="modal fade">
          <div class="modal-dialog" style="width:70%;">
            <div class="modal-content">
              <div class="modal-header">
                <button type="button" class="close" onClick="$('#modalImport').modal('hide');return false;" aria-hidden="true">×</button>
                <h4 class="modal-title" id="modalLabel">
                  导入表单
                </h4>
              </div>
              <div class="modal-body">

                <div class="form-horizontal">
                  <div class="form-group">
                    <label class="col-md-3 control-label">文件</label>
                    <div class="col-md-6">
                      <input type="file" id="HifImport" class="form-control" runat="server" />
                    </div>
                    <div class="col-md-3">
                      <span class="help-block"></span>
                    </div>
                  </div>
                </div>

              </div>
              <div class="modal-footer">
                <asp:Button class="btn btn-primary" runat="server" Text="导 入"></asp:Button>
                <button type="button" class="btn btn-default" onClick="$('#modalImport').modal('hide');return false;">关 闭</button>
              </div>
            </div>
          </div>
        </div>
        <script>
          $(document).ready(function () {
            $('#modalImport').modal();
          });
        </script>
      </asp:PlaceHolder>
    </form>
  </body>

  </html>