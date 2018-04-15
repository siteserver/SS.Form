<%@ Page Language="C#" Inherits="SS.Form.Pages.PageFields" %>
  <!DOCTYPE html>
  <html>

  <head>
    <meta charset="utf-8">
    <link href="assets/css/bootstrap.min.css" rel="stylesheet" type="text/css" />
    <link href="assets/css/siteserver.min.css" rel="stylesheet" type="text/css" />
    <link href="assets/css/ionicons.min.css" rel="stylesheet" type="text/css" />
  </head>

  <body>

    <form runat="server">

      <ul class="nav nav-tabs tabs-bordered nav-justified">
        <li class="nav-item">
          <a class="nav-link" href="<%=PageLogsUrl%>">数据列表</a>
        </li>
        <li class="nav-item">
          <a class="nav-link" href="<%=PageResultsUrl%>">数据统计</a>
        </li>
        <li class="nav-item">
          <a class="nav-link active" href="<%=PageFieldsUrl%>">字段管理</a>
        </li>
        <li class="nav-item">
          <a class="nav-link" href="<%=PageSettingsUrl%>">选项设置</a>
        </li>
        <li class="nav-item">
          <asp:LinkButton id="LbTemplate" class="nav-link" onclick="LbTemplate_Click" runat="server">
            自定义模板
          </asp:LinkButton>
        </li>
        <li class="nav-item">
          <a class="nav-link" href="<%=ReturnUrl%>">
            <i class="ion-ios-undo"></i>
            返回列表
          </a>
        </li>
      </ul>

      <div class="m-3">

        <div class="row">
          <div class="m-2 col-12">
            <div class="float-right">
              <a href="http://plugins.siteserver.cn/ss.form/docs/" target="_blank">使用教程</a>
              模板标签：
              <code>
                  &lt;stl:form title="<%=FormTitle%>"&gt;&lt;/stl:form&gt;
              </code>
            </div>
            <h4 class="float-left">
              <%=FormTitle%>
            </h4>
          </div>
        </div>

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

    </form>
  </body>

  </html>
  <script src="assets/plugin-utils/js/jquery.min.js"></script>
  <script src="assets/plugin-utils/js/bootstrap.min.js"></script>
  <script src="assets/layer/layer.min.js" type="text/javascript"></script>
  <script src="assets/sweetalert/sweetalert.min.js" type="text/javascript"></script>