<%@ Page Language="C#" Inherits="SS.Form.Pages.PageLogs" %>
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

      <ul class="nav nav-tabs">
        <li class="nav-item active">
          <a class="nav-link" href="<%=PageLogsUrl%>">数据列表</a>
        </li>
        <li class="nav-item">
          <a class="nav-link" href="<%=PageResultsUrl%>">数据统计</a>
        </li>
        <li class="nav-item">
          <a class="nav-link" href="<%=PageFieldsUrl%>">字段管理</a>
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

      <div class="container">
        <div class="m-b-25"></div>
        <div class="row">
          <div class="col-sm-12">
            <div class="card-box">
              <h4 class="text-dark  header-title m-t-0">数据提交记录</h4>
              <p class="text-muted m-b-25 font-13"></p>
              <asp:Literal id="LtlMessage" runat="server" />

              <table class="tablesaw m-t-20 table m-b-0 tablesaw-stack">
                <thead>
                  <tr>
                    <asp:Literal id="LtlFieldNames" runat="server" />
                    <th scope="col">提交时间</th>
                    <th scope="col"></th>
                  </tr>
                </thead>
                <tbody>
                  <asp:repeater id="RptLogs" runat="server">
                    <itemtemplate>
                      <tr>
                        <asp:Literal id="ltlValues" runat="server" />
                        <td style="min-width: 120px;">
                          <asp:Literal id="ltlAddDate" runat="server" />
                        </td>
                        <td style="min-width: 120px;">
                          <asp:Literal id="ltlOperation" runat="server" />
                        </td>
                      </tr>
                    </itemtemplate>
                  </asp:repeater>
                </tbody>
              </table>

              <div class="m-b-25"></div>

              <asp:Button class="btn" id="BtnExport" onclick="BtnExport_Click" Text="导 出" runat="server" />
              <asp:Button class="btn" id="BtnSettings" Text="显示项" runat="server" />

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